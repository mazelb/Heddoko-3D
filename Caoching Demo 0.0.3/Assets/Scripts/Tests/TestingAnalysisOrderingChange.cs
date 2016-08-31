// /**
// * @file TestingAnalysisOrderingChange.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// TEsts the ordering of analaysis segment serialization
    /// </summary>
    public class TestingAnalysisOrderingChange : MonoBehaviour
    {
        public SegmentAnalysisRecordingController SegmentAnalysisRecordingController;

        public void PrintOrder()
        {
            var vDataStore = SegmentAnalysisRecordingController.mDataStore;
            var vSettings = vDataStore.AnaylsisDataStoreSettings;
            var vOrderFieldList = vSettings.GetOrderedFieldList();
            foreach (var vSome in vOrderFieldList)
            {
                Debug.Log(vSome.ToString());

            }
        }

        public void Save()
        {
            var vDataStore = SegmentAnalysisRecordingController.mDataStore;
            var vSettings = vDataStore.AnaylsisDataStoreSettings;
            vSettings.SetAttributesFromFile();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PrintOrder();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                Save();
            }
        }

    }
}