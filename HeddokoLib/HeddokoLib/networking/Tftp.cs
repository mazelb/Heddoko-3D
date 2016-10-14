// /**
// * @file Tftp.cs
// * @brief Contains a tftp client server protocol
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HeddokoLib.networking
{
    public class Tftp
    {
        public string ErrorMessage = "";
        public int ErrorCode = -1;
        private int mTftpPort;
        private string mTftpServer;
        private int mServerTimeout = 1000;
        /// <summary>
        /// Get the Port of the server
        /// </summary>
        public int Port
        {
            get { return mTftpPort; }
            private set { mTftpPort = value; }
        }

        /// <summary>
        /// Get the name of the server
        /// </summary>
        public string Server
        {
            get { return mTftpServer; }
            private set { mTftpServer = value; }
        }

        public int ServerTimeout
        {
            get { return mServerTimeout; }
            private set { mServerTimeout = value; }
        }

        /// <summary>
        /// Initializes a new instance of the Tftp class
        /// </summary>
        /// <param name="vServer">The server</param>
        /// <param name="vPort">the port number</param>
        public Tftp(string vServer, int vPort)
        {
            Server = vServer;
            Port = vPort;
        }


        /// <summary>
        /// Gets the specified remote file and place it locally
        /// </summary>
        /// <param name="vRemoteFile">the remote file name</param>
        /// <param name="vLocalFile">the location to be placed in </param>
        /// <param name="vServerTimeout">the timeout period</param>
        public void GetFile(string vRemoteFile, string vLocalFile, int vServerTimeout = 1000)
        {
            GetFile(vRemoteFile, vLocalFile, TftpModes.Octet, vServerTimeout);
        }

        /// <summary>
        /// Gets the specified remote file and place it locally with spefic mode
        /// </summary>
        /// <param name="vRemoteFile">the remote file name</param>
        /// <param name="vLocalFile">the location to be placed in </param>
        /// <param name="vModes">the tftp mode</param>
        /// <param name="vTimeout">The timeout period</param>
        public void GetFile(string vRemoteFile, string vLocalFile, TftpModes vModes, int vTimeout)
        {
            int vPacketNum = 1;
            byte[] vSendBuffer = CreateRequestPacket(OpCodes.Read, vRemoteFile, vModes);
            byte[] vReceiverBuffer = new byte[516];
            ServerTimeout = vTimeout;

            BinaryWriter vFileStream = new BinaryWriter(new FileStream(vLocalFile, FileMode.Create, FileAccess.Write, FileShare.Read));
            IPHostEntry vHostEntry = Dns.GetHostEntry(Server);
            IPEndPoint vServerEp = new IPEndPoint(vHostEntry.AddressList[0], Port);
            EndPoint vDataEndPoint = vServerEp;
            Socket vTftpSock = new Socket(vServerEp.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            //request and receive first data packet from tftp server
            vTftpSock.SendTo(vSendBuffer, vSendBuffer.Length, SocketFlags.None, vServerEp);
            vTftpSock.ReceiveTimeout = ServerTimeout;
            var vLeng = vTftpSock.ReceiveFrom(vReceiverBuffer, ref vDataEndPoint);

            //keep track of the TiD
            vServerEp.Port = ((IPEndPoint)vDataEndPoint).Port;

            while (true)
            {
                //handle errors
                if (((OpCodes)vReceiverBuffer[1]) == OpCodes.Error)
                {
                    vFileStream.Close();
                    vTftpSock.Close();
                    throw new TftpException(((vReceiverBuffer[2] << 8) & 0xff00) | vReceiverBuffer[3], Encoding.ASCII.GetString(vReceiverBuffer, 4, vReceiverBuffer.Length - 5).Trim('\0'));
                }

                //expect the next packet
                if (((vReceiverBuffer[2] << 8) & 0xff00 | vReceiverBuffer[3]) == vPacketNum)
                {
                    vFileStream.Write(vReceiverBuffer, 4, vLeng - 4);
                    //send ack packet to tftp server
                    vSendBuffer = CreateAckPacket(vPacketNum++);
                    vTftpSock.SendTo(vSendBuffer, vSendBuffer.Length, SocketFlags.None, vServerEp);
                }

                //was this the last packet?
                if (vLeng < 516)
                {
                    break;
                }
                else
                {
                    vLeng = vTftpSock.ReceiveFrom(vReceiverBuffer, ref vDataEndPoint);
                }
            }
            vTftpSock.Close();
            vFileStream.Close();
        }

        /// <summary>
        /// Places the specified remote file 
        /// </summary>
        /// <param name="vRemoteFile">the remote file</param>
        /// <param name="vLocalFile">the local file</param>
        public void Put(string vRemoteFile, string vLocalFile)
        {
            Put(vRemoteFile, vLocalFile, TftpModes.Octet);
        }


        /// <summary>
        /// puts the specified remote file 
        /// </summary>
        /// <param name="vRemoteFile">the remote file to put</param>
        /// <param name="vLocalFile">the local file</param>
        /// <param name="vModes">the operation mode</param>
        public void Put(string vRemoteFile, string vLocalFile, TftpModes vModes)
        {
            int vPacketNum = 0;
            byte[] vSendBuffer = CreateRequestPacket(OpCodes.Write, vRemoteFile, vModes);
            byte[] vRecBuffer = new byte[516];

            BinaryReader vFileStream =
                new BinaryReader(new FileStream(vLocalFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            IPHostEntry vHostEntry = Dns.GetHostEntry(Server);
            IPEndPoint vServerEp = new IPEndPoint(vHostEntry.AddressList[0], Port);
            EndPoint vDataEp = vServerEp;
            Socket vTftpSock = new Socket(vServerEp.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            vTftpSock.SendTo(vSendBuffer, vSendBuffer.Length, SocketFlags.None, vServerEp);
            vTftpSock.ReceiveTimeout = ServerTimeout;
            vTftpSock.ReceiveFrom(vRecBuffer, ref vDataEp);

            vServerEp.Port = ((IPEndPoint)vDataEp).Port;

            while (true)
            {
                if (((OpCodes)vRecBuffer[1]) == OpCodes.Error)
                {
                    vFileStream.Close();
                    vTftpSock.Close();
                    throw new TftpException(((vRecBuffer[2] << 8) & 0xff00) | vRecBuffer[3], Encoding.ASCII.GetString(vRecBuffer, 4, vRecBuffer.Length - 5).Trim('\0'));
                }

                if ((((OpCodes)vRecBuffer[1] == OpCodes.Ack)) && (((vRecBuffer[2] << 8) & 0xff00) | vRecBuffer[3]) == vPacketNum)
                {

                    vSendBuffer = CreateDataPacket(++vPacketNum, vFileStream.ReadBytes(512));
                    vTftpSock.SendTo(vSendBuffer, vSendBuffer.Length, SocketFlags.None, vServerEp);
                }

                if (vSendBuffer.Length < 516)
                {
                    break;
                }
                else
                {
                    vTftpSock.ReceiveFrom(vRecBuffer, ref vDataEp);
                }
            }
            vTftpSock.Close();
            vFileStream.Close();
        }

        /// <summary>
        /// Creates the request packet
        /// </summary>
        /// <param name="vOpCode">the op code</param>
        /// <param name="vRemoteFile">the remote file</param>
        /// <param name="vMode">the tftp mode to use</param>
        /// <returns></returns>
        private byte[] CreateRequestPacket(OpCodes vOpCode, string vRemoteFile, TftpModes vMode)
        {
            //create new byte array to hold initial read req packet
            int vPos = 0;
            string vModeAscii = vMode.ToString().ToLowerInvariant();
            byte[] vRet = new byte[vModeAscii.Length + vRemoteFile.Length +4];

            //set the first opcode of packet ot indicate if this is read or write request
            vRet[vPos++] = 0;
            vRet[vPos++] = (byte) vOpCode;

            //convert filename to a char array
            vPos += Encoding.ASCII.GetBytes(vRemoteFile, 0, vRemoteFile.Length, vRet, vPos);
            vRet[vPos++] = 0;
            vPos += Encoding.ASCII.GetBytes(vModeAscii, 0, vModeAscii.Length, vRet, vPos);
            vRet[vPos] = 0;

            return vRet;
        }


        /// <summary>
        /// Create the data packet
        /// </summary>
        /// <param name="vBlockNr"></param>
        /// <param name="vData"></param>
        /// <returns></returns>
        private byte[] CreateDataPacket(int vBlockNr, byte[] vData)
        {
            byte[] vRet = new byte[4 + vData.Length];

            vRet[0] = 0;
            vRet[1] = (byte) OpCodes.Data;
            vRet[2] = (byte)((vBlockNr >> 8) & 0xff);
            vRet[3] = (byte)(vBlockNr & 0xff);
             
            Array.Copy(vData,0 ,vRet,4 , vData.Length);
            return vRet;
        }

        /// <summary>
        /// create an ack packet
        /// </summary>
        /// <param name="vBlockNr"></param>
        /// <returns></returns>
        private byte[] CreateAckPacket(int vBlockNr)
        {
            byte[] vRet = new byte[4];

            vRet[0] = 0;
            vRet[1] = (byte) OpCodes.Ack;

            vRet[2] = (byte) ((vBlockNr >> 8) & 0xff);
            vRet[3] = (byte)(vBlockNr & 0xff);
            return vRet;
        }
    }



    public class TftpException : Exception
    {

        public string ErrorMessage;
        public int ErrorCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TftpException"/> class.
        /// </summary>
        /// <param name="vErrCode">The err code.</param>
        /// <param name="vErrMsg">The err MSG.</param>
        public TftpException(int vErrCode, string vErrMsg)
        {
            ErrorCode = vErrCode;
            ErrorMessage = vErrMsg;
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        /// <filterPriority>1</filterPriority>
        /// <permissionSet class="System.Security.permissionSet" version="1">
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        /// </permissionSet>
        public override string ToString()
        {
            return String.Format("TFTPException: ErrorCode: {0} Message: {1}", ErrorCode, ErrorMessage);
        }
    }

    public enum OpCodes
    {
        Unknown = 0,
        Read = 1,
        Write = 2,
        Data = 3,
        Ack = 4,
        Error = 5
    }

    public enum TftpModes
    {
        Unknown = 0,
        NetAscii = 1,
        Octet = 2,
        Mail = 3
    }
}