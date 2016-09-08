/**
* @file AnalysisDataStore.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Body_Data.CalibrationData.TposeSelection;
using Assets.Scripts.Body_Pipeline.Analysis.Settings;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// A data storage mechanism for analysis segments, mapping to a set of SegmentAnalyis. 
    /// </summary>
    public class AnalysisDataStore
    {
        private Dictionary<SegmentAnalysis, Dictionary<FieldInfo, AnalysisFieldDataStructure>> mStorage = new Dictionary<SegmentAnalysis, Dictionary<FieldInfo, AnalysisFieldDataStructure>>();
        //Keep track of the body frames
        
        public List<Dictionary<FieldInfo, string>> SerializedList = new List<Dictionary<FieldInfo, string>>();
        private List<int> mFrameIndices = new List<int>();
        private List<float> mTimeStamps = new List<float>();
        private int[] mPoseSelectionIndicies;
        private int mFrameCount = -1;
        internal AnaylsisDataStoreSettings AnaylsisDataStoreSettings;
        public AnalysisDataStoreSerialization Serialization;
        private int mFieldInfoCount;
        private int mCounter;
        private int mSubCount = 0;
        public bool Ignore { get; set; }

        /// <summary>
        /// Constructor accepting a list of analysis segment, using reflection, sifts through fields with AnalysisAttributes marked as do not ignore.  
        /// </summary>
        public AnalysisDataStore(List<SegmentAnalysis> vAnalysisSegments)
        {
            AnaylsisDataStoreSettings = new AnaylsisDataStoreSettings(vAnalysisSegments);
            Serialization = new AnalysisDataStoreSerialization(this);
            foreach (var vKvPairing in AnaylsisDataStoreSettings.StoredAnalysisFields)
            {
                var vAnalysisTrackingDataStructure = new Dictionary<FieldInfo, AnalysisFieldDataStructure>(vKvPairing.Value.Count);
                foreach (var vField in vKvPairing.Value)
                {
                    var vAnalysisDataStruct = new AnalysisFieldDataStructure { FieldInfoKey = vField };
                    vAnalysisTrackingDataStructure.Add(vField, vAnalysisDataStruct);

                    mFieldInfoCount++;

                }
                mStorage.Add(vKvPairing.Key, vAnalysisTrackingDataStructure);
            }
        }

        public Dictionary<SegmentAnalysis, Dictionary<FieldInfo, AnalysisFieldDataStructure>> Storage
        {
            set { mStorage = value; }
            get { return mStorage; }
        }

        public List<int> FrameIndices
        {
            set { mFrameIndices = value; }
            get { return mFrameIndices; }
        }

        public List<float> TimeStamps
        {
            set { mTimeStamps = value; }
            get { return mTimeStamps; }
        }

        private List<TPoseSelection> mPoseSelections;
        public List<TPoseSelection> PoseSelectionList
        {
            get
            {
                return mPoseSelections;
            }
            set
            {
                mPoseSelections = value;
                mPoseSelectionIndicies = new int[mFrameCount];
                if (mPoseSelections != null)
                {
                    //Initializes the tpose selection index account to the passed in selecton list
                    for (int vI = 0; vI < mPoseSelections.Count; vI++)
                    {
                        var vObj = mPoseSelections[vI];
                        //set the main pose to 1
                        mPoseSelectionIndicies[vObj.PoseIndex] = 1;
                        for (int vJ = vObj.PoseIndexLeft; vJ <= vObj.PoseIndexRight; vJ++)
                        {
                            if (mPoseSelectionIndicies[vJ] != 1)
                            {
                                mPoseSelectionIndicies[vJ] = 2;
                            }
                        }
                    }
                }
            }
        }

        public int[] PoseSelectionIndicies
        {
            get { return mPoseSelectionIndicies; }
        }


        /// <summary>
        /// Removes an analysis field from being tracked
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vValue"></param>
        public void RemoveAnalysisFieldInfo(SegmentAnalysis vKey, FieldInfo vValue)
        {
            AnaylsisDataStoreSettings.RemoveAnalysisFieldInfo(vKey, vValue);
            if (mStorage.ContainsKey(vKey))
            {
                if (mStorage[vKey].ContainsKey(vValue))
                {
                    mStorage[vKey].Remove(vValue);
                }
            }

        }
        /// <summary>
        /// Removes an entire analysis segment from being tracked
        /// </summary>
        /// <param name="vKey"></param>
        public void RemoveAnalysisType(SegmentAnalysis vKey)
        {
            AnaylsisDataStoreSettings.RemoveSegment(vKey);
            if (mStorage.ContainsKey(vKey))
            {
                mStorage[vKey] = new Dictionary<FieldInfo, AnalysisFieldDataStructure>();
            }
        }

        public void AddAnalyisSegment(SegmentAnalysis vKey)
        {
            AnaylsisDataStoreSettings.Reset(vKey);
            if (mStorage.ContainsKey(vKey))
            {
                mStorage[vKey] = new Dictionary<FieldInfo, AnalysisFieldDataStructure>();
            }
        }

        /// <summary>
        /// Adds a new timestamp from the given body frame
        /// </summary>
        /// <param name="vFrame"></param>
        public void Update(BodyFrame vFrame)
        {
             mTimeStamps.Add(vFrame.Timestamp);
         
            mFrameIndices.Add(vFrame.Index);
        }

        /// <summary>
        /// Updates segment field info. 
        /// </summary>
        /// <param name="vKey"></param>
        public void UpdateSegmentFieldInfo(SegmentAnalysis vKey)
        {
            if (Ignore)
            {
                return;
            }
            if (!mStorage.ContainsKey(vKey))
            {
                Debug.Log("no key found");
                return;
            }
            var vFields = mStorage[vKey];
            if (mCounter == 0)
            {
                SerializedList.Add(new Dictionary<FieldInfo, string>());
            }
            var vList = SerializedList.ElementAt(SerializedList.Count - 1);

            foreach (var vKvPair in vFields)
            {
                try
                {
                    var vPassedInValue = (float)vKvPair.Key.GetValue(vKey);
                    vKvPair.Value.Add(vPassedInValue);
                    vList.Add(vKvPair.Key, vPassedInValue + "");
                    mCounter++;

                }
                catch (Exception vE)
                {

                }

            }
            if (mCounter >= mFieldInfoCount)
            {
                mCounter = 0;
            }

        }

        /// <summary>
        /// Sets the passed in parameters to the list of stored timestamps and the data store
        /// </summary>
        /// <param name="vTimeStamps"></param>
        /// <param name="vStorage"></param>
        public void GetTimestampsAndDataStore(out List<float> vTimeStamps,
            out Dictionary<SegmentAnalysis, Dictionary<FieldInfo, AnalysisFieldDataStructure>> vStorage)
        {
            vTimeStamps = mTimeStamps;
            vStorage = mStorage;
        }

        /// <summary>
        /// Clears up the datastore
        /// </summary>
        public void Clear()
        {
            foreach (var vKvPair in mStorage)
            {
                foreach (var vAnalysisFieldDataStructure in vKvPair.Value)
                {
                    vAnalysisFieldDataStructure.Value.ClearDataCollection();
                }
            }
            SerializedList.Clear();
            mTimeStamps.Clear();
            mFrameIndices.Clear();
            mCounter = 0;
            mSubCount = 0;
        }

        /// <summary>
        /// Prepares the data store for serialization. 
        /// </summary>
        public void PrepareDataStore()
        {
            if (mCounter < mFieldInfoCount && mCounter != 0)
            {
                SerializedList.RemoveAt(SerializedList.Count - 1);
            }
        }

        /// <summary>
        /// Sets the total number of indicies 
        /// </summary>
        /// <param name="vRawFramesCount"></param>
        public void SetNumberOfIndices(int vRawFramesCount)
        {
            mFrameCount = vRawFramesCount;
        }
    }
}