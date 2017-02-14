﻿// /**
// * @file UnityAnalysisFramesControllerWrapper.cs
// * @brief Contains the UnityAnalysisFramesControllerWrapper class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date February 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using Assets.Scripts.Body_Pipeline.Analysis.Serialization;
using Assets.Scripts.Localization;
using Assets.Scripts.Notification;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
 using Assets.Scripts.UI.RecordingLoading;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Controller
{
    /// <summary>
    /// Wrapper for AnalysisFramesCollectionController
    /// </summary>
    public class UnityAnalysisFramesControllerWrapper : MonoBehaviour
    {
        private AnalysisFramesCollectionController mController;
        public RecordingPlayerView PlayerView;
        private PlaybackControlPanel mPlaybackControlPanel;
        private bool mCollectingData;
        //   public UnityEngine.UI.Button CollectDataButton;
        //   public UnityEngine.UI.Text CollectDataButtonText;

        public GameObject DisablingPanel;
        private Body mCurrentBody;
        public AnalysisTextViewController AnalysisTextViewController;
        public UnityEngine.UI.Button ExportDataButton;
        public UnityEngine.UI.Text ExportDataText;

        void Awake()
        {
            IAnalysisFramesSerializer vSerializer = new CsvAnalysisFramesSerializer();
            mController = new AnalysisFramesCollectionController(vSerializer);
            PlayerView.RecordingPlayerViewLayoutCreatedEvent += RecordingPlayerViewLayoutCreatedEvent;
            ExportDataButton.enabled = false;
            ExportDataButton.onClick.AddListener(LaunchSaveFileWindow);
        }

        void OnDisable()
        {
            mController.MovementSet = false;
            StopDataCollection();

        }
        /// <summary>
        /// event when the recording playerview has been created. 
        /// </summary>
        /// <param name="vView"></param>
        private void RecordingPlayerViewLayoutCreatedEvent(RecordingPlayerView vView)
        {
            mController.SetBody(vView.CurrBody);
            mPlaybackControlPanel = vView.PbControlPanel;
            mPlaybackControlPanel.RecordingUpdatedEvent += RecordingUpdatedEvent;
            mPlaybackControlPanel.NewRecordingSelectedEvent += mController.ResetCollection;
            mPlaybackControlPanel.NewRecordingSelectedEvent += () => mController.MovementSet = true;
            vView.CurrBody.View.BodyFrameUpdatedEvent += BodyFrameUpdatedEvent;
            vView.CurrBody.View.TrackingUpdateEvent += UpdateViews;
            mPlaybackControlPanel.NewRecordingSelectedEvent += StopDataCollection;
        }

        private void UpdateViews(BodyFrame vNewFrame)
        {
            int vIndex = vNewFrame.Index;
            var vFrame = mController.GetAnalysisFrame(vIndex);
            if (vFrame == null)
            {

                //give the option to collect all data frames or single data frame
                //frame hasn't been analyzed yet
                var vMessage = LocalizationBinderContainer.GetString(KeyMessage.NoAnalysisFrameSet);
            }
            else
            {
                AnalysisTextViewController.UpdateView(vFrame);
            }
        }


        private void BodyFrameUpdatedEvent(BodyFrame vNewFrame)
        {
           
        }

        /// <summary>
        /// A new recording has been selected
        /// </summary>
        /// <param name="vControlPanel"></param>
        private void RecordingUpdatedEvent(PlaybackControlPanel vControlPanel)
        {
            ExportDataButton.enabled = true;
            mController.SetMaxCount(vControlPanel.mPlaybackTask.ConvertedFrames.Count);
            mController.ResetCollection();
            mController.Body.AnalysisFramesSet.MaxFramesReachedEvent += AnalysisFramesSetMaxFramesReachedEvent;
            StartDataCollection();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="vCollectedFrames"></param>
        /// <param name="vMaxFrames"></param>
        private void AnalysisFramesSetMaxFramesReachedEvent(int vCollectedFrames, int vMaxFrames)
        {
            string vMessage =
                string.Format(
                    "{0} frames have been analyzed and completed. Click on the export data button to save analysis data to a csv file.",
                    vCollectedFrames);
            NotificationManager.CreateNotification(vMessage, NotificationManager.NotificationUrgency.Low);
        }

        void OnApplicationQuit()
        {
            mPlaybackControlPanel.RecordingUpdatedEvent -= RecordingUpdatedEvent;
            PlayerView.RecordingPlayerViewLayoutCreatedEvent -= RecordingPlayerViewLayoutCreatedEvent;
        }
        /// <summary>
        /// Stops data collection  
        /// </summary>
        public void StopDataCollection()
        {
            if (mCollectingData)
            {
                mCollectingData = false;
                mController.Stop();
                mController.ResetCollection();
            }
        }


        /// <summary>
        /// Launches the save file window
        /// </summary>
        public void LaunchSaveFileWindow()
        {

            //pause recording
            mPlaybackControlPanel.Pause();
            DisablingPanel.SetActive(true);

            //initialize the browser settings
            UniFileBrowser.use.SetFileExtensions(new[] { "csv" });
            UniFileBrowser.use.useDeleteButton = false;
            UniFileBrowser.use.allowMultiSelect = false;
            UniFileBrowser.use.showVolumes = true;
            UniFileBrowser.use.allowWindowClose = true;
            UniFileBrowser.use.allowWindowDrag = true;
            UniFileBrowser.use.allowWindowResize = true;
            UniFileBrowser.use.CloseMessageWindow(false);
            UniFileBrowser.use.CloseFileWindow();
            UniFileBrowser.use.enabled = true;
            UniFileBrowser.use.SaveFileWindow(ExportCSVData);

            UniFileBrowser.use.OnEscape = Escaped;
        }
        /// <summary>
        /// Window has been escaped.  Datacollection has been ended. 
        /// </summary>
        /// <param name="vArg0"></param>
        private void Escaped(int vArg0)
        {
            mPlaybackControlPanel.ChangeState(PlaybackState.Play);
        }

        /// <summary>
        /// export set to a csv file
        /// </summary>
        /// <param name="vArg0"></param>
        private void ExportCSVData(string vArg0)
        {
            mController.SerializeSet(vArg0);
            string vMsg = LocalizationBinderContainer.GetString(KeyMessage.AnalysisSaveMsg);

            Notify.Template("fade").Show(vMsg + vArg0 + ".csv", 5f, sequenceType: NotifySequence.First);
            DisablingPanel.SetActive(false);
            mPlaybackControlPanel.ChangeState(PlaybackState.Play);

        }
        /// <summary>
        /// Commence data collection
        /// </summary>
        public void StartDataCollection()
        {
            mCollectingData = true;
            mController.Start();
        }

        /// <summary>
        /// Enable controls
        /// </summary>
        public void EnableControl()
        {
            //    CollectDataButton.interactable = true;
        }
        /// <summary>
        /// Disable controls
        /// </summary>
        public void DisableControl()
        {
            //       CollectDataButton.interactable = true;
        }
    }
}