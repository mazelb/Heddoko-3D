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
using UnityEngine;

namespace Assets.Scripts.MainApp
{
    public delegate void HeddokoDriveFound(DirectoryInfo vDrive);

    public delegate void HeddokoDriveDisconnected();
    /// <summary>
    /// Searches for sd cards with "HEDDOKO" in their names, triggering events on found or disconnected
    /// </summary>
    public class HeddokoSdCardSearcher
    {
        public event HeddokoDriveFound DriveFoundEvent;
        public event HeddokoDriveDisconnected HeddokoDriveDisconnectedEvent;
        private DirectoryInfo mFoundDrive;
        private Thread mWorkerThread;
        private bool mIsWorking;
        public HeddokoSdCardSearcher()
        {
            mWorkerThread = new Thread(Search);
        }

        public DirectoryInfo FoundDrive
        {
            get { return mFoundDrive; }
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
                    if (mFoundDrive == null)
                    {
                        if (vDir !=null)
                        {
                            AssignDrive(vDir); 
                        }
                    }

                    else
                    {
                        try
                        {
                            var vDirectoryInfo = new DirectoryInfo(mFoundDrive.Name);
                            var vFiles = vDirectoryInfo.GetFiles();
                        }
                        catch (DirectoryNotFoundException)
                        {

                           UnassignDrive();
                        }
                    }

                }
            }
            catch (Exception vE)
            {
                string vMes = vE.Message;
                 Debug.Log("exception! "+ vMes);
            }
           

        }

        /// <summary>
        /// Assign the passed in drive info as the currently connected drive.
        /// </summary>
        /// <param name="vDriveInfo">the found drive</param>
        private void AssignDrive(DirectoryInfo vDriveInfo)
        {
            mFoundDrive = vDriveInfo;
            if (DriveFoundEvent != null)
            {
                DriveFoundEvent(mFoundDrive);
            }
        }

        /// <summary>
        /// Set the found heddoko drive to null
        /// </summary>
        private void UnassignDrive()
        {
            mFoundDrive = null;
            if (HeddokoDriveDisconnectedEvent != null)
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





    }
}