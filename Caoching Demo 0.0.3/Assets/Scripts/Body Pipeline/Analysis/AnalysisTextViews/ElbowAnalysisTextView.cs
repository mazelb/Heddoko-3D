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
        public Text RightElbowPronation;
        public Text LeftElbowPronation;

        public override string LabelName
        {
            get { return mLabelName; }
        }

        protected override void BodyUpdated()
        {

        }

        public void UpdateLeftArmTextView(TPosedAnalysisFrame vFrame)
        {
            LeftFlexionText.text = FeedbackAngleToString(vFrame.LeftElbowFlexionAngle);
            LeftElbowPronation.text = FeedbackAngleToString(vFrame.LeftForeArmPronationSignedAngle);
        }

        public void UpdateRightArmTextView(TPosedAnalysisFrame vFrame)
        {

            RightFlexionText.text = FeedbackAngleToString(vFrame.RightElbowFlexionAngle);
            RightElbowPronation.text = FeedbackAngleToString(vFrame.RightForeArmPronationSignedAngle);
        }


        public override void ClearText()
        {
            RightFlexionText.text = "";
            LeftFlexionText.text = "";
        }
        /// <summary>
        /// updates the text view
        /// </summary>
        /// <param name="vFrame"></param>
        public void UpdateView(TPosedAnalysisFrame vFrame)
        {
            UpdateLeftArmTextView(vFrame);
            UpdateRightArmTextView(vFrame);
        }
    }
}