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
using Assets.Scripts.Utils.DebugContext.logging;
using heddoko;
using HeddokoLib.heddokoProtobuff.Decoder;
using ProtoBuf;

namespace Assets.Scripts.Communication.Communicators
{
    public delegate void OnSuitDataReceivedEvent(StateObject vObject);


    public delegate void OnSuitConnectionEvent();
    public class NetworkedSuitControlConnection
    {

        public event OnSuitDataReceivedEvent DataReceivedEvent;
        public event Action<BrainpackConnectionStateChange> ConnectionStateChangeEvent;
        private BrainpackConnectionState mPreviousState = BrainpackConnectionState.Disconnected;
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
        public bool StartConnection(string vIpEndPoint, int vPortNum = 8844)
        {
            try
            {
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Beginning connection");
                Port = vPortNum;
                IPAddress vIpAddress = IPAddress.Parse(vIpEndPoint);
                SuitIp = vIpAddress;
                IPEndPoint vRemoteEp = new IPEndPoint(SuitIp, Port);

                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //begin to connect to the suit
                mPreviousState = BrainpackConnectionState.Disconnected;
                InvokeStateChange(mPreviousState, BrainpackConnectionState.Connecting);
                mSocket.ReceiveTimeout = 10000;
                mSocket.SendTimeout = 10000;
                mSocket.BeginConnect(vRemoteEp, new AsyncCallback(ConnectCallback), mSocket);
            }
            catch (ArgumentOutOfRangeException vException)
            {
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "exception thrown in network suit control connection : Line: 74 ServerListener_StartServer_Port_number_is_invalid" + vException);
                //throw new ArgumentOutOfRangeException(Resources.ServerListener_StartServer_Port_number_is_invalid, vArgument);
            }
            catch (SocketException vException)
            {
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "exception thrown in network suit control connection : Line: 75 Could not create socket, check to make sure that port is not being used by another socke" + vException);

                //   throw new ApplicationException("Could not create socket, check to make sure that port is not being used by another socket", vException);
            }
            catch (Exception vException)
            {
                UnityEngine.Debug.Log("Line: 81 Error occured while binding socket, check inner exception" + vException);
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "exception thrown in network suit control connection : Line: 81 Error occured while binding socket, check inner exception" + vException);
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
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Connection callback initiated");

                Socket vSocket = (Socket)vAr.AsyncState;
                //setup current socket
                StateObject vSocketConnection = new StateObject();
                vSocketConnection.Socket = vSocket;
                vSocketConnection.Buffer = new byte[1024];
                mSocket.EndConnect(vAr);
                mPreviousState = BrainpackConnectionState.Disconnected;
                InvokeStateChange(BrainpackConnectionState.Disconnected, BrainpackConnectionState.Connected);

