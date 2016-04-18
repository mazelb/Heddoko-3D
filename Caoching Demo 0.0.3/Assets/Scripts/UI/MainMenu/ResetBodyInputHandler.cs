
/** 
* @file ResetBodyInputHandler.cs
* @brief Contains the ResetBodyInputHandler class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;

namespace Assets.Scripts.UI.MainMenu
{
    /// <summary>
    /// a component added to a view, resets the body
    /// </summary>
    public class ResetBodyInputHandler : MonoBehaviour
    {

        public PlayerStreamManager Manager;

        private void OnEnable()
        {
            InputHandler.RegisterKeyboardAction(HeddokoDebugKeyMappings.ResetFrame, Manager.ResetBody);
        }

        private void OnDisable()
        {
            InputHandler.RegisterKeyboardAction(HeddokoDebugKeyMappings.ResetFrame, Manager.ResetBody);
        }
    }
}


