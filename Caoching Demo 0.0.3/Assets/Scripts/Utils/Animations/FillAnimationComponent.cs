
/** 
* @file FillAnimationComponent.cs
* @brief Contains the FillAnimationComponent class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.Animations
{
    /// <summary>
    /// From a given image, fills or empties its fill component 
    /// </summary>
 public class FillAnimationComponent: MonoBehaviour
    {
        public float FillTime = 50f;
        public float MaxValue = 1f;
        public Image FillImage;
        public Text DisplayText;
        private bool mContinueAnimation;


        /// <summary>
        /// Start the animation with the new given value
        /// </summary>
        /// <param name="vNewValue"></param>
        public void StartAnimation(float vNewValue)
        {
            StopAllCoroutines();
            StartCoroutine(StartFillAnimation(vNewValue));
        }

        /// <summary>
        /// Start the fill animation
        /// </summary>
        /// <param name="vNewVal">the new fill value</param>
        /// <returns></returns>
        private IEnumerator StartFillAnimation(float vNewVal)
        { 
            float vInitialVal = HeddokoMathTools.Map(FillImage.fillAmount, 0.108f, 1f, 0f, MaxValue); 
            float vTimeTaken = 0f;
            while (true)
            {
                vTimeTaken += Time.deltaTime; 
                float vPercentage = vTimeTaken/FillTime;
                float vPreMappedFillVal = Mathf.Lerp(vInitialVal, vNewVal, vPercentage);

                if (vPercentage > 1)
                {
                    FillImage.fillAmount = HeddokoMathTools.Map(vNewVal, 0f, MaxValue, 0.108f, 1f);
                    DisplayText.text = (int)vNewVal + "";
                    break;
                }
                float vMappedFilledVal = HeddokoMathTools.Map(vPreMappedFillVal, 0f, MaxValue, 0.108f, 1f);
                FillImage.fillAmount = vMappedFilledVal;
                DisplayText.text = (int)vNewVal + "";

                yield return null;
            }
        }
    }
}
