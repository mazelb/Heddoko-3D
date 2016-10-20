/** 
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
                //remove listeners first
                if (mLeftArmAnalysis != null)
                {
                    mLeftArmAnalysis.AnalysisCompletedEvent -= UpdateLeftShoulderTextView;
                }
                if (mRightArmAnalysis != null)
                {
                    mRightArmAnalysis.AnalysisCompletedEvent -= UpdateRightArmTextView;

                }
                //Register listeners
                mLeftArmAnalysis = BodyToAnalyze.LeftArmAnalysis;
                mRightArmAnalysis = BodyToAnalyze.RightArmAnalysis;
                mLeftArmAnalysis.AnalysisCompletedEvent += UpdateLeftShoulderTextView;
                mRightArmAnalysis.AnalysisCompletedEvent += UpdateRightArmTextView;


            }
            else
            {
                //remove listeners first
                if (mLeftArmAnalysis != null)
                {
                    mLeftArmAnalysis.AnalysisCompletedEvent -= UpdateLeftShoulderTextView;
                }
                if (mRightArmAnalysis != null)
                {
                    mRightArmAnalysis.AnalysisCompletedEvent -= UpdateRightArmTextView;

                }
                mRightArmAnalysis = null;
                mLeftArmAnalysis = null;
                ClearText();
            }
        }

        private void UpdateLeftShoulderTextView(SegmentAnalysis vAnalysis)
        {
            if (mLeftArmAnalysis != null)
            {
                LeftShoulderFlexionText.text = FeedbackAngleToString(mLeftArmAnalysis.LeftShoulderFlexionSignedAngle);
                LeftShoulderAbductionText.text = FeedbackAngleToString(mLeftArmAnalysis.LeftShoulderVerticalAbductionSignedAngle);
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
                RightShoulderFlexionText.text = FeedbackAngleToString(mRightArmAnalysis.RightShoulderFlexionSignedAngle);
                RightShoulderAbductionText.text = FeedbackAngleToString(mRightArmAnalysis.RightShoulderVerticalAbductionSignedAngle);
            }
            else
            {
                ClearText();
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