/** 
* @file CircularCountdownTimer.cs
* @brief Contains the BrainpackConnectionModule class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Loading
{
    /// <summary>
    /// Countdown timer
    /// </summary>
    public class CircularCountdownTimer : MonoBehaviour
    {
        public Image InnerFill;
        public Image Outline;
        public Text CountdownText;
        public Func<IEnumerator>  OnCompletionAnim;
        /// <summary>
        /// Invoke an animation when 97% close to completion
        /// </summary>
        public Func<IEnumerator> NearCompletionAnim; 
        public float SecondsToAnimate = 1;
        private RectTransform mRect;
        private Vector2 mOutlineStartMinAnch;
        private Vector2 mOutlineStartMaxAnch;


        /// <summary>
        /// Object's RectTransform
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (mRect == null)
                {
                    mRect = GetComponent<RectTransform>();
                }
                return mRect;
            }
        }

        public RectTransform OutlineRectTransform
        {
            get { return Outline.rectTransform; }
        }

        public void ResetOutline()
        {

            OutlineRectTransform.anchorMax = mOutlineStartMaxAnch;
            OutlineRectTransform.anchorMin = mOutlineStartMinAnch;
        }

        /// <summary>
        /// On awake check if the image is set to fill, starts from the top, and clockwise
        /// </summary>
        private void OnAwake()
        {
            InnerFill.type = Image.Type.Filled;
            InnerFill.fillMethod = Image.FillMethod.Radial360;
            InnerFill.fillAmount = 0;
            InnerFill.fillClockwise = true;
            mOutlineStartMaxAnch = OutlineRectTransform.anchorMax;
            mOutlineStartMinAnch = OutlineRectTransform.anchorMin;
        }

        /// <summary>
        /// Initializes the timer with completion, and an optional parameters to be invoked 
        /// </summary>
        /// <param name="vSeconds"></param>
        /// <param name="vCompletionAnim">invoked when animation has completed</param>
        /// <param name="vNearCompletion">invoke when animation is close to 97% complete</param>
        public void Init(float vSeconds, Func<IEnumerator> vCompletionAnim = null, Func<IEnumerator> vNearCompletion = null)
        {
            SecondsToAnimate = vSeconds;
            OnCompletionAnim = vCompletionAnim;
            NearCompletionAnim = vNearCompletion;
        }
        public void StartAnimation()
        {
            StartCoroutine(FillAnimation());
        }

        public IEnumerator FillAnimation()
        {
            float vStartTime = Time.time;
            float vFillAmount = 0;
            bool vNearCompletionInitiated = false;
            while (true)
            {
                float vCurrTime = Time.time - vStartTime;
                float vPercentage = vCurrTime / SecondsToAnimate;
                int vRemaining = (int)(SecondsToAnimate - vCurrTime)+1;
                SetCountdownText(vRemaining);
                vFillAmount = Mathf.Lerp(0, 1, vPercentage);
                if (!vNearCompletionInitiated && vPercentage >= 0.97f)
                { 
                    vNearCompletionInitiated = true;
                    StartNearCompletionAnim();
                }
                if (vFillAmount >= 1)
                {
                    InnerFill.fillAmount = 1;
                    SetCountdownText(0);
                    break;
                }
                InnerFill.fillAmount = vFillAmount;
                yield return null;
            }
            OnCompletion();
            
        }
        

        /// <summary>
        /// Called upon animation completion
        /// </summary>
        private void OnCompletion()
        {
            if (OnCompletionAnim != null)
            { 
                IEnumerator vcompletion = OnCompletionAnim();
                StartCoroutine(vcompletion);
            }
        }

        private void SetCountdownText(int vSecond)
        {
            if (CountdownText != null)
            {
                CountdownText.text = "" + vSecond;
            }
        }

        private void StartNearCompletionAnim()
        {
            if (NearCompletionAnim != null)
            {
                Debug.Log("invoking");
                IEnumerator vNearCompletion = NearCompletionAnim();
                StartCoroutine(vNearCompletion);
            }
        }



    }
}
