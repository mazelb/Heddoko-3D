
/** 
* @file SynchronousClient.cs
* @brief Contains the SynchronousClient class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Utils.DebugContext.logging;
using HeddokoLib.adt;

//using Assets.Scripts.Utils.Debugging;
using HeddokoLib.networking;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Communication
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    // State object for receiving data from remote device.

    /// <summary>
    /// Synchrounous socket client 
    /// </summary>

    public class SynchronousClient
    {
        private Thread mWorkerThread;
        private const int sTimeout = 30000;
        private Semaphore mSemaphore = new Semaphore(1, 1);

        public SynchronousClient()
        {
            mWorkerThread = new Thread(ThreadWorker);
            mIsworking = true;
            mWorkerThread.Start();
            string vLogMessage = "Synchronous Client instantiated, current timeout is set to " + sTimeout;
            DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, vLogMessage);
        }

        private PriorityQueue<PriorityMessage> mPriorityMessages = new PriorityQueue<PriorityMessage>();
        private static bool mReceivedMessage = true;


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

                if (mReceivedMessage == false)
                {
                    continue;
                }
                else
                {
                    mReceivedMessage = false;
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
            byte[] bytes = new byte[1024];
            mReceivedMessage = false;
            string vLogmessage = "";
            Stopwatch vStopwatch = new Stopwatch();
            vStopwatch.Start();
            // Connect to a remote device.
            try
            {
                IPHostEntry vIpHostEntry = Dns.Resolve("localhost");
                IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                IPEndPoint vRemoteEndPoint = new IPEndPoint(vIpAddress, 11000);
                // Create a TCP/IP  socket.
                Socket vSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                vSender.ReceiveTimeout = sTimeout;
                vSender.SendTimeout = sTimeout;

                try
                {
                    vSender.Connect(vRemoteEndPoint);
                    // Encode the data string into a byte array.
                    byte[] msg = PacketSetting.Encoding.GetBytes(vMsg);

                    // Send the data through the socket.
                    vSender.Send(msg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, "Sending... " + vMsg);

                    // Receive the response from the remote device.
                    vSender.Receive(bytes);

                    HeddokoPacket vHPacket = new HeddokoPacket(bytes, PacketSetting.PacketCommandSize);
                    PacketCommandRouter.Instance.Process(vSender, vHPacket);

                    // Release the socket.
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();

                    // vMsg = PacketSetting.Encoding.GetString(bytes);
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
                catch (Exception e)
                {
                    vMsg = "Unexpected exception " + e;
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();
                    Debug.Log(vMsg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);

                }

            }
            catch (Exception e)
            {
                vMsg = "Unexpected exception " + e;
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                vMsg = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                Debug.Log(vMsg);
            }
            mReceivedMessage = true;
            vStopwatch.Stop();
            vMsg = "time taken from start send message to end " + vStopwatch.ElapsedMilliseconds + " ms";
            //    DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, vMsg);


        }

        public void StartClientAndSendData(PriorityMessage vMsg)
        {
            byte[] bytes = new byte[1024];
            mReceivedMessage = false;
            string vLogmessage = "";
            Stopwatch vStopwatch = new Stopwatch();
            vStopwatch.Start();
            string vLogMessage = "";
            // Connect to a remote device.
            try
            {
                IPHostEntry vIpHostEntry = Dns.Resolve("localhost");
                IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                IPEndPoint vRemoteEndPoint = new IPEndPoint(vIpAddress, 11000);
                // Create a TCP/IP  socket.
                Socket vSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                vSender.ReceiveTimeout = sTimeout;
                vSender.SendTimeout = sTimeout;


                try
                {
                    vSender.Connect(vRemoteEndPoint);
                    // Encode the data string into a byte array.
                    byte[] msg = PacketSetting.Encoding.GetBytes(vMsg.MessagePayload);

                    // Send the data through the socket.
                    vSender.Send(msg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, "Sending... " + vMsg);

                    // Receive the response from the remote device.
                    vSender.Receive(bytes);

                    HeddokoPacket vHPacket = new HeddokoPacket(bytes, PacketSetting.PacketCommandSize);
                    PacketCommandRouter.Instance.Process(vSender, vHPacket);

                    // Release the socket.
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();

                    // vMsg = PacketSetting.Encoding.GetString(bytes);
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
                catch (Exception e)
                {
                    vLogMessage = "Unexpected exception " + e;
                    vSender.Shutdown(SocketShutdown.Both);
                    vSender.Close();
                    Debug.Log(vMsg);
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);
                    vLogMessage = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);

                }

            }
            catch (Exception e)
            {
                vLogMessage = "Unexpected exception " + e;
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);
                vLogMessage = "time taken from start until this exception " + vStopwatch.ElapsedMilliseconds + " ms";
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vLogMessage);
                Debug.Log(vMsg);
            }
            mReceivedMessage = true;
            vStopwatch.Stop();
            vLogMessage = "time taken from start send message to end " + vStopwatch.ElapsedMilliseconds + " ms";
            //    DebugLogger.Instance.LogMessage(LogType.SocketClientSettings, vMsg);


        }
        private bool mIsworking;
        public void Stop()
        {
            while (mPriorityMessages.Count != 0) ;
            mIsworking = false;
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
            ;
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
