// /**
// * @file UnityAnalysisFramesControllerWrapper.cs
// * @brief Contains the UnityAnalysisFramesControllerWrapper class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date February 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using Assets.Scripts.Body_Pipeline.Analysis.Serialization;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using Assets.Scripts.UI.RecordingLoading;
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

        void Start()
        {
            IAnalysisFramesSerialization vSerializer  = new CsvAnalysisFramesSerialization();
            mController = new AnalysisFramesCollectionController(vSerializer);
            PlayerView.RecordingPlayerViewLayoutCreatedEvent += RecordingPlayerViewLayoutCreatedEvent;
        }

        /// <summary>
        /// event when the recording playerview has been created. 
        /// </summary>
        /// <param name="vView"></param>
        private void RecordingPlayerViewLayoutCreatedEvent(RecordingPlayerView vView)
        {
            mController.SetBody(vView.CurrBody);
            mPlaybackControlPanel = vView.PbControlPanel;
            mPlaybackControlPanel.FinalFramePositionEvent += StopDataCollection;
            mPlaybackControlPanel.RecordingUpdatedEvent += UpdateNumberOfMaxPossibleFrames;
        }

        /// <summary>
        /// updates the max number of possible frames
        /// </summary>
        /// <param name="vControlPanel"></param>
        private void UpdateNumberOfMaxPossibleFrames(PlaybackControlPanel vControlPanel)
        {
            mController.SetMaxCount(vControlPanel.mPlaybackTask.ConvertedFrames.Count);
        }

        void OnApplicationQuit()
        {
            mPlaybackControlPanel.FinalFramePositionEvent -= StopDataCollection;
            mPlaybackControlPanel.RecordingUpdatedEvent -= UpdateNumberOfMaxPossibleFrames;
            PlayerView.RecordingPlayerViewLayoutCreatedEvent -= RecordingPlayerViewLayoutCreatedEvent;
        }
        /// <summary>
        /// Stops data collection  
        /// </summary>
        public  void StopDataCollection()
        {
            if (mCollectingData)
            {
                mCollectingData = false;
                mController.Stop();
                mPlaybackControlPanel.SetSliderControlFunctionality(true);
            }
        }

        /// <summary>
        /// Commence data collection
        /// </summary>

        public void StartDataCollection()
        {
            mController.Start();
            mPlaybackControlPanel.SetSliderControlFunctionality(false);
        }


    }
}