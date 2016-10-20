using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisTextViews
{
    /// <summary>
    /// Elbow analysis
    /// </summary>
    public class ElbowAnalysisTextView : AnaylsisTextView
    {
        private string mLabelName = "Elbow Analysis";
        private RightArmAnalysis mRightArmAnalysis;
        private LeftArmAnalysis mLeftArmAnalysis;
        public Text RightFlexionText;
        public Text LeftFlexionText;

        public override string LabelName
        {
            get { return mLabelName; }
        }

        protected override void BodyUpdated()
        {
            //remove listeners
            if (mRightArmAnalysis != null)
            {
                mRightArmAnalysis.AnalysisCompletedEvent -= UpdateRightArmTextView;
            }

            if (mLeftArmAnalysis != null)
            {
                mLeftArmAnalysis.AnalysisCompletedEvent -= UpdateLeftArmTextView;
            }
            if (BodyToAnalyze == null)
            {
                mRightArmAnalysis = null;
                mLeftArmAnalysis = null;
            }
            else
            {
                mRightArmAnalysis = BodyToAnalyze.RightArmAnalysis;
                mLeftArmAnalysis = BodyToAnalyze.LeftArmAnalysis;
                //register listeners
                if (mRightArmAnalysis != null)
                {
                    mRightArmAnalysis.AnalysisCompletedEvent += UpdateRightArmTextView;
                }

                if (mLeftArmAnalysis != null)
                {
                    mLeftArmAnalysis.AnalysisCompletedEvent += UpdateLeftArmTextView;
                }
            }
        }

        private void UpdateLeftArmTextView(SegmentAnalysis vAnalysis)
        {
            if (mLeftArmAnalysis != null)
            {
                LeftFlexionText.text = FeedbackAngleToString(mLeftArmAnalysis.LeftElbowFlexionAngle);
            }

            else
            {
                ClearText();
            }
        }

        private void UpdateRightArmTextView(SegmentAnalysis vAnalysis)
        {
            if (mRightArmAnalysis != null)
            {
                RightFlexionText.text = FeedbackAngleToString(mRightArmAnalysis.RightElbowFlexionAngle);

            }
            else
            {
                ClearText();
            }
        }


        protected override void ClearText()
        {
            RightFlexionText.text = "";
            LeftFlexionText.text = "";
        }
    }
}