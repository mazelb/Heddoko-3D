// /**
// * @file SuitConnectionManager.cs
// * @brief Contains the SuitConnectionManager and requirements to run this class and its functions
// * @author Mohammed Haider(mohammed@heddoko.com )
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.Settings;
using heddoko;
using HeddokoLib.heddokoProtobuff.Decoder; 
using ProtoBuf;

namespace Assets.Scripts.Communication.Communicators
{
 
    /// <summary>
    /// A connection manager for the suit 
    /// </summary>
    public class SuitConnectionManager 
    {
        private BLEConnection mBleConnection;
        private NetworkedSuitConnection mNetworkedSuitConnection;
        private ProtobuffDispatchRouter mDispatchRouter;
        private StreamToRawPacketDecoder mDecoder;
        private FirmwareUpdateManager mFirmwareUpdater;
        public OnSuitConnectionEvent NetworkSuitConnectionEstablishedEvent;

        private NetworkedSuitConnection SuitConnection
        {
            get
            {
                return mNetworkedSuitConnection;
            }
        }

     
        public bool IsConnectedToSuitViaNetwork
        {
            get { return SuitConnection.Connected; }
        }

        /// <summary>
        /// Suit data received handler
        /// </summary>
        /// <param name="vObject"></param>
        /// <param name="vData"></param>
        private void SuitDataReceivedHandler(StateObject vObject, byte[] vData)
        {
            //look for messages that request
            string vMsg = Encoding.ASCII.GetString(vData);
            if (vMsg.Contains("GetLatestFirmware<EOL>"))
            {
                FirmwareUpdateHandler();
            }
        }

        private void FirmwareUpdateHandler()
        {

        }
 


        public SuitConnectionManager()
        {
            mNetworkedSuitConnection = new NetworkedSuitConnection();
            mNetworkedSuitConnection.DataReceivedEvent += SuitDataReceivedHandler;
            mNetworkedSuitConnection.SuitConnectionEvent += NetworkedSuitConnected;
            RegisterProtobufEvents();
        }
        /// <summary>
        /// 
        /// </summary>
        private void RegisterProtobufEvents()
        {


        }

        private void NetworkedSuitConnected()
        {
            if (NetworkSuitConnectionEstablishedEvent != null)
            {
                NetworkSuitConnectionEstablishedEvent();
            }
        }

        /// <summary>
        /// Starts a connection to the suit via the given ip endpoint and its port number
        /// </summary>
        /// <param name="vIpEndPoint"></param>
        /// <param name="vPortNum"></param>
        public void ConnectToSuit(string vIpEndPoint, int vPortNum)
        {
            SuitConnection.Start(vIpEndPoint, vPortNum);
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
                mFirmwareUpdater = new FirmwareUpdateManager("C:\\downl\\server", vCurrentEndPoint ,ApplicationSettings.TftpPort);
            }
            mNetworkedSuitConnection.Send(vRawBytes);
           

        }

        public void CleanUp()
        {
            if (NetworkSuitConnectionEstablishedEvent != null)
            {
                SuitConnection.SuitConnectionEvent -= NetworkedSuitConnected;
            }
            if (mFirmwareUpdater != null)
            {
                mFirmwareUpdater.CleanUp();
            }
             
        }

         
    }
}