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
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Body_Pipeline.Analysis.Settings
{
    /// <summary>
    /// An analysis setting mechanism, allowing the caller to select specific fields to be stored and maintained
    /// </summary>
    public class AnaylsisDataStoreSettings
    {
        private Dictionary<SegmentAnalysis, List<FieldInfo>> mStoredAnalysisFields;
        private List<SegmentAnalysis> mAnalsisSegmentTemplate;
        private Dictionary<FieldInfo, AnalysisSerialization> mSerializedAnalyisFieldOrderMap = new Dictionary<FieldInfo, AnalysisSerialization>();
        private Dictionary<string, FieldInfo> mFieldMapping = new Dictionary<string, FieldInfo>();
        public AnaylsisDataStoreSettings(List<SegmentAnalysis> vAnalsisSegments)
        {
            mStoredAnalysisFields = new Dictionary<SegmentAnalysis, List<FieldInfo>>();
            AddAnalysisSegments(ref mStoredAnalysisFields, vAnalsisSegments);
            //once all analysis segments are finished, then get the sorting map
            foreach (var vKeyValuePair in mStoredAnalysisFields)
            {
                foreach (var vField in vKeyValuePair.Value)
                {
                    var vCustomAttrList = vField.GetCustomAttributes(typeof(AnalysisSerialization), true);
                    foreach (var vAttri in vCustomAttrList)
                    {
                        var vAnalysisSerializedData = (AnalysisSerialization)vAttri; 
                        mSerializedAnalyisFieldOrderMap.Add(vField, vAnalysisSerializedData);
                        //store the fields into a map
                        mFieldMapping.Add(vField.ToString(), vField);
                    }
                }
            }
            SetAttributesFromFile();
        }

        public void SaveAttributeToFile( )
        {
            var vPath = ApplicationSettings.AttributeFileOrderingPath;
            JsonUtilities.ConvertObjectToJson(vPath, mSerializedAnalyisFieldOrderMap);

        }
        /// <summary>
        /// Tries to set the attributes from a file
        /// </summary>
        public void SetAttributesFromFile()
        {
            var vPath = ApplicationSettings.AttributeFileOrderingPath;

            if (!System.IO.File.Exists(vPath))
            {
                SaveAttributeToFile();
            }
            else
            {
                try
                {
                    var vVal = JsonUtilities.JsonFileToObject<Dictionary<string, AnalysisSerialization>>(vPath);
                    foreach (var vAnalysisSerialization in vVal)
                    {
                        var vSegmentAnalysis = vAnalysisSerialization.Value;
                        bool vIsNeg = vSegmentAnalysis.IsSignNegative;
                        string vName = vAnalysisSerialization.Key;
                        int vSign = vIsNeg ? -1 : 1;
                        SegmentAnalysis.SetSign(vName, vSign);
                        if (mFieldMapping.ContainsKey(vName))
                        {
                            var vAnalysisAttr = vAnalysisSerialization.Value;
                            mSerializedAnalyisFieldOrderMap[mFieldMapping[vName]] = vAnalysisAttr; 
                        }
                    }
                }
                catch (Exception vE)
                {
                   UnityEngine.Debug.Log(vE.Message); 
                }
            }

        }

         public AnalysisSerialization GetAnalysisSerializationItem(string vKey)
        {
            if (!mFieldMapping.ContainsKey(vKey))
            {
                return null;
            }
             return mSerializedAnalyisFieldOrderMap[mFieldMapping[vKey]];
        }
        //if (!System.IO.File.Exists(vPath))
        //{
        //    List<FieldInfo> vPreserialized = new List<FieldInfo>();
        //    foreach (var vKeyValuePair in mStoredAnalysisFields)
        //    {
        //        foreach (var vField in vKeyValuePair.Value)
        //        {
        //            var vCustomAttrList = vField.GetCustomAttributes(typeof(AnalysisSerialization), true);
        //            foreach (var vAttri in vCustomAttrList)
        //            {
        //                var vItem = (AnalysisSerialization) vAttri;
        //                vPreserialized.Add(item:vItem);
        //            }
        //        }
        //    }

        //Create a list for the first time based on the default settings 

        //else
        //{

        //}



        /// <summary>
        /// Sets the analysis's field info order
        /// </summary>
        /// <param name="vInfo">the analysis field to set</param>
        /// <param name="vValue">its corresponding order value</param>
        public void SetAnalysisFieldOrder(FieldInfo vInfo, AnalysisSerialization vValue)
        {
            if (mSerializedAnalyisFieldOrderMap.ContainsKey(vInfo))
            {
                mSerializedAnalyisFieldOrderMap[vInfo] = vValue;
            }

        }

        /// <summary>
        /// Get a list of order fields
        /// </summary>
        /// <returns></returns>
        public List<FieldInfo> GetOrderedFieldList()
        {
            List<KeyValuePair<FieldInfo, AnalysisSerialization>> vKeyValuePairs = mSerializedAnalyisFieldOrderMap.ToList();
            vKeyValuePairs.Sort(
                delegate (KeyValuePair<FieldInfo, AnalysisSerialization> vPair1,
                    KeyValuePair<FieldInfo, AnalysisSerialization> vPair2)
                {
                    return vPair1.Value.CompareTo(vPair2.Value);
                }

                );
            List<FieldInfo> vReturn = new List<FieldInfo>();
            foreach (var vKeyValuePair in vKeyValuePairs)
            {
                vReturn.Add(vKeyValuePair.Key);
            }
            return vReturn;
        }


        /// <summary>
        /// Returns the order of the analysis field. If not found, then -1 is returned. 
        /// </summary>
        /// <param name="vInfo">The field to look for</param>
        /// <returns></returns>
        internal int GetOrderOfAnalysisField(FieldInfo vInfo)
        {
            int vResult = -1;
            if (mSerializedAnalyisFieldOrderMap.ContainsKey(vInfo))
            {
                vResult = mSerializedAnalyisFieldOrderMap[vInfo].Order;
            }
            return vResult;
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
                var vCustomAttribute = vField.GetCustomAttributes(typeof(AnalysisSerialization), true);
                foreach (var vAttri in vCustomAttribute)
                {
                    if (((AnalysisSerialization)vAttri).IgnoreAttribute)
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