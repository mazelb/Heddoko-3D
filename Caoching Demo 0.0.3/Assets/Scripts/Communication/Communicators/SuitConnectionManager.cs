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
using System.Net.NetworkInformation;
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

    public delegate void ConcerningReportIncluded(bool vIsPain, Packet vPacket);

    public delegate void ImuDataFrameReceived(Packet vPacket);
    /// <summary>
    /// A connection manager for the suit 
    /// </summary>
    public class SuitConnectionManager
    {
        /// <summary>
        /// Event triggered when the status of a brainpack has been updated
        /// </summary>
        private event BrainpackStatusUpdated BrainpackUpdateEvent;
        public event ConcerningReportIncluded ConcerningReportIncludedEvent;
        /// <summary>
        /// Event triggered on the reception of an Imu frame. The full packet is returned
        /// </summary>
        public event ImuDataFrameReceived ImuDataFrameReceivedEvent;
        /// <summary>
        /// Event where the TCP control socket connection has changed. 
        /// </summary>
        public event Action<BrainpackConnectionStateChange, BrainpackNetworkingModel> BrainpackConnectionStateChange;
        /// <summary>
        /// Event where the brainpack returns a status response message
        /// </summary>
        public event Action<Packet> StatusResponseEvent;
        /// <summary>
        /// The network model to base the connection off of. 
        /// </summary>
        private BrainpackNetworkingModel mBrainpackModel;
        private BLEConnection mBleConnection;
        private NetworkedSuitControlConnection mNetworkedSuitControlConnection;
        private NetworkedSuitUdpConnection mNetworkSuitUdpConnection;
        private ProtobuffDispatchRouter mDispatchRouter;
        private StreamToRawPacketDecoder mDecoder;
        private FirmwareUpdateManager mFirmwareUpdater;

        public string FirmwareLocationPath = "C:\\downl\\server";

        /// <summary>
        /// Adds an event handler to brainpack status update
        /// </summary>
        /// <param name="vBrainpackUpdatedHandle"></param>
        public void AddBrainpackUpdatedHandler(BrainpackStatusUpdated vBrainpackUpdatedHandle)
        {
            BrainpackUpdateEvent += vBrainpackUpdatedHandle;
        }


        /// <summary>
        /// Adds an event handler to brainpack status update
        /// </summary>
        /// <param name="vBrainpackUpdatedHandle"></param>
        public void RemoveBrainpackUpdatedHandler(BrainpackStatusUpdated vBrainpackUpdatedHandle)
        {
            BrainpackUpdateEvent -= vBrainpackUpdatedHandle;
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
        /// <param name="vBrainpackModel"></param>
        public SuitConnectionManager(BrainpackNetworkingModel vBrainpackModel) : this()
        {
            mBrainpackModel = vBrainpackModel;

        }

        public SuitConnectionManager()
        {
            mNetworkedSuitControlConnection = new NetworkedSuitControlConnection();
            mNetworkedSuitControlConnection.DataReceivedEvent += SuitControlDataReceivedHandler;
            mNetworkedSuitControlConnection.ConnectionStateChangeEvent += SuitConnectionChangeHandler;
            mDispatchRouter = new ProtobuffDispatchRouter();
            RegisterProtobufEvents();
        }

        /// <summary>
        /// Handler for suit change events
        /// </summary>
        /// <param name="vObj"></param>
        private void SuitConnectionChangeHandler(BrainpackConnectionStateChange vObj)
        {
            mBrainpackModel.Point = new IPEndPoint(mNetworkedSuitControlConnection.SuitIp, mNetworkedSuitControlConnection.Port);
            if (BrainpackConnectionStateChange != null)
            {
                BrainpackConnectionStateChange(vObj, mBrainpackModel);
            }
        }

        /// <summary>
        /// Sets the model of the suit connection manager. Warning: closes existing connections
        /// </summary>
        /// <param name="vModel"></param>
        public void SetBrainpack(BrainpackNetworkingModel vModel)
        {
            if (mNetworkSuitUdpConnection != null)
            {
                mNetworkSuitUdpConnection.Dispose();
            }

            mBrainpackModel = vModel;
        }

        /// <summary>
        /// Registers protobuffer events
        /// </summary>
        private void RegisterProtobufEvents()
        {
            mDispatchRouter.Add(PacketType.StatusResponse, SetBrainpackStatus);
            mDispatchRouter.Add(PacketType.DataFrame, ProcessDataFrame);
            mDispatchRouter.Add(PacketType.StatusResponse, StatusResponseHandler);
        }

        /// <summary>
        /// Status response handler
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vVargs"></param>
        private void StatusResponseHandler(object vVsender, object vVargs)
        {
            if (StatusResponseEvent != null)
            {
                Packet vPacket = (Packet)vVargs;
                StatusResponseEvent(vPacket);
            }
        }


        /// <summary>
        /// Process the incoming data frame
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vArgs"></param>
        private void ProcessDataFrame(object vVsender, object vArgs)
        {
            Packet vPacket = (Packet)vArgs;
            if (vPacket.fullDataFrame.reportTypeSpecified)
            {
                if (ConcerningReportIncludedEvent != null)
                {
                    ConcerningReportIncludedEvent(vPacket.fullDataFrame.reportType == ReportType.pain, vPacket);
                }
            }
            if (ImuDataFrameReceivedEvent != null)
            {
                ImuDataFrameReceivedEvent(vPacket);
            }
        }


        /// <summary>
        /// Requests a data stream from the brainpack. 
        /// </summary>
        /// <param name="vUdpPort">The Udp port to listen on  </param>
        /// <param name="vIpAddress"></param>
        public void RequestDataStreamFromBrainpack(int vUdpPort, string vIpAddress = null)
        {
            if (string.IsNullOrEmpty(vIpAddress))
            {
                //get the ip address that is in the same subnet as the brainpack's endpoint
                vIpAddress = GetIpAddressInRangeOfBrainpackAddress(mBrainpackModel.TcpIpEndPoint);
            }

            ListenToSuitOnUdp(vUdpPort, vIpAddress);
            Packet vProtoPacket = new Packet();
            Endpoint vProtoEndpoint = new Endpoint();
            vProtoPacket.type = PacketType.StartDataStream;
            vProtoPacket.recordingFilename = "defult";
            vProtoPacket.recordingFilenameSpecified = true;
            vProtoPacket.sensorMask = 0x1FF; //all sensors
            vProtoPacket.sensorMaskSpecified = true;
            vProtoPacket.recordingRateSpecified = true;
            vProtoPacket.recordingRate = 20;
            vProtoEndpoint.address = vIpAddress;
            vProtoEndpoint.port = (uint)vUdpPort;
            vProtoPacket.endpoint = vProtoEndpoint;
            mNetworkedSuitControlConnection.Send(vProtoPacket);
        }

        /// <summary>
        /// Listen to suit on UDP port. Disconnects an  Note: will throw an exception if the port is already in use by another process. 
        /// </summary>
        /// <param name="vUdpPort"></param>
        private void ListenToSuitOnUdp(int vUdpPort, string vIpAddress)
        {
            //set up the udp listener before sending a request to create a data stream start
            if (mNetworkSuitUdpConnection == null)
            {
                mNetworkSuitUdpConnection = new NetworkedSuitUdpConnection(vIpAddress);
                mNetworkSuitUdpConnection.DataReceivedEvent += UdpDataReceived;
            }
            mNetworkSuitUdpConnection.StartListen(vUdpPort);
        }

        /// <summary>
        /// Udp data received event.
        /// </summary>
        /// <param name="vPacket"></param>
        private void UdpDataReceived(Packet vPacket)
        {
            mDispatchRouter.Process(vPacket.type, this, vPacket);
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
            mBrainpackModel.Version = vBrainpackVersion;
            if (BrainpackUpdateEvent != null)
            {
                BrainpackUpdateEvent(vPacket);
            }
        }

        /// <summary>
        /// Starts a connection to the suit via the given BrainpackNetworkingModel
        /// </summary> 
        public bool ConnectToSuitControlSocket(BrainpackNetworkingModel vModel)
        {
            SetBrainpack(vModel);
            return SuitControlConnection.StartConnection(vModel.TcpIpEndPoint, vModel.TcpControlPort);
        }

        private string GetCurrIpAddress()
        {
            var vHost = Dns.GetHostEntry(Dns.GetHostName());
            string vCurrentEndPoint = "";
            foreach (var vIp in vHost.AddressList)
            {
                if (vIp.AddressFamily == AddressFamily.InterNetwork)
                {
                    vCurrentEndPoint = vIp.ToString();
                    break;
                }
            }
            return vCurrentEndPoint;
        }

        private string GetIpAddressInRangeOfBrainpackAddress(string vBrainpackAddress)
        {
            var vBpAdd = IPAddress.Parse(vBrainpackAddress);

            var vHost = Dns.GetHostEntry(Dns.GetHostName());
            string vCurrentEndPoint = "";
            foreach (var vIp in vHost.AddressList)
            {
                if (vIp.AddressFamily == AddressFamily.InterNetwork)
                {
                    var vSubNet = IPAddressExtensions.GetSubnetMask(vIp);
                    if (vIp.IsInSameSubnet(vBpAdd, vSubNet))
                    {
                        vCurrentEndPoint = vIp.ToString();
                    }

                }
            }
            return vCurrentEndPoint;
        }


        /// <summary>
        /// Begins the async file transfer process
        /// </summary>
        public void UpdateFirmware(string vFileName)
        {
            Packet vPacket = new Packet();
            vPacket.type = PacketType.UpdateFirmwareRequest;
            vPacket.firmwareUpdate = new FirmwareUpdate();
            vPacket.firmwareUpdate.fwEndpoint = new Endpoint();
            string vCurrentEndPoint = GetCurrIpAddress();
            vPacket.firmwareUpdate.fwEndpoint.address = vCurrentEndPoint;
            vPacket.firmwareUpdate.fwEndpoint.port = (uint)ApplicationSettings.TftpPort;
            vPacket.firmwareUpdate.fwFilename = vFileName;
            //Start the firmware update manager before sending the request to update the firmware
            if (mFirmwareUpdater == null)
            {
                mFirmwareUpdater = new FirmwareUpdateManager(FirmwareLocationPath, vCurrentEndPoint, ApplicationSettings.TftpPort);
            }
            mNetworkedSuitControlConnection.Send(vPacket);
        }

        public void SendPacket(Packet vPacket)
        {
            mNetworkedSuitControlConnection.Send(vPacket);
        }


        /// <summary>
        /// Clean up existing  connection
        /// </summary>
        public void CleanUp()
        {
            if (mFirmwareUpdater != null)
            {
                mFirmwareUpdater.CleanUp();
            }
            if (mNetworkSuitUdpConnection != null)
            {
                mNetworkSuitUdpConnection.Dispose();
            }
            if (SuitControlConnection != null)
            {
                SuitControlConnection.ConnectionStateChangeEvent -= SuitConnectionChangeHandler;
                SuitControlConnection.Dispose();
            }
       }

        /// <summary>
        /// Send a request to the retrieve the state of the suit. 
        /// </summary>
        public void RequestSuitStatus()
        {
            Packet vPacket = new Packet();
            vPacket.type = PacketType.StatusRequest;
            mNetworkedSuitControlConnection.Send(vPacket);
        }

        /// <summary>
        /// Send a request to the retrieve the state of the suit. 
        /// </summary>
        public void RequestStreamFromBrainpackStop()
        {
            Packet vPacket = new Packet();
            vPacket.type = PacketType.StopDataStream;
            mNetworkedSuitControlConnection.Send(vPacket);
        }

    }
}