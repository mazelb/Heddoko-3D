using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.View.Anaylsis
{
    public class KneeAnalysisTextView : AnaylsisTextView
    {
        private string mLabelName = "Knee Analysis";
        private RightLegAnalysis mRightLegAnalysis;
        private LeftLegAnalysis mLeftLegAnalysis;
        public Text RightKneeFlexionText;
        public Text LeftKneeFlexionText;
        public override string LabelName
        {
            get { return mLabelName; }
        }



        protected override void BodyUpdated()
        {
            if (BodyToAnalyze == null)
            {
                //Remove listeners
                if (mRightLegAnalysis != null)
                {
                    mRightLegAnalysis.AnalysisCompletedEvent -= UpdateRightKneeTextView;
                }
                if (mLeftLegAnalysis != null)
                {
                    mLeftLegAnalysis.AnalysisCompletedEvent -= UpdateLeftKneeTextView;
                }
                mRightLegAnalysis = null;
                mLeftLegAnalysis = null;
                ClearText();
            }

            //register listeners
            else
            {
                //Remove listeners
                if (mRightLegAnalysis != null)
                {
                    mRightLegAnalysis.AnalysisCompletedEvent -= UpdateRightKneeTextView;
                }
                if (mLeftLegAnalysis != null)
                {
                    mLeftLegAnalysis.AnalysisCompletedEvent -= UpdateLeftKneeTextView;
                }
                mRightLegAnalysis = BodyToAnalyze.RightLegAnalysis;
                mLeftLegAnalysis = BodyToAnalyze.LeftLegAnalysis;
                if (mRightLegAnalysis != null)
                {
                    mRightLegAnalysis.AnalysisCompletedEvent += UpdateRightKneeTextView;
                }
                if (mLeftLegAnalysis != null)
                {
                    mLeftLegAnalysis.AnalysisCompletedEvent += UpdateLeftKneeTextView;
                }
            }
        }


        private void UpdateLeftKneeTextView(SegmentAnalysis vAnalysis)
        {
            if (mLeftLegAnalysis != null)
            {
                LeftKneeFlexionText.text = FeedbackAngleToString(mLeftLegAnalysis.LeftKneeFlexionSignedAngle);
            }

            else
            {
                ClearText();
            }
        }

        private void UpdateRightKneeTextView(SegmentAnalysis vAnalysis)
        {
            if (mRightLegAnalysis != null)
            {
                RightKneeFlexionText.text = FeedbackAngleToString(mRightLegAnalysis.RightKneeFlexionSignedAngle);
            }
            else
            {
                ClearText();
            }
        }



        protected override void ClearText()
        {
            RightKneeFlexionText.text = "";
            LeftKneeFlexionText.text = "";
        }
    }
}