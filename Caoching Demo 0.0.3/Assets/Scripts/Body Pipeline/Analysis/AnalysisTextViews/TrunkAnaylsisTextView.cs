/** 
* @file TrunkAnaylsisTextView.cs
* @brief Contains the TrunkAnaylsisTextView  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.Torso;
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
        [SerializeField]
        private TorsoAnalysis mTorsoAnalysis;


        public override string LabelName
        {
            get { return mLabelName; }
        }
        /// <summary>
        /// The body has been updated
        /// </summary>
        protected override void BodyUpdated()
        {
            if (BodyToAnalyze != null)
            {
                mTorsoAnalysis = BodyToAnalyze.TorsoAnalysis;
            }
            else
            {
                mTorsoAnalysis = null;
                ClearText();
            }
        }

        protected override void BodyFrameUpdated(BodyFrame vFrame)
        {
            if (mTorsoAnalysis != null)
            {
                LateralBendingAngle.text = FeedbackAngleToString(mTorsoAnalysis.AngleTorsoLateral);
                InclinationAngle.text = FeedbackAngleToString(mTorsoAnalysis.AngleTorsoFlexion);
                RotationAngle.text = FeedbackAngleToString( mTorsoAnalysis.AngleTorsoRotation);
            }
        }

        protected override void ClearText()
        {
            LateralBendingAngle.text = "";
            InclinationAngle.text = "";
            RotationAngle.text = "";
        }
    }
}