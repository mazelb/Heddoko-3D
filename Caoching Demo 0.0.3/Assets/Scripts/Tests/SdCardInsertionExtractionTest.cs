// /**
// * @file SdCardInsertionExtractionTest.cs
// * @brief Contains the SdCardInsertionExtractionTest class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.IO;
using System.Linq;
using Assets.Scripts.MainApp;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Test to verify functionality of HeddokoSdCardSearcher
    /// </summary>
    public class SdCardInsertionExtractionTest: MonoBehaviour
    {
        private HeddokoSdCardSearcher mHeddokoSdCardSearcher;
        public void Awake()
        {
            var dir = Directory.GetLogicalDrives();

            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(()=>Debug.Log("nothing"));

            //foreach (var vS in dir)
            //{

            //    DirectoryInfo vInfo = new DirectoryInfo(vS);
            //    foreach (var vFileSystemInfo in vInfo.GetFileSystemInfos())
            //    {
            //        var vFilesy = vFileSystemInfo;
            //        Debug.Log(vFilesy.Name);
            //    }
            //    Debug.Log(vInfo.Attributes);
            //}
            mHeddokoSdCardSearcher = new HeddokoSdCardSearcher();
            mHeddokoSdCardSearcher.DriveFoundEvent += DriveFoundHandler;
            mHeddokoSdCardSearcher.HeddokoDriveDisconnectedEvent += DriveDisconnectedEvent;
            mHeddokoSdCardSearcher.Start();
        }

        void OnApplicationQuit()
        {
            mHeddokoSdCardSearcher.Stop();
            
        }
        private void DriveDisconnectedEvent()
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => Debug.Log("Heddoko sd card has been disconnected"));

            
        }

        private void DriveFoundHandler(DirectoryInfo vVdrive)
        {  
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(()=>Debug.Log("Drive has been connected "+ vVdrive.Name));
        }
    }
}