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
        public Text LeftShoulderHorizontalAdductionText;
        public Text RightShoulderHorizontalAdductionText;
        public Text LeftInternalExternalRotation;
        public Text RightInternalExternalRotation;
        public Text RightShoulderAbductionText;
        public Text LeftShoulderAbductionText;
        public override string LabelName
        {
            get { return mLabelName; }
        }

        protected override void BodyUpdated()
        {
            
        }

        /// <summary>
        /// Updates the left arm text view
        /// </summary>
        /// <param name="vFrame"></param>
        private void UpdateLeftShoulderTextView(TPosedAnalysisFrame vFrame)
        {

            LeftShoulderFlexionText.text = FeedbackAngleToString(vFrame.LeftShoulderFlexionSignedAngle);
            LeftShoulderAbductionText.text = FeedbackAngleToString(vFrame.LeftShoulderVerticalAbductionSignedAngle);
            LeftShoulderHorizontalAdductionText.text = FeedbackAngleToString(vFrame.LeftShoulderHorizontalAbductionSignedAngle);
            LeftInternalExternalRotation.text = FeedbackAngleToString(vFrame.LeftShoulderRotationSignedAngle);
        }

        /// <summary>
        /// Updates the right arm text view
        /// </summary>
        /// <param name="vFrame"></param>
        private void UpdateRightArmTextView(TPosedAnalysisFrame vFrame)
        {
            RightShoulderFlexionText.text = FeedbackAngleToString(vFrame.RightShoulderFlexionSignedAngle);
            RightShoulderAbductionText.text = FeedbackAngleToString(vFrame.RightShoulderVerticalAbductionSignedAngle);
            RightShoulderHorizontalAdductionText.text = FeedbackAngleToString(vFrame.RightShoulderHorizontalAbductionSignedAngle);
            RightInternalExternalRotation.text = FeedbackAngleToString(vFrame.RightShoulderRotationSignedAngle);
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
            UpdateLeftShoulderTextView(vFrame);
            UpdateRightArmTextView(vFrame);
        }
    }
}