                mSocket.BeginReceive(vSocketConnection.Buffer, 0, vSocketConnection.Buffer.Length,
                                  SocketFlags.None, new AsyncCallback(ReceiveCallback), vSocketConnection);
            }
            catch (SocketException vSocketException)
            {

                string vMsg = "Line: 118 Error beggining accept, check inner exception" + vSocketException + " code " + vSocketException.ErrorCode;
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "exception thrown in network suit control connection : Line: 103 Error beggining accept, check inner exception" + vSocketException + " code " + vSocketException.ErrorCode);

                string vInnerExceptionMsg = "";
                if (vSocketException.InnerException != null)
                {
                    vInnerExceptionMsg = vSocketException.InnerException.ToString();
                }
                UnityEngine.Debug.Log(vMsg + " \r\n" + vInnerExceptionMsg);
                mPreviousState = BrainpackConnectionState.Disconnected;
                InvokeStateChange(BrainpackConnectionState.Disconnected, BrainpackConnectionState.Disconnected);
            }
            catch (Exception vE)
            {
                string vMsg = "Line: 133  Error occured starting listener, check inner exception" + vE;
                string vInnerExceptionMsg = "";
                vMsg += vE.Message;
                vMsg += "\r\n" + vE.GetBaseException();
                if (vE.InnerException != null)
                {
                    vInnerExceptionMsg = vE.InnerException.ToString();
                }
                UnityEngine.Debug.Log(vMsg + " \r\n" + vInnerExceptionMsg);
                InvokeStateChange(BrainpackConnectionState.Disconnected, BrainpackConnectionState.Disconnected);
                mPreviousState = BrainpackConnectionState.Disconnected;
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "exception thrown in network suit control connection : " + vMsg + " \r\n" + vInnerExceptionMsg);
            } 
        }

        /// <summary>
        /// Invokes a connection state change event
        /// </summary>
        /// <param name="vOld"></param>
        /// <param name="vNew"></param>
        private void InvokeStateChange(BrainpackConnectionState vOld, BrainpackConnectionState vNew)
        {
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "control connection : invoking suit change<old,new>" + vOld + "," + vNew);

            if (ConnectionStateChangeEvent != null)
            {
                BrainpackConnectionStateChange vState = new BrainpackConnectionStateChange();
                vState.NewState = vNew;
                vState.OldState = vOld;
                ConnectionStateChangeEvent(vState);

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
                    try
                    {
                        vConnection.Socket.Send(vData, vData.Length, SocketFlags.None);
                    }
                    catch (Exception vE)
                    {
                        DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "control connection : Problem sending byte: " + vData.Length);
                        mPreviousState = BrainpackConnectionState.Connected;
                        InvokeStateChange(mPreviousState, BrainpackConnectionState.Disconnected);
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public void Send(Packet vPacket)
        {
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Seding packet of type "+vPacket.type);
            MemoryStream vStream = new MemoryStream();
            Serializer.Serialize(vStream, vPacket);
            RawPacket vRawPacket = new RawPacket();
            int vRawSize;
            var vRawBytes = vRawPacket.GetRawPacketByteArray(out vRawSize, vStream);
              Send(vRawBytes, vRawSize  );
        }
        public void Send(byte[] vData, int vBuffersize)
        {

            if (mSocket != null)
            {
                lock (mSocket)
                {
                    try
                    {
                        mSocket.BeginSend(vData, 0, vBuffersize, 0, new AsyncCallback(SendCallback),mSocket);
                      //  mSocket.Send(vData, vBuffersize, SocketFlags.None);
                    }
                    catch (SocketException vE)
                    {
                        string vMsg = ReturnErrorString(vE);
                        DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "control connection Socket exception: Problem sending byte: " + vData.Length + " buffer size input " + vBuffersize);
                        DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, vMsg);
                    }
                    catch (Exception vE)
                    {
                        string vMsg = ReturnErrorString(vE);
                        DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "control connection : Problem sending byte: " + vData.Length + " buffer size input " + vBuffersize);
                        DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, vMsg);
                        //    InvokeStateChange(mPreviousState, BrainpackConnectionState.Disconnected);
                        //      mPreviousState = BrainpackConnectionState.Connected;
                    }
                }
            }
           
        }

        private void SendCallback(IAsyncResult vAr)
        {
            try
            {
                Socket vSo = (Socket)vAr.AsyncState;

                int send = vSo.EndSend(vAr);
            }
            catch (Exception vE)
            {
                string vMsg = ReturnErrorString(vE);
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand,"control connection on send callback: "+ vMsg);
            }
           
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
                string vMsg = ReturnErrorString(vE);
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "control connection : error on receive  " + vMsg);
                CloseAndRemoveStateObject(vIncomingConnection);
            }
            catch (Exception vE)
            {
                string vMsg = ReturnErrorString(vE);
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "control connection : error on receive  " + vMsg);

                CloseAndRemoveStateObject(vIncomingConnection);
            }
        }

        /// <summary>
        /// Close StateObject
        /// </summary>
        /// <param name="vIncomingConnection"></param>
        private void CloseAndRemoveStateObject(StateObject vIncomingConnection)
        {
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "control connection : closing connection  ");
            try
            {
                if (vIncomingConnection.Socket != null)
                {
                    vIncomingConnection.Socket.Shutdown(SocketShutdown.Both);
                    vIncomingConnection.Socket.Close();
                    mPreviousState = BrainpackConnectionState.Connected;
                    InvokeStateChange(mPreviousState, BrainpackConnectionState.Disconnected);

                }
            }
            catch (Exception vE)
            {
                string vMsg = ReturnErrorString(vE);
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "control connection : error on closing connection  " + vMsg);

            }
        }

        private static string ReturnErrorString(Exception vE)
        {
            string vMsg = vE.Message;
            if (vE.InnerException != null)
            {
                vMsg += vE.InnerException;
            }
            return vMsg;
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
                mPreviousState = BrainpackConnectionState.Connected;
                InvokeStateChange(mPreviousState, BrainpackConnectionState.Disconnected);
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