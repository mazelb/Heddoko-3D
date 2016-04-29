
/** 
* @file DebugContextChecker.cs
* @brief Contains the DebugContextChecker class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.Utils.DebugContext;
using Assets.Scripts.Utils.DebugContext.logging;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    /// <summary>
    /// The purpose of this script is to check if the platform is in a debug/development context. If so, then the children are
    /// enabled, else left disabled
    /// </summary>
    public class DebugContextChecker : MonoBehaviour
    {
        public bool DisableDebugging = false;
        public BrainpackComPortText BrainpackComPortText;
        private bool ResetTPosButtonEnabled;
        private int mHomeTPoseKeyCounter = 0;
        private int mDebugContextEnablerCounter = 0;
        [SerializeField]
        private GameObject mChildren;
        [SerializeField]
        private GameObject[] mDebuggingItems;
        [SerializeField]
        private GameObject mSegmentOptions;
        public PlayerStreamManager PlayerStreamManager;
        public Text DebugToggleInfo;
        public Animator DebugTextAnimator;
       
        [SerializeField]
        private bool mDebuggingActive = false;

        void Awake()
        {
            DebugLogger.Settings.LogAll = false ;
            if (!DisableDebugging)
            {
                DebugLogger.Settings.LogAll = true;
                bool vIsDebug = false;
#if UNITY_EDITOR
                vIsDebug = true;

#endif
#if DEVELOPMENT_BUILD
           vIsDebug = true;
#endif
                mDebuggingActive = vIsDebug;
                mChildren.SetActive(mDebuggingActive);
                mSegmentOptions.SetActive(mDebuggingActive);
                foreach (var vDebuggingItems in mDebuggingItems)
                {
                    vDebuggingItems.SetActive(mDebuggingActive);
                }
            }
            if (DisableDebugging)
            {
                DebugLogger.Settings.LogAll = false;
                bool vIsDebug = false; 
                mDebuggingActive = false;
                mChildren.SetActive(false);
                mSegmentOptions.SetActive(false);
                foreach (var vDebuggingItems in mDebuggingItems)
                {
                    vDebuggingItems.SetActive(false);
                }

            }

        }
        public void ToggleDebugContext()
        {
            //DebugTextAnimator.clip = Clip; 
           
            mDebuggingActive = !mDebuggingActive;
            if (DebugTextAnimator && DebugToggleInfo)
            {
                DebugToggleInfo.text = mDebuggingActive ? "DEBUG ENABLED" : "DEBUG DISABLED";
                if (mDebuggingActive && !DebugTextAnimator.enabled)
                {
                    DebugTextAnimator.enabled = true;
                }
                DebugTextAnimator.Play("Debug text animation", -1, 0f);
            }
            mChildren.SetActive(mDebuggingActive);
            mSegmentOptions.SetActive(mDebuggingActive);
     
            foreach (var vDebuggingItems in mDebuggingItems)
            {
                vDebuggingItems.SetActive(mDebuggingActive);
            }
        }


        void OnGUI()
        {
            if (!DisableDebugging)
            {
                Event e = Event.current;
                if (!ResetTPosButtonEnabled && Input.anyKeyDown && e.isKey)
                {
                    if (e.keyCode == KeyCode.Home)
                    {
                        mHomeTPoseKeyCounter++;
                        if (mHomeTPoseKeyCounter == 5)
                        {
                            InputHandler.RegisterKeyboardAction(HeddokoDebugKeyMappings.ResetFrame, PlayerStreamManager.ResetBody);
                            mHomeTPoseKeyCounter = 0;
                        }
                    }
                    if (e.keyCode == KeyCode.F12)
                    {
                        mDebugContextEnablerCounter++;
                        if (mDebugContextEnablerCounter == 5)
                        {
                            ToggleDebugContext();
                            if (BrainpackComPortText)
                            {
                                BrainpackComPortText.EnableDisable();
                            }
                            mDebugContextEnablerCounter = 0;
                        }
                    }

                    else if (e.keyCode != KeyCode.Home)
                    {
                        mHomeTPoseKeyCounter = 0;
                    }
                    else if (e.keyCode == KeyCode.F12)
                    {
                        mDebugContextEnablerCounter = 0;
                    }
                }
            }
            

        }
    }


}
