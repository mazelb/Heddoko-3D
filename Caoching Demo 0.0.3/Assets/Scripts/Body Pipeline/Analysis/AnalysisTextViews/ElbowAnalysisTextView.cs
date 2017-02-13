using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;
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

        public Text RightFlexionText;
        public Text LeftFlexionText;

        public override string LabelName
        {
            get { return mLabelName; }
        }

        protected override void BodyUpdated()
        {

        }

        public void UpdateLeftArmTextView(float vLeftElbowFlexionAngle)
        {
            LeftFlexionText.text = FeedbackAngleToString(vLeftElbowFlexionAngle);

        }

        public void UpdateRightArmTextView(float vRightElbowFlexionAngle)
        {

            RightFlexionText.text = FeedbackAngleToString(vRightElbowFlexionAngle);

        }


        public override void ClearText()
        {
            RightFlexionText.text = "";
            LeftFlexionText.text = "";
        }

        public void UpdateView(TPosedAnalysisFrame vFrame)
        {
            UpdateLeftArmTextView(vFrame.LeftElbowFlexionSignedAngle);
            UpdateRightArmTextView(vFrame.RightElbowFlexionSignedAngle);
        }
    }
}