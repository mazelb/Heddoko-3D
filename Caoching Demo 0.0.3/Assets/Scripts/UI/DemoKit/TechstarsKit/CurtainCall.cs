/**
* @file CurtainCall.cs
* @brief Contains the CurtainCallclass
* @author Mohammed Haider( mohammed@heddoko.com)
* @date 05 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    [RequireComponent(typeof(SlideBlock))]
    public class CurtainCall: MonoBehaviour
    {
        private SlideBlock mSlideBlockComponent; 

        void Awake()
        {
            mSlideBlockComponent = GetComponent<SlideBlock>();
        }

    public void TriggerAction()
        {
            if (mSlideBlockComponent.IsOpen)
            {
                mSlideBlockComponent.Toggle();
                StartCoroutine(DisableOnFinishCurtain());
            }
        }

        private IEnumerator DisableOnFinishCurtain()
        {
           float mTime = mSlideBlockComponent.Curve.keys[mSlideBlockComponent.Curve.keys.Length - 1].time;
            mTime *= 1.1f;
            yield return new WaitForSeconds(mTime);
            gameObject.SetActive(false);
        }
    }
}