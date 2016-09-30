// /**
// * @file UploadController.cs
// * @brief Contains the UploadController
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using System.IO;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.DebugContext.logging;
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

        void Start()
        {
            mCardContentUploadController = new SdCardContentUploadController(UserSessionManager.Instance.UserProfile);
            mCardContentUploadController.DriveFoundEvent += DriveFoundHandler;
            mCardContentUploadController.ContentsCompletedUploadEvent += ContentsCompletedUploadingEvent;
            mCardContentUploadController.SingleUploadEndEvent += x =>
            {
               OutterThreadToUnityThreadIntermediary.QueueActionInUnity(RecordingListViewController.ResetDownloadList);
            };

            mCardContentUploadController.FoundFileListEvent += FoundFileListHandler;
            mCardContentUploadController.ProblemUploadingContentEvent += ProblemUploadEventHandler;
            mCardContentUploadController.DriveDisconnectedEvent += DriveDisconnectedHandler;
            mCardContentUploadController.UploadingStartEvent += UploadingItemStarted;
        }

        void OnApplicationQuit()
        {
            mCardContentUploadController.DriveFoundEvent -= DriveFoundHandler;
            mCardContentUploadController.ContentsCompletedUploadEvent -= ContentsCompletedUploadingEvent;
            mCardContentUploadController.SingleUploadEndEvent -= x =>
            {
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(RecordingListViewController.ResetDownloadList);
            };

            mCardContentUploadController.FoundFileListEvent -= FoundFileListHandler;
            mCardContentUploadController.ProblemUploadingContentEvent -= ProblemUploadEventHandler;
            mCardContentUploadController.DriveDisconnectedEvent -= DriveDisconnectedHandler;
            mCardContentUploadController.UploadingStartEvent -= UploadingItemStarted;
        }
        public void BeginUpload()
        {
            Notify.Template("fade").Show("Beginning upload process, please ensure SD card is inserted and secured in place", 5f, sequenceType: NotifySequence.First);
            mCardContentUploadController.StartContentUpload();
        }
        /// <summary>
        /// Handles events when the detected heddoko device has been disconnected
        /// </summary>
        private void DriveDisconnectedHandler()
        {
            Notify.Template("fade").Show("Heddoko SD card has been disconnected", 5f, sequenceType: NotifySequence.First);
        }

        void UploadingItemStarted(UploadableListItem vItem)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => Debug.Log("started uploading " + vItem.FileName));

        }
        private void ProblemUploadEventHandler(ErrorUploadEventArgs vItem)
        {
            //append to log file
            for (int i = 0; i < vItem.ErrorCollection.Errors.Count; i++)
            {
                string vMsg = "Error Uploading: ";
                if (vItem.Object != null)
                {
                    vMsg += vItem.Object.FileName;
                }
                vMsg += " ERROR CODE "+ vItem.ErrorCollection.Errors[i].Code + " ERROR MSG "+ vItem.ErrorCollection.Errors[i].Message;
                DebugLogger.Instance.LogMessage(LogType.Uploading, vMsg);
            }
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => Notify.Template("fade").Show("There was an issue uploading one or more recording files. For more details please see log file " + DebugLogger.Instance.GetLogPath(LogType.Uploading),customHideDelay:15f));
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
                    string vRecString = "recording";
                    if (vCount == 1)
                    {
                        vRecString = "recordings";
                    }

                    var vMsg = "New " + vRecString + " on the SD file were found. Click the Sync button to start uploading";
                   // Notify.Template("SyncNotification").Show(vMsg, 5f, sequenceType: NotifySequence.First);
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
                Notify.Template("fade").Show("Upload complete", 15f, sequenceType: NotifySequence.First);
            });



        }

        private void DriveFoundHandler(DirectoryInfo vVdrive)
        {
            //todo
        }


    }
}