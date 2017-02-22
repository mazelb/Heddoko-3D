/** 
* @file SynchronousClient.cs
* @brief Contains the SynchronousClient class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Diagnostics;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Utils.DebugContext.logging;
using HeddokoLib.adt;
using HeddokoLib.networking;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Communication
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// Synchrounous socket client 
    /// </summary>

    public class SynchronousClient
    {
        private Thread mWorkerThread;
        private const int Timeout = 65000;
        private Semaphore mSemaphore = new Semaphore(1, 1);
        public SynchronousClient()
        {
            mWorkerThread = new Thread(ThreadWorker);
            mWorkerThread.IsBackground = true;
            mIsworking = true;
            mWorkerThread.Start();
            string vLogMessage = "Synchronous Client instantiated, current timeout is set to " + Timeout;
            DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, vLogMessage);
        }

        private PriorityQueue<PriorityMessage> mPriorityMessages = new PriorityQueue<PriorityMessage>();
        private static bool sReceivedMessage = true;


        private void ThreadWorker()
        {
            while (true)
            {
                if (!mIsworking)
                {
                    break;
                }
                if (mPriorityMessages.Count == 0)
                {
                    continue;
                }
                if (sReceivedMessage)
                {
                    sReceivedMessage = false;
                    mSemaphore.WaitOne();
                    PriorityMessage vMsg = mPriorityMessages.RemoveFirstItem();
                    mSemaphore.Release();
                    StartClientAndSendData(vMsg);
                }
            }
        }

        public void AddMessage(PriorityMessage vMsg)
        {
            mSemaphore.WaitOne();
            mPriorityMessages.Add(vMsg);
            mSemaphore.Release();
        }
        /// <summary>
        /// Starts a client socket and sends the message data. 
        /// </summary>
        /// <param name="vMsg"></param>
        public void StartClientAndSendData(string vMsg)
        {
            byte[] vBytes = new byte[1024];
            sReceivedMessage = false;
            Stopwatch vStopwatch = new Stopwatch();
            vStopwatch.Start();
            // Connect to a remote device.
            try
            {
                IPAddress vIpAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint vRemoteEndPoint = new IPEndPoint(vIpAddress, 11000);
                // Create a TCP/IP  socket.
                Socket vSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                vSender.ReceiveTimeout = Timeout;
                vSender.SendTimeout = Timeout;

                byte[] vBuffer = PacketSetting.Encoding.GetBytes(vMsg);
                try
                {
                    vSender.Connect(vRemoteEndPoint);
                    vSender.Send(vBuffer);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, "Sending... " + vMsg);
                    // Receive the response from the remote device.
                    vSender.Receive(vBytes);
                    HeddokoPacket vHPacket = new HeddokoPacket(vBytes, PacketSetting.PacketCommandSize);
                    PacketCommandRouter.Instance.Process(vSender, vHPacket);
                    // Release the socket.
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();
                }
                catch (TimeoutException vE)
                {
                    vSender.Shutdown(SocketShutdown.Both);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vE.Message);
                    vMsg = "Timeout exception:time taken from start" + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    vSender.Close();
                }
                catch (ArgumentNullException vE)
                {
                    vMsg = "ArgumentNullException  " + vE;
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    Debug.Log(vMsg);
                }
                catch (SocketException vE)
                {
                    vMsg = "SocketException  " + vE.ErrorCode + "\r\n" + vE;
                    vMsg += vE.InnerException;
                    vSender.Close();
                    Debug.Log(vMsg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    mSemaphore.WaitOne();
                    mPriorityMessages.Clear();
                    mSemaphore.Release();
                }
                catch (Exception vE)
                {
                    vMsg = "Unexpected exception " + vE;
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();
                    Debug.Log(vMsg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);

                }

            }
            catch (Exception vE)
            {
                vMsg = "Unexpected exception " + vE;
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                Debug.Log(vMsg);
            }
            sReceivedMessage = true;
            vStopwatch.Stop();
        }

        /// <summary>
        /// open a socket and send a message
        /// </summary>
        /// <param name="vMsg"></param>
        public void StartClientAndSendData(PriorityMessage vMsg)
        {
            byte[] vBytes = new byte[1024];
            sReceivedMessage = false;
            Stopwatch vStopwatch = new Stopwatch();
            vStopwatch.Start();
            string vLogMessage;
            // Connect to a remote device.
            try
            {
                IPAddress vIpAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint vRemoteEndPoint = new IPEndPoint(vIpAddress, 11000);
                // Create a TCP/IP  socket.
                Socket vSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                vSender.ReceiveTimeout = Timeout;
                vSender.SendTimeout = Timeout;
                try
                {
                    vSender.Connect(vRemoteEndPoint);
                    // Encode the data string into a byte array.
                    byte[] vMessage = PacketSetting.Encoding.GetBytes(vMsg.MessagePayload);

                    // Send the data through the socket.
                    vSender.Send(vMessage);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, "Sending... " + vMsg);

                    // Receive the response from the remote device.
                    vSender.Receive(vBytes);
                    HeddokoPacket vHPacket = new HeddokoPacket(vBytes, PacketSetting.PacketCommandSize);
                    PacketCommandRouter.Instance.Process(vSender, vHPacket);

                    // Release the socket.
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();
                }
                catch (TimeoutException vE)
                {
                    vSender.Shutdown(SocketShutdown.Both);
                    vLogMessage = "Time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vE.Message);
                    var vLogMsg = string.Format("Timedout on on sending {0} \n{1}", vMsg.MessagePayload, vLogMessage);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMsg);
                    HeddokoPacket vPacket = new HeddokoPacket("TimeoutException", string.Empty);
                    PacketCommandRouter.Instance.Process(this, vPacket);
                    mSemaphore.WaitOne();
                    mPriorityMessages.Clear();
                    mSemaphore.Release();
                    vSender.Close();
                }
                catch (ArgumentNullException vE)
                {
                    vLogMessage = "ArgumentNullException  " + vE;
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);
                    vLogMessage = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);

                    Debug.Log(vMsg);
                }
                catch (SocketException vE)
                {
                    mSemaphore.WaitOne();
                    mPriorityMessages.Clear();
                    mSemaphore.Release();

                    vLogMessage = "Time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    vLogMessage += "\r\nSocketException  " + vE.ErrorCode + "\r\n" + vE;
                    vLogMessage += "\r\n" + vE.InnerException;
                    var vLogMsg = string.Format("Socket exception on on sending {0} \n{1}", vMsg.MessagePayload, vLogMessage);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMsg);
                    Debug.Log(vLogMsg);
                    vSender.Close();
                    HeddokoPacket vPacket = new HeddokoPacket("TimeoutException", string.Empty);
                    PacketCommandRouter.Instance.Process(this, vPacket);

                }
                catch (Exception vE)
                {
                    vLogMessage = "Unexpected exception " + vE;
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();
                    Debug.Log(vMsg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);
                    vLogMessage = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);
                }

            }
            catch (Exception vE)
            {
                vLogMessage = "Unexpected exception " + vE;
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);
                vLogMessage = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);
                Debug.Log(vMsg);
            }
            sReceivedMessage = true;
            vStopwatch.Stop();
        }
        private bool mIsworking;
        public void Stop()
        {
            Debug.Log("Stopping sync client");
            mIsworking = false;
            try
            {
                mWorkerThread.Abort();
            }
            catch (Exception)
            {
                Debug.Log("aborting sync client");
            }
        }
    }
    /// <summary>
    ///  A priority message
    /// </summary>
    public class PriorityMessage : IPriorityQueueItem<PriorityMessage>
    {
        public Priority Priority { get; set; }
        public string MessagePayload;
        public int CompareTo(object vObj)
        {
            PriorityMessage vOtherMessage = (PriorityMessage)vObj;
            return CompareTo(vOtherMessage);
        }

        /// <summary>
        /// Compare one message against another based on their priority
        /// </summary>
        /// <param name="vMessage"></param>
        /// <returns></returns>
        public int CompareTo(PriorityMessage vMessage)
        {
            int vCompareA = (int)Priority;
            int vCompareB = (int)vMessage.Priority;
            int vComparison = vCompareA.CompareTo(vCompareB);
            return vComparison;
        }

        public int HeapIndex { get; set; }

        public override string ToString()
        {
            string vReturn = "";
            vReturn += "Priority: " + EnumUtil.GetName(Priority) + " PriorityValue: " + (int)Priority + " HeapIndex: " +
                       HeapIndex;
            return vReturn;
        }
    }

    public enum Priority
    {
        Urgent = 0,
        High = 1,
        Medium = 2,
        Low = 3
    }
}
