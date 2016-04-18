/** 
* @file FadeInFadeOutEffect.cs
* @brief Contains the FadeInFadeOutEffect class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.UnityUtilities
{
    /// <summary>
    /// Applies a fade in and fade out effect to gui images
    /// </summary> 
    /**
    * Body class
    * @brief FadeInFadeOutEffect class: Applies a fade in and fade out effect to gui images  
    */
    public class FadeInFadeOutEffect : MonoBehaviour
    {
        [SerializeField]
        private bool mInProgress;
        [SerializeField]
        private Image mCurrentImage;
        [SerializeField]
        public float MinAlpha;
        [SerializeField]
        public float MaxAlpha;
        [SerializeField]
        private bool mLerpUpwards;
        [SerializeField]
        private float mCurrentLerpTime = 0f;
        [SerializeField]
        public float FadeEffectTime;
        [SerializeField]
        private float mLerpPercentage;
        [SerializeField]
        public float mCurrentAlpha = 0;

        private IEnumerator FadingRoutine;
        [SerializeField] private bool mStartEffect;

        /**
        * StartEffect 
        * @brief  Start the effect 
        */

        /// <summary>
        /// Start the effect
        /// </summary>
        public bool StartEffect {
            get
            {
                return mStartEffect;
            }
            set
            {
                if (!value)
                {
                    Color newColor = CurrentImage.color;
                    newColor.a = MaxAlpha;
                    CurrentImage.color = newColor;
                    mLerpUpwards = false;
                    if (FadingRoutine != null)
                    {
                        StopCoroutine(FadingRoutine);
                    }
                }
                mStartEffect = value;
            }
        }
        /**
        * StartEffect 
        * @brief  Image attached to the game object
        */
        /// <summary>
        /// Current Image attached to the game object
        /// </summary>
        private Image CurrentImage
        {
            get
            {
                if (mCurrentImage == null)
                {
                    mCurrentImage = GetComponent<Image>();
                    //if its still null, check if the current gameobject is inactive and try to grab it
 
                }
                return mCurrentImage;
            }
        }
         
        void Start()
        {
            StartEffect = true;
            Color vCurrColor = CurrentImage.color;

           // vCurrColor.a = 0;
            CurrentImage.color = vCurrColor;
            mLerpUpwards = true;
            MinAlpha /= 255f;
            MaxAlpha /= 255f;
        }
        /**
        * Update()
        * @brief  In every update loop, check if the effect is started and then fade in and out accordingly
        */
        void Update()
        {
            if (StartEffect)
            {
                if (!mInProgress)
                {
                    if (mLerpUpwards)
                    {
                        mInProgress = true;
                        FadingRoutine = FadeImage(FadeEffectTime, MaxAlpha);
                        StartCoroutine(FadingRoutine);
                    }
                    else
                    {
                        mInProgress = true;
                        FadingRoutine = FadeImage(FadeEffectTime, MinAlpha);
                        StartCoroutine(FadingRoutine);
                    }
                }
            }
            Color vCurrColor = CurrentImage.color;
            mCurrentAlpha = vCurrColor.a;

        }
        /**
        * FadeImage(float vDuration, float vTargetOpacity)
        * @brief  Fades in/out 
        * @param float vDuration: duration fading affect, vTargetOpacity: the target of the opacity
        */
        private IEnumerator FadeImage(float vDuration, float vTargetOpacity)
        {

            float alpha = CurrentImage.color.a;

            for (float i = 0; i < 1.0f; i += Time.deltaTime / vDuration)
            {
                Color newColor = CurrentImage.color;
                newColor.a = Mathf.SmoothStep(alpha, vTargetOpacity, i);
                CurrentImage.color = newColor;
                yield return null;
            }
            mInProgress = false;
            mLerpUpwards = !mLerpUpwards;
        }
    }
}
