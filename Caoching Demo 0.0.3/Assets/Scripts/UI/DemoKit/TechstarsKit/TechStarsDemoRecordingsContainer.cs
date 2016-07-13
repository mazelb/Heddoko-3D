/**
* @file TechStarsDemoRecordingsContainer.cs
* @brief Contains the TechStarsDemoRecordingsContainer class
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Frames_Recorder.FramesRecording;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    /// <summary>
    /// A container of recordings to be used at the tech stars demo
    /// </summary>
    public class TechStarsDemoRecordingsContainer 
    {
        private string mRecordingsDirectory;
        private List<  BodyFramesRecordingBase> mRecordings = new List<BodyFramesRecordingBase>();
        private int mCurrentRecordingIndex = 0;
        public List<BodyFramesRecordingBase> Recordings
        {
            get { return mRecordings; }
            set { mRecordings = value; }
        }

        public void ScanRecordings(string vRecordingsDir)
        {
            mRecordingsDirectory = vRecordingsDir;
            DirectoryInfo vDirectoryInfo =  new DirectoryInfo(mRecordingsDirectory);
            var vCsvFiles = vDirectoryInfo.GetFiles("*.csv");
            var vDatFiles = vDirectoryInfo.GetFiles("*.dat");
            FileInfo[] vFilesInfos = new FileInfo[vCsvFiles.Length+ vDatFiles.Length];
            Array.Copy(vCsvFiles,vFilesInfos,vCsvFiles.Length);
            Array.Copy(vDatFiles,0,vFilesInfos, vCsvFiles.Length,vDatFiles.Length);

             Array.Sort(vFilesInfos);  
            for (int i = 0; i < vCsvFiles.Length; i++)
            { 
                BodyRecordingsMgr.Instance.ReadRecordingFile(vCsvFiles[i].FullName,x=> mRecordings.Add(x));
            } 

        }


        public string NextRecording()
        {
            return "";
        }

        public string PreviousRecording()
        {
            return "";
        }
    }

}
