/**
* @file RenameRecordingModule.cs
* @brief Contains the RenameRecordingModule class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using System.Text.RegularExpressions;
using Assets.Scripts.Communication.Controller;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Allows the user to change the name of the next recording
    /// </summary>
    public class RenameRecordingModule : MonoBehaviour
    {
        public Button ButtonSet;
        public InputField RecordingNameField;
        public SlideBlock SlideBlock;
        private string mInvalidInputs = "[^\\w\\s]" ;
        private string mMinOneAlphaCharRegex = "(?=.*[a-z])|(?=.*[A-Z])|(?=.*[0-9])";
        public GameObject TooltipObj;
        public Text TooltipText;
        private string mInputString;
        private bool mToolTipStarted;
        public BrainpackConnectionController BpController;
        void Awake()
        {
            ButtonSet.interactable = false; 
            RecordingNameField.onValueChanged.AddListener(ValidateInput);
            RecordingNameField.onEndEdit.AddListener(OnEditingEnd);
            ButtonSet.onClick.AddListener(Submit);
            
        }

        public void Init(BrainpackConnectionController vBpController)
        {
            BpController = vBpController;
            if (BpController != null)
            {
                BpController.DisconnectedStateEvent += Hide;
            }
        }
        /// <summary>
        /// Validates input, making sure no special characters are entered
        /// </summary>
        /// <param name="vInputString"></param>
        private void  ValidateInput(string vInputString)
        {
            bool vInvalidString = Regex.IsMatch(vInputString, mInvalidInputs) ;
            if (vInvalidString)
            {
                StopCoroutine(HideToolTip());
                ShowTooltip("PLEASE USE ALPHANUMERIC\nCHARACTERS ONLY");
                ButtonSet.interactable = false;
            }
            else if (vInputString.Length < 1  )
            {
                StopCoroutine(HideToolTip());
                ButtonSet.interactable = false;
            }
            else if (!Regex.IsMatch(vInputString, mMinOneAlphaCharRegex))
            {
                StopCoroutine(HideToolTip());
                ShowTooltip("MAKE SURE TO HAVE AT\nLEAST ONE ALPHANUMERIC CHARACTER");
                ButtonSet.interactable = false;
            }
            else if (vInputString.Length > 26)
            {
                StopCoroutine(HideToolTip());
                ShowTooltip("NAME LENGTH TOO LONG\n MAXIMUM ALLOWED IS 26");
                ButtonSet.interactable = false;
            }
            else
            {
                StopCoroutine(HideToolTip());
                mToolTipStarted = false;
                ButtonSet.interactable = true;
                mInputString = vInputString;
            }
        }


        /// <summary>
        /// On enter, submit
        /// </summary>
        /// <param name="vInput"></param>
        void OnEditingEnd(string vInput)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                //verify if the button is interactable
                if (ButtonSet.interactable)
                {
                    Submit();
                }
            }
        }

        /// <summary>
        /// Show the Module
        /// </summary>
        public void Show()
        {
            if (!SlideBlock.IsOpen)
            {
                SlideBlock.Toggle();
            }
        }

        /// <summary>
        /// Submit information
        /// </summary>
        private void Submit()
        {
            string vSubmittable = mInputString.TrimStart(new[] {' ', '\n'});
            vSubmittable = mInputString.TrimEnd(new[] {' ', '\n'});
            RecordingNameField.text = "";
            ButtonSet.interactable = false;
           BpController.ChangeRecordingPrefix(vSubmittable);
        }

        /// <summary>
        /// Hides the module
        /// </summary>
        public void Hide()
        {
            if (SlideBlock.IsOpen)
            {
                SlideBlock.Toggle();
            }
        }
     
        /// <summary>
        /// Shows a tool tip with a message 
        /// </summary>
        /// <param name="vMessage"></param>
        void ShowTooltip(string vMessage)
        {
            if (!mToolTipStarted)
            {
                mToolTipStarted = true;
                TooltipObj.SetActive(true);
                TooltipText.text = vMessage;
                StartCoroutine(HideToolTip());
            }
        }

        IEnumerator HideToolTip()
        {
            yield return new WaitForSeconds(2f);
            mToolTipStarted = false;
            TooltipObj.SetActive(false);
        }

        /// <summary>
        /// Toggle between hide and show
        /// </summary>
        public void Toggle()
        {
            SlideBlock.Toggle();
        }
    }
}