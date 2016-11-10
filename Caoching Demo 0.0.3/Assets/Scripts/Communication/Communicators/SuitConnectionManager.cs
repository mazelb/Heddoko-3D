// /**
// * @file SuitConnectionManager.cs
// * @brief Contains the SuitConnectionManager and requirements to run this class and its functions
// * @author Mohammed Haider(mohammed@heddoko.com )
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
 
using System.IO;
using System.Net;
using System.Net.Sockets; 
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.Settings;
using heddoko;
using HeddokoLib.heddokoProtobuff.Decoder;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using ProtoBuf;

namespace Assets.Scripts.Communication.Communicators
{
    public delegate void BrainpackStatusUpdated(Packet vPacket);
    /// <summary>
    /// A connection manager for the suit 
    /// </summary>
    public class SuitConnectionManager
    {
        private Brainpack mBrainpack;
        private BLEConnection mBleConnection;
        private NetworkedSuitControlConnection mNetworkedSuitControlConnection;
        private NetworkedSuitUdpConnection mNetworkSuitUdpConnection = new NetworkedSuitUdpConnection();
        private ProtobuffDispatchRouter mDispatchRouter;
        private StreamToRawPacketDecoder mDecoder;
        private FirmwareUpdateManager mFirmwareUpdater;
        public OnSuitConnectionEvent NetworkSuitConnectionEstablishedEvent;
        private event BrainpackStatusUpdated mBrainpackUpdateEvent;

        /// <summary>
        /// Adds an event handler to brainpack status update
        /// </summary>
        /// <param name="vBrainpackUpdatedHandle"></param>
        public void AddBrainpackUpdatedHandler(BrainpackStatusUpdated vBrainpackUpdatedHandle)
        {
            mBrainpackUpdateEvent += vBrainpackUpdatedHandle;
        }

       
        /// <summary>
        /// Adds an event handler to brainpack status update
        /// </summary>
        /// <param name="vBrainpackUpdatedHandle"></param>
        public void RemoveBrainpackUpdatedHandler(BrainpackStatusUpdated vBrainpackUpdatedHandle)
        {
            mBrainpackUpdateEvent -= vBrainpackUpdatedHandle;
        }


        private NetworkedSuitControlConnection SuitControlConnection
        {
            get
            {
                return mNetworkedSuitControlConnection;
            }
        }


        public bool IsConnectedToSuitViaNetwork
        {
            get { return SuitControlConnection.Connected; }
        }

        /// <summary>
        /// Suit data received handler
        /// </summary>
        /// <param name="vObject"></param>
        /// <param name="vData"></param>
        private void SuitControlDataReceivedHandler(StateObject vObject)
        {
            var vRawPacket = vObject.OutgoingRawPacket;
            MemoryStream vMemorySteam = new MemoryStream();
            if (vRawPacket.Payload[0] == 0x04)
            {
                //reset the stream pointer, write and reset.
                vMemorySteam.Seek(0, SeekOrigin.Begin);
                vMemorySteam.Write(vRawPacket.Payload, 1, (int)vRawPacket.PayloadSize - 1);
                vMemorySteam.Seek(0, SeekOrigin.Begin);
                Packet vProtoPacket = Serializer.Deserialize<Packet>(vMemorySteam);
                var vMsgType = vProtoPacket.type;
                mDispatchRouter.Process(vMsgType, vObject, vProtoPacket);


            }
        }

        private void FirmwareUpdateHandler()
        {

        }


        /// <summary>
        /// Instantiates a suit connection manager for the specified brainpack
        /// </summary>
        /// <param name="vBrainpack"></param>
        public SuitConnectionManager(Brainpack vBrainpack)
        {
            mBrainpack = vBrainpack;
            mNetworkedSuitControlConnection = new NetworkedSuitControlConnection();
            mNetworkedSuitControlConnection.DataReceivedEvent += SuitControlDataReceivedHandler;
            mNetworkedSuitControlConnection.SuitConnectionEvent += NetworkedSuitControlConnected;
            mDispatchRouter = new ProtobuffDispatchRouter();
            RegisterProtobufEvents();

        }
        /// <summary>
        /// 
        /// </summary>
        private void RegisterProtobufEvents()
        {
            mDispatchRouter.Add(PacketType.StatusResponse, SetBrainpackStatus);

        }

