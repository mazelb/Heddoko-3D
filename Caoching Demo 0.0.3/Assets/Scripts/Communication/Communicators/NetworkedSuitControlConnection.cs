// /**
// * @file NetworkedSuitControlConnection.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
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
    public delegate void OnSuitDataReceivedEvent(StateObject vObject);


    public delegate void OnSuitConnectionEvent();
    public class NetworkedSuitControlConnection : IDisposable
    {

        public event OnSuitDataReceivedEvent DataReceivedEvent;
        public event OnSuitConnectionEvent SuitConnectionEvent;
        private object mLockIsConectedLock = new object();
        private Socket mSocket;
        public int Port = 8845;
        public IPAddress SuitIp;

        /// <summary>
        /// Is the suit currently connected?
        /// </summary>
        public bool Connected
        {
            get
            {
                lock (mLockIsConectedLock)
                {
                    if (mSocket == null)
                    {
                        return false;
                    }
                    return mSocket.Connected;
                }
            }
        }


        /// <summary>
        /// Start the network connection to the suit
        /// </summary>
        public bool Start(string vIpEndPoint, int vPortNum = 8844)
        {
            try
            {
                Port = vPortNum;
                IPAddress vIpAddress = IPAddress.Parse(vIpEndPoint);
                SuitIp = vIpAddress;
                IPEndPoint vRemoteEp = new IPEndPoint(vIpAddress, Port);

                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //begin to connect to the suit
                mSocket.BeginConnect(vRemoteEp, new AsyncCallback(ConnectCallback), mSocket);
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
            return true;
        }

        /// <summary>
        /// Async callback on first connection
        /// </summary>
        /// <param name="vAr"></param>
        private void ConnectCallback(IAsyncResult vAr)
        {
            //Register the suit's end point socket
            try
            {
                Socket vSocket = (Socket)vAr.AsyncState;
                //setup current socket

                StateObject vSocketConnection = new StateObject();
                vSocketConnection.Socket = vSocket;
                vSocketConnection.Buffer = new byte[1024];
                vSocket.EndConnect(vAr);
                if (SuitConnectionEvent != null)
                {
                    SuitConnectionEvent();
                }
                vSocket.BeginReceive(vSocketConnection.Buffer, 0, vSocketConnection.Buffer.Length,
                                   SocketFlags.None, new AsyncCallback(ReceiveCallback), vSocketConnection);
            }
            catch (SocketException vSocketException)
            {

                string vMsg = "Line: 103 Error beggining accept, check inner exception" + vSocketException + " code " + vSocketException.ErrorCode;
                string vInnerExceptionMsg = "";
                if (vSocketException.InnerException != null)
                {
                    vInnerExceptionMsg = vSocketException.InnerException.ToString();
                }
                UnityEngine.Debug.Log(vMsg + " \r\n" + vInnerExceptionMsg);
            }
            catch (Exception vE)
            {
                string vMsg = "Line: 114  Error occured starting listener, check inner exception" + vE;
                string vInnerExceptionMsg = "";
                vMsg += vE.Message;
                vMsg += "\r\n" + vE.GetBaseException();
                if (vE.InnerException != null)
                {
                    vInnerExceptionMsg = vE.InnerException.ToString();
                }
                UnityEngine.Debug.Log(vMsg + " \r\n" + vInnerExceptionMsg);
                //throw new ApplicationException("Error occured starting listener, check inner exception", vE);
            }

            //on start connection begin to listen to incoming data

        }

        private void AcceptCallback(IAsyncResult vAr)
        {
            StateObject vIncomingConnection = new StateObject();
            try
            {
                Socket vSocket = (Socket)vAr.AsyncState;
                vIncomingConnection.Socket = vSocket.EndAccept(vAr);
                byte[] vBuffer = new byte[1024];

                vIncomingConnection.Socket.BeginReceive(vBuffer, 0, vBuffer.Length,
                    SocketFlags.None, new AsyncCallback(ReceiveCallback), vIncomingConnection);
                mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
            }
            catch (SocketException vException)
            {
                CloseAndRemoveStateObject(vIncomingConnection);
                //Begin receiving data in case there is more data to be received. 
                mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
            }
        }

        /// <summary>
        /// Sends data to the suit
        /// </summary>
        /// <param name="vData"></param>
        /// <param name="vStateObject"></param>
        /// <returns></returns>
        public bool Send(byte[] vData, object vStateObject)
        {
            StateObject vConnection = (StateObject)vStateObject;
            if (vConnection != null && vConnection.Socket.Connected)
            {
                lock (vConnection.Socket)
                {
                    vConnection.Socket.Send(vData, vData.Length, SocketFlags.None);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool Send(Packet vPacket)
        {
            MemoryStream vStream = new MemoryStream();
            Serializer.Serialize(vStream, vPacket);
            RawPacket vRawPacket = new RawPacket();
            int vRawSize;
            var vRawBytes = vRawPacket.GetRawPacketByteArray(out vRawSize, vStream);
            return Send(vRawBytes, vRawSize);
        }
        public bool Send(byte[] vData, int vBuffersize)
        {

            if (mSocket != null)
            {
                lock (mSocket)
                {
                    mSocket.Send(vData, vBuffersize, SocketFlags.None);

                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private void SendCallbackBack(IAsyncResult vAr)
        {
        }


        private void ReceiveCallback(IAsyncResult vAr)
        {
            StateObject vIncomingConnection = (StateObject)vAr.AsyncState;
            try
            {
                int vBytesRead = vIncomingConnection.Socket.EndReceive(vAr);
                if (vBytesRead > 0)
                {
                    //invoke data received event 

                    //add the bytes to the state object's raw packet
                    PacketStatus vPacketStatus = PacketStatus.Processing;
                    for (int i = 0; i < vIncomingConnection.Buffer.Length; i++)
                    {
                        vPacketStatus = vIncomingConnection.IncomingRawPacket.ProcessByte(vIncomingConnection.Buffer[i]);
                        if (vPacketStatus == PacketStatus.PacketComplete)
                        {
                            if (DataReceivedEvent != null)
                            {
                                //todo: deep copy the packet, and send out an event that a raw packet
                                //has been processed and is ready to be processed internally.
                                //clear out buffer and state objects raw packet.
                                RawPacket vDeepCopy = new RawPacket(vIncomingConnection.IncomingRawPacket);
                                vIncomingConnection.OutgoingRawPacket = vDeepCopy;
                                vIncomingConnection.IncomingRawPacket.Clear();
                                DataReceivedEvent(vIncomingConnection);

                            }
                        }
                        if (vPacketStatus == PacketStatus.PacketError)
                        {
                            vIncomingConnection.IncomingRawPacket.Clear();
                        }
                    }

                    vIncomingConnection.Socket.BeginReceive(vIncomingConnection.Buffer, 0,
                        vIncomingConnection.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback),
                        vIncomingConnection);
                }
            }
            catch (SocketException vE)
            {
                CloseAndRemoveStateObject(vIncomingConnection);
            }
        }

        /// <summary>
        /// Close StateObject
        /// </summary>
        /// <param name="vIncomingConnection"></param>
        private void CloseAndRemoveStateObject(StateObject vIncomingConnection)
        {
            if (vIncomingConnection.Socket != null)
            {
                vIncomingConnection.Socket.Shutdown(SocketShutdown.Both);
                vIncomingConnection.Socket.Close();
            }
        }


        public void Dispose()
        {
            CloseCurrentSocket();
        }
        /// <summary>
        /// Closes and shuts down the current client socket
        /// </summary>
        public void CloseCurrentSocket()
        {
            if (mSocket != null && (mSocket.Connected))
            {
                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();
            }
        }
    }

    public class StateObject
    {
        public const int gPacketSize = 1024;
        public Socket Socket;
        public byte[] Buffer = new byte[gPacketSize];
        public RawPacket IncomingRawPacket = new RawPacket();
        public RawPacket OutgoingRawPacket = new RawPacket();

        public void ClearIncomingData()
        {
            IncomingRawPacket.Clear();
            Buffer = new byte[gPacketSize];
        }
    }

}