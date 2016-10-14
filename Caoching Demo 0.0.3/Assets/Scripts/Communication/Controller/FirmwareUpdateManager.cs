// /**
// * @file FirmwareUpdateManager.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Assets.Scripts.Communication.Communicators;
using Tftp.Net;
using UnityEngine;

namespace Assets.Scripts.Communication.Controller
{

    /// <summary>
    /// A firmware update manager
    /// </summary>
    public class FirmwareUpdateManager
    {
        private int mPort;
        public static string ServerDirectory;
        private TftpServer mTftpServer;
        public FirmwareUpdateManager(string vServerDirectory,string vIpEndpoint, int vPort)
        {
            ServerDirectory = vServerDirectory;
            IPAddress vIp = IPAddress.Parse(vIpEndpoint);
            IPEndPoint vEp = new IPEndPoint(vIp,vPort);
            mPort = vPort;
            //run the server
            mTftpServer = new TftpServer(vEp);
            mTftpServer.OnReadRequest += new TftpServerEventHandler(ServerOnReadRequest);
            mTftpServer.OnWriteRequest += new TftpServerEventHandler(ServerOnWriteRequest);
            mTftpServer.OnError += new TftpServerErrorHandler(HandleErrors);
            mTftpServer.Start();
             
             
        }

        private void HandleErrors(TftpTransferError vError)
        {
            UnityEngine.Debug.Log("Error from tftp "+ vError);
        }

        /// <summary>
        /// On write request handler
        /// </summary>
        /// <param name="vTransfer"></param>
        /// <param name="vClient"></param>
        private void ServerOnWriteRequest(ITftpTransfer vTransfer, EndPoint vClient)
        {
            string vFile = Path.Combine(ServerDirectory, vTransfer.Filename);
            if (File.Exists(vFile))
            {
                CancelTransfer(vTransfer, TftpErrorPacket.FileAlreadyExists);
            }
            else
            {
                StartTransfer(vTransfer, new FileStream(vFile, FileMode.CreateNew));
            }
        }



        /// <summary>
        /// On read request handler
        /// </summary>
        /// <param name="vTransfer"></param>
        /// <param name="vClient"></param>
        private void ServerOnReadRequest(ITftpTransfer vTransfer, EndPoint vClient)
        {
            string vPath = Path.Combine(ServerDirectory, vTransfer.Filename);
            FileInfo vFile = new FileInfo(vPath);

            if (!vFile.FullName.StartsWith(ServerDirectory, StringComparison.InvariantCulture))
            {
                CancelTransfer(vTransfer, TftpErrorPacket.AccessViolation);
            }
            else if (!vFile.Exists)
            {
                CancelTransfer(vTransfer, TftpErrorPacket.FileNotFound);
            }
            else
            {
                StartTransfer(vTransfer, new FileStream(vPath, FileMode.Open));
            }
        }

        /// <summary>
        /// Begin the vTransfer process and register handlers
        /// </summary>
        /// <param name="vTransfer"></param>
        /// <param name="vStream"></param>
        private void StartTransfer(ITftpTransfer vTransfer, Stream vStream)
        {
            vTransfer.OnProgress += new TftpProgressHandler(ProgressTransferHandler);
            vTransfer.OnError += new TftpErrorHandler(TransferOnErrorHandler);
            vTransfer.OnFinished += new TftpEventHandler(TransferFinishedHandler);
            vTransfer.Start(vStream);
        }

        private void CancelTransfer(ITftpTransfer vTransfer, TftpErrorPacket vErrorPacket)
        {
            vTransfer.Cancel(vErrorPacket);
        }

        private void TransferOnErrorHandler(ITftpTransfer vTransfer, TftpTransferError vError)
        {


        }

        private void TransferFinishedHandler(ITftpTransfer vTransfer)
        {
        }

        private void ProgressTransferHandler(ITftpTransfer vTransfer, TftpTransferProgress vProgress)
        {
        }

        public void CleanUp()
        {
            mTftpServer.Dispose();
        }

        ////   private TcpListener mListener;
        ////   private TcpClient mClient;
        //private Socket mSocket;
        //private string mFilePath;
        //public bool Start(string vCurreendPointIp, string vFilePath)
        //{
        //    //open a socket  System.Net.IPEndPoint vServerEndPoint
        //    mFilePath = vFilePath;
        //    try
        //    {
        //        IPHostEntry vLocalHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        //        //  System.Net.IPEndPoint vServerEndPoint = 
        //        IPAddress vIpAddress = IPAddress.Parse(vCurreendPointIp);
        //        //foreach (var ip in vLocalHost.AddressList)
        //        //{
        //        //    if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        //    {
        //        //        vIpAddress = ip;
        //        //        break;
        //        //    }
        //        //}
        //        System.Net.IPEndPoint vServerEndPoint = new IPEndPoint(vIpAddress, Port);
        //        mSocket = new Socket(vServerEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //        mSocket.Bind(vServerEndPoint);
        //        mSocket.Listen(10);
        //    }
        //    catch (ArgumentOutOfRangeException vException)
        //    {
        //        UnityEngine.Debug.Log("ServerListener_StartServer_Port_number_is_invalid" + vException);
        //        return false;
        //        //throw new ArgumentOutOfRangeException(Resources.ServerListener_StartServer_Port_number_is_invalid, vArgument);
        //    }
        //    catch (SocketException vException)
        //    {
        //        UnityEngine.Debug.Log("Could not create socket, check to make sure that port is not being used by another socket " + vException);
        //        return false;
        //        //   throw new ApplicationException("Could not create socket, check to make sure that port is not being used by another socket", vException);
        //    }
        //    catch (Exception vException)
        //    {
        //        UnityEngine.Debug.Log("Error occured while binding socket, check inner exception" + vException);
        //        return false;
        //        // throw new ApplicationException("Error occured while binding socket, check inner exception", vException);
        //    }
        //    try
        //    {
        //        mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
        //    }
        //    catch (Exception vE)
        //    {
        //        UnityEngine.Debug.Log("Error occured starting listener, check inner exception" + vE);
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// Accept callback
        ///// </summary>
        ///// <param name="vAr"></param>
        //private void AcceptCallback(IAsyncResult vAr)
        //{
        //    StateObject vRemoteSocket = new StateObject();
        //    try
        //    {
        //        //finish accepting the connection
        //        Socket vSocket = (Socket)vAr.AsyncState;
        //        vRemoteSocket = new StateObject();
        //        vRemoteSocket.Socket = vSocket.EndAccept(vAr);
        //        vRemoteSocket.Buffer = new byte[1024];

