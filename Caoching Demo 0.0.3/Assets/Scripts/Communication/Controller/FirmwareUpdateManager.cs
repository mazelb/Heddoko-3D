// /**
// * @file FirmwareUpdateManager.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Assets.Scripts.Communication.Communicators;

namespace Assets.Scripts.Communication.Controller
{

    /// <summary>
    /// A firmware update manager
    /// </summary>
    public class FirmwareUpdateManager: IDisposable
    {
        public int Port = 8846;
        private TcpListener mListener;
        private TcpClient mClient;
        private Socket mSocket;
        private string mFilePath;
       public  bool Start(string vFilePath)
        {
            //open a socket  System.Net.IPEndPoint vServerEndPoint
            mFilePath = vFilePath;
            try
            {
                IPHostEntry vLocalHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                System.Net.IPEndPoint vServerEndPoint = null;
                foreach (var vAddress in vLocalHost.AddressList)
                {
                    if (vAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        vServerEndPoint = new IPEndPoint(vAddress, Port);
                    }
                }

                mSocket = new Socket(vServerEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mSocket.Bind(vServerEndPoint);
                mSocket.Listen(1);
             }
            catch (ArgumentOutOfRangeException vException)
            {
                UnityEngine.Debug.Log("ServerListener_StartServer_Port_number_is_invalid" + vException);
                return false;
                //throw new ArgumentOutOfRangeException(Resources.ServerListener_StartServer_Port_number_is_invalid, vArgument);
            }
            catch (SocketException vException)
            {
                UnityEngine.Debug.Log("Could not create socket, check to make sure that port is not being used by another socket " + vException);
                return false;
                //   throw new ApplicationException("Could not create socket, check to make sure that port is not being used by another socket", vException);
            }
            catch (Exception vException)
            {
                UnityEngine.Debug.Log("Error occured while binding socket, check inner exception" + vException);
                return false;
                // throw new ApplicationException("Error occured while binding socket, check inner exception", vException);
            }
            try
            {
                mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
            }
            catch (Exception vE)
            {
                UnityEngine.Debug.Log("Error occured starting listener, check inner exception" + vE);
                return false;
            }
           return true;
        }

        /// <summary>
        /// Accept callback
        /// </summary>
        /// <param name="vAr"></param>
        private void AcceptCallback(IAsyncResult vAr)
        {
            StateObject vIncomingConnection = new StateObject();
            try
            {
                 //finish accepting the connection
                Socket vSocket = (Socket)vAr.AsyncState;
                vIncomingConnection = new StateObject();
                vIncomingConnection.Socket = vSocket.EndAccept(vAr);
                vIncomingConnection.Buffer = new byte[1024];
               
                vIncomingConnection.Socket.BeginReceive(vIncomingConnection.Buffer, 0, vIncomingConnection.Buffer.Length,
                    SocketFlags.None, new AsyncCallback(ReceiveCallback), vIncomingConnection);
                mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
            }
            catch (SocketException vException)
            {
                CloseAndRemoveStateObject(vIncomingConnection);
                mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
            }
            catch (Exception vE)
            {
                CloseAndRemoveStateObject(vIncomingConnection);
                mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
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

        private void ReceiveCallback(IAsyncResult vAr)
        {
            StateObject vIncomingConnection = (StateObject)vAr.AsyncState;
            try
            {
                int vBytesRead = vIncomingConnection.Socket.EndReceive(vAr);
                if (vBytesRead > 0)
                {
                    //get message from the suit that is requesting a firmware update
                    string vMsg = Encoding.ASCII.GetString(vIncomingConnection.Buffer);
                    if (vMsg.Contains("GetLatestFirmware<EOL>"))
                    {
                        mSocket.BeginSendFile(mFilePath, new AsyncCallback(FileSendCallback), mSocket);
                    } 
              
                }
            }
            catch (SocketException vE)
            {
                CloseAndRemoveStateObject(vIncomingConnection);

            }
        }

        private void FileSendCallback(IAsyncResult vAr)
        {
            Socket vClient = (Socket)vAr.AsyncState;

            // Complete sending the data to the remote device.
            vClient.EndSendFile(vAr);
            mSocket.Shutdown(SocketShutdown.Both);
            mSocket.Close();
        }

        public void Dispose()
        {
            CloseCurrentSocket();
        }

        public void CloseCurrentSocket()
        {
            if (mSocket != null && (mSocket.Connected))
            {
                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();
            }
        }

    }
}