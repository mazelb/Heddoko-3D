// /**
// * @file HeddokoSdCardSearch.cs
// * @brief Contains the HeddokoSdCardSearcher class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.IO;
using System.Threading; 
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Scripts.MainApp
{
    public delegate void HeddokoDriveFound(DirectoryInfo vDrive);

    public delegate void HeddokoDriveDisconnected();
    /// <summary>
    /// Searches for sd cards with based on their file hierarchy, triggering events on found or disconnected
    /// </summary>
    public class HeddokoSdCardSearcher
    {
        public event HeddokoDriveFound DriveFoundEvent;
        public event HeddokoDriveDisconnected HeddokoDriveDisconnectedEvent;
         
        private BrainpackSdCardStruct mHeddokoSdCardStruct = new BrainpackSdCardStruct();
        private Thread mWorkerThread;
        private object mLockObject= new object();
        private bool mSdCardIsConnected;

        /// <summary>
        /// is the sd card current connected?
        /// </summary>
        public bool SdCardIsConnected
        {
            get
            {
                lock (mLockObject)
                {
                    return mSdCardIsConnected;
                }
            }
           private set
            {
                lock (mLockObject)
                {
                      mSdCardIsConnected =value;
                }
            }

        }
        private bool mIsWorking;
        public HeddokoSdCardSearcher()
        {
            mWorkerThread = new Thread(Search);
            mWorkerThread.IsBackground = true;
        }

        public DirectoryInfo FoundDrive
        {
            get { return mHeddokoSdCardStruct.DirectoryInfo; }
        }

        /// <summary>
        /// Search for sd drives with the name containing HEDDOKO
        /// </summary>
        private void Search()
        {
            string[] vDriveInfo;
            try
            {
                while (mIsWorking)
                {
                    //play nice with other threads
                    Thread.Sleep(100);
                    vDriveInfo = Directory.GetLogicalDrives();
                    var vDir = SearchForDriveLabel(vDriveInfo);
                    if (mHeddokoSdCardStruct.DirectoryInfo == null)
                    {
                        if (vDir !=null)
                        {
                            AssignDrive(vDir);
                            GetSerialNumFromSdCard();
                        }
                    }

                    else
                    {
                        try
                        {
                            var vDirectoryInfo = new DirectoryInfo(mHeddokoSdCardStruct.DirectoryInfo.Name);
                            var vFiles = vDirectoryInfo.GetFiles();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            UnassignDrive();
                            mHeddokoSdCardStruct.BrainpackSerialNumber = null;
                        }
                    }

                }
            }
            catch (Exception vE)
            {
                string vMes = vE.Message;
                 UnityEngine.Debug.Log("exception! "+ vMes);
            }
        }

        /// <summary>
        /// Assign the passed in drive info as the currently connected drive.
        /// </summary>
        /// <param name="vDriveInfo">the found drive</param>
        private void AssignDrive(DirectoryInfo vDriveInfo)
        {
            SdCardIsConnected = true;
            mHeddokoSdCardStruct.DirectoryInfo = vDriveInfo;
            if (DriveFoundEvent != null && mIsWorking)
            {
                DriveFoundEvent(mHeddokoSdCardStruct.DirectoryInfo);
            }
        }

        /// <summary>
        /// Set the found heddoko drive to null
        /// </summary>
        private void UnassignDrive()
        {
            UnityEngine.Debug.Log("unassignerino ");
            SdCardIsConnected = false;
            mHeddokoSdCardStruct.DirectoryInfo = null;
            if (HeddokoDriveDisconnectedEvent != null && mIsWorking)
            {
                HeddokoDriveDisconnectedEvent();
            }
        }

        /// <summary>
        /// Searches for a heddoko sd card drive
        /// </summary>
        /// <param name="vDrives">the array of drives</param>
        /// <returns></returns>
        private DirectoryInfo SearchForDriveLabel(string[] vDrives)
        { 
            for (int vI = 0; vI < vDrives.Length; vI++)
            {
                try
                {
                    var vDirectoryInfo = new DirectoryInfo(vDrives[vI]);
                    var vSysHdk = vDirectoryInfo.GetFiles("sysHdk.bin");
                    var vSettings = vDirectoryInfo.GetFiles("settings.dat");
                    if (vSysHdk.Length == 1 && vSettings.Length == 1)
                    {
                        return vDirectoryInfo;
                    }

                }
                catch (IOException vE)
                {
                    UnityEngine.Debug.Log(vE);
                    continue;
                }
            } 
                return null;
        }

      
       
        /// <summary>
        /// starts to search for Heddoko sd drives
        /// </summary>
        public void Start()
        {
            mIsWorking = true;
            mWorkerThread.Start();
        }

        public void Stop()
        {
            mIsWorking = false;
        }

        /// <summary>
        /// Returns the brainpack serial number found on the sd card
        /// </summary>
        /// <returns></returns>
        public string GetSerialNumFromSdCard()
        {
            if (mHeddokoSdCardStruct.BrainpackSerialNumber == null)
            {
                var vFiles = mHeddokoSdCardStruct.DirectoryInfo.GetFiles();
                var vLogFile = vFiles.First(vX => vX.Name.Contains("sysHdk.bin"));
                string vBrainpackSerial = null;
                if (vLogFile != null)
                {
                    //get brainpack name
                    using (StreamReader vSr = new StreamReader(mHeddokoSdCardStruct.DirectoryInfo.Name + Path.DirectorySeparatorChar + "sysHdk.bin"))
                    {
                        string vLine;
                        while ((vLine = vSr.ReadLine()) != null)
                        {
                            var vMatch = Regex.Match(vLine, @"S\\d\\d\\d\\d\\d_");
                            if (vMatch.Index >= 0)
                            {
                                vBrainpackSerial = vLine.Substring(vMatch.Index, 6);
                                break;
                            }
                        }
                    }
                }
                mHeddokoSdCardStruct.BrainpackSerialNumber =  vBrainpackSerial;
            }
            return mHeddokoSdCardStruct.BrainpackSerialNumber;
        }
         
        private struct BrainpackSdCardStruct
        {
            public DirectoryInfo DirectoryInfo;
            public string BrainpackSerialNumber;

        }
    }

   
}