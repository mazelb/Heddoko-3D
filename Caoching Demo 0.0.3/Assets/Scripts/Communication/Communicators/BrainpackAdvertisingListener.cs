// /**
// * @file BrainpackAdvertisingListener.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using Assets.Scripts.Utils.DebugContext.logging;
using heddoko;
using HeddokoLib.heddokoProtobuff.Decoder;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using ProtoBuf;
using Timer = System.Timers.Timer;

namespace Assets.Scripts.Communication.Communicators
{
    public delegate void BrainpackFound(BrainpackNetworkingModel vBrainpack);

    public delegate void BrainpackLost(BrainpackNetworkingModel vBrainpack);
    public class BrainpackAdvertisingListener
    {
        private UdpClient mClient;
        private int mPort;
        private event BrainpackFound BrainpackFoundEvent;
        private event BrainpackLost BrainpackLostEvent;
        private double mTimer;
        private Timer mBrainpackTimer;
        Dictionary<string, BrainpackItemStructure> mFoundBrainpacks = new Dictionary<string, BrainpackItemStructure>();

        /// <summary>
        /// Instatiates a Brainpack Advertising listener and sets the alive time for brainpacks lists
        /// </summary>
        /// <param name="vTimer">interval timer in seconds</param>
        public BrainpackAdvertisingListener(double vTimer)
        {
            mTimer = vTimer;
            mBrainpackTimer = new Timer(10000);
            mBrainpackTimer.AutoReset = true;
            mBrainpackTimer.Elapsed += VerifyBrainpackCallTime;
            mBrainpackTimer.Start();
        }

        public void RegisterBrainpackFoundEventHandler(BrainpackFound vHandler)
        {
            BrainpackFoundEvent += vHandler;
        }

        public void RemoveBrainpackFoundEventHandler(BrainpackFound vHandler)
        {
            BrainpackFoundEvent -= vHandler;
        }
        public void RegisterBrainpackLostEventHandler(BrainpackLost vHandler)
        {
            BrainpackLostEvent += vHandler;
        }


        public void RemoveBrainpackLostEventHandler(BrainpackLost vHandler)
        {
            BrainpackLostEvent -= vHandler;
        }
        
