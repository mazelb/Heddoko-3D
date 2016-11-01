// /**
// * @file UploadController.cs
// * @brief Contains the UploadController
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Localization;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.DebugContext.logging;
using HeddokoSDK.Models;
using UIWidgets;
using UnityEngine;
using LogType = Assets.Scripts.Utils.DebugContext.logging.LogType;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Upload controller, handling upload events with a heddoko client
    /// </summary>
    public class UploadController : MonoBehaviour
    {
        private SdCardContentUploadController mCardContentUploadController;
     public RecordingListViewController RecordingListViewController;
        private int mErrorCount;
        void Start()
        {
            mCardContentUploadController = new SdCardContentUploadController(UserSessionManager.Instance.UserProfile);
            mCardContentUploadController.DriveFoundEvent += DriveFoundHandler;
            mCardContentUploadController.ContentsCompletedUploadEvent += ContentsCompletedUploadingEvent;
             
            mCardContentUploadController.FoundFileListEvent += FoundFileListHandler;
            mCardContentUploadController.ProblemUploadingContentEvent += ProblemUploadEventHandler;
            mCardContentUploadController.DriveDisconnectedEvent += DriveDisconnectedHandler;
            mCardContentUploadController.UploadingStartEvent += UploadingItemStarted;
        }

        void OnApplicationQuit()
        {
            mCardContentUploadController.DriveFoundEvent -= DriveFoundHandler;
            mCardContentUploadController.ContentsCompletedUploadEvent -= ContentsCompletedUploadingEvent;
            
            mCardContentUploadController.FoundFileListEvent -= FoundFileListHandler;
            mCardContentUploadController.ProblemUploadingContentEvent -= ProblemUploadEventHandler;
            mCardContentUploadController.DriveDisconnectedEvent -= DriveDisconnectedHandler;
            mCardContentUploadController.UploadingStartEvent -= UploadingItemStarted;

            mCardContentUploadController.CleanUp();
        }

        private void SingleRecordingUploaded(UploadableListItem vVitem)
        {

        }

        public void BeginUpload()
        {
            mErrorCount = 0;
            var vMsg = LocalizationBinderContainer.GetString(KeyMessage.BeginUploadProcessMsg);
            Notify.Template("fade").Show(vMsg, 5f, sequenceType: NotifySequence.First);
            mCardContentUploadController.StartContentUpload();
        }
        /// <summary>
        /// Handles events when the detected heddoko device has been disconnected
        /// </summary>
        private void DriveDisconnectedHandler()
        {
            var vMsg = LocalizationBinderContainer.GetString(KeyMessage.DisconnectedSDCardWithLogMsg);
            Notify.Template("fade").Show(vMsg, 5f, sequenceType: NotifySequence.First);
        }

        void UploadingItemStarted(UploadableListItem vItem)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => Debug.Log("started uploading " + vItem.FileName));

        }
        private void ProblemUploadEventHandler(List<ErrorUploadEventArgs> vErrorList)
        {
            //append to log file
            mErrorCount = vErrorList.Count;
            int vLogFileError = 0;
            string vMsg = "Error Uploading: ";
            for (int i = 0; i < mErrorCount; i++)
            {
                var vItem = vErrorList[i];
                try
                {
                    if (vItem.Object.AssetType != AssetType.Record)
                    {
                        vLogFileError ++;
                        continue;
                    }
                    if (vItem.Object != null && vItem.Object.FileName != null)
                    {
                        vMsg += vItem.Object.FileName;
                    }
                    if (vItem.ErrorCollection != null && vItem.ErrorCollection.Errors[0] != null)
                    {
                        vMsg += " ERROR CODE " + vItem.ErrorCollection.Errors[0].Code + ";";
                    }
                    if (vItem.ErrorCollection != null && vItem.ErrorCollection.Errors[0] != null && vItem.ErrorCollection.Errors[0].Message != null)
                    {
                        vMsg += " ERROR MSG " + vItem.ErrorCollection.Errors[0].Message + ";";
                    }
                    if (vItem.ExceptionArgs != null)
                    {
                        vMsg += " EXCEPTION " + vItem.ExceptionArgs;
                    }
                }
                catch (Exception vE)

                {
                    UnityEngine.Debug.Log("err:  "+vE);
                     
                }
            }
            if (mErrorCount > 0)
            {
                mErrorCount -= vLogFileError;
            }
            DebugLogger.Instance.LogMessage(LogType.Uploading, vMsg);
            string vErrMsg = LocalizationBinderContainer.GetString(KeyMessage.IssueUploadingRecordingsMsg) + DebugLogger.Instance.GetLogPath(LogType.Uploading);
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => Notify.Template("fade").Show(vErrMsg, customHideDelay: 15f));
        }

        /// <summary>
        /// Found a list of files to upload
        /// </summary>
        /// <param name="vFileInfos"></param>
        private void FoundFileListHandler(List<FileInfo> vFileInfos)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {

                int vCount = vFileInfos.Count;

                if (vCount > 0)
                {
                    string vMsg = LocalizationBinderContainer.GetString(KeyMessage.NewRecFoundSyncBeginMsg, vCount > 1);
                    var vTemp = Notify.Template("SyncNotification");
                    vTemp.gameObject.GetComponent<NotifyWithButtonExtension>()
                        .RegisterCallbackAndRemovePreviousCallback(BeginUpload);
                    vTemp.Show(vMsg, 30f, sequenceType: NotifySequence.First);
                }
            });
        }

        private void ContentsCompletedUploadingEvent()
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                //   string vMsg = LocalizationBinderContainer.GetString(KeyMessage.UploadCompleteMsg) ;
                int vCount = mCardContentUploadController.FoundRecordingsList.Count;
                int vFailCount = mErrorCount;
                int vSucess = vCount - vFailCount;
                string vMsg = String.Format("The total number of sucessful uploads is {0} and failed uploads {1}",
                    vSucess, vFailCount);
                Notify.Template("fade").Show(vMsg, 15f, sequenceType: NotifySequence.First);
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(RecordingListViewController.ResetDownloadList);
            });



        }

        private void DriveFoundHandler(DirectoryInfo vVdrive)
        {
            //todo
        }


    }
}