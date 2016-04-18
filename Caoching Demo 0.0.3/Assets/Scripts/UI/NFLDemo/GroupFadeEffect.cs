/** 
* @file GroupFadeEffect.cs
* @brief Contains the GroupFadeEffect class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// Fades/displays a list of images, texts, and disables button interaction
    /// </summary>
    [Serializable]
    public class GroupFadeEffect : MonoBehaviour
    {
        public delegate void AnimationCompletedDel();

        public event AnimationCompletedDel AnimationCompleteTrigger;
        public Button[] Buttons;
        public Image[] Images;
        public Text[] Text;
        [SerializeField]
        private bool mFinishedAnimating;
        /// <summary>
        /// The time it takes to hide/show items
        /// </summary>
        public float HideShowTimer = 1f;

        /// <summary>
        /// Has the effect finished animating? public getter, private setter
        /// </summary>
        public bool FinishedAnimating
        {
            get
            {
                return mFinishedAnimating;
            }
            private set { mFinishedAnimating = value; }
        }
        /// <summary>
        /// Hides the group
        /// </summary>
        public void Hide()
        {
            FinishedAnimating = false;
            StartCoroutine(AlterInteraction(false, 0f));
        }


        /// <summary>
        /// Shows the group
        /// </summary>
        public void Show()
        {
            FinishedAnimating = false;
            StartCoroutine(AlterInteraction(true, 1f));
        }

        /// <summary>
        /// Forces the group to render in scene
        /// </summary>
        public void ForceShow()
        {
            float vStartTime = 0;

            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].interactable = true;
            }


            for (int i = 0; i < Images.Length; i++)
            {
                Color vCurrCol = Images[i].color;
                vCurrCol.a = 1;
                Images[i].color = vCurrCol;
            }
            for (int i = 0; i < Text.Length; i++)
            {
                Color vCurrCol = Text[i].color;
                vCurrCol.a = 1;
                Text[i].color = vCurrCol;
            }



            if (AnimationCompleteTrigger != null)
            {
                AnimationCompleteTrigger();
            }
        }

        /// <summary>
        /// Alters the interaction of all items
        /// </summary>
        /// <returns></returns>
        private IEnumerator AlterInteraction(bool vInteractive, float vNewAlpha)
        {
            float vStartTime = 0;

               for (int i = 0; i < Buttons.Length; i++)
               {
                   Buttons[i].interactable = vInteractive;
               } 
            float vStartAlpha = vNewAlpha < 0.1f ? 1f : 0f;
            while (true)
            {
                vStartTime += Time.deltaTime;
                float vPercentage = vStartTime / HideShowTimer;
                float vNextAlpha = Mathf.Lerp(vStartAlpha, vNewAlpha, vPercentage);
                for (int i = 0; i < Images.Length; i++)
                {
                    Color vCurrCol = Images[i].color;
                    vCurrCol.a = vNextAlpha;
                    Images[i].color = vCurrCol;
                }
                for (int i = 0; i < Text.Length; i++)
                {
                    Color vCurrCol = Text[i].color;
                    vCurrCol.a = vNextAlpha;
                    Text[i].color = vCurrCol;
                }
                if (vPercentage >= 1f)
                {
                    FinishedAnimating = true;
                    break;
                }
                yield return null;
            }
            if (AnimationCompleteTrigger != null)
            {
                AnimationCompleteTrigger();
            }
        }

    }
}
