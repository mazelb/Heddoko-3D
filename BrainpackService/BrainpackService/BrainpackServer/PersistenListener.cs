
/** 
* @file PersistenListener.cs
* @brief Contains the PersistenListener class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using BrainpackService.Tools_and_Utilities;
using BrainpackService.Tools_and_Utilities.Debugging;
using HeddokoLib.networking;

namespace BrainpackService.BrainpackServer
{
    /// <summary>
    /// PersistenListener to handle multiple connection stream requests, with persistent connection
    /// </summary>
    public class PersistenListener: IBrainpackServer
    {
        private List<StateObject> mConnections = new List<StateObject>();
        private IPEndPoint mLocalEndpoint;
        private Socket mServerSocket;
        private int mBacklog = 10;
        public ServerCommandRouter ServerCommandRouter { get; set; }
        
        /// <summary>
        /// Starts listening
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            IPHostEntry vIpHostEntry = Dns.GetHostEntry("localhost");
            IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);//.AddressList[0];

            try
            {
                mLocalEndpoint = new IPEndPoint(vIpAddress, 11000);
            }
            catch (Exception vE)
            {
                BrainpackEventLogManager.InvokeEventLogMessage("Error creating local end point \n" + vE);
                DebugLogger.Instance.LogMessage(LogType.ServerSocketException, "Error creating local endpoint. Details in local event viewer");
            }

            try
            {
                mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SocketException vE)
            {
                BrainpackEventLogManager.InvokeEventLogMessage("Error creating local end point \n" + vE);
                DebugLogger.Instance.LogMessage(LogType.ServerSocketException, "Error creating server socket. Details in local event viewer");
            }
            try
            {
                mServerSocket.Bind(mLocalEndpoint);
                mServerSocket.Listen(mBacklog);
            }
            catch (Exception vE)
            {
                BrainpackEventLogManager.InvokeEventLogMessage("Error binding server socket. \n" + vE + " \n" + vE.InnerException);
                DebugLogger.Instance.LogMessage(LogType.ServerSocketException, "Error binding server socket. Details in local event viewer");
            }
            try
            {
                mServerSocket.BeginAccept(AcceptCallback, mServerSocket);
            }
            catch (Exception vE)
            {
                BrainpackEventLogManager.InvokeEventLogMessage("Error beggining accept server socket. \n" + vE + " \n" + vE.InnerException);
                DebugLogger.Instance.LogMessage(LogType.ServerSocketException, "Error beggining accept server socket. Details in local event viewer");

            }
            return true;
        }

        /// <summary>
        /// callback upon entering
        /// </summary>
        /// <param name="vAsyncResult"></param>
        private void AcceptCallback(IAsyncResult vAsyncResult)
        {
            StateObject vStateObject = new StateObject();
            try
            {
                Socket vStateSocket = (Socket)vAsyncResult.AsyncState;
                vStateObject = new StateObject();
                vStateObject.mClientSocket = vStateSocket.EndAccept(vAsyncResult);
                lock (mConnections)
                {
                    mConnections.Add(vStateObject);
                }
                vStateObject.mClientSocket.BeginReceive(vStateObject.Buffer, 0, vStateObject.Buffer.Length,
                    SocketFlags.None, new AsyncCallback(ReceiveCallback), vStateObject);
                mServerSocket.BeginAccept(AcceptCallback, mServerSocket);
            }
            catch (SocketException vE)
            {
                if (vStateObject.mClientSocket != null)
                {
                    vStateObject.mClientSocket.Close();
                    lock (mConnections)
                    {
                        if (mConnections.Contains(vStateObject))
                        {
                            mConnections.Remove(vStateObject);
                        }
                    }
                }
                mServerSocket.BeginAccept(AcceptCallback, mServerSocket);
            }
            catch (Exception vE)
            {
                if (vStateObject.mClientSocket != null)
                {
                    vStateObject.mClientSocket.Close();
                    lock (mConnections)
                    {
                        if (mConnections.Contains(vStateObject))
                        {
                            mConnections.Remove(vStateObject);
                        }
                    }
                }
                mServerSocket.BeginAccept(AcceptCallback, mServerSocket);
            }
        }

        private void ReceiveCallback(IAsyncResult vAsyncResult)
        {
            StateObject vStateObject = (StateObject) vAsyncResult.AsyncState;
            string vContent = string.Empty;
            try
            {
                // grab the number of bytes read from the buffer
                int vBytesRead = vStateObject.mClientSocket.EndReceive(vAsyncResult);
                if (vBytesRead > 0)
                {
                    //there might be more data, so state the data received so far
                    vStateObject.mStringBuilder.Append(PacketSetting.Encoding.GetString(vStateObject.Buffer, 0,
                        vBytesRead));
                    //check for EOF tag, if not there read more data
                    vContent = vStateObject.mStringBuilder.ToString();
                    if (vContent.IndexOf(PacketSetting.EndOfPacketDelim) > -1)
                    {
                        byte[] vData = vStateObject.Buffer;
                        //first 4 bytes are the command codes
                        HeddokoPacket vHPacket = new HeddokoPacket(vData, PacketSetting.PacketCommandSize);
                        ServerCommandRouter.Process(vStateObject, vHPacket);
                    }
                    //There is more data to be received
                    else 
                    {
                        //todo: 
                    }
                    //Queue up the next receive
                    vStateObject.mClientSocket.BeginReceive(vStateObject.Buffer, 0, vStateObject.Buffer.Length,
                        SocketFlags.None, new AsyncCallback(ReceiveCallback), vStateObject);

                }
            }
            catch (SocketException vE)
            {
                if (vStateObject != null)
                {

                    BrainpackEventLogManager.InvokeEventLogMessage("Error receiving from socket. \n" + vE + " \n" + vE);
                    DebugLogger.Instance.LogMessage(LogType.ServerSocketException,
                        "Error receiving from socket. Details in local event viewer");
                    vStateObject.mClientSocket.Close();
                    lock (mConnections)
                    {
                        if (mConnections.Contains(vStateObject))
                        {
                            mConnections.Remove(vStateObject);
                        }
                    }
                }
            }
            catch (Exception vE)
            {
                if (vStateObject != null)
                {

                    BrainpackEventLogManager.InvokeEventLogMessage("Error receiving from socket. Checking inner exception \n" + vE + " \n" + vE.InnerException);
                    DebugLogger.Instance.LogMessage(LogType.ServerSocketException,
                        "Error receiving from socket. Details in local event viewer");
                    vStateObject.mClientSocket.Close();
                    lock (mConnections)
                    {
                        if (mConnections.Contains(vStateObject))
                        {
                            mConnections.Remove(vStateObject);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            //send a message to all connection that the server is now shutting down
        }

        /// <summary>
        /// Send data to socket
        /// </summary>
        /// <param name="vHandler"></param>
        /// <param name="vObject"></param>
        /// <param name="vData"></param>
        /// <returns></returns>
        public  bool Send(object vObject, string vData)
        {
            StateObject vHandler = (StateObject) vObject;
            if (vHandler != null && vHandler.mClientSocket.Connected)
            {
                lock (vHandler.mClientSocket)
                {
                    byte[] vByteData = PacketSetting.Encoding.GetBytes(vData);
                    vHandler.mClientSocket.Send(vByteData, vByteData.Length, SocketFlags.None);
                }
            }
            else
            {
                return false;
            }
            return true;

        }
        public bool Send(object vObject, byte[] vByteData)
        {
            StateObject vHandler = (StateObject)vObject;
            if (vHandler != null && vHandler.mClientSocket.Connected)
            {
                lock (vHandler.mClientSocket)
                { 
                    vHandler.mClientSocket.Send(vByteData, vByteData.Length, SocketFlags.None);
                }
            }
            else
            {
                return false;
            }
            return true;

        }
    }
}
