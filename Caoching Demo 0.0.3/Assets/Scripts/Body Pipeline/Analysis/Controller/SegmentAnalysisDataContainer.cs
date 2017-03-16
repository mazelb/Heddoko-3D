 /**
 * @file SegmentAnalysisDataContainer.cs
 * @brief Contains the 
 * @author Mohammed Haider( mohammed@heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */

using System.Collections.Generic;
 using Assets.Scripts.Body_Pipeline.Analysis; 
using HeddokoLib.genericPatterns;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording
{
    public delegate void NewConvertedFramesReceived(BodyFrame[] vFrames);
    /// <summary>
    /// A container of data stores.
    /// </summary>
    public class SegmentAnalysisDataContainer : MonoBehaviour
    {
         private Pool<RecordingAnalysisSegmentSectionView> mPool;
        private List<RecordingAnalysisSegmentSectionView> mList = new List<RecordingAnalysisSegmentSectionView>();
        private BodyFrame[] mConvertedFrames;
        private Body mBody;
        public event NewConvertedFramesReceived NewConvertedFramesReceivedEvent;
        public PlaybackControlPanel PlaybackControlPanel;
        public RecordingAnalysisSegmentSectionView Template;
        private static RecordingAnalysisSegmentSectionView sTemplate;
        private static Transform sTemplateParent;

        public BodyFrame[] ConvertedFrames
        {
            get { return mConvertedFrames; }
        }

        public List<RecordingAnalysisSegmentSectionView> RecordingAnalysisSegmentSectionList
        {
            get { return mList; }
        }

        public Body Body
        {
            get { return mBody; }
            set { mBody = value; }
        }

        public void Init()
        {
            sTemplate = Template;
            sTemplateParent = transform;
            PlaybackControlPanel.RecordingUpdatedEvent += UpdateConvertedFrames;
            mPool = new Pool<RecordingAnalysisSegmentSectionView>(20, ConstructAnalysisSection, Pool<RecordingAnalysisSegmentSectionView>.LoadingMode.Lazy);
        }

        private static RecordingAnalysisSegmentSectionView ConstructAnalysisSection()
        {
            var vGo = GameObject.Instantiate(sTemplate);
            RecordingAnalysisSegmentSectionView vReturn = vGo.GetComponent<RecordingAnalysisSegmentSectionView>();
            //set the parent to the current container
            vGo.transform.SetParent(sTemplateParent, false);
            return vReturn;
        }
        void OnApplicationQuit()
        {
            PlaybackControlPanel.RecordingUpdatedEvent -= UpdateConvertedFrames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vControlPanel"></param>

        void UpdateConvertedFrames(PlaybackControlPanel vControlPanel)
        {
            mConvertedFrames = vControlPanel.PlaybackTask.ConvertedFrames.ToArray();
            if (NewConvertedFramesReceivedEvent != null)
            {
                NewConvertedFramesReceivedEvent(mConvertedFrames);
            }
        }

        /// <summary>
        /// Get a list of data stores in the container
        /// </summary>
        /// <returns></returns>
        public List<AnalysisDataStore> GetDataStoreList()
        {
            List<AnalysisDataStore> vDataStores = new List<AnalysisDataStore>();
            foreach (var vSection in mList)
            {
                vDataStores.Add(vSection.AnalysisSectionModel.DataStore);
            }
            return vDataStores;
        }
        /// <summary>
        /// Clears entire list
        /// </summary>
        public void Clear()
        {

            foreach (var vSegmentSection in mList)
            {
                RemoveSegmentSection(vSegmentSection);
            }
            mList.Clear();
        }

        /// <summary>
        /// Removes one AnalysisSegmentSection
        /// </summary>
        /// <param name="vSegmentSectionView"></param>
        public void RemoveSegmentSection(RecordingAnalysisSegmentSectionView vSegmentSectionView)
        {
            mPool.Release(vSegmentSectionView);
        }

        public void AddNewSegmentSection()
        {
            var vObj = mPool.Get();
            vObj.AnalysisSectionModel.Init(mConvertedFrames.Length - 1);
            var vSegments = new List<SegmentAnalysis>();
            vSegments.AddRange(mBody.AnalysisSegments.Values);
            vObj.Init(vSegments);
            mList.Add(vObj);
        }
       
    }
}