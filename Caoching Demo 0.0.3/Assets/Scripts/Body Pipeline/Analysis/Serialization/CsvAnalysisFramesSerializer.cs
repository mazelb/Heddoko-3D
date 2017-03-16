// /**
// * @file CsvAnalysisFramesSerializer.cs
// * @brief Contains the CsvAnalysisFramesSerializer class
// * @author Mohammed Haider( mohammed@ heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

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
            if (!vPath.EndsWith(".csv"))
            {
                vPath += ".csv";
            }
            StringBuilder vBuilder = new StringBuilder();
            AddHeader(ref vBuilder);
            var vValue = vSet.GetIterableList();
            for (int vI = 0; vI < vValue.Count; vI++)
            {
                vBuilder.Append(vValue[vI].TimeStamp + ",");
                vBuilder.Append(vValue[vI].Index + ",");
                vBuilder.Append((int) vValue[vI].Status + ",");
                foreach (var vAnalysisFieldNameTuple in mFilteredFieldTuples)
                {
                    var vItemValue = (float)vAnalysisFieldNameTuple.CachedPropertyInfo.GetValue(vValue[vI], null);
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
            //sort the fields
            mFilteredFieldTuples=  mFilteredFieldTuples.OrderBy(vAllowedField => vAllowedField.OrderNumber).ToList();
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