        /// <summary>
        /// Sets the status of the brainpack
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SetBrainpackStatus(object vSender, object vArgs)
        {
            var vPacket = (Packet)vArgs;
            var vBrainpackVersion = vPacket.firmwareVersion;
            mBrainpack.Version = vBrainpackVersion;
            if (mBrainpackUpdateEvent != null)
            {
                mBrainpackUpdateEvent(vPacket);
            }
        }

        private void NetworkedSuitControlConnected()
        {
            if (NetworkSuitConnectionEstablishedEvent != null)
            {
                NetworkSuitConnectionEstablishedEvent();
            }
            //set up the end point of the brainpack
            mBrainpack.Point = new IPEndPoint(mNetworkedSuitControlConnection.SuitIp, mNetworkedSuitControlConnection.Port);
        }


        /// <summary>
        /// Starts a connection to the suit via the given ip endpoint and its port number
        /// </summary>
        /// <param name="vIpEndPoint"></param>
        /// <param name="vPortNum"></param>
        public void ConnectToSuit(string vIpEndPoint, int vPortNum)
        {
            SuitControlConnection.Start(vIpEndPoint, vPortNum);
        }


        /// <summary>
        /// Begins the async file transfer process
        /// </summary>
        public void UpdateFirmware(string vFileName)
        {
            Packet vPacket = new Packet();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string vCurrentEndPoint = "";
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    vCurrentEndPoint = ip.ToString();
                    break;
                }
            }
            vPacket.type = PacketType.UpdateFirmwareRequest;
            vPacket.firmwareUpdate = new FirmwareUpdate();
            vPacket.firmwareUpdate.fwEndpoint = new Endpoint();
            vPacket.firmwareUpdate.fwEndpoint.address = vCurrentEndPoint;
            vPacket.firmwareUpdate.fwEndpoint.port = (uint)ApplicationSettings.TftpPort;
            vPacket.firmwareUpdate.fwFilename = vFileName;
            MemoryStream vStream = new MemoryStream();
            Serializer.Serialize(vStream, vPacket);
            RawPacket vRawPacket = new RawPacket();

            int vRawSize;
            var vRawBytes = vRawPacket.GetRawPacketByteArray(out vRawSize, vStream);
            //Start the firmware update manager before sending the request to update the firmware
            if (mFirmwareUpdater == null)
            {
                mFirmwareUpdater = new FirmwareUpdateManager("C:\\downl\\server", vCurrentEndPoint, ApplicationSettings.TftpPort);
            }
            mNetworkedSuitControlConnection.Send(vRawBytes, vRawSize);


        }

        public void SendPacket(Packet vPacket)
        {
            MemoryStream vStream = new MemoryStream();
            Serializer.Serialize(vStream, vPacket);
            RawPacket vRawPacket = new RawPacket();

            int vRawSize;
            var vRawBytes = vRawPacket.GetRawPacketByteArray(out vRawSize, vStream);
           // vRawBytes[1] = 4;
            //Start the firmware update manager before sending the request to update the firmware
            mNetworkedSuitControlConnection.Send(vRawBytes, vRawSize);
        }

 

        public void CleanUp()
        {
            if (NetworkSuitConnectionEstablishedEvent != null)
            {
                SuitControlConnection.SuitConnectionEvent -= NetworkedSuitControlConnected;
            }
            if (mFirmwareUpdater != null)
            {
                mFirmwareUpdater.CleanUp();
            }

        }


        public void RequestSuitStatus()
        {
            Packet vPacket = new Packet();
            vPacket.type = PacketType.StatusRequest;
            MemoryStream vStream = new MemoryStream();
            Serializer.Serialize(vStream, vPacket);
            RawPacket vRawPacket = new RawPacket();
            int vRawSize;
            var vRawBytes = vRawPacket.GetRawPacketByteArray(out vRawSize, vStream);
            mNetworkedSuitControlConnection.Send(vRawBytes, vRawSize);
        }

       
    }
}