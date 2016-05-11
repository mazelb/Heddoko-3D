using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews
{
    /// <summary>
    /// Parent class for analysis text views. 
    /// </summary>
    public abstract class AnaylsisTextView : MonoBehaviour
    {
        [SerializeField]
        private ColorBlock mHiddenColorBlock;

        [SerializeField]
        private ColorBlock mSelectedColorBlock;
        private Color mSelectedColor;
        private Color mNonSelectedColor;
        private bool mIsInitiazed;
        internal Body mBodyToAnalyze;
        public abstract string LabelName { get; }
        public Button ControlButton;
        public void Hide()
        {
            if (!mIsInitiazed)
            {
                InitializeColors();
            }
            ControlButton.colors = mHiddenColorBlock;

            gameObject.SetActive(false);
        }

        

        public void Show()
        {
            if (!mIsInitiazed)
            {
                InitializeColors();

            }
            ControlButton.colors = mSelectedColorBlock;

            gameObject.SetActive(true);
        }

        void InitializeColors()
        {
            mSelectedColor = new Color(1, 1, 1, 1);
            mNonSelectedColor = new Color(1, 1, 1, 0);
            mIsInitiazed = true;
            mSelectedColorBlock = new ColorBlock();
            mHiddenColorBlock = new ColorBlock();
            mSelectedColorBlock.colorMultiplier = mHiddenColorBlock.colorMultiplier = 1;
            mSelectedColorBlock.fadeDuration = mHiddenColorBlock.fadeDuration = 0.2f;
            mSelectedColorBlock.normalColor =
                mSelectedColorBlock.highlightedColor = mSelectedColorBlock.pressedColor = mSelectedColor;
            mHiddenColorBlock.normalColor = mHiddenColorBlock.highlightedColor = mHiddenColorBlock.pressedColor = mNonSelectedColor;
        }
        public Body BodyToAnalyze
        {
            get { return mBodyToAnalyze; }
            set
            {
                if (mBodyToAnalyze != null)
                {
                    mBodyToAnalyze.View.BodyFrameUpdatedEvent -= BodyFrameUpdated;
                }
                mBodyToAnalyze = value;
                if (mBodyToAnalyze != null)
                {
                    BodyUpdated();
                }
                if (mBodyToAnalyze != null)
                {
                    mBodyToAnalyze.View.BodyFrameUpdatedEvent += BodyFrameUpdated;
                }
            }
        }

        protected abstract void BodyUpdated();

        protected abstract void BodyFrameUpdated(BodyFrame vFrame);


        protected abstract void ClearText();


        /// <summary>
        /// Returns a formatted string of an angle
        /// </summary>
        /// <param name="vFeedbackAngle">the angle to format to</param>
        /// <returns></returns>
        internal static string FeedbackAngleToString(float vFeedbackAngle)
        {
            return vFeedbackAngle.ToString("0.00") + "°";
        }



    }
}
