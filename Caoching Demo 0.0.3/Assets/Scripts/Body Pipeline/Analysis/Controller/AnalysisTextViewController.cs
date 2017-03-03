// /**
// * @file AnalysisTextViewController.cs
// * @brief Contains the AnalysisTextViewController class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using Assets.Scripts.Body_Data.View.Anaylsis;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisTextViews;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Controller
{
    /// <summary>
    /// A controller for analysis text views. Data is fetched from a Analysis set
    /// </summary>
    public class AnalysisTextViewController : MonoBehaviour
    {
        public AnalysisFramesCollectionController FramesCollectionController;

        public ElbowAnalysisTextView ElbowText;
        public KneeAnalysisTextView KneeText;
        public HipsAnalysisTextView HipsText;
        public ShoulderAnalyisTextView ShoulderText;
        public TrunkAnaylsisTextView TrunkText;


        public void UpdateView(TPosedAnalysisFrame vFrame)
        {
            ElbowText.UpdateView(vFrame);
            KneeText.UpdateView(vFrame);
            HipsText.UpdateView(vFrame);
            ShoulderText.UpdateView(vFrame);
            TrunkText.UpdateView(vFrame);
        }
    }
}