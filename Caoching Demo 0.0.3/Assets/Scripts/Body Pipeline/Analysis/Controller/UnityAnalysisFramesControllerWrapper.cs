// /**
// * @file UnityAnalysisFramesControllerWrapper.cs
// * @brief Contains the UnityAnalysisFramesControllerWrapper class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date February 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using Assets.Scripts.Body_Pipeline.Analysis.Serialization;
using Assets.Scripts.Frames_Pipeline;
using Assets.Scripts.Localization;
using Assets.Scripts.Notification;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using Assets.Scripts.UI.RecordingLoading;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Pipeline.Analysis.Controller
{
    /// <summary>
    /// Wrapper for AnalysisFramesCollectionController
    /// </summary>
    public class UnityAnalysisFramesControllerWrapper : MonoBehaviour
    {
        private AnalysisFramesCollectionController mController;
        public LiveSuitFeedView LiveSuitFeedView;
        public RecordingPlayerView PlayerView;
        private PlaybackControlPanel mPlaybackControlPanel;
        private bool mCollectingData;
        private bool InLiveView = false;
        public GameObject DisablingPanel;
        public AnalysisTextViewController AnalysisTextViewController;
        public Button ExportDataButton;
        public Text ExportDataText;
        private AnalysisCollectionIndexSet StartStop;
        private bool mCompleteRecordingPlayed;
        internal void Awake()
        {
            IAnalysisFramesSerializer vSerializer = new CsvAnalysisFramesSerializer();
            mController = new AnalysisFramesCollectionController(vSerializer);
            PlayerView.RecordingPlayerViewLayoutCreatedEvent += RecordingPlayerViewLayoutCreatedHandler;
            LiveSuitFeedView.LiveSuitFeedViewLayoutCreatedEvent += LiveSuitFeedViewLayoutCreatedHandler;
            LiveSuitFeedView.LiveSuitFeedViewLayoutEnabled += LiveSuitFeedViewLayoutEnabledHandler;
            PlayerView.RecordingPlayerViewLayoutEnabled += RecordingPlayerViewLayoutEnabledHandler;
            ExportDataButton.enabled = false;
            ExportDataText.text = "START ANALYSIS COLLECTION";
            ExportDataButton.onClick.AddListener(PrepDataCollection);
        }

        /// <summary>
        /// Starts data collection and clears out previous frames. 
        /// </summary>
        private void PrepDataCollection()
        {
            //remove all current listeners
            ExportDataButton.onClick.RemoveAllListeners();
            ExportDataText.text = "EXPORT DATA TO CSV";
            mCompleteRecordingPlayed = false;
            //Add listener for data collection start
            ExportDataButton.onClick.AddListener(LaunchSaveFileWindow);
            StartStop.StartIndex = mController.GetBodyFrameIndex();
        }
        private void RecordingPlayerViewLayoutEnabledHandler(RecordingPlayerView vObj)
        {
            if (LiveSuitFeedView.Initialized)
            {
                UnRegisterLiveSuitFeedViewHandlers(LiveSuitFeedView);
            }
            RegisterPlayerViewHandlers(PlayerView);
        }

        private void LiveSuitFeedViewLayoutEnabledHandler(LiveSuitFeedView vObj)
        {
            if (LiveSuitFeedView.Initialized)
            {
                UnRegisterPlayerViewHandlers(PlayerView);
            }

            RegisterLiveSuitFeedViewHandlers(LiveSuitFeedView);

        }

        private void LiveSuitFeedViewLayoutCreatedHandler(LiveSuitFeedView vView)
        {
            if (PlayerView.Initialized)
            {
                UnRegisterPlayerViewHandlers(PlayerView);
            }
            vView.BrainpackBody.AnalysisFramesSet = new LiveAnalysisFramesSet();
            RegisterLiveSuitFeedViewHandlers(vView);

        }

        private void RegisterLiveSuitFeedViewHandlers(LiveSuitFeedView vView)
        {
            vView.BrainpackBody.View.BodyFrameUpdatedEvent += BodyFrameUpdatedEvent;
            vView.BrainpackBody.View.TrackingUpdateEvent += UpdateViews;
            mController.SetBody(vView.BrainpackBody);
            mController.MovementSet = true;
            mController.Start();
        }

        private void UnRegisterLiveSuitFeedViewHandlers(LiveSuitFeedView vView)
        {
            vView.BrainpackBody.View.BodyFrameUpdatedEvent -= BodyFrameUpdatedEvent;
            vView.BrainpackBody.View.TrackingUpdateEvent -= UpdateViews;
            mController.MovementSet = false;
            mController.Stop();
        }

        internal void OnDisable()
        {
            mController.MovementSet = false;
            StopDataCollection();

        }
        /// <summary>
        /// event when the recording playerview has been created. 
        /// </summary>
        /// <param name="vView"></param>
        private void RecordingPlayerViewLayoutCreatedHandler(RecordingPlayerView vView)
        {
            if (LiveSuitFeedView.Initialized)
            {
                UnRegisterLiveSuitFeedViewHandlers(LiveSuitFeedView);
            }
            mController.SetBody(vView.CurrBody);
            RegisterPlayerViewHandlers(vView);
        }

        private void UnRegisterPlayerViewHandlers(RecordingPlayerView vView)
        {
            mPlaybackControlPanel = vView.PbControlPanel;
            mPlaybackControlPanel.RecordingUpdatedEvent -= RecordingUpdatedEvent;
            mPlaybackControlPanel.NewRecordingSelectedEvent -= mController.ResetCollection;
            mPlaybackControlPanel.NewRecordingSelectedEvent -= () => mController.MovementSet = true;
            vView.CurrBody.View.BodyFrameUpdatedEvent -= BodyFrameUpdatedEvent;
            vView.CurrBody.View.TrackingUpdateEvent -= UpdateViews;
            mPlaybackControlPanel.NewRecordingSelectedEvent -= StopDataCollection;
        }
        void RegisterPlayerViewHandlers(RecordingPlayerView vView)
        {
            if (LiveSuitFeedView.Initialized)
            {
                UnRegisterLiveSuitFeedViewHandlers(LiveSuitFeedView);
            }
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
                LocalizationBinderContainer.GetString(KeyMessage.NoAnalysisFrameSet);
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
            if (PlayerView.CurrBody.AnalysisFramesSet == null)
            {
                PlayerView.CurrBody.AnalysisFramesSet =
                    new AnalysisFramesSet(vControlPanel.mPlaybackTask.ConvertedFrames.Count);
            }
            else
            {
                PlayerView.CurrBody.AnalysisFramesSet.SetMaxFrameCount(vControlPanel.mPlaybackTask.ConvertedFrames.Count);
            }
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
            mController.Body.AnalysisFramesSet.MaxFramesReachedEvent -= AnalysisFramesSetMaxFramesReachedEvent;
            mCompleteRecordingPlayed = true;
        }

        internal void OnApplicationQuit()
        {
            if (mPlaybackControlPanel != null)
            {
                mPlaybackControlPanel.RecordingUpdatedEvent -= RecordingUpdatedEvent;
            }
            if (PlayerView != null)
            {
                PlayerView.RecordingPlayerViewLayoutCreatedEvent -= RecordingPlayerViewLayoutCreatedHandler;
            }
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

            //remove all current listeners
            ExportDataButton.onClick.RemoveAllListeners();
            ExportDataText.text = "START ANALYSIS COLLECTION";
            //Add listener for data collection start
            ExportDataButton.onClick.AddListener(PrepDataCollection);

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
            UniFileBrowser.use.SaveFileWindow(ExportCsvData);
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
        private void ExportCsvData(string vArg0)
        {
            StartStop.EndIndex = mController.GetBodyFrameIndex();
            if (mCompleteRecordingPlayed)
            {
                mController.SerializeSet(vArg0);
            }
            else
            {
                mController.SerializeSet(vArg0, StartStop.StartIndex, StartStop.EndIndex);
            }
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
        /// Designate the start and stop index of the analysis set to serialize
        /// </summary>
        private struct AnalysisCollectionIndexSet
        {
            public int StartIndex;
            public int EndIndex;
        }

    }
}