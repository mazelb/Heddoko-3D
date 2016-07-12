﻿/** 
* @file ShoulderAnalyisTextView.cs
* @brief Contains the ShoulderAnalyisTextView  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
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
        private RightArmAnalysis mRightArmAnalysis;
        private LeftArmAnalysis mLeftArmAnalysis;

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

            if (BodyToAnalyze != null)
            {
                mLeftArmAnalysis = BodyToAnalyze.LeftArmAnalysis;
                mRightArmAnalysis = BodyToAnalyze.RightArmAnalysis;
            }
            else
            {
                mRightArmAnalysis = null;
                mLeftArmAnalysis = null;
                ClearText();
            }
        }

        protected override void BodyFrameUpdated(BodyFrame vFrame)
        {
            if (mLeftArmAnalysis != null && mRightArmAnalysis != null)
            {
<<<<<<< HEAD
                RightShoulderFlexionText.text = FeedbackAngleToString(mRightArmAnalysis.SignedShoulderFlexion);
                LeftShoulderFlexionText.text = FeedbackAngleToString(mLeftArmAnalysis.SignedShoulderFlexion);
                RightShoulderAbductionText.text = FeedbackAngleToString(mRightArmAnalysis.SignedShoulderVerticalAbduction);
                LeftShoulderAbductionText.text = FeedbackAngleToString(-1f * mLeftArmAnalysis.SignedShoulderVerticalAbduction); 
=======
                RightShoulderFlexionText.text = FeedbackAngleToString(mRightArmAnalysis.SignedAngleShoulderFlexion);
                LeftShoulderFlexionText.text = FeedbackAngleToString(mLeftArmAnalysis.SignedAngleElbowFlexion);
                RightShoulderAbductionText.text = FeedbackAngleToString(mRightArmAnalysis.SignedAngleElbowAdduction);
                LeftShoulderAbductionText.text = FeedbackAngleToString(mLeftArmAnalysis.SignedAngleElbowAdduction); 
>>>>>>> 096bb2ae014b51e65bce63c5e77e735a22c23b39
            }
        }

        /// <summary>
        /// Clears the text
        /// </summary>
        protected override void ClearText()
        {
            RightShoulderFlexionText.text = "";
            LeftShoulderFlexionText.text = "";
            RightShoulderAbductionText.text = "";
            LeftShoulderAbductionText.text = "";
         
        }
    }
}