        private void VerifyBrainpackCallTime(object vSender, ElapsedEventArgs vE)
        {
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand,"verifying brainpack calltime");
            var vTimeNow = DateTime.Now;
            //indices of brainpacks that need to be removed
            List<string> vKeysToRemoves = new List<string>();
            foreach (var vFoundBrainpack in mFoundBrainpacks)
            {
                var vDiff = vTimeNow.Subtract(vFoundBrainpack.Value.LastTimeFound).TotalSeconds;
                if (vDiff >= mTimer)
                {
                    vKeysToRemoves.Add(vFoundBrainpack.Key);
                }
            }
            for (int vI = 0; vI < vKeysToRemoves.Count; vI++)
            {
                var vRemoved = mFoundBrainpacks[vKeysToRemoves[vI]];
                mFoundBrainpacks.Remove(vKeysToRemoves[vI]);
                if (BrainpackLostEvent != null)
                {
                    DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "lost a brainpack Id: "+ vRemoved.BrainpackModel.Id);

                    BrainpackLostEvent(vRemoved.BrainpackModel);
                }
            }
        }


        public void StartListener(int vPort)
        {
            mPort = vPort;
            IPEndPoint vEp = new IPEndPoint(IPAddress.Any, vPort);
            mClient = new UdpClient(mPort);
            UdpState vState = new UdpState();

            vState.Client = mClient;
            vState.EndPoint = vEp;
            mClient.BeginReceive(new AsyncCallback(OnReceive), vState);
        }

        private void OnReceive(IAsyncResult vAr)
        {
            UdpState vIncomingConnection = (UdpState)vAr.AsyncState;
            IPEndPoint vEndpoint = new IPEndPoint(IPAddress.Any,  mPort);
            try
            {
                byte[] vBuffer = vIncomingConnection.Client.EndReceive(vAr, ref vEndpoint);
                int vBytesRead = vBuffer.Length;
                if (vBytesRead > 0)
                {
                    //invoke data received event 
                    //add the bytes to the state object's raw packet
                    PacketStatus vPacketStatus = PacketStatus.Processing;
                    for (int i = 0; i < vBytesRead; i++)
                    {
                        vPacketStatus = vIncomingConnection.IncomingRawPacket.ProcessByte(vBuffer[i]);
                        if (vPacketStatus == PacketStatus.PacketComplete)
                        {
                            if (BrainpackFoundEvent != null)
                            {
                                //has been processed and is ready to be processed internally.
                                //clear out buffer and state objects raw packet.
                                RawPacket vDeepCopy = new RawPacket(vIncomingConnection.IncomingRawPacket);
                                //deserialize the packet 
                                MemoryStream vMemorySteam = new MemoryStream();
                                if (vDeepCopy.Payload[0] == 0x04)
                                {
                                    //reset the stream pointer, write and reset.
                                    vMemorySteam.Seek(0, SeekOrigin.Begin);
                                    vMemorySteam.Write(vDeepCopy.Payload, 1, (int) vDeepCopy.PayloadSize - 1);
                                    vMemorySteam.Seek(0, SeekOrigin.Begin);
                                    Packet vProtoPacket = Serializer.Deserialize<Packet>(vMemorySteam);
                                    var vMsgType = vProtoPacket.type;
                                    if (vMsgType == PacketType.AdvertisingPacket)
                                    {
                                        //get the serial number of the advertising brainpack
                                        if (!mFoundBrainpacks.ContainsKey(vProtoPacket.serialNumber))
                                        {
                                            BrainpackNetworkingModel vBp = new BrainpackNetworkingModel();
                                            vBp.Version = vProtoPacket.firmwareVersion;
                                            vBp.Id = vProtoPacket.serialNumber;
                                            vBp.Point = vEndpoint;
                                            vBp.TcpControlPort = (int) vProtoPacket.configurationPort;
                                            vBp.TcpIpEndPoint = vEndpoint.Address.ToString();
                                            if (BrainpackFoundEvent != null)
                                            {
                                                BrainpackFoundEvent(vBp);
                                            }
                                            BrainpackItemStructure vItemStruct = new BrainpackItemStructure();
                                            vItemStruct.LastTimeFound = DateTime.Now;
                                            vItemStruct.BrainpackModel = vBp;
                                            mFoundBrainpacks.Add(vBp.Id, vItemStruct);
                                        }
                                        else
                                        {
                                            mFoundBrainpacks[vProtoPacket.serialNumber].LastTimeFound = DateTime.Now;
                                        }
                                    }

                                }
                            }
                            if (vPacketStatus == PacketStatus.PacketError)
                            {
                                vIncomingConnection.IncomingRawPacket.Clear();
                            }
                        }
                    }
                    mClient.BeginReceive(new AsyncCallback(OnReceive), vIncomingConnection);
                }
            }

            catch
                (SocketException vE)
            {
                string vMsg = "Exception caughtin advertising listener :" + vE;
                vMsg += ";";
                if (vE.InnerException != null)
                {
                    vMsg += vE.InnerException;
                }
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, vMsg);
                UnityEngine.Debug.Log("error in bp advertising listener" + vE.Message);
            }
            catch (Exception vE)
            {
                string vMsg = "Socket Exception caughtin advertising listener :" + vE;
                vMsg += ";";
                if (vE.InnerException != null)
                {
                    vMsg += vE.InnerException;
                }
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, vMsg);
            }

        }



        public void StopListening()
        {
            try
            {
                mClient.Close();
            }
            catch (Exception vE)
            {
                string vMsg = "Socket Exception caughtin advertising listener :" + vE;
                vMsg += ";";
                if (vE.InnerException != null)
                {
                    vMsg += vE.InnerException;
                }
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, vMsg);
            }


        }

        /// <summary>
        /// A class that references a found brainpack and the last time it has sent out a heartbeat
        /// </summary>
        private class BrainpackItemStructure
        {
            public DateTime LastTimeFound;
            public BrainpackNetworkingModel BrainpackModel;
        }
    }

    public class UdpState
    {
        public UdpClient Client;
        public IPEndPoint EndPoint;
        public RawPacket IncomingRawPacket = new RawPacket();

    }
}