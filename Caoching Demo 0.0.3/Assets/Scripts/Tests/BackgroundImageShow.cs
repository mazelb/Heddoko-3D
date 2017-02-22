// /**
// * @file BackgroundImageShow.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections;
using Assets.Scripts.UI;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
    public class BackgroundImageShow : MonoBehaviour
    {
        public Image Image;
        public float Duration = 12f;
        public float EndAlpha = 0.3f;

        void Start()
        {
            StartCoroutine(Appear());
        }

        IEnumerator Appear()
        {
            float vLastCallTime = Time.time;
            float vDuration = 0;
            float vStart = 0;
            float vDistance = EndAlpha;
            Color vColor = Image.color;
            vColor.a = 0;
            Image.color = vColor;
            float vTimeToComplete = Duration;
            while (vDuration < vTimeToComplete)
            {

                float vDeltaTime = Time.time - vLastCallTime;
                float vValue = AnimationHelpers.CircIn(vStart, vDistance, vDuration, vTimeToComplete);
                vDuration += vDeltaTime; 
                vColor.a = vValue;
                Image.color = vColor;
                vLastCallTime = Time.time;
                yield return null;
            }


        }
    }
}