using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
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
            //remove listeners
            if (mLeftLegAnalysis != null)
            {
                mLeftLegAnalysis.AnalysisCompletedEvent -= UpdateLeftHipTextView;
            }
            if (mRightLegAnalysis != null)
            {
                mRightLegAnalysis.AnalysisCompletedEvent -= UpdateRightHipTextView;
            }
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
                //add event handlers
                mLeftLegAnalysis.AnalysisCompletedEvent += UpdateLeftHipTextView;
                mRightLegAnalysis.AnalysisCompletedEvent += UpdateRightHipTextView;
            }
        }


        private void UpdateLeftHipTextView(SegmentAnalysis vAnalysis)
        {
            if (mLeftLegAnalysis != null)
            {
                LeftHipFlexionText.text = FeedbackAngleToString(mLeftLegAnalysis.LeftSignedHipFlexionAngle);
                LeftHipAbductionText.text = FeedbackAngleToString(mLeftLegAnalysis.LeftSignedHipAbductionAngle);
                LeftHipRotationText.text = FeedbackAngleToString(mLeftLegAnalysis.LeftHipRotationAngle);
            }

            else
            {
                ClearText();
            }
        }

        private void UpdateRightHipTextView(SegmentAnalysis vAnalysis)
        {
            if (mRightLegAnalysis != null)
            {
                RightHipFlexionText.text = FeedbackAngleToString(mRightLegAnalysis.SignedAngleHipFlexion);
                RightHipAbductionText.text = FeedbackAngleToString(mRightLegAnalysis.SignedRightHipAbductionAngle);
                RightHipRotation.text = FeedbackAngleToString(mRightLegAnalysis.SignedRightHipRotation);
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