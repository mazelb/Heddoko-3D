/** 
* @file DebugLogger.cs
* @brief Contains the DebugLogger  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils.DebugContext.logging
{
    /// <summary>
    /// 
    /// </summary>
    public class DebugLogger
    {
        public static DebugLogSettings Settings = new DebugLogSettings();
        private static DebugLogger sInstance;
        private static object mInstanceLock = new object();
        // DebugSettingsFileMonitor mFileMonitor = new DebugSettingsFileMonitor();

        private string mLogDirPath;

        private Queue<Log> mMessageQueue = new Queue<Log>();
        private Dictionary<LogType, Func<bool>> sSettingsRegistry = new Dictionary<LogType, Func<bool>>();
        private Dictionary<LogType, OutputLogPath> mLogTypeToLogpathType = new Dictionary<LogType, OutputLogPath>();

        private bool mContinueWorking;
        public static DebugLogger Instance
        {
            get
            {
                lock (mInstanceLock)
                {
                    if (sInstance == null)
                    {

                        sInstance = new DebugLogger();
                        sInstance.Register();
                        sInstance.Start();
                    }
                }
                return sInstance;
            }
        }
 
        public void LogMessage(LogType vType, string vMsg)
        {
            try
            {
                bool vCanLog = sSettingsRegistry[vType].Invoke();
                if (vCanLog)
                {
                    Log vLog = new Log();
                    vLog.LogType = vType;
                    string vLogmsg = DateTime.Now.ToString("HH:mm:ss.fff tt") + " , " + ((int)vLog.LogType) + " , " +
                                     vMsg;
                    vLog.Message = vLogmsg;
                    Instance.mMessageQueue.Enqueue(vLog);
                }
            }
            catch (Exception e)
            {

                Debug.Log("<color=red>Fatal error:</color> " + e);
            }

        }
        private void Register()
        {
            RegisterSetting();
            RegisterPaths();
        }

        private void RegisterSetting()
        {
            sSettingsRegistry.Add(LogType.ApplicationCommand, () =>
            {
                if (Settings.LogAll || Settings.LogAllApplicationContext)
                    return true;
                return Settings.ApplicationCommandLog;
            });

            sSettingsRegistry.Add(LogType.ApplicationFrame, () =>
            {
                if (Settings.LogAll || Settings.LogAllApplicationContext)
                    return true;
                return Settings.ApplicationFrameData;
            });

            sSettingsRegistry.Add(LogType.ApplicationResponse, () =>
            {
                if (Settings.LogAll || Settings.LogAllApplicationContext)
                    return true;
                return Settings.ApplicationResponseLog;
            });

            sSettingsRegistry.Add(LogType.BrainpackCommand, () =>
            {
                if (Settings.LogAll || Settings.LogAllBrainpackContext)
                    return true;
                return Settings.BrainpackCommandLog;
            });
            sSettingsRegistry.Add(LogType.BrainpackFrame, () =>
            {
                if (Settings.LogAll || Settings.LogAllBrainpackContext)
                    return true;
                return Settings.BrainpackFrameData;
            });
            sSettingsRegistry.Add(LogType.BrainpackResponse, () =>
            {
                if (Settings.LogAll || Settings.LogAllBrainpackContext)
                    return true;
                return Settings.BrainpackResponseLog;
            });
            sSettingsRegistry.Add(LogType.SocketClientSend, () =>
            {
                if (Settings.LogAll)
                    return true;
                return Settings.SocketClientLogging;
            });
            sSettingsRegistry.Add(LogType.SocketClientSettings, () =>
            {
                if (Settings.LogAll)
                    return true;
                return Settings.SocketClientLogging;
            });
            sSettingsRegistry.Add(LogType.SocketClientReceive, () =>
            {
                if (Settings.LogAll)
                    return true;
                return Settings.SocketClientLogging;
            });
            sSettingsRegistry.Add(LogType.SocketClientError, () =>
            {
                if (Settings.LogAll)
                    return true;
                return Settings.SocketClientLogging;
            });
            sSettingsRegistry.Add(LogType.UnitTest, () => { return true; });
            sSettingsRegistry.Add(LogType.InboundBpBufferException, () =>
            {
                if (Settings.LogAll)
                {
                    return true;
                }
                return false;
            });
            sSettingsRegistry.Add(LogType.SegmentUpdateStart, () =>
            {
                if (Settings.LogAll)
                {
                    return true;
                }
                return false;
            });
            sSettingsRegistry.Add(LogType.SegmentUpdateFinish, () =>
            {
                if (Settings.LogAll)
                {
                    return true;
                }
                return false;
            });
            sSettingsRegistry.Add(LogType.FrameRenderingFinish, () =>
            {
                if (Settings.LogAll)
                {
                    return true;
                }
                return false;
            });
            sSettingsRegistry.Add(LogType.FrameRenderingStart, () =>
            {
                if (Settings.LogAll)
                {
                    return true;
                }
                return false;
            });
            sSettingsRegistry.Add(LogType.BodyFrameThreadStart, () =>
            {
                if (Settings.LogAll)
                {
                    return true;
                }
                return false;
            });
            sSettingsRegistry.Add(LogType.BodyFrameThreadEnd, () =>
            {
                if (Settings.LogAll)
                {
                    return true;
                }
                return false;
            });
            sSettingsRegistry.Add(LogType.Downloading, () =>
            {
                return true;
            } );
            sSettingsRegistry.Add(LogType.Uploading, () =>
            {
                return true;
            });

        }

        private void RegisterPaths()
        { 
            string vms = Instance.mLogDirPath = OutterThreadToUnityThreadIntermediary.Instance.ApplicationPath + "\\Application_logs";

            mLogTypeToLogpathType.Add(LogType.ApplicationCommand, OutputLogPath.ApplicationLog);
            mLogTypeToLogpathType.Add(LogType.ApplicationResponse, OutputLogPath.ApplicationLog);
            mLogTypeToLogpathType.Add(LogType.ApplicationFrame, OutputLogPath.ApplicationFrames);
            mLogTypeToLogpathType.Add(LogType.FileConversionException, OutputLogPath.ApplicationLog);
            mLogTypeToLogpathType.Add(LogType.BrainpackCommand, OutputLogPath.BrainpackMsgLog);
            mLogTypeToLogpathType.Add(LogType.BrainpackResponse, OutputLogPath.BrainpackMsgLog);
            mLogTypeToLogpathType.Add(LogType.BrainpackFrame, OutputLogPath.BrainpackFrames);
            mLogTypeToLogpathType.Add(LogType.SocketClientReceive, OutputLogPath.SocketClientLog);
            mLogTypeToLogpathType.Add(LogType.SocketClientSend, OutputLogPath.SocketClientLog);
            mLogTypeToLogpathType.Add(LogType.SocketClientSettings, OutputLogPath.SocketClientLog);
            mLogTypeToLogpathType.Add(LogType.SocketClientError, OutputLogPath.SocketClientLog);
            mLogTypeToLogpathType.Add(LogType.UnitTest, OutputLogPath.UnitTestPriorityQueue);
            mLogTypeToLogpathType.Add(LogType.InboundBpBufferException, OutputLogPath.ApplicationLog);
            mLogTypeToLogpathType.Add(LogType.SegmentUpdateStart, OutputLogPath.MappingLog);
            mLogTypeToLogpathType.Add(LogType.SegmentUpdateFinish, OutputLogPath.MappingLog);
            mLogTypeToLogpathType.Add(LogType.FrameRenderingStart, OutputLogPath.MappingLog);
            mLogTypeToLogpathType.Add(LogType.FrameRenderingFinish, OutputLogPath.MappingLog);
            mLogTypeToLogpathType.Add(LogType.BodyFrameThreadStart, OutputLogPath.MappingLog);
            mLogTypeToLogpathType.Add(LogType.BodyFrameThreadEnd, OutputLogPath.MappingLog);
            mLogTypeToLogpathType.Add(LogType.Uploading, OutputLogPath.ApplicationLog);
            mLogTypeToLogpathType.Add(LogType.Downloading, OutputLogPath.ApplicationLog);

        }

        public void Start()
        {
            mContinueWorking = true;
            Thread vThread = new Thread(Instance.WorkerTask);
            vThread.Start();
            vThread.IsBackground = true;
            // mFileMonitor.Start();
        }

        public void Stop()
        {
            mContinueWorking = false; 
        }
        private void WorkerTask()
        {
            while (mContinueWorking)
            {
                if (mMessageQueue.Count != 0)
                {
                    Log vLog = mMessageQueue.Dequeue();
                    WriteFile(vLog);
                    
                    Thread.Sleep(1);
                }
            }
        }

        public string GetLogPath(LogType vType)
        {
            string vLogType = mLogTypeToLogpathType[vType].ToString();
            string vTodaysDate = DateTime.Now.ToString("yy-MM-dd");
            string vCurrentFilePath = mLogDirPath + "\\" + vLogType + vTodaysDate;
            return vCurrentFilePath;
        }
        private void WriteFile(Log vLog)
        {
            try
            {
                string vLogType = mLogTypeToLogpathType[vLog.LogType].ToString();
                string vCurrentFilePath = GetLogPath(vLog.LogType) + ".csv";
                string vTodaysDate = DateTime.Now.ToString("yy-MM-dd");
                //check if log directory exists first, create it
                if (!Directory.Exists(mLogDirPath))
                {
                    Directory.CreateDirectory(mLogDirPath);
                }


                //Get all the current files stored in this directory
                //
                string[] vFiles = Directory.GetFiles(mLogDirPath);
                //get the files that are of the log type and are less than one and are from todays day
                string[] vFound = vFiles.Where(f =>
                  f.Contains(vLogType.ToString())
                  && f.Contains(vTodaysDate)
                  && (new FileInfo(f).Length / 1000000) < Settings.MaxFileSizeMb
                  ).ToArray();

                if (vFound.Length > 0)
                {
                    vCurrentFilePath = vFound[0];
                }
                else
                {
                    int vCount = vFound.Length;
                    vCurrentFilePath = mLogDirPath + "\\" + vLogType.ToString() + vCount + "_" + vTodaysDate + ".csv";
                    FileStream vFs = File.Create(vCurrentFilePath);
                    vFs.Close();
                }

                //append to the file
                FileStream vFile = new FileStream(vCurrentFilePath, FileMode.Append, FileAccess.Write);
                StreamWriter vStreamWriter = new StreamWriter(vFile);
                vStreamWriter.WriteLine(((int)vLog.LogType) + " , " + vLog.Message);
                vStreamWriter.Close();
            }
            catch (Exception e)
            {
                string vMessage = vLog.LogType.ToString() + " " + vLog.Message;
                Debug.Log("<color=red>Failed to write to log type:</color> " + vLog + " " + e);
            }
        }
    }

    public enum LogType
    {
        BrainpackCommand = 1,
        BrainpackResponse = 2,
        BrainpackFrame = 3,
        ApplicationCommand = 4,
        ApplicationResponse = 5,
        ApplicationFrame = 6,
        SocketClientSend = 7,
        SocketClientReceive = 8,
        SocketClientError = 9,
        SocketClientSettings = 10,
        UnitTest = 11,
        FileConversionException = 12,
        InboundBpBufferException = 13,
        FrameRenderingStart,
        FrameRenderingFinish,
        SegmentUpdateStart,
        SegmentUpdateFinish,
        BodyFrameThreadEnd,
        BodyFrameThreadStart,
        Uploading,
        Downloading
    }

    public enum OutputLogPath
    {
        ApplicationLog,
        BrainpackMsgLog,
        BrainpackFrames,
        ApplicationFrames,
        SocketClientLog,
        UnitTestPriorityQueue,
        MappingLog
    }

    public struct Log
    {
        public string Message;
        public LogType LogType;
    }
}
