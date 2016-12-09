// /**
// * @file NetworkedSuitUdpConnection.cs
// * @brief Contains the  NetworkedSuitUdpConnection class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Scripts.Utils.DebugContext.logging;
using heddoko;
using HeddokoLib.heddokoProtobuff.Decoder;
using ProtoBuf;
using UnityEngine;
using LogType = Assets.Scripts.Utils.DebugContext.logging.LogType;

namespace Assets.Scripts.Communication.Communicators
{
    public delegate void PacketReceivedEvent(Packet packet);
    /// <summary>
    /// Using Udp, listen to an incoming stream from a brainpack
    /// </summary>
    public class NetworkedSuitUdpConnection : IDisposable
    {
        public event PacketReceivedEvent DataReceivedEvent;
        private UdpClient mListener;
        public int Port = 0;
        public IPAddress SuitIp;
        private string mIpAddress;
        private bool mSuccesfullyInitialized = false;
        /// <summary>
        /// Start an instance of a networked suit udp connection with a given ip address to listen to 
        /// </summary>
        /// <param name="vIpAddress"></param>
        public NetworkedSuitUdpConnection(string vIpAddress)
        {
            mIpAddress = vIpAddress;
        }

        /// <summary>
        /// is the current connection succesfully initialized?
        /// </summary>
        public bool SuccesfullyInitialized
        {
            get { return mSuccesfullyInitialized; }
        }

        /// <summary>
        /// begin listening on the specified port.
        /// </summary>
        /// <param name="vPort"></param>
        public void StartListen(int vPort)
        {
            try
            {
                if (mListener != null)
                {
                    if (vPort != Port)
                    {
                        mListener.Close();
                    }
                    if (vPort == Port && mSuccesfullyInitialized)
                    {
                        return;
                    }
                }
                if (mThread != null && mProcessBytes)
                {
                    try
                    {
                        mProcessBytes = false;
                        mThread.Abort();

                    }
                    catch (ThreadAbortException vE)
                    {

                    }
                }
                Port = vPort;
                UdpState vState = new UdpState();
                var vIpAdd = IPAddress.Parse(mIpAddress);
                IPEndPoint vEndpoint = new IPEndPoint(vIpAdd, Port);
                vState.IncomingRawPacket = new RawPacket();
                vState.EndPoint = vEndpoint;
                mListener = new UdpClient(Port);
                vState.Client = mListener;
                mListener.BeginReceive(new AsyncCallback(ReceiveCallback), vState);
                mSuccesfullyInitialized = true;
                mProcessBytes = true;
             
                mThread = new Thread(WorkerFunc);
                mThread.IsBackground = true;
                mThread.Start();
            }
            catch (ArgumentOutOfRangeException vException)
            {
                mSuccesfullyInitialized = false;
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "exception thrown in udp suit connection : Line: 70 ServerListener_StartServer_Port_number_is_invalid" + vException);
            }
            catch (SocketException vException)
            {
                mSuccesfullyInitialized = false;
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "exception thrown in udp suit connection :Line: 75 Could not create socket, check to make sure that port is not being used by another socket " + vException);

                throw;
            }
            catch (Exception vException)
            {
                mSuccesfullyInitialized = false;
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "exception thrown in udp suit connection :Line: 81 Error occured while binding socket, check inner exception" + vException);

            }

        }

        private void ReceiveCallback(IAsyncResult vAr)
        {
            try
            {
                UdpState vIncomingConnection = (UdpState)vAr.AsyncState;
                var vClient = vIncomingConnection.Client;
                byte[] vBuffer = vClient.EndReceive(vAr, ref vIncomingConnection.EndPoint);
                //Process message
                int vBytesRead = vBuffer.Length;
                if (vBytesRead > 0)
                {
                    byte[] vBytes = new byte[vBuffer.Length];
                    Array.Copy(vBuffer, vBytes, vBuffer.Length);
                    mQueueByte.Enqueue(vBytes);
                    vIncomingConnection.Client.BeginReceive(new AsyncCallback(ReceiveCallback), vIncomingConnection);
                }
            }
            catch (Exception vE)
            {
                string vmsg = vE.Message;
                Debug.Log(vmsg);
            }
        }

        private ConcurrentQueue<byte[]> mQueueByte = new ConcurrentQueue<byte[]>();
        private Thread mThread;
        private bool mProcessBytes = false;
        private void WorkerFunc()
        {
            while (mProcessBytes)
            {
                try
                {
                    byte[] vByteBuffer;
                    mQueueByte.TryDequeue(out vByteBuffer);
                    if (vByteBuffer == null)
                    {
                        continue;
                    }
                    PacketStatus vPacketStatus = PacketStatus.Processing;
                    RawPacket vRawPacket = new RawPacket();
                    for (int i = 0; i < vByteBuffer.Length; i++)
                    {
                        vPacketStatus = vRawPacket.ProcessByte(vByteBuffer[i]);
                        if (vPacketStatus == PacketStatus.PacketComplete)
                        {
                            if (DataReceivedEvent != null)
                            {
                                //has been processed and is ready to be processed internally.
                                //clear out buffer and state objects raw packet.
                                RawPacket vDeepCopy = new RawPacket(vRawPacket);
                                //deserialize the packet 
                                MemoryStream vMemorySteam = new MemoryStream();
                                if (vDeepCopy.Payload[0] == 0x04)
                                {
                                    //reset the stream pointer, write and reset.
                                    vMemorySteam.Seek(0, SeekOrigin.Begin);
                                    vMemorySteam.Write(vDeepCopy.Payload, 1, (int)vDeepCopy.PayloadSize - 1);
                                    vMemorySteam.Seek(0, SeekOrigin.Begin);
                                    Packet vProtoPacket = Serializer.Deserialize<Packet>(vMemorySteam);
                                    DataReceivedEvent(vProtoPacket);
                                }
                            }
                            if (vPacketStatus == PacketStatus.PacketError)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception VE)
                {

                    DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Error in udp thread " + VE.Message);
                }
            }
        }
        public void Dispose()
        {
            mProcessBytes = false;
            if (mListener != null)
            {

                mListener.Close();
            }
        }
    }
}