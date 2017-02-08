// /**
// * @file IAnalysisFramesSerialization.cs
// * @brief Contains the IAnalysisFramesSerialization interface
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */
namespace Assets.Scripts.Body_Pipeline.Analysis.Serialization
{
    /// <summary>
    /// An interface for serialization AnalysisFrames
    /// </summary>
    public interface IAnalysisFramesSerialization
    {
        /// <summary>
        /// The path to serialize frames to
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// Serialize a set of analysis frames
        /// </summary>
        /// <param name="vSet">The object to serialize</param>
        void Serialize(AnalysisFramesSet vSet);

        
    }
}