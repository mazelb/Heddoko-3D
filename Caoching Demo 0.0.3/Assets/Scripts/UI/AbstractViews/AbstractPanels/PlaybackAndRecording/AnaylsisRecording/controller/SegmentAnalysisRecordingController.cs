/**
* @file SegmentAnalysisDataContainerController.cs
* @brief Contains the SegmentAnalysisDataContainerController
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/


using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.UI.RecordingLoading;
using HeddokoSDK.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording
{

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


        public void Awake()
        {
            ExportDataButton.onClick.AddListener(OnStart);
            ExportDataButtonText.text = "COLLECT ANALYTICS";
        }

        /// <summary>
        /// When start is triggered, assign a new data store and start collecting data. 
        /// </summary>
        private void OnStart()
        {
            BodyModel = RecordingPlayerView.CurrBody;
            //register listeners
            RecordingPlayerView.PbControlPanel.FinalFramePositionEvent += OnEnd;
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
            //initialize the browser settings
            UniFileBrowser.use.SetFileExtensions(new [] { "csv"});
            UniFileBrowser.use.useDeleteButton = false;
            UniFileBrowser.use.allowMultiSelect = false;
            UniFileBrowser.use.showVolumes = true;
            UniFileBrowser.use.SaveFileWindow(SaveFile);

        }

        /// <summary>
        /// Save file.
        /// </summary>
        /// <param name="vArg0"></param>
        private void SaveFile(string vArg0)
        { 
            AnalysisDataStoreSerialization.WriteFile(mDataStore,vArg0+".csv"); 
            mDataStore.Clear();
        }
    }
}