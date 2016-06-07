/**
* @file AnaylsisDataStoreSettings.cs
* @brief Contains the 
* @author Mohammed Haider(mohammed@heddoko.com) 
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets.Scripts.Body_Pipeline.Analysis.Settings
{
    /// <summary>
    /// An analysis setting mechanism, allowing the caller to select specific fields to be stored and maintained
    /// </summary>
    public class AnaylsisDataStoreSettings
    {
        private Dictionary<SegmentAnalysis, List<FieldInfo>> mStoredAnalysisFields;
        private List<SegmentAnalysis> mAnalsisSegmentTemplate;
        public AnaylsisDataStoreSettings(List<SegmentAnalysis> vAnalsisSegments)
        {
            mStoredAnalysisFields = new Dictionary<SegmentAnalysis, List<FieldInfo>>();
            AddAnalysisSegments(ref mStoredAnalysisFields, vAnalsisSegments);
        }

        public Dictionary<SegmentAnalysis, List<FieldInfo>> StoredAnalysisFields
        {
            get { return mStoredAnalysisFields; }
        }

        /// <summary>
        /// Removes an analysis field info  . If it exists, then it is succesfully removed. 
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vValue"></param>
        /// <returns>Was the field info removed succesfully?</returns>
        public bool RemoveAnalysisFieldInfo(SegmentAnalysis vKey, FieldInfo vValue)
        {
            bool vResult = true;
            try
            {
                StoredAnalysisFields[vKey].Remove(vValue);
            }
            catch (Exception vE)
            {
                UnityEngine.Debug.Log(vE);
                vResult = false;
            }
            return vResult;
        }

        /// <summary>
        /// add an analysis field info  . If it doesn't exists, then it is succesfully added. 
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vValue"></param>
        /// <returns>Was the field info added succesfully?</returns>
        public bool AnalysisAnalysisFieldInfo(SegmentAnalysis vKey, FieldInfo vValue)
        {
            if (!StoredAnalysisFields[vKey].Contains(vValue))
            {
                StoredAnalysisFields[vKey].Add(vValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all of a Analysis segment's values 
        /// </summary>
        /// <param name="vKey"></param>
        internal void RemoveSegment(SegmentAnalysis vKey)
        {
            StoredAnalysisFields[vKey].Clear();
        }

        /// <summary>
        /// Resets the analysis fields back to default
        /// </summary>
        public void Reset()
        {
            AddAnalysisSegments(ref mStoredAnalysisFields, mAnalsisSegmentTemplate);
        }

        /// <summary>
        /// Reset an analyis field to its default state
        /// </summary>
        /// <param name="vKey"></param>
        public void Reset(SegmentAnalysis vKey)
        {
            AddAnalysisSegment(vKey, ref mStoredAnalysisFields);
        }
        /// <summary>
        /// Adds analysis segments
        /// </summary>
        private static void AddAnalysisSegments(ref Dictionary<SegmentAnalysis, List<FieldInfo>> vDictionary, List<SegmentAnalysis> vAnalsisSegmentTemplate)
        {
            //Don't fill segment's fields info that are not in template
            foreach (var vAnalysisSegment in vAnalsisSegmentTemplate)
            {
                AddAnalysisSegment(vAnalysisSegment, ref vDictionary);
            }
        }

        /// <summary>
        /// Fill out an analysis segment's field info list
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vDictionary"></param>
        private static void AddAnalysisSegment(SegmentAnalysis vKey, ref Dictionary<SegmentAnalysis, List<FieldInfo>> vDictionary)
        {
            if (vDictionary.ContainsKey(vKey) && vDictionary[vKey].Count > 0)
            {
                vDictionary[vKey].Clear();
            }
            //get all the field information the analysis segments that are of type System.Single
            var vFields = vKey.GetType().GetFields().Where(vField => vField.FieldType == typeof(System.Single));
            //get their types
            List<FieldInfo> vFieldTypes = new List<FieldInfo>();
            foreach (var vField in vFields)
            {
                var vCustomAttribute = vField.GetCustomAttributes(typeof (AnalysisAttribute), true);
                foreach (var vAttri in vCustomAttribute)
                {
                    if (((AnalysisAttribute) vAttri).IgnoreAttribute)
                    {
                        continue;
                    }
                    vFieldTypes.Add(vField);
                }
                 
              
            }
            //store it
            vDictionary.Add(vKey, vFieldTypes);
        }
    }
}