using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text; 
using BrainpackService.Tools_and_Utilities;
using HeddokoLib.networking;
using HeddokoLib.utils;

namespace BrainpackService.BrainpackServer
{
    public class BrainpackServer
    { 
        private static Socket mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int mPortNumber = NetworkingReferences.ServerPort;
        private int mBacklog = 50;
        private static List<Socket> mClientsToServe = new List<Socket>(5);
        private int mTotalConnectedSockets;
        private AsyncCallback mWorkerCallback;
        public ServerCommandRouter ServerCommandRouter { get; set; }
        public void SetupServer()
        {
            IPHostEntry vIpHostEntry = Dns.GetHostEntry("localhost");
            IPAddress vIpAddress = vIpHostEntry.AddressList.First(vAddress => vAddress.AddressFamily == AddressFamily.InterNetwork); 
            IPEndPoint vLocalEndpoint = new IPEndPoint(vIpAddress, mPortNumber);

            mServerSocket.Bind(vLocalEndpoint);
            mServerSocket.Listen(mBacklog);
            mServerSocket.BeginAccept(OnClientConnect, null);
        }

        void OnClientConnect(IAsyncResult vAr)
        {
            try
            {
                //Complete the begin accept async call 
                Socket vNewWorkerSocket = mServerSocket.EndAccept(vAr);
                mClientsToServe.Add(vNewWorkerSocket);
                //Let the worker soket do the procesing for the connected client
                WaitForData(mClientsToServe[mTotalConnectedSockets]);
                //increment the client count
                mTotalConnectedSockets++;

                mServerSocket.BeginAccept(OnClientConnect, null);

            }
            catch (ObjectDisposedException)
            {
                BrainpackEventLogManager.InvokeEventLogMessage("Client Connection: socket has been closed");
            }
            catch (SocketException)
            {
                
            }
        }

        void WaitForData(Socket vSoc)
        {
            try
            {
                if (mWorkerCallback == null)
                {
                    //specify the callback function to be invoke if there is any 
                    //write activity
                    mWorkerCallback = OnDataReceived;
                }
                HeddokoPacket vSocketPacket = new HeddokoPacket {Socket = vSoc};
                vSoc.BeginReceive(vSocketPacket.Payload,
                    0, vSocketPacket.Payload.Length, SocketFlags.None,
                    mWorkerCallback, vSocketPacket);
            }
                // ReSharper disable once RedundantCatchClause
            catch (SocketException)
            {
                
                throw;
            }
        }

        void OnDataReceived(IAsyncResult vAr)
        {
            try
            {
                HeddokoPacket vSocketData = (HeddokoPacket) vAr.AsyncState;
                //complete BeginReceive() by EndReceive(), returning the number of bytes
                //written to the stream
                var vBytesRead = vSocketData.Socket.EndReceive(vAr);
                if (vBytesRead > 0)
                {
                    StringBuilder vSb = new StringBuilder();
                    vSb.Append(PacketSetting.Encoding.GetString(vSocketData.Payload, 0, vBytesRead));
                    //there might be more data, so state the data received so far
                    string vUnwrappedString = vSb.ToString();
                    // ReSharper disable once StringIndexOfIsCultureSpecific.1
                    if (vUnwrappedString.IndexOf(PacketSetting.EndOfPacketDelim) > -1)
                    {
                        vSocketData.Command = HeddokoCommands.ExtractCommandFromBytes(0, 4, vSocketData.Payload); //extract the command out of the packet
                       
                        ServerCommandRouter.Process(vSocketData.Socket, vSocketData);  
                    }
                    else
                    {
                        WaitForData(vSocketData.Socket);
                    }
                } 
            }
                // ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                
                throw;
            }
        }
 

        public void ShutDown()
        {
            if (mServerSocket != null)
            {
                //send a message to all connected clients that server is shutting down

            }
        }

        public bool IsConnected()
        {
            if (mServerSocket != null)
            {
                return mServerSocket.Connected;
            }
            return false;
        }

        public void Send(Socket vSocket, string vResultPacket)
        {
            if (IsConnected() && vSocket.Connected)
            {
                try
                { 
                    byte[] vToSend = PacketSetting.Encoding.GetBytes(vResultPacket);
                    vSocket.Send(vToSend);
                }
                catch (SocketException)
                {
                    //ignored
                }
              
            }
        
        }
    }
}
