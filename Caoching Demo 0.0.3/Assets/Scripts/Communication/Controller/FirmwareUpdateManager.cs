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
using Tftp.Net; 

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

    }   
}