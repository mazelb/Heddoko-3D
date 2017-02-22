using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.View.Anaylsis
{
    public class KneeAnalysisTextView : AnaylsisTextView
    {
        private string mLabelName = "Knee Analysis";

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
                ClearText();
            }

            //register listeners
            else
            {

            }
        }


        public void UpdateLeftKneeTextView(float vLeftKneeFlexionSignedAngle)
        {
            LeftKneeFlexionText.text = FeedbackAngleToString(vLeftKneeFlexionSignedAngle);
        }

        public void UpdateRightKneeTextView(float vRightKneeFlexionSignedAngle)
        {
            RightKneeFlexionText.text = FeedbackAngleToString(vRightKneeFlexionSignedAngle);
        }

        public override void ClearText()
        {
            RightKneeFlexionText.text = "";
            LeftKneeFlexionText.text = "";
        }

        public void UpdateView(TPosedAnalysisFrame vFrame)
        {
            UpdateLeftKneeTextView(vFrame.LeftKneeFlexionSignedAngle);
            UpdateRightKneeTextView(vFrame.RightElbowFlexionSignedAngle);
        }
    }
}