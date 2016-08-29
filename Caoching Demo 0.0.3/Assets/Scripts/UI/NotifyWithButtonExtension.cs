// /**
// * @file NotifyWithButtonExtension.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using UIWidgets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class NotifyWithButtonExtension: Notify
    {
        public Button OptionButton;
        
        /// <summary>
        /// Removes all callbacks and registers a new callback
        /// </summary>
        /// <param name="vCallbackAction">the callback action to perform</param>
        public void RegisterCallbackAndRemovePreviousCallback(UnityAction vCallbackAction)
        {
            OptionButton.onClick.RemoveAllListeners();
            OptionButton.onClick.RemoveListener(vCallbackAction);
            OptionButton.onClick.AddListener(vCallbackAction);
            OptionButton.onClick.AddListener(Hide);
        }

        void Debudg()
        {
            UnityEngine.Debug.Log("fdafad");
        }
    }
}