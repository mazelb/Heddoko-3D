// /**
// * @file CsvAnalysisFramesSerializer.cs
// * @brief Contains the CsvAnalysisFramesSerializer class
// * @author Mohammed Haider( mohammed@ heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;

namespace Assets.Scripts.Body_Pipeline.Analysis.Serialization
{
    /// <summary>
    /// Serialize frames to a csv file
    /// </summary>
    public class CsvAnalysisFramesSerializer : IAnalysisFramesSerializer
    {

        private List<AnalysisFieldNameTuple> mFilteredFieldTuples = new List<AnalysisFieldNameTuple>();
        /// <summary>
        /// The path to serialize frames to
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Serializes an AnalysisFrame set instance to a csv file.
        /// </summary>
        /// <param name="vSet">the set to serialize</param>
        /// <param name="vPath">the path to serialize to</param>
        public void Serialize(IAnalysisFramesSet vSet, string vPath)
        {
           
           
            var vValue = vSet.GetIterableList();
            Serialize(vValue, vPath);
        }

        private void Serialize(List<TPosedAnalysisFrame> vList, string vPath)
        {
            if (!vPath.EndsWith(".csv"))
            {
                vPath += ".csv";
            }
            StringBuilder vBuilder = new StringBuilder();
            AddHeader(ref vBuilder);
            
            for (int vI = 0; vI < vList.Count; vI++)
            {
                vBuilder.Append(vList[vI].TimeStamp + ",");
                vBuilder.Append(vList[vI].Index + ",");
                vBuilder.Append((int)vList[vI].Status + ",");
                foreach (var vAnalysisFieldNameTuple in mFilteredFieldTuples)
                {
                    var vItemValue = (float)vAnalysisFieldNameTuple.CachedPropertyInfo.GetValue(vList[vI], null);
                    //truncate down to three decimals
                    vBuilder.Append(vItemValue.ToString("0.000") + ",");
                }
                vBuilder.AppendLine();
            }
            using (StreamWriter vFileWriter = new StreamWriter(vPath))
            {
                vFileWriter.Write(vBuilder.ToString());
            }
        }

        /// <summary>
        /// Sets the analysis segments, and filters out only allowed fields
        /// </summary>
        /// <param name="vSegments"></param>
        public void SetAnalysisSegments(List<SegmentAnalysis> vSegments)
        {
            mFilteredFieldTuples.Clear();
            for (int vI = 0; vI < vSegments.Count; vI++)
            {
                AddAllowedFields(vSegments[vI]);
            }
            //sort the fields according to their order number
            var vList = mFilteredFieldTuples.ToList<AnalysisFieldNameTuple>();
            mFilteredFieldTuples = vList.OrderBy(vAllowedField => vAllowedField.OrderNumber).ToList();
            mFilteredFieldTuples.Clear();
            for (int i = 0; i < vList.Count; i++)
            {
                mFilteredFieldTuples.Add(vList[i]);
            }
        }


        /// <summary>
        /// Adds a header to the referenced builder
        /// </summary>
        /// <param name="vBuilder"></param>
        private void AddHeader(ref StringBuilder vBuilder)
        {
            vBuilder.Append("Timestamp, Frame Index, TPose Value,");
            for (int vI = 0; vI < mFilteredFieldTuples.Count; vI++)
            {
                vBuilder.Append(mFilteredFieldTuples[vI].HeaderName + ",");
            }
            vBuilder.AppendLine();
        }

        /// <summary>
        /// Adds 
        /// </summary>
        /// <param name="vKey"></param>
        private void AddAllowedFields(SegmentAnalysis vKey)
        {
            var vTposeAnalysisPlaceholder = new TPosedAnalysisFrame();
            var vFields = vKey.GetType().GetFields().Where(vField => vField.FieldType == typeof(System.Single));
            foreach (var vField in vFields)
            {
                var vCustomAttribute = vField.GetCustomAttributes(typeof(AnalysisSerialization), true);
                foreach (var vAttri in vCustomAttribute)
                {
                    if (((AnalysisSerialization)vAttri).IgnoreAttribute)
                    {
                        continue;
                    }
                    var vSet = new AnalysisFieldNameTuple
                    {
                        HeaderName = ((AnalysisSerialization)vAttri).AttributeName,
                        Name = vField.Name,
                        CachedPropertyInfo = vTposeAnalysisPlaceholder.GetType().GetProperty(vField.Name),
                        OrderNumber = ((AnalysisSerialization)vAttri).Order
                    };
                    mFilteredFieldTuples.Add(vSet);
                }
            }
        }

        public void Serialize(IAnalysisFramesSet analysisFramesSet, string vPath, int startIndex, int endIndex)
        {
            List<TPosedAnalysisFrame> vFilteredList = new List<TPosedAnalysisFrame>();
            var iterableFrameSet = analysisFramesSet.GetIterableList();
            if (startIndex < endIndex)
            {
                var vEnumerable = from item in iterableFrameSet
                                  where item.Index >= startIndex && item.Index <= endIndex
                                  orderby item.Index
                                  select item;
                  vFilteredList = vEnumerable.ToList();
            }
            else
            {
                var vEnumerable = from item in iterableFrameSet
                                  where item.Index <= endIndex && item.Index <= startIndex
                                  orderby item.Index
                                  select item;
                vFilteredList = vEnumerable.ToList();
            }
            Serialize(vFilteredList, vPath);
        }

        /// <summary>
        /// A structure that holds the name of the field, its order in the output csv and a cached PropertyInfo
        /// </summary>
        private class AnalysisFieldNameTuple
        {
            public string Name { get; set; }
            public string HeaderName { get; set; }

            public int OrderNumber { get; set; }
            public PropertyInfo CachedPropertyInfo { get; set; }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }
    }

}