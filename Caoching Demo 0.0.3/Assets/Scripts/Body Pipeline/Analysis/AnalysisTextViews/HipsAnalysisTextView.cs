﻿using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisTextViews
{
    public class HipsAnalysisTextView : AnaylsisTextView
    {
        private string mLabelName = "Hips Analysis";
        public Text RightHipFlexionText;
        public Text LeftHipFlexionText;
        public Text RightHipAbductionText;
        public Text LeftHipAbductionText;
        public Text RightHipRotation;
        public Text LeftHipRotationText;

        private LeftLegAnalysis mLeftLegAnalysis;
        private RightLegAnalysis mRightLegAnalysis;

        public override string LabelName
        {
            get
            {
                return mLabelName;
            }
        }

        protected override void BodyUpdated()
        {
            if (BodyToAnalyze == null)
            {
                mLeftLegAnalysis = null;
                mRightLegAnalysis = null;
                ClearText();
            }
            else
            {
                mLeftLegAnalysis = BodyToAnalyze.LeftLegAnalysis;
                mRightLegAnalysis = BodyToAnalyze.RightLegAnalysis;
            }
        }

        protected override void BodyFrameUpdated(BodyFrame vFrame)
        {
            if (mLeftLegAnalysis != null && mRightLegAnalysis != null)
            {
                RightHipFlexionText.text = FeedbackAngleToString(mRightLegAnalysis.AngleHipFlexion);
                LeftHipFlexionText.text = FeedbackAngleToString(mLeftLegAnalysis.AngleHipFlexion);
                RightHipAbductionText.text = FeedbackAngleToString(mRightLegAnalysis.AngleHipAbduction);
                LeftHipAbductionText.text = FeedbackAngleToString(mLeftLegAnalysis.AngleHipAbduction);
                RightHipRotation.text = FeedbackAngleToString(mRightLegAnalysis.AngleHipRotation);
                LeftHipRotationText.text = FeedbackAngleToString(mLeftLegAnalysis.AngleHipRotation);
            }
            else
            {
                ClearText();
            }
        }

        protected override void ClearText()
        {
            RightHipFlexionText.text = "";
            LeftHipFlexionText.text = "";
            RightHipAbductionText.text = "";
            LeftHipAbductionText.text = "";
            RightHipRotation.text = "";
            LeftHipRotationText.text = "";
        }



    }
}