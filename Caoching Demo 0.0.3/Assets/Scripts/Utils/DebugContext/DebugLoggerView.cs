
/** 
* @file DebugLoggerView.cs
* @brief Contains the DebugLoggerView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using Assets.Scripts.Communication;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.DebugContext
{
    /// <summary>
    /// A monobehaviour that enables the current debug
    /// </summary>
    public class DebugLoggerView : MonoBehaviour
    {
        public Text DebugLabelStatus;
        public PlayerStreamManager StreamManager;
        private bool mIsDebugging;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                
                mIsDebugging = !mIsDebugging;
                 Body vCurrentBody = StreamManager.CurrentBodyInPlay;
                if (vCurrentBody != null)
                {
                   // vCurrentBody.View.IsDebugging = mIsDebugging;
                    vCurrentBody.MBodyFrameThread.IsDebugging = mIsDebugging;
                } 
            }
        }
    }
}
