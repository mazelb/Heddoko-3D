/**
* @file SegmentAnalysisDataContainerController.cs
* @brief Contains the SegmentAnalysisDataContainerController
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

 
using System.Linq;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.UI.RecordingLoading; 
using HeddokoSDK.Models;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording
{
    public delegate void DataCollectionStarted();
    public delegate void DataCollectionEnded();
    /// <summary>
    /// A controller that allows a user 
    /// </summary>
    [UserRolePermission(new[] { UserRoleType.Analyst })]
    public class SegmentAnalysisRecordingController : MonoBehaviour
    {
        public Button ExportDataButton;
        public Text ExportDataButtonText;
        public Body BodyModel;
        private AnalysisDataStore mDataStore;
        public RecordingPlayerView RecordingPlayerView;
        public event  DataCollectionStarted DataCollectionStartedEvent;
        public event DataCollectionEnded DataCollectionEndedEvent;
        public GameObject DisablingPanel;
        public void Awake()
        {
            ExportDataButton.onClick.AddListener(OnStart);
            ExportDataButtonText.text = "COLLECT ANALYTICS";

        }

        public void DisableControl()
        {
            ExportDataButton.interactable = false;
        }

        public void EnableControl()
        {

            ExportDataButton.interactable = true;
        }
        /// <summary>
        /// When start is triggered, assign a new data store and start collecting data. 
        /// </summary>
        private void OnStart()
        {
            if (DataCollectionStartedEvent != null)
            {
                DataCollectionStartedEvent();
            }
            BodyModel = RecordingPlayerView.CurrBody;
            //register listeners
            RecordingPlayerView.PbControlPanel.FinalFramePositionEvent += OnEnd;
            //if the recording has been changed, finish data collection
            RecordingPlayerView.PbControlPanel.NewRecordingSelectedEvent += OnEnd;
            BodyModel.View.BodyFrameUpdatedEvent += CollectTimeStampData;
            //Remove button listener and register correct listener
            ExportDataButton.onClick.RemoveAllListeners();
            ExportDataButton.onClick.AddListener(OnEnd);
            ExportDataButtonText.text = "EXPORT MOVEMENT DATA";

            var vSegmentList = BodyModel.AnalysisSegments.Values.ToList();

            if (mDataStore == null)
            {
                mDataStore = new AnalysisDataStore(vSegmentList);
            }
            

            foreach (var vSegmentAnalysis in vSegmentList)
            {
                vSegmentAnalysis.AddAnalysisCompletionListener(mDataStore.UpdateSegmentFieldInfo);
            }

            Notify.Template("fade").Show("Analysis collection has started", 5f, sequenceType: NotifySequence.First);
        }

        /// <summary>
        /// Gets the frames timestamp.
        /// </summary>
        /// <param name="vBodyFrame"></param>
        private void CollectTimeStampData(BodyFrame vBodyFrame)
        {
            mDataStore.AddNewTimestamp(vBodyFrame);
        }


        /// <summary>
        /// when invoked, this begins to serialize data store data. 
        /// </summary>
        private void OnEnd()
        {
            //Remove listeners
            BodyModel.View.BodyFrameUpdatedEvent -= CollectTimeStampData;
            RecordingPlayerView.PbControlPanel.FinalFramePositionEvent -= OnEnd;
            RecordingPlayerView.PbControlPanel.NewRecordingSelectedEvent -= OnEnd;
            ExportDataButton.onClick.RemoveAllListeners();
            ExportDataButton.onClick.AddListener(OnStart);
            ExportDataButtonText.text = "COLLECT ANALYTICS";
            var vSegmentList = BodyModel.AnalysisSegments.Values.ToList();
            foreach (var vSegmentAnalysis in vSegmentList)
            {
                vSegmentAnalysis.RemoveAnalysisCompletionListener(mDataStore.UpdateSegmentFieldInfo);
            }
            LaunchSaveFileWindow();
        }

        /// <summary>
        /// Launches the save file window
        /// </summary>
        void LaunchSaveFileWindow()
        {

            DisablingPanel.SetActive(true);
            //initialize the browser settings
            UniFileBrowser.use.SetFileExtensions(new [] { "csv"});
            UniFileBrowser.use.useDeleteButton = false;
            UniFileBrowser.use.allowMultiSelect = false;
            UniFileBrowser.use.showVolumes = true;
            UniFileBrowser.use.allowWindowClose = true;
            UniFileBrowser.use.allowWindowDrag = true;
            UniFileBrowser.use.allowWindowResize = true;
            UniFileBrowser.use.CloseMessageWindow(false);
            UniFileBrowser.use.CloseFileWindow();
            UniFileBrowser.use.enabled = true;
            UniFileBrowser.use.SaveFileWindow(SaveFile);

            UniFileBrowser.use.OnEscape = Escaped;
        }

        /// <summary>
        /// Window has been escaped.  Datacollection has been ended. 
        /// </summary>
        /// <param name="vArg0"></param>
        private void Escaped(int vArg0)
        {
            if (DataCollectionEndedEvent != null)
            {
                DataCollectionEndedEvent();
            }
        }


        /// <summary>
        /// Save file.
        /// </summary>
        /// <param name="vArg0"></param>
        private void SaveFile(string vArg0)
        {
            AnalysisDataStoreSerialization.WriteFile(mDataStore,vArg0+".csv"); 

            mDataStore.Clear();
            if (DataCollectionEndedEvent != null)
            {
                DataCollectionEndedEvent();
            }
            Notify.Template("fade").Show("File has been saved to "+ vArg0 + ".csv", 5f, sequenceType: NotifySequence.First);
            DisablingPanel.SetActive(false);
        }
    }
}