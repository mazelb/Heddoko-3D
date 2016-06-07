/**
* @file MaskableGraphicFadeOutEffect.cs
* @brief Contains the MaskableGraphicFadeOutEffect class
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.UnityUtilities
{
    public delegate void FadeoutCompleted();
    /// <summary>
    /// Fades out a maskable graphic in a set time
    /// </summary>
    public class MaskableGraphicFadeOutEffect :MonoBehaviour
    {
        //default to 1.5f
        public float FadeOutSpeed = 1.5f;
        public MaskableGraphic MaskableGraphic;
        public event FadeoutCompleted FadeoutCompletedEvent;
        public void StartEffect()
        {
            StartCoroutine(FadeOut());
        }

   
        private IEnumerator FadeOut()
        {
            float vStatTime = Time.time;
            float vPercentage = 0;
            float vNewAlpha = 0;
            float vStartAlpha = MaskableGraphic.color.a;
            Color vNewColor = MaskableGraphic.color;
            while (vPercentage <= 1f)
            {
                vPercentage = (Time.time-  vStatTime)/ FadeOutSpeed;
                //t = t*t*t * (t * (6f*t - 15f) + 10f)
                vPercentage = vPercentage*vPercentage;
                //  vPercentage = vPercentage*vPercentage*vPercentage* (vPercentage*(6f*vPercentage - 15f) + 10f);
                vNewAlpha = Mathf.Lerp(vStartAlpha, 0, vPercentage);
                
                vNewColor.a = vNewAlpha;
                MaskableGraphic.color = vNewColor;
                yield return null;
            }
            yield return new WaitForSeconds(FadeOutSpeed*0.1f);
            if (FadeoutCompletedEvent != null)
            {
                FadeoutCompletedEvent();
            }
        }
    }
}