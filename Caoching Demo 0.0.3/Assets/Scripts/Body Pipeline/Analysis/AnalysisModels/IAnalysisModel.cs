/** 
* @file IAnalysisModel.cs
* @brief Contains the IAnalysisModel  interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels
{
    /// <summary>
    /// Provides an interface for Analysis Sub components(REBA/RULA SCORE,NIOSH, SNOOKTABLE, ETC)
    /// </summary>
    public interface IAnalysisModel
    {
        /// <summary>
        /// Update the Analysis Sub Component
        /// </summary>
        /// <param name="vSegmentAnalysis"></param>
        void Update(SegmentAnalysis vSegmentAnalysis);
    }
}