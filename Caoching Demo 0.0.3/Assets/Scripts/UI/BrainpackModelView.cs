// /**
// * @file BrainpackModelView.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Assets.Scripts.UI
{
    public class BrainpackModelView: MonoBehaviour
    {
        private Brainpack mBrainpack;
        public Button Button;
        public Text BrainpackIdText;
        private Action<Brainpack> mCallbackAction;
        public ShineEffector Shine;
        public void Initialize(Brainpack vBrainpack, Action<Brainpack> vCallbackAction)
        {
            mBrainpack = vBrainpack;
            mCallbackAction = vCallbackAction;
            BrainpackIdText.text = vBrainpack.Id;
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(ButtonCallbackAction);
            StartCoroutine(ShineEffect());
        }

        void Start()
        {
          StartCoroutine(ShineEffect());
        }

        IEnumerator ShineEffect()
        {
            while (true)
            { 
                float vLastCallTime = Time.time;
                float vDuration = 0;
                float vStart = 1;
                float vDistance =  -1.5f;
                Shine.yOffset = 1;
                float vTimeToComplete = 5f;
                while (vDuration < vTimeToComplete)
                {
                    Shine.effectRoot.SetActive(true);
                    float vDeltaTime = Time.time - vLastCallTime;
                    float vValue = AnimCurveUtilities.ExpoOut(vStart, vDistance, vDuration, vTimeToComplete);
                    vDuration += vDeltaTime;
                    Shine.YOffset= vValue;
                    vLastCallTime = Time.time;
                    yield return null;
                }
                yield return null;
            }
     
        }
        private void ButtonCallbackAction()
        {
            if (mCallbackAction != null)
            {
                mCallbackAction(mBrainpack);
            }
        }

        public void Clear()
        {
            Button.onClick.RemoveAllListeners();
            StopCoroutine(ShineEffect());
            
        }
    }
}