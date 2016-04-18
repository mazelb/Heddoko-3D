using System;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using BrainpackService.bluetooth_connector.BrainpackInterfaces;
using BrainpackService.BrainpackServer;
using BrainpackService.Tools_and_Utilities.Debugging;
using HeddokoLib.adt;
using HeddokoLib.networking;
using HeddokoLib.utils;


namespace BrainpackService.brainpack_serial_connect
{
    public class BrainpackSerialConnector : IBrainpackConnection
    {
        private Thread mWorkerThread;
        private bool IsWorking { get; set; }
        private string mPortName; 
        private object mLatestStateLock = new object();
        //in milliseconds
        private const int gSerialPortTimeout = 15000;
        private static BrainpackSerialConnector sInstance; 
        private bool mSerialIsBeingProbed = false;
        string mBpStateSearchPattern = "(?i)Reset(?-i)|(?i)Idle(?-i)|(?i)Recording(?-i)|(?i)Error(?-i)";
        private bool mIsRecording;
        private string mLatestState { get; set; }
        SerialPort Serialport { get; set; } = new SerialPort();
       
        CircularQueue<string> OutboundBuffer { get; } = new CircularQueue<string>(10, true);
        CircularQueue<string> ResponseBuffer { get; } = new CircularQueue<string>(50, true);
        //private CircularQueue<string> mStateBuffer { get; } = new CircularQueue<string>(10, true);
        public ServerCommandRouter ServerCommandRouter { get; set; }
        public static BrainpackSerialConnector Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new BrainpackSerialConnector();
                }
                return sInstance;
            }
        }
        /// <summary>
        /// Initialize the serialport with the given serialport name
        /// </summary>
        /// <param name="vBrainpackName"> The serial portn name</param>
        public void Initialize(string vBrainpackName)
        {
            SerialPortOpen(vBrainpackName);
        }
        /// <summary>
        /// Try to set the serial port
        /// </summary>
        /// <param name="vNewPort">The name of the serial port</param>
        private void SerialPortOpen(string vNewPort)
        {
            try
            {

                if (vNewPort != mPortName)
                {
                    Stop();
                    CloseSerialPort();
                    Serialport = new SerialPort();
                    Serialport.ReadTimeout = gSerialPortTimeout;
                    Serialport.WriteTimeout = gSerialPortTimeout;
                    mPortName = vNewPort;
                    Serialport.PortName = mPortName;
                    Serialport.NewLine = "\r\n";
                    mWorkerThread = new Thread(ThreadedFunction);
                    Serialport.BaudRate = 115200;
                    Serialport.Open();
                    Start();
                }
                if (vNewPort == mPortName && Serialport != null && !Serialport.IsOpen)
                {


                    CloseSerialPort();
                    mWorkerThread = new Thread(ThreadedFunction);
                    Serialport = new SerialPort();
                    Serialport.ReadTimeout = gSerialPortTimeout;
                    Serialport.WriteTimeout = gSerialPortTimeout;
                    mPortName = vNewPort;
                    Serialport.PortName = mPortName;
                    Serialport.NewLine = "\r\n";
                    mWorkerThread = new Thread(ThreadedFunction);
                    Serialport.BaudRate = 115200;
                    Serialport.Open();
                    Start();
                }

                else
                {
                    OutboundBuffer.Clear();
                    Start();
                }

            }

            catch (Exception vE)
            {
                DebugLogger.Instance.LogMessage(LogType.BrainpackSerialPortException, vE.StackTrace);
            }

        }
        /// <summary>
        /// Safely closes the serial port
        /// </summary>
        public void CloseSerialPort()
        {
            if (Serialport.IsOpen)
            {
                try
                {
                    Serialport.Close();
                }
                catch (Exception vE)
                {
                    DebugLogger.Instance.LogMessage(LogType.BrainpackSerialPortException, vE.StackTrace);
                }
            }
        }
        /// <summary>
        /// pulls data from the brainpack
        /// </summary>
        public void ThreadedFunction()
        {

            while (true)
            {

                if (Serialport.IsOpen)
                {
                    try
                    {
                        if (mSerialIsBeingProbed)
                        {
                            return;
                        }
                        string vReadLine = Serialport.ReadLine();

                        DebugLogger.Instance.LogMessage(LogType.BrainpackFrame, vReadLine);
                        if (vReadLine.Length >= 176)
                        {
                            HeddokoPacket vPossibleBrainpackData = new HeddokoPacket(HeddokoCommands.SendBPData, vReadLine);
                            ServerCommandRouter.Process(this, vPossibleBrainpackData);
                        }
                        else if ( vReadLine.Length <= 25 && vReadLine.Length > 0)
                        {
                            string vTemp = vReadLine;

                            lock (mLatestStateLock)
                            {
                                //check for matches and check if a state is set to idle or Recording
                                foreach (var vMatch in Regex.Matches(vReadLine, mBpStateSearchPattern))
                                {

                                    string vState = vMatch.ToString();

                                    string vIdlePattern = @"(?i)Idle(?-i)";
                                    string vRecPattern = @"(?i)Recording(?-i)";
                                    if (Regex.IsMatch(vState, vIdlePattern, RegexOptions.IgnoreCase))
                                    {
                                        mIsRecording = false;
                                    }
                                    else if (Regex.IsMatch(vState, vRecPattern, RegexOptions.IgnoreCase))
                                    {
                                        mIsRecording = true;
                                    }
                                    mLatestState = vState;
                                    vTemp = vTemp.Replace(vState, "");
                                    DebugLogger.Instance.LogMessage(LogType.BrainpackResponse, vReadLine);

                                }

                            }

                            if (vTemp.Length > 10 || vTemp.Contains("ack") || vTemp.Contains("Ack"))
                            {
                                ResponseBuffer.Enqueue(vReadLine);
                           //     HeddokoPacket vResponseData = new HeddokoPacket(HeddokoCommands.SendBPData, vReadLine);
                                
                            }
                        }
                       

                    }
                    catch (IOException vIoException)
                    {
                        DebugLogger.Instance.LogMessage(LogType.BrainpackSerialPortException,
                            vIoException.StackTrace);

                        //BrainpackEventLogManager.InvokeEventLogError(vIoException + "\r\n" + vIoException.StackTrace);

                    }
                    catch (NullReferenceException vNullReferenceException)
                    {
                        DebugLogger.Instance.LogMessage(LogType.BrainpackSerialPortException,
                            vNullReferenceException.StackTrace);

                        //BrainpackEventLogManager.InvokeEventLogError(vNullReferenceException + "\r\n" + vNullReferenceException.StackTrace);
                    }
                    catch (TimeoutException vE)
                    {
                        lock (mLatestStateLock)
                        {
                            mLatestState = "Timeout";
                        }
                        DebugLogger.Instance.LogMessage(LogType.BrainpackSerialPortException, vE.StackTrace);

                        continue;
                    }
                    catch (Exception vE)
                    {
                        DebugLogger.Instance.LogMessage(LogType.BrainpackSerialPortException, vE.StackTrace);

                    }
                }
                if (!IsWorking)
                {
                    Stop();
                    break;
                }
            }


        }



        public void SendCommandToBrainpack(string vMsg)
        {

            try
            {
                Serialport.Write(vMsg + "\r\n");

                DebugLogger.Instance.LogMessage(LogType.BrainpackCommand, vMsg);
            }
            catch (Exception vE)
            {
                DebugLogger.Instance.LogMessage(LogType.BrainpackCommand, vMsg);

            }

        }

        /// <summary>
        /// Send a message to the brainpack to request its current state
        /// </summary>
        private void GetState()
        {
            SendCommandToBrainpack("GetState");
        }

        /// <summary>
        /// Get the most up to date state
        /// </summary>
        /// <returns></returns>
        public string GetLatestState()
        {
            string vTemp = "";
            lock (mLatestStateLock)
            {
                vTemp = mLatestState;
            }
            return vTemp;
        }


        /// <summary>
        /// Probes the brainpack and checks if it recording. only valid if the brainpack 
        /// is connected
        /// </summary>
        /// <returns></returns>
        public bool IsRecording()
        {
            return mIsRecording;
        }
        public void Start()
        {
            if (!IsWorking)
            {
                IsWorking = true;
                mWorkerThread.Start();
            }

        }

        /// <summary>
        /// Stop the connection
        /// </summary>
        public void Stop()
        {
            IsWorking = false;
            OutboundBuffer.Clear();
            if (IsConnected)
            {
                Serialport.Close();
            }
            lock (mLatestStateLock)
            {
                mLatestState = "";
            }
        }

        /// <summary>
        /// Verify if the serial port is open and ready to send message back
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (Serialport != null)
                {
                    return Serialport.IsOpen;
                }
                return false;
            }
        }

        public string GetNextFrame()
        {
            //reset the stop watch 
            if (OutboundBuffer.Count == 0)
            {
                return "";
            }
            else
            {
                return OutboundBuffer.Dequeue();
            }
        }
        /// <summary>
        /// Flushes the current brainpack buffers
        /// </summary>
        public void Clear()
        {
            if (Serialport != null && Serialport.IsOpen)
            {
                Serialport.DiscardInBuffer();
                Serialport.DiscardOutBuffer();
            }
            OutboundBuffer.Clear();
        }

        /// <summary>
        /// From the response queue, return the response received by the brainpack
        /// </summary>
        /// <returns></returns>
        public string GetResponse()
        {
            string vReturn = "";

            if (ResponseBuffer.Count > 0)
            {
                vReturn = ResponseBuffer.Dequeue();
            }

            return vReturn;
        }
    }
}
