﻿// /**
// * @file NetworkedSuitConnection.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Communication.Controller;

namespace Assets.Scripts.Communication.Communicators
{
    public delegate void OnSuitDataReceivedEvent(StateObject vObject, byte[] vData);
    public class NetworkedSuitConnection : IDisposable
    {

        public event OnSuitDataReceivedEvent DataReceivedEvent;
        private object mLockIsConectedLock = new  object();
        private Socket mSocket;
        public int Port = 8845;
   

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
          //  IPHostEntry vLocalHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            try
            {
                IPHostEntry vEndPointHostEntry = Dns.Resolve(vIpEndPoint);
                IPAddress vIpAddress = vEndPointHostEntry.AddressList[0];
                IPEndPoint vRemoteEp = new IPEndPoint(vIpAddress, vPortNum);

                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //begin to connect to the suit
                mSocket.BeginConnect(vRemoteEp, new AsyncCallback(ConnectCallback), mSocket);
                //  EndPoint vServerEndpoint = new IPEndPoint(vHostEntry.AddressList[0], vPortNum);
                //  
                // mSocket.Connect(vServerEndpoint);
                //  StateObject vServerEndSocket = new StateObject();

                // vServerEndSocket.Socket = mSocket.EndConnect(fda)
                //mSocket.BeginReceive(vBuffer, 0, vBuffer.Length,
                // SocketFlags.None, new AsyncCallback(ReceiveCallback), vIncomingConnection);
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
                Socket vCurrentSocket = (Socket)vAr.AsyncState;
                //setup current socket
                vCurrentSocket.EndConnect(vAr);
                mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
            }
            catch (SocketException vSocketException)
            {

                string vMsg = "Line: 104 Error occured starting listener, check inner exception" + vSocketException + " code " + vSocketException.ErrorCode;
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
        public bool Send(byte[] vData )
        {
      
            if (mSocket != null )
            {
                lock (mSocket)
                {
                    mSocket.Send(vData, vData.Length, SocketFlags.None);
                }
            }
            else
            {
                return false;
            }
            return true;
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
                    if (DataReceivedEvent != null)
                    {
                        DataReceivedEvent(vIncomingConnection, vIncomingConnection.Buffer);
                    }
                    //get more data in case 
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
        public Socket Socket;
        public byte[] Buffer;
    }

}