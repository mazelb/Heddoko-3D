﻿/** 
* @file TrunkAnaylsisTextView.cs
* @brief Contains the TrunkAnaylsisTextView  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;
using Assets.Scripts.Body_Pipeline.Analysis.Trunk;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisTextViews
{
    /// <summary>
    /// Trunk analysis view
    /// </summary>
    public class TrunkAnaylsisTextView : AnaylsisTextView
    {
        private string mLabelName = "Trunk Analysis";
        public Text LateralBendingAngle;
        public Text InclinationAngle;
        public Text RotationAngle;



        public override string LabelName
        {
            get { return mLabelName; }
        }
        /// <summary>
        /// The body has been updated
        /// </summary>
        protected override void BodyUpdated()
        {

        }


        private void UpdateTrunkAnalysisTextView(float vTrunkLateralSignedAngle, float vTrunkFlexionSignedAngle, float vTrunkRotationSignedAngle)
        {

            LateralBendingAngle.text = FeedbackAngleToString(vTrunkLateralSignedAngle);
            InclinationAngle.text = FeedbackAngleToString(vTrunkFlexionSignedAngle);
            RotationAngle.text = FeedbackAngleToString(vTrunkRotationSignedAngle);

        }

        public override void ClearText()
        {
            LateralBendingAngle.text = "";
            InclinationAngle.text = "";
            RotationAngle.text = "";
        }

        public void UpdateView(TPosedAnalysisFrame vFrame)
        {
            UpdateTrunkAnalysisTextView(vFrame.TrunkLateralSignedAngle, vFrame.TrunkFlexionSignedAngle,
                vFrame.TrunkRotationSignedAngle);
        }
    }
}