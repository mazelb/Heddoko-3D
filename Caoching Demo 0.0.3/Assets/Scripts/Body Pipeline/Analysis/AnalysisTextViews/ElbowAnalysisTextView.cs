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
            if (BodyToAnalyze == null)
            {
                mRightArmAnalysis = null;
                mLeftArmAnalysis = null;
            }
            else
            {
                mRightArmAnalysis = BodyToAnalyze.RightArmAnalysis;
                mLeftArmAnalysis = BodyToAnalyze.LeftArmAnalysis;
            }
        }

        protected override void BodyFrameUpdated(BodyFrame vFrame)
        {
            if (mRightArmAnalysis != null && mLeftArmAnalysis != null)
            {
                RightFlexionText.text = FeedbackAngleToString(mRightArmAnalysis.RightElbowFlexionAngle);
                LeftFlexionText.text = FeedbackAngleToString(mLeftArmAnalysis.LeftElbowFlexionAngle);
            }
        }

        protected override void ClearText()
        {
            RightFlexionText.text = "";
            LeftFlexionText.text = "";
        }
    }
}