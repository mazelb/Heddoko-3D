﻿ /**
 * @file AnalysisDataStore.cs
 * @brief Contains the 
 * @author Mohammed Haider( mohammed@heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.Scripts.Body_Pipeline.Analysis.Settings;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// A data storage mechanism for analysis segments
    /// </summary>
    public class AnalysisDataStore
    {
        private AnaylsisDataStoreSettings mAnaylsisDataStoreSettings; 
        private Dictionary<SegmentAnalysis, Dictionary<FieldInfo,AnalysisFieldDataStructure>> mStorage = new Dictionary<SegmentAnalysis, Dictionary<FieldInfo,AnalysisFieldDataStructure>>();
        private List<float> mTimeStamps = new List<float>();
        public AnalysisDataStoreSerialization mSerialization;
        public List<Dictionary<FieldInfo, string>> mSerializedList = new List<Dictionary<FieldInfo, string>>();
        private int mFieldInfoCount;
        private int mCounter;
        private int mSubCount = 0;
        public bool Ignore=false;
        /// <summary>
        /// Initialize component 
        /// </summary>
        public AnalysisDataStore(List<SegmentAnalysis> vAnalysisSegments )
        {
            mAnaylsisDataStoreSettings=  new AnaylsisDataStoreSettings(vAnalysisSegments);
            mSerialization = new AnalysisDataStoreSerialization(this);
            foreach (var vKvPairing in mAnaylsisDataStoreSettings.StoredAnalysisFields)
            { 
                var  vAnalysisTrackingDataStructure =new   Dictionary<FieldInfo,  AnalysisFieldDataStructure>(vKvPairing.Value.Count);
                foreach (var vField in vKvPairing.Value)
                {
                    var vAnalysisDataStruct = new AnalysisFieldDataStructure();
                    vAnalysisDataStruct.FieldInfoKey = vField;
                    vAnalysisTrackingDataStructure.Add(vField, vAnalysisDataStruct);
                    mFieldInfoCount++;
                }
                mStorage.Add(vKvPairing.Key,vAnalysisTrackingDataStructure);
            }
        }

        public Dictionary<SegmentAnalysis, Dictionary<FieldInfo, AnalysisFieldDataStructure>> Storage
        {
            set { mStorage = value; }
            get { return mStorage; }
        }

        public List<float> TimeStamps
        {
            set { mTimeStamps = value; }
            get { return mTimeStamps; }
        }


        /// <summary>
        /// Removes an analysis field from being tracked
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vValue"></param>
        public void RemoveAnalysisFieldInfo(SegmentAnalysis vKey, FieldInfo vValue)
        {
            mAnaylsisDataStoreSettings.RemoveAnalysisFieldInfo(vKey, vValue);
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
            mAnaylsisDataStoreSettings.RemoveSegment(vKey);
            if (mStorage.ContainsKey(vKey))
            {
                mStorage[vKey] = new Dictionary<FieldInfo, AnalysisFieldDataStructure>();
            }
        }

        public void AddAnalyisSegment(SegmentAnalysis vKey)
        {
            mAnaylsisDataStoreSettings.Reset(vKey);
            if (mStorage.ContainsKey(vKey))
            {
                mStorage[vKey] = new Dictionary<FieldInfo, AnalysisFieldDataStructure>();
            }
        }

        /// <summary>
        /// Adds a new timestamp from the given body frame
        /// </summary>
        /// <param name="vFrame"></param>
        public void AddNewTimestamp(BodyFrame vFrame)
        {
         mTimeStamps.Add(vFrame.Timestamp);   
        }

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
                mSerializedList.Add( new Dictionary<FieldInfo, string>());
            }
            var vList = mSerializedList.ElementAt(mSerializedList.Count - 1);
           
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

                    Debug.Log("Key "+vKvPair.Key);
                    throw;
                }
                
            }
            if (mCounter >=mFieldInfoCount   )
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
                vKvPair.Value.Clear();
            }
        }
    }
}