        //        vRemoteSocket.Socket.BeginReceive(vRemoteSocket.Buffer, 0, vRemoteSocket.Buffer.Length,
        //             SocketFlags.None, new AsyncCallback(ReceiveCallback), vRemoteSocket);
        //        //  mSocket.BeginReceive(vRemoteSocket.Buffer, 0, vRemoteSocket.Buffer.Length,
        //        //     SocketFlags.None, new AsyncCallback(ReceiveCallback), vRemoteSocket);
        //        mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
        //    }
        //    catch (SocketException vException)
        //    {
        //        CloseAndRemoveStateObject(vRemoteSocket);
        //        mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
        //    }
        //    catch (Exception vE)
        //    {
        //        CloseAndRemoveStateObject(vRemoteSocket);
        //        mSocket.BeginAccept(new AsyncCallback(AcceptCallback), mSocket);
        //    }
        //}

        ///// <summary>
        ///// Close StateObject
        ///// </summary>
        ///// <param name="vIncomingConnection"></param>
        //private void CloseAndRemoveStateObject(StateObject vIncomingConnection)
        //{
        //    if (vIncomingConnection.Socket != null)
        //    {
        //        vIncomingConnection.Socket.Shutdown(SocketShutdown.Both);
        //        vIncomingConnection.Socket.Close();
        //    }
        //}


        //private void ReceiveCallback(IAsyncResult vAr)
        //{
        //    StateObject vRemoteSocket = (StateObject)vAr.AsyncState;
        //    try
        //    {
        //        int vBytesRead = vRemoteSocket.Socket.EndReceive(vAr);
        //        Socket vClient = vRemoteSocket.Socket;
        //        //   vRemoteSocket.Socket.EndReceive(vAr);


        //        if (vBytesRead > 0)
        //        {
        //            //there might be more data, store it
        //            // vClient.BeginReceive(vRemoteSocket.Buffer, 0, 1024, 0, new AsyncCallback(ReceiveCallback), vRemoteSocket);
        //            string vMsg = Encoding.ASCII.GetString(vRemoteSocket.Buffer);
        //            if (vMsg.Contains("GetLatestFirmware<EOL>"))
        //            {
        //                vClient.BeginSendFile(mFilePath, new AsyncCallback(ReceiveCallback), vRemoteSocket);
        //                // SendFile(vRemoteSocket, mFilePath);
        //            }
        //            vClient.BeginReceive(vRemoteSocket.Buffer, 0, 1024, 0, new AsyncCallback(ReceiveCallback), vRemoteSocket);
        //        }
        //        else
        //        {
        //            //get message from the suit that is requesting a firmware update

        //        }
        //    }
        //    catch (SocketException vE)
        //    {
        //        CloseAndRemoveStateObject(vRemoteSocket);
        //    }
        //}

        //private void SendFile(StateObject vObj, string vFilePath)
        //{
        //    //end receive
        //    string vMsg1 = "<BoF>";
        //    var vMsg1InBytes = Encoding.ASCII.GetBytes(vMsg1);

        //    byte[] vBuffer = File.ReadAllBytes(mFilePath);
        //    byte[] vMsgAndBuffer = new byte[vMsg1InBytes.Length + vBuffer.Length];
        //    vMsg1InBytes.CopyTo(vMsgAndBuffer, 0);
        //    vBuffer.CopyTo(vMsgAndBuffer, vMsg1InBytes.Length);
        //    mSocket.BeginSend(vMsgAndBuffer, 0, vMsgAndBuffer.Length, SocketFlags.None, new AsyncCallback(FileSendCallback), mSocket);
        //}

        //private void FileSendCallback(IAsyncResult vAr)
        //{
        //    Socket vClient = (Socket)vAr.AsyncState;
        //    // Complete sending the data to the remote device.
        //    vClient.EndSend(vAr);
        //    mSocket.Shutdown(SocketShutdown.Both);
        //    mSocket.Close();
        //}

        //public void Dispose()
        //{
        //    CloseCurrentSocket();
        //}

        //public void CloseCurrentSocket()
        //{
        //    if (mSocket != null && (mSocket.Connected))
        //    {
        //        mSocket.Shutdown(SocketShutdown.Both);
        //        mSocket.Close();
        //    }
        //}

    }
}