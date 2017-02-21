// /**
// * @file SdCardContentUploadController.cs
// * @brief Contains the SdCardContentUploadController
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils.DebugContext.logging;
using HeddokoSDK.Models;
using UnityEngine;
using LogType = Assets.Scripts.Utils.DebugContext.logging.LogType;

namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void ContentsCompletedUpload();
    public delegate void ProblemUploadingContent(List<ErrorUploadEventArgs> vArgs);

    public delegate void SingleUploadingStartEvent(UploadableListItem vItem);

    public delegate void SingleUploadingCompleted(UploadableListItem vItem);
    /// <summary>
    /// A controller to allow contents from the sd card to be uploaded to the cloud
    /// </summary>
    public class SdCardContentUploadController
    {
        public event ContentsCompletedUpload ContentsCompletedUploadEvent;
        public event ProblemUploadingContent ProblemUploadingContentEvent;
        public event HeddokoDriveFound DriveFoundEvent;
        public event HeddokoDriveDisconnected DriveDisconnectedEvent;
        public event SingleUploadingStartEvent UploadingStartEvent;
        public event SingleUploadingCompleted SingleUploadEndEvent;
        public event FoundFileList FoundFileListEvent;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private UserProfileModel mUserProfileModel;
        private AssetUploader mUploader;
        private HeddokoDirectoryContentSearch mHeddokoDirectoryContentSearch;
        private HeddokoSdCardSearcher mSearcher;
        private List<string> mForbiddenFileList;
        private UploadRecordingStatus mUploadRecordingStatus;
        private bool mIsWorking;
        private object mLockObject = new object();
        private Thread mWorker;
        private List<FileInfo> mFoundRecordingsList = new List<FileInfo>();

        public UploadableListItem BrainpackLogFileItem;
        /// <summary>
        /// Is the current thread worker working?
        /// </summary>
        private bool IsWorking
        {
            get
            {
                bool vReturn;
                lock (mLockObject)
                {
                    vReturn = mIsWorking;
                }
                return vReturn;
            }
            set
            {
                lock (mLockObject)
                {
                    mIsWorking = value;
                }
            }
        }


        public bool CanUpload { get; set; }

        public List<FileInfo> FoundRecordingsList
        {
            get { return mFoundRecordingsList; }
        }

        /// <summary>
        /// Constructor accepting a profile model. This constructor builds the recordingsuploader and sets a set of
        /// rules for fetching files from the sd card
        /// </summary>
        /// <param name="vProfileModel"></param>
        public SdCardContentUploadController(UserProfileModel vProfileModel)
        {
            mForbiddenFileList = new List<string> { "logIndex.dat", "settings.dat" };
            mUserProfileModel = vProfileModel;
            mSearcher = new HeddokoSdCardSearcher();
            mSearcher.DriveFoundEvent += FoundSdCardEventHandler;
            mSearcher.Start();
            mUploader = new AssetUploader(mUserProfileModel);
            mUploader.UploadCompleteEvent += UploadCompletionHandler;
            mUploader.UploadErrorEvent += SingleRecordingErrorHandler;
        }

        /// <summary>
        /// A single recording error handler
        /// </summary>
        /// <param name="vArgs"></param>
        private void SingleRecordingErrorHandler(ErrorUploadEventArgs vArgs)
        {
            UploadableListItem vItem = vArgs.Object;
            if (vItem != null)
            {
                if (!mUploadRecordingStatus.ProblematicUploads.ContainsKey(vItem.FileName))
                {
                    mUploadRecordingStatus.ProblematicUploads.Add( vItem.FileName, vArgs);
                }
            }
        }

        /// <summary>
        /// upload completion handler
        /// </summary>
        /// <param name="vItem"></param>
        private void UploadCompletionHandler(UploadableListItem vItem)
        {
            switch (vItem.AssetType)
            {
                case AssetType.Log:
                    BrainpackLogFileUploadHandler();
                    break;
                case AssetType.Record:
                    RecordingUploadHandler(vItem);
                    break;
                case AssetType.RawFrameData:
                    RecordingUploadHandler(vItem);
                    break;
            }

        }

        /// <summary>
        /// A handler on successful recording upload
        /// </summary>
        /// <param name="vItem"></param>
        void RecordingUploadHandler(UploadableListItem vItem)
        {
            mUploadRecordingStatus.SucessfullyUploadedRecordings.Add(vItem);
            if (SingleUploadEndEvent != null)
            {
                SingleUploadEndEvent(vItem);
            }
            try
            {
                //move a file to a backup  
                var vFileName =vItem.FileName;
                var vFullFilePath  = ApplicationSettings.BrainpackDownloadCacheFolderPath + Path.DirectorySeparatorChar + vFileName;
                //rename file if exists
                int vCounter = 0;
                if (File.Exists(vFullFilePath))
                {
                    var vFileInfo = new FileInfo(vFullFilePath);
                    while (File.Exists(vFullFilePath))
                    {
                        var vTempFilePath = Path.GetFileNameWithoutExtension(vFullFilePath);
                        vTempFilePath += vCounter++;
                        vTempFilePath += vFileInfo.Extension;
                        vFullFilePath = vTempFilePath;
                    }
                }
                File.Move(vItem.RelativePath,vFullFilePath); 
            }
            catch (Exception vE)
            {
               DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Issue moving file "+vE.Message + " ,"+ vE.StackTrace);
            }

        }

        /// <summary>
        /// Handler for succesful file upload handler.      
        /// </summary>
        void BrainpackLogFileUploadHandler()
        {
            //delete file
            FileInfo vBpLogFileItem = new FileInfo(BrainpackLogFileItem.RelativePath);

            var vBackupDirPath = vBpLogFileItem.DirectoryName + Path.DirectorySeparatorChar + "Backup";
            if (!Directory.Exists(vBackupDirPath))
            {
                Directory.CreateDirectory(vBackupDirPath);
            }

            var vFileName = vBpLogFileItem.Name;
            int vIndex = 0;
            while (true)
            {
                if (File.Exists(vBackupDirPath + Path.DirectorySeparatorChar + vFileName + vIndex))
                {
                    vIndex++;
                    continue;
                }

                vBpLogFileItem.CopyTo(vBackupDirPath + Path.DirectorySeparatorChar + vFileName + vIndex);
                vBpLogFileItem.Delete();
                break;
            }

        }

        /// <summary>
        /// Starts a worker and begins to search for recordings from the sd card.
        /// </summary>
        public void StartRecordingsSearch()
        {
            if (mHeddokoDirectoryContentSearch != null)
            {
                //unregister from previous events. 
                mHeddokoDirectoryContentSearch.FoundFileListEvent -= FoundRecordingsEventHandler;
            }
            //reinitialize mUploadRecordingStatus
            mUploadRecordingStatus = new UploadRecordingStatus();
            //instantiate a new Directory Content searcher
            mHeddokoDirectoryContentSearch = new HeddokoDirectoryContentSearch(mSearcher.FoundDrive, "*.dat", mForbiddenFileList);
            mHeddokoDirectoryContentSearch.FoundFileListEvent += FoundRecordingsEventHandler;
            mHeddokoDirectoryContentSearch.Search();
        }

        /// <summary>
        ///   handler that processes recordings found events
        /// </summary>
        /// <param name="vFileInfos"></param>
        private void FoundRecordingsEventHandler(List<FileInfo> vFileInfos)
        {
            if (FoundFileListEvent != null)
            {
                FoundFileListEvent(vFileInfos);
            }
            mFoundRecordingsList = vFileInfos;
        }

        /// <summary>
        /// Begins the recordings upload from the list of found recordings. Non blocking.
        /// </summary>
        public void StartContentUpload()
        {
            mIsWorking = false;
            mUploadRecordingStatus.ProblematicUploads.Clear();
            mWorker = new Thread(UploadFoundContent);
            mWorker.IsBackground = true;
            mWorker.Start();
        }
        public void FilterFileInfo(ref List<FileInfo> vFileInfos)
        {
            //remove files, use a stack to insert largest indices last.
            Stack<int> vIndices = new Stack<int>();
            for (int i = 0; i < vFileInfos.Count; i++)
            {
                if (vFileInfos[i].Name.Contains("logIndex.dat") || vFileInfos[i].Name.Contains("logindex.dat"))
                {
                    vIndices.Push(i);
                }
                //remove directories
                else
                {
                    FileAttributes vAttr = File.GetAttributes(vFileInfos[i].FullName);
                    if ((vAttr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        vIndices.Push(i);
                    }
                }
            }
            while (vIndices.Count > 0)
            {
                int vIndex = vIndices.Pop();
                vFileInfos.RemoveAt(vIndex);
            }
        }
        /// <summary>
        /// uploads the current list of found recordings items, a blocking operation
        /// </summary>
        public void UploadFoundContent()
        {
            mIsWorking = true;
            //upload recordings
            FilterFileInfo(ref mFoundRecordingsList);
            if (mFoundRecordingsList.Count != 0)
            {
                for (int vI = 0; vI < mFoundRecordingsList.Count; vI++)
                {
                    var vRecItem = mFoundRecordingsList[vI];
                    if (!IsWorking)
                    {
                        break;
                    }
                    var vUploadItem = new UploadableListItem();
                    vUploadItem.AssetType = AssetType.RawFrameData;
                    vUploadItem.RelativePath = vRecItem.FullName;
                    vUploadItem.FileName = vRecItem.Name;
                    vUploadItem.IsNew = true;
                    vUploadItem.BrainpackSerialNumber = BrainpackLogFileItem.BrainpackSerialNumber;

                    if (UploadingStartEvent != null)
                    {
                        UploadingStartEvent(vUploadItem);
                    }
                    mUploader.UploadSingleItem(vUploadItem);
                }
                UploadBrainpackLogData();
            }

            //On completion handle errors and succesful uploads
            bool vHasProblematicResults = mUploadRecordingStatus.ProblematicUploads != null && mUploadRecordingStatus.ProblematicUploads.Count > 0;
            if (vHasProblematicResults)
            {
                if (ProblemUploadingContentEvent != null)
                {
                    ProblemUploadingContentEvent(mUploadRecordingStatus.ProblematicUploads.Values.ToList());
                }
            }

            if (ContentsCompletedUploadEvent != null)
            {
                ContentsCompletedUploadEvent();
            }
            if (mUploadRecordingStatus.ProblematicUploads != null)
            {
                mUploadRecordingStatus.ProblematicUploads.Clear();
            }
        }

        /// <summary>
        /// Upload brainpack log data
        /// </summary>
        void UploadBrainpackLogData()
        {
            if (BrainpackLogFileItem != null && mSearcher.HeddokoSdCardStruct.LogFileInRootDir)
            {
                mUploader.UploadSingleItem(BrainpackLogFileItem);
            }
        }


        /// <summary>
        /// Handler that handles SD card found events
        /// </summary>
        /// <param name="vDriveInfo"></param>
        private void FoundSdCardEventHandler(DirectoryInfo vDriveInfo)
        {
            if (DriveFoundEvent != null)
            {
                DriveFoundEvent(vDriveInfo);
            }
            UpdateBrainpackLogInfo(vDriveInfo);
            StartRecordingsSearch();
        }

        /// <summary>
        /// Cleans up resources after use
        /// </summary>
        public void CleanUp()
        {
            mSearcher.Stop();
            IsWorking = false;
        }

        protected virtual void OnDriveDisconnectedEvent()
        {
            var vHandler = DriveDisconnectedEvent;
            if (vHandler != null)
            {
                vHandler();
            }
        }

        /// <summary>
        /// Update BrainpackLogFile information
        /// </summary>
        /// <param name="vDrive"></param>
        private void UpdateBrainpackLogInfo(DirectoryInfo vDrive)
        {
            //get brainpack serial number
            string vBpSerial = mSearcher.GetSerialNumFromSdCard();
             var vLogFileInfo = mSearcher.HeddokoSdCardStruct;
            if (vBpSerial != null)
            {
                BrainpackLogFileItem = new UploadableListItem()
                {
                    FileName = vLogFileInfo.LogFileName,
                    RelativePath = vLogFileInfo.LogFilePath,
                    BrainpackSerialNumber = vBpSerial,
                    AssetType = AssetType.Log
                };
            } 
        }
    }


    public class UploadRecordingStatus
    {
        /// <summary>
        /// a list of recordings who have had problems uploading
        /// </summary>
        public Dictionary<string,ErrorUploadEventArgs> ProblematicUploads = new Dictionary<string,ErrorUploadEventArgs>();
        public List<UploadableListItem> SucessfullyUploadedRecordings = new List<UploadableListItem>();
    }
}
