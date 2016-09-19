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

namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void ContentsCompletedUpload();
    public delegate void ProblemUploadingContent(ErrorUploadEventArgs vArgs);

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
        private UserProfileModel mUserProfileModel;
        private RecordingUploader mUploader;
        private HeddokoDirectoryContentSearch mHeddokoDirectoryContentSearch;
        private HeddokoSdCardSearcher mSearcher;
        private List<string> vForbiddenFileList = new List<string>();
        private UploadRecordingStatus mUploadRecordingStatus;
        private bool mIsWorking;
        private object mLockObject = new object();
        private Thread mWorker;
        private List<FileInfo> mFoundRecordingsList = new List<FileInfo>();
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

        /// <summary>
        /// Constructor accepting a profile model. This constructor builds the recordingsuploader and sets a set of
        /// rules for fetching files from the sd card
        /// </summary>
        /// <param name="vProfileModel"></param>
        public SdCardContentUploadController(UserProfileModel vProfileModel)
        {
            vForbiddenFileList = new List<string>();
            vForbiddenFileList.Add("logIndex.dat");
            mUserProfileModel = vProfileModel;
            mSearcher = new HeddokoSdCardSearcher();
            mSearcher.DriveFoundEvent += FoundSdCardEventHandler;
            mSearcher.Start();
            mUploader = new RecordingUploader(mUserProfileModel);
            mUploader.UploadCompleteEvent += SingleRecordingCompletionHandler;
            mUploader.UploadErrorEvent += SingleRecordingErrorHandler;
        }

        /// <summary>
        /// A single recording error handler
        /// </summary>
        /// <param name="vArgs"></param>
        private void SingleRecordingErrorHandler(ErrorUploadEventArgs vArgs)
        {
            UploadableListItem vItem = vArgs.Object as UploadableListItem;
            if (vItem != null)
            {
               mUploadRecordingStatus.ProblematicUploads =vArgs;
            }
        }

        /// <summary>
        /// A single recording completion handler
        /// </summary>
        /// <param name="vItem"></param>
        private void SingleRecordingCompletionHandler(UploadableListItem vItem)
        {
            mUploadRecordingStatus.SucessfullyUploadedRecordings.Add(vItem);
            if (SingleUploadEndEvent != null)
            {
                SingleUploadEndEvent(vItem);
            }
            try
            {
                File.Delete(vItem.RelativePath); 
            }
            catch (Exception vE)
            {
                 
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
            mHeddokoDirectoryContentSearch = new HeddokoDirectoryContentSearch(mSearcher.FoundDrive, "*.dat", vForbiddenFileList);
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
        public void StartRecordingsUpload()
        {
            mIsWorking = false;
            mWorker = new Thread(CurrentListOfFoundListOfRecordings);
            mWorker.Start();
        }

        /// <summary>
        /// uploads the current list of found recordings items, a blocking operation
        /// </summary>
        public void CurrentListOfFoundListOfRecordings()
        {
            mIsWorking = true;
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
                    vUploadItem.RelativePath = vRecItem.FullName;
                    vUploadItem.FileName = vRecItem.Name;
                    vUploadItem.IsNew = true;

                    if (UploadingStartEvent != null)
                    {
                        UploadingStartEvent(vUploadItem);
                    }
                    mUploader.UploadSingleRecording(vUploadItem);
                }
                 
            }
            //On completion handle errors and succesful  uploads
            if (mUploadRecordingStatus.ProblematicUploads != null && mUploadRecordingStatus.ProblematicUploads.ErrorCollection.Errors.Count > 0)
            {
                ProblemUploadingContentEvent(mUploadRecordingStatus.ProblematicUploads);
            }
            else
            {
                if (ContentsCompletedUploadEvent != null)
                {
                    ContentsCompletedUploadEvent();
                }
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

    }


    public class UploadRecordingStatus
    {
        /// <summary>
        /// a list of recordings who have had problems uploading
        /// </summary>
        public ErrorUploadEventArgs  ProblematicUploads; 
        public List<UploadableListItem> SucessfullyUploadedRecordings = new List<UploadableListItem>();
    }
}