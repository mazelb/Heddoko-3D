// /**
// * @file IAnalysisFramesSerializer.cs
// * @brief Contains the IAnalysisFramesSerializer interface
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System.Collections.Generic;

namespace Assets.Scripts.Body_Pipeline.Analysis.Serialization
{
    /// <summary>
    /// An interface for serialization AnalysisFrames
    /// </summary>
    public interface IAnalysisFramesSerializer
    {
        /// <summary>
        /// The path to serialize frames to
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Serialize a set of analysis frames
        /// </summary>
        /// <param name="vSet">The object to serialize</param>
        /// <param name="vPath">the path to serialize to</param>
        void Serialize(IAnalysisFramesSet vSet, string vPath);

        void SetAnalysisSegments(List<SegmentAnalysis> vSegments);


    }
}