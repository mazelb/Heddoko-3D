/** 
* @file ShoulderAnalyisTextView.cs
* @brief Contains the ShoulderAnalyisTextView  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisTextViews
{
    /// <summary>
    /// The text view for the shoulder analysis
    /// </summary>
    public class ShoulderAnalyisTextView : AnaylsisTextView
    {
        private string mLabelName = "Shoulder Analysis";
        public Text RightShoulderFlexionText;
        public Text LeftShoulderFlexionText;
        public Text RightShoulderAbductionText;
        public Text LeftShoulderAbductionText;
        public override string LabelName
        {
            get { return mLabelName; }
        }

        protected override void BodyUpdated()
        {
            //show text field maybe?
        }

        private void UpdateLeftShoulderTextView(float vLeftShoulderFlexionSignedAngle, float vLeftShoulderVerticalAbductionSignedAngle)
        {

            LeftShoulderFlexionText.text = FeedbackAngleToString(vLeftShoulderFlexionSignedAngle);
            LeftShoulderAbductionText.text = FeedbackAngleToString(vLeftShoulderVerticalAbductionSignedAngle);

        }

        private void UpdateRightArmTextView(float vRightShoulderFlexionSignedAngle, float vRightShoulderVerticalAbductionSignedAngle)
        {
            RightShoulderFlexionText.text = FeedbackAngleToString(vRightShoulderFlexionSignedAngle);
            RightShoulderAbductionText.text = FeedbackAngleToString(vRightShoulderVerticalAbductionSignedAngle);

        }


        /// <summary>
        /// Clears the text
        /// </summary>
        public override void ClearText()
        {
            RightShoulderFlexionText.text = "";
            LeftShoulderFlexionText.text = "";
            RightShoulderAbductionText.text = "";
            LeftShoulderAbductionText.text = "";

        }

        public void UpdateView(TPosedAnalysisFrame vFrame)
        {
            UpdateLeftShoulderTextView(vFrame.LeftShoulderFlexionSignedAngle, vFrame.LeftShoulderVerticalAbductionSignedAngle);
            UpdateRightArmTextView(vFrame.RightShoulderFlexionSignedAngle, vFrame.RightShoulderVerticalAbductionSignedAngle);
        }
    }
}