// /**
// * @file CsvAnalysisFramesSerialization.cs
// * @brief Contains the CsvAnalysisFramesSerialization class
// * @author Mohammed Haider( mohammed@ heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
namespace Assets.Scripts.Body_Pipeline.Analysis.Serialization
{
    /// <summary>
    /// Serialize frames to a csv file
    /// </summary>
    public class CsvAnalysisFramesSerialization : IAnalysisFramesSerialization
    {
        /// <summary>
        /// The path to serialize frames to
        /// </summary>
        public string Path { get; set; }
        public void Serialize(AnalysisFramesSet vSet)
        {
            UnityEngine.Debug.Log("commence serialization");
        }
    }
}