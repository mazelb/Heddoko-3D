// /**
// * @file TechStarsCurtainCallController.cs
// * @brief Contains the TechStarsCurtainCallController  class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date May 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Utils.UnityUtilities;
using Newtonsoft.Json.Bson;
using UnityEngine;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    /// <summary>
    /// Automated controller for the curtain call. If no input is received after a set time, automates 
    /// the logo to fade out then slide the curtain
    /// </summary>
    public class TechStarsCurtainCallController : MonoBehaviour
    {
        public CurtainCall Curtain;
        public MaskableGraphicFadeOutEffect FadeoutEffect;
        public float Timer = 4f;
        public bool EventStarted = false;
        [SerializeField]
        private bool mUseTimer;

        void Awake()
        {
            FadeoutEffect.FadeoutCompletedEvent += StartCurtainCall;

        }
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.C) && !EventStarted)
            {
                EventStarted = true;
                FadeoutEffect.StartEffect();
            }
            if (mUseTimer)
            {
                if (!EventStarted)
                {
                    Timer -= Time.deltaTime;
                    if (Timer < 0)
                    {
                        EventStarted = true;
                        FadeoutEffect.StartEffect();
                    }
                }
            }

        }

        //Start curtain call
        private void StartCurtainCall()
        {
            Curtain.TriggerAction();
        }
    }
}