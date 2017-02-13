// /**
// * @file TPosedAnalysisFrame.cs
// * @brief Contains the TPosedAnalysisFrame class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date February 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using Heddoko;

namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels
{
    /// <summary>
    /// A model of an AnalysisFrame that is considered to be tposed
    /// </summary>
    public class TPosedAnalysisFrame : AnalysisFrame
    {
        public TposeStatus Status { get; set; }
        public int Index { get; set; }

        
    }

    public enum TposeStatus
    {
        NonTPose=0,
        PossibleTpose =1,
        Tpose =2
    }
}