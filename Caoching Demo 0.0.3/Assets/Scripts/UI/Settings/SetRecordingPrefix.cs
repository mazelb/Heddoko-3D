/** 
* @file SetRecordingPrefix.cs
* @brief Contains the SetRecordingPrefix  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Text.RegularExpressions;
using Assets.Scripts.Communication.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{
    /// <summary>
    /// a script that validates and checks recording names before they are sent to the brainpack
    /// </summary>
    public class SetRecordingPrefix : MonoBehaviour
    {
        public InputField RecordingPrefixInputField;
        public Button SetRecordingPrefixButton;
        public Text WarningBox;
        private const int MAX_REC_LENGTH = 65;
        [SerializeField]
        private string mRecordingPrefix = "";
        void Awake()
        {
            RecordingPrefixInputField.onValueChange.AddListener(ValidateInputField);
            SetRecordingPrefixButton.onClick.AddListener(SetPrefix);
        }

        /// <summary>
        /// Validate the input field, discards those that have non alphanumeric characters
        /// </summary>
        /// <param name="vNewInput"></param>
        public void ValidateInputField(string vNewInput)
        {
            bool vHasSpecialChars = false;
            bool vIsGreaterThanMax;
            bool vZeroLength = false;
            string vError = "";
            string vStrRegex = @"^[a-zA-Z0-9]+$";

            vHasSpecialChars = !Regex.IsMatch(vNewInput, vStrRegex);
            vIsGreaterThanMax = vNewInput.Length > MAX_REC_LENGTH;
            vZeroLength = string.IsNullOrEmpty(vNewInput);
            if (vHasSpecialChars)
            {
                vError += "You cannot use any special characters.\n";
                if (vZeroLength)
                {
                    vError = "";
                }
            }
            if (vIsGreaterThanMax)
            {
                vError += "The maximum permitted file length is "+ MAX_REC_LENGTH+". Please shorten.";
            }
            if (vHasSpecialChars || vIsGreaterThanMax || vZeroLength)
            {
                SetRecordingPrefixButton.interactable = false;
            }
            else
            {
                SetRecordingPrefixButton.interactable = true;
            }

            WarningBox.text = vError;
            mRecordingPrefix = vNewInput;
        }

        private void SetPrefix()
        {
            BrainpackConnectionController.Instance.ChangeRecordingPrefix(mRecordingPrefix);
        }
        public void SetInteraction(bool vFlag)
        {
            RecordingPrefixInputField.interactable = vFlag;
            SetRecordingPrefixButton.interactable = vFlag;
        }


    }
}
