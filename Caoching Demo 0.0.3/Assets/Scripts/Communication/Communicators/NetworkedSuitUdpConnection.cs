// /**
// * @file NetworkedSuitUdpConnection.cs
// * @brief Contains the  NetworkedSuitUdpConnection class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using heddoko;
using HeddokoLib.heddokoProtobuff.Decoder; 
using ProtoBuf;

namespace Assets.Scripts.Communication.Communicators
{
    public delegate  void PacketReceivedEvent(Packet packet);
    /// <summary>
    /// Using Udp, listen to an incoming stream from a brainpack
    /// </summary>
    public class NetworkedSuitUdpConnection : IDisposable
    {
        public event PacketReceivedEvent DataReceivedEvent;
         private UdpClient mListener;
        public int Port = 0;
        public IPAddress SuitIp;

        public void StartListen(int vPort)
        {
            try
            {
                Port = vPort;
                if (mListener != null)
                {
                    mListener.Close();
                }
                BrainpackAdvertisingListener.UdpState  vState = new BrainpackAdvertisingListener.UdpState();
                IPEndPoint vEndpoint = new IPEndPoint(IPAddress.Any, Port);
                vState.Client = mListener;
                vState.IncomingRawPacket = new RawPacket();
                vState.EndPoint = vEndpoint;
                mListener = new UdpClient(Port);

                mListener.BeginReceive(new AsyncCallback(ReceiveCallback), vState);

            }
            catch (ArgumentOutOfRangeException vException)
            {
                UnityEngine.Debug.Log("Line: 70 ServerListener_StartServer_Port_number_is_invalid" + vException);
                //throw new ArgumentOutOfRangeException(Resources.ServerListener_StartServer_Port_number_is_invalid, vArgument);
            }
            catch (SocketException vException)
            {
                UnityEngine.Debug.Log("Line: 75 Could not create socket, check to make sure that port is not being used by another socket " + vException);

                //   throw new ApplicationException("Could not create socket, check to make sure that port is not being used by another socket", vException);
            }
            catch (Exception vException)
            {
                UnityEngine.Debug.Log("Line: 81 Error occured while binding socket, check inner exception" + vException);
                // throw new ApplicationException("Error occured while binding socket, check inner exception", vException);
            }
             
        }

        private void ReceiveCallback(IAsyncResult vAr)
        {
            BrainpackAdvertisingListener.UdpState vIncomingConnection = (BrainpackAdvertisingListener.UdpState) vAr.AsyncState;
            var vClient = vIncomingConnection.Client;
            byte[] vBuffer = vClient.EndReceive(vAr, ref vIncomingConnection.EndPoint);
            //Process message
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
                        if (DataReceivedEvent != null)
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
                                vMemorySteam.Write(vDeepCopy.Payload, 1, (int)vDeepCopy.PayloadSize - 1);
                                vMemorySteam.Seek(0, SeekOrigin.Begin);
                                Packet vProtoPacket = Serializer.Deserialize<Packet>(vMemorySteam);
                                DataReceivedEvent(vProtoPacket);
                            }
                        }
                        if (vPacketStatus == PacketStatus.PacketError)
                        {
                            vIncomingConnection.IncomingRawPacket.Clear();
                        }
                    }
                }
                vIncomingConnection.Client.BeginReceive(new AsyncCallback(ReceiveCallback), vIncomingConnection);
            }
        }

        public void Dispose()
        {
            if (mListener != null)
            {
                mListener.Close();
            }
        }
    }
}