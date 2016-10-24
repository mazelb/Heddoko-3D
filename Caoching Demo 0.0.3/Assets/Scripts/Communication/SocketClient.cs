/** 
* @file SocketClient.cs
* @brief Contains the SocketClient clas
* @author Mohammed Haider (mohammed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Assets.Scripts.Communication.Controller;
using HeddokoLib.networking;
using HeddokoLib.utils;
 


namespace Assets.Scripts.Communication
{
    /**
    * SocketClient class 
    * @brief SocketClient class: a socket connection to the windows Brainpack service
 
    */

    [Obsolete("Not used anymore,instead Synchronous client is used. ", true)]
    public class SocketClient
    {
        private TcpClient mSocket;
        NetworkStream mNetworkStream;
        StreamWriter mStreamWriter;
        private StreamWriter mLogWriter;
        StreamReader mStreamReader;
        private bool mSocketReady { get; set; }
        private bool mSocketOpen = false;
        private bool mIsWorking = false;
        private object mSocketLock = new object();
        private Thread mWorkerThread;
        private ISocketClientSetting mClientSocketSettings;
        public ISocketClientSetting ClientSocketSettings
        {
            get { return mClientSocketSettings; }
            set { mClientSocketSettings = value; }
        }
        /// <summary>
        /// Sends a message to the server with the provided message
        /// </summary>
        ///<param name="vMsg">The message to send to the server</param>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /**
        * SendMessage(string vMsg)
        * @brief Sends a message to the server with the provided message
        * @param string vMessage The message to send to the server
        * @note Please not that this will throw an IOException if an I/O error occurs when attempting to write to the stream
        */
        public void SendMessage(string vMsg)
        {
            if (!mSocketReady)
            {
                OpenSocket();
            }
            try
            {
                mStreamWriter.WriteLine(vMsg);
                mStreamWriter.Flush();
            }
            catch (IOException e)
            {
                string vMessage =
                     "\r\n Message sent:" + "vMsg" +
                    "A connection to the Heddoko service failed. Please ensure that the service is operational and try again." +
                    "\r\n ---Beginning of Stacktrace---" + e.StackTrace + "---End of Stacktrace---" +
                    "\r\n  ---Beginning of BaseException---" + e.GetBaseException() + "---End of GetBaseException---" +
                    "\r\n  ---Beginning of BaseException Stack trace " + e.GetBaseException().StackTrace +
                    "---End of GetBaseException stacktrace---" +
                    "\r\n  ----Beginning of InnerException" + e.InnerException + "---End of InnerException---" +
                    "\r\n  ----Beginning of InnerException StackTrace" + e.InnerException.StackTrace +
                    "---End of InnerException StackTrace---";
                WriteToLogFile(vMessage);
                HeddokoPacket vPacket = new HeddokoPacket(HeddokoCommands.ClientError, vMessage);
                PacketCommandRouter.Instance.Process(this, vPacket);
            }
            catch (NullReferenceException vNullReferenceException)
            {
                //this error occurs if the mstreamwriter hasn't been instantiated and most likely will occur because the socket failed
                //to open a connection to the brainpack service. It was handled before
                //todo: check if there are any other cases where this exception can be raised

            }
            catch (Exception e)
            {
                string vMessage =
                    "\r\n Message sent:" + "vMsg" +
                       "A connection to the Heddoko service failed. Please ensure that the service is operational and try again." +
                       "\r\n  ---Beginning of Stacktrace---" + e.StackTrace + "---End of Stacktrace---" +
                       "\r\n  ---Beginning of BaseException---" + e.GetBaseException() + "---End of GetBaseException---" +
                       "\r\n  ---Beginning of BaseException Stack trace " + e.GetBaseException().StackTrace +
                       "---End of GetBaseException stacktrace---" +
                       "\r\n  ----Beginning of InnerException" + e.InnerException + "---End of InnerException---" +
                       "\r\n  ----Beginning of InnerException StackTrace" + e.InnerException.StackTrace +
                       "---End of InnerException StackTrace---";

                mSocketReady = false;
                WriteToLogFile(vMessage);

            }
        }

        /// <summary>
        /// Attempt to open the client socket to the windows service
        /// </summary> 
        /// <exception cref="InvalidOperationException"> The TcpClient is not connected to a remote host.</exception>
        /// <exception cref="ObjectDisposedException">The TcpClient has been closed.</exception>      
         /**
        * OpenSocket( )
        * @brief Sends a message to the server with the provided message 
        * @note Please not that this will throw an exception if TcpClient is not connected to a remote host 
        * or if the TcpClient has been closed
        */
        public void OpenSocket()
        {
            try
            {
                mSocket = new TcpClient(ClientSocketSettings.ConnectionName, ClientSocketSettings.Port);
                mSocket.NoDelay = true;
                mNetworkStream = mSocket.GetStream();
                mStreamWriter = new StreamWriter(mNetworkStream);
                mStreamReader = new StreamReader(mNetworkStream);
                mSocketReady = true;
            }
            catch (SocketException)
            {
                HeddokoPacket vPacket = new HeddokoPacket(HeddokoCommands.ClientError, "A connection to the Heddoko service failed. Please ensure that " +
                                                                                       "the service is operational and try again.");
                PacketCommandRouter.Instance.Process(this, vPacket);

            }
            catch (Exception e)
            {

                mSocketReady = false;
            }
        }

        /// <summary>
        /// Threaded function that will receive data from the network stream if avaiable, routes the commands over 
        /// to the CommandRouter singleton. Closes the thread if no longer working
        /// </summary>
        /**
       * ReadServerSocket( )
       * @brief Threaded function that will receive data from the network stream if avaiable, routes the commands over 
       * to the CommandRouter singleton. Closes the thread if no longer working
       *  
       */
        int resetClientSocket = -1;
        public void ReadServerSocket()
        {
            resetClientSocket++;
            string debugcommand = "";
            while (true)
            {
                string result = "";
                try
                {
                    if (mNetworkStream.DataAvailable)
                    {
                        result = mStreamReader.ReadLine();

                        if (!string.IsNullOrEmpty(result) && result.Contains("<EOL>"))
                        {
                            byte[] vConverted = PacketSetting.Encoding.GetBytes(result);
                            HeddokoPacket vPacket = new HeddokoPacket(vConverted, 4);
                            string command = vPacket.Command;

                            PacketCommandRouter.Instance.Process(command, vPacket);
                            mNetworkStream.Flush();
                        }
                        mSocketReady = false;
                    }
                    if (!mIsWorking)
                    {
                        CloseSocket();
                        break;
                    }
                }
                catch (Exception e)
                {
                    string vMessage = "Internal failure with  " +
                       "  command" + debugcommand + ". ResetValues client times: " + resetClientSocket + " at Current time " + DateTime.Now +
                       "\r\n ---Beginning of Stacktrace---" + e.StackTrace + "---End of Stacktrace---" +
                       "\r\n ---Beginning of BaseException---" + e.GetBaseException() + "---End of GetBaseException---" +
                       "\r\n ---Beginning of BaseException Stack trace " + e.GetBaseException().StackTrace +
                       "---End of GetBaseException stacktrace---" +
                       "\r\n ----Beginning of InnerException" + e.InnerException + "---End of InnerException---" +
                       "\r\n ----Beginning of InnerException StackTrace" + e.InnerException.StackTrace +
                       "---End of InnerException StackTrace---"
                    + "\r\n Message received: " + result;
                    WriteToLogFile(vMessage); 
                    mSocketReady = false;
                }

            }

        }

        /// <summary>
        /// Writes to a log file 
        /// </summary>
        /// <param name="vPacket"></param>
        public static void WriteToLogFile(string vPacket)
        {
            //Check if folder exists

            string vSubPath = Directory.GetCurrentDirectory() + "/Logs";
            bool vPathExists = Directory.Exists(vSubPath);
            if (!vPathExists)
            {
                Directory.CreateDirectory(vSubPath);
            }

            string vLogMessage = "\r\n<Date = " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + "\\Date>";
            vLogMessage += "\r\n <Log>" + vPacket + "\r\n<\\Log>";

            //file name
            string vPath = vSubPath + "/RawLog_" + DateTime.Now.ToString(@"MM-dd-yyyy") + ".txt";

            //check if file exists
            bool mFileExists = File.Exists(vPath);
            if (!mFileExists)
            {
                using (StreamWriter sw = File.CreateText(vPath))
                {
                    sw.Write(vLogMessage);
                }

            }
            else
            {
                //check the file size first, if its over 250 mb, then don't write.
                FileInfo vFileInfo = new FileInfo(vPath);
                if (vFileInfo.Length > 250000000)
                {
                    return;
                }
                using (StreamWriter sw = File.AppendText(vPath))
                {
                    sw.Write(vLogMessage);
                }
            }

        }

        private void WriteToLog(string vLog)
        {

        }
        /// <summary>
        /// Closes the socket and releases the current resources
        /// </summary> 
        public void CloseSocket()
        {
            if (!mSocketReady)
            {
                return;
            }
            mStreamWriter.Close();
            mStreamReader.Close();
            mSocket.Close();
            mSocketReady = false;
        }
        /// <summary>
        /// Stops the current thread
        /// </summary>
        /**
       * Stop( )
       * @brief Stops the current thread
       *  
       */
        public void Stop()
        {
            mIsWorking = false;
        }

        /// <summary>
        /// Initializes the current SocketClient with the given socket client settings
        /// </summary>
        /// <param name="vSettings">ISocketClientSetting vSettings: socket client settings to initialize the current socket with </param>
        /**
        *Initialize(ISocketClientSetting vSettings)
        * @brief Initializes the current SocketClient with the given socket client
        * @param ISocketClientSetting vSettings: socket client settings to initialize the current socket with 
        */
        public void Initialize(ISocketClientSetting vSettings)
        {
            ClientSocketSettings = vSettings;
            mWorkerThread = new Thread(ReadServerSocket);
            mWorkerThread.IsBackground = true;
            OpenSocket();
        }
        /// <summary>
        /// Starts the current worker thread
        /// </summary> 
        /**
        * StartWorking()
        * @brief  Starts the current worker thread 
        */
        public void StartWorking()
        {
            if (mWorkerThread != null)
            {
                mIsWorking = true;
                mWorkerThread.Start();
            }
        }
    }
}
