// /**
// * @file DeterminateProgressBar.cs
// * @brief Contains the DeterminateProgressBar class 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// A Determinate Progress Bar, needing a time to completion, and with different animation options
    /// </summary>

    public class DeterminateProgressBar : MonoBehaviour
    {
        public float TimeToCompletion = 1;
        public UnityEngine.UI.Image FillImage;
        private Coroutine mAnimationRoutine;

        public void Clear()
        {
            FillImage.fillAmount = 0;
        }

        /// <summary>
        ///Load the appear animation
        /// </summary>
        public void Appear(float vEndPosition, float vTotalTime)
        {
            if (mAnimationRoutine != null)
            {
                StopCoroutine(mAnimationRoutine);
            }
            TimeToCompletion = vTotalTime;
            mAnimationRoutine = StartCoroutine(BeginAppear(vEndPosition));

        }


        private IEnumerator BeginAppear(float vEndPosition)
        {
            float vLastCallTime = Time.time;
            float vDuration = 0;
            float vStart = FillImage.fillAmount;
            float vDistance = vEndPosition - FillImage.fillAmount;
            while (vDuration < TimeToCompletion)
            {
                float vDeltaTime = Time.time - vLastCallTime;
                float vValue = AnimCurveUtilities.CircOut(vStart, vDistance, vDuration, TimeToCompletion);
                vDuration += vDeltaTime;
                FillImage.fillAmount = vValue;
                vLastCallTime = Time.time;
                yield return null;
            }

        }

        /// <summary>
        /// Load the dissapear animation
        /// </summary>
        public void Dissapear(float vEndPosition, float vTotalTime)
        {
            if (mAnimationRoutine != null)
            {
                StopCoroutine(mAnimationRoutine);
            }
            TimeToCompletion = vTotalTime;
            mAnimationRoutine = StartCoroutine(StartDissapear(vEndPosition));
        }

        private IEnumerator StartDissapear(float vEndPosition)
        {
            float vLastCallTime = Time.time;
            float vDuration = 0;
            float vStart = FillImage.fillAmount;
            float vDistance = vEndPosition - FillImage.fillAmount;
            while (vDuration < TimeToCompletion)
            {
                float vDeltaTime = Time.time - vLastCallTime;
                float vValue = AnimCurveUtilities.CircOut(vStart, vDistance, vDuration, TimeToCompletion);
                vDuration += vDeltaTime;
                FillImage.fillAmount = vValue;
                vLastCallTime = Time.time;
                yield return null;
            }
        }

    }

    public static class AnimCurveUtilities
    {
        public static float CircOut(float vStart, float vDistance, float vElapsedTime, float vTotalDuration)
        {
            vElapsedTime = (vElapsedTime > vTotalDuration) ? 1.0f : vElapsedTime / vTotalDuration;
            vElapsedTime--;
            return vDistance * Mathf.Sqrt(1 - vElapsedTime * vElapsedTime) + vStart;
        }
        public static float ExpoOut(float vStart, float vDistance, float vElapsedTime, float vTotalDuration)
        {
            // clamp elapsedTime to be <= duration
            if (vElapsedTime > vTotalDuration) { vElapsedTime = vTotalDuration; }
            return vDistance * (-Mathf.Pow(2, -10 * vElapsedTime / vTotalDuration) + 1) + vStart;
        }

        public static float CircIn(float vStart, float vDistance, float vElapsedTime, float vTotalDuration)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            vElapsedTime = (vElapsedTime > vTotalDuration) ? 1.0f : vElapsedTime / vTotalDuration;
            return -vDistance * (Mathf.Sqrt(1 - vElapsedTime * vElapsedTime) - 1) + vStart;

        }


        public enum AnimationType
        {
            EaseInCirc,
            EaseOutCirc
        }
    }
}