
/** 
* @file UdpSender.cs
* @brief Contains the UdpSender class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BrainpackService.Tools_and_Utilities.Debugging;
using HeddokoLib.networking;

namespace BrainpackService.BrainpackServer.client
{
    /// <summary>
    /// Sends data to a single endpoint
    /// </summary>
    public class UdpSender
    {
        private IPAddress mEndPointIp;
        private IPEndPoint mEndPoint;
        private Semaphore mSemaphore;
        private UdpClient mClient;

        public UdpSender()
        {
            mSemaphore = new Semaphore(1, 1);
        }
        /// <summary>
        /// register the ip endpoint to send data to 
        /// </summary>
        /// <param name="vEndPointSocket"></param>
        public void RegisterIpEndPoint(Socket vEndPointSocket)
        {
            IPEndPoint vTemp = vEndPointSocket.RemoteEndPoint as IPEndPoint;
            if (vTemp != null)
            {
                mEndPointIp = vTemp.Address;
                mEndPoint = new IPEndPoint(mEndPointIp, 11000);
                mClient = new UdpClient();
                mClient.Connect(mEndPoint);
            }
        }


        /// <summary>
        /// Send a datagram packet to the outgoing endpoint
        /// </summary>
        public void SendDatagram(HeddokoPacket vPacket)
        {
            mSemaphore.WaitOne();
            if (mEndPoint != null)
            {
                try
                {
                    string vWrapped = HeddokoPacket.Wrap(vPacket);
                    byte[] vByteData = PacketSetting.Encoding.GetBytes(vWrapped);
                  //  mClient.Send(vByteData, vByteData.Length);
                    mClient.BeginSend(vByteData, vByteData.Length, new AsyncCallback(SendCallback), mClient);
                    DebugLogger.Instance.LogMessage(LogType.ApplicationFrame, vWrapped);
                }
                catch (Exception vE)
                {
                    string vMsg = "There was a problem sending a datagram. \n" + vE + "\n" + vE.InnerException;
                    DebugLogger.Instance.LogMessage(LogType.ServerSocketException, vMsg);
                }
            }
            mSemaphore.Release();
        }

        /// <summary>
        /// Completes the send
        /// </summary>
        /// <param name="vAr"></param>
        private void SendCallback(IAsyncResult vAr)
        {
            try
            {
                UdpClient vClient = (UdpClient)vAr.AsyncState;
                vClient.EndSend(vAr);
                string vMsg = "End_send_dataframe";
                DebugLogger.Instance.LogMessage(LogType.ApplicationFrame, vMsg);
            }
            catch (Exception vE)
            {
                string vMsg = "There was a problem sending a datagram. \n" + vE + "\n" + vE.InnerException;
                DebugLogger.Instance.LogMessage(LogType.ServerSocketException, vMsg);
            }
        }

        /// <summary>
        /// Clears out the end points, preventing sending to the end point
        /// </summary>
        public void Clear()
        {
            mSemaphore.WaitOne();
            mEndPoint = null;
            mSemaphore.Release();
        }
    }
}