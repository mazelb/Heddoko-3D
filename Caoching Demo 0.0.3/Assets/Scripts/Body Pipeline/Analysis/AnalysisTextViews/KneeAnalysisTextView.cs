using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
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
                mRightLegAnalysis = null;
                mLeftLegAnalysis = null;
                ClearText();
            }
            else
            {
                mRightLegAnalysis = BodyToAnalyze.RightLegAnalysis;
                mLeftLegAnalysis = BodyToAnalyze.LeftLegAnalysis;
            }
        }

        protected override void BodyFrameUpdated(BodyFrame vFrame)
        {
            if (mRightLegAnalysis != null && mLeftLegAnalysis != null)
            {
                RightKneeFlexionText.text = FeedbackAngleToString(mRightLegAnalysis.AngleKneeFlexion);
                LeftKneeFlexionText.text = FeedbackAngleToString(mLeftLegAnalysis.AngleKneeFlexion);
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