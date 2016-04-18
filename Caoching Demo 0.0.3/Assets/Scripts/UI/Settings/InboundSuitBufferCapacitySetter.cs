/** 
* @file InboundSuitBufferCapacitySetter.cs
* @brief Contains the InboundSuitBufferCapacitySetter  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Communication.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{
    /// <summary>
    /// Allows to set the inbound suit buffer size
    /// </summary>
    public class InboundSuitBufferCapacitySetter : MonoBehaviour
    {
        public Button SetterButton;
        public InputField SettingInputField;
        public Text Label;
        private int mInputFieldValue;
        void Awake()
        {
            SetterButton.onClick.AddListener(SetBufferSize);
            SettingInputField.onValueChange.AddListener(UpdateInputFieldValue);
        }


        /// <summary>
        /// Update the value from the input field
        /// </summary>
        /// <param name="vInputString"></param>
        public void UpdateInputFieldValue(string vInputString)
        {
            int vTemp = 0;
            try
            {
                Int32.TryParse(vInputString, out vTemp);
            }
            catch 
            {
                
            }
            if (vTemp >= 2000)
            {
                vTemp = 2000;
            }
            mInputFieldValue = vTemp;
        }


        public void SetBufferSize()
        {
            BodyFrameThread.InboundSuitBufferCap = mInputFieldValue;
            int vVal = BodyFrameThread.InboundSuitBufferCap;
            Label.text = string.Format("Suit buffer size: {0}", vVal);

        }
    }
}
