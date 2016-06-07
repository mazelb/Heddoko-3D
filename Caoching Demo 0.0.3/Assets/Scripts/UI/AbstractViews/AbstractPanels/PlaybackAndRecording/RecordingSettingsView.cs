// /**
// * @file RecordingSettingsView.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections;
 using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    public class RecordingSettingsView : MonoBehaviour
    {
        public RecordingPlayerView RecordingPlayerView;

        public InputField FrameSkippingInput;
        public Text FrameSkippingLabel;
        private IEnumerator mFrameLabelError;
        private int mFrameSkippingValue;

        public InputField FrameSkippingInputMulitiplier;
        public Text FrameSkippingMulitplierLabel;
        private IEnumerator mMultiLabelError;
        private int mFrameMultiSkippingValue;
        public Button ApplyButton;
        void Awake()
        {
            RecordingPlayerView.RecordingPlayerViewLayoutCreatedEvent += RegisterSettingsPanel;
            FrameSkippingInput.onValueChanged.AddListener(ValidateFrameSkippingInput);
            FrameSkippingInputMulitiplier.onValueChanged.AddListener(ValidateFrameMultiplierInput);
            ApplyButton.onClick.AddListener(ApplySettings);
        }

       

        /// <summary>
        /// Validate the frame mulitplier input
        /// </summary>
        /// <param name="vArg0"></param>
        private void ValidateFrameMultiplierInput(string vArg0)
        {
            
            int.TryParse(vArg0, out mFrameMultiSkippingValue);
            if (mFrameMultiSkippingValue > 0 & mFrameMultiSkippingValue <= 10)
            {
                return;
            }
            if (mMultiLabelError != null)
            {
                StopCoroutine(mMultiLabelError);
            }
            var vMsg = "";
            if (mFrameMultiSkippingValue <= 0)
            {
                SetInputFieldValue(FrameSkippingInputMulitiplier, out mFrameMultiSkippingValue, 1);
                vMsg = "VALUE MUST BE GREATER OR EQUAL TO 1";
            }
            else if (mFrameMultiSkippingValue > 10)
            {
                vMsg = "VALUE MUST BE LESS OR EQUAL TO 10";
                SetInputFieldValue(FrameSkippingInputMulitiplier, out mFrameMultiSkippingValue, 10);

            }
            mMultiLabelError = AnimationHelpers.FadeTextBoxWithMessage(vMsg, FrameSkippingMulitplierLabel);
            StartCoroutine(mMultiLabelError);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
        private void ValidateFrameSkippingInput(string vArg0)
        {
             
            int.TryParse(vArg0, out mFrameSkippingValue);
            if (mFrameSkippingValue > 0 & mFrameSkippingValue <= 50)
            {
                return;
            }
            if (mFrameLabelError != null)
            {
                StopCoroutine(mFrameLabelError);
            }
            var vMsg = "";
            if (mFrameSkippingValue <= 0)
            {
                vMsg = "VALUE MUST BE GREATER OR EQUAL TO 1";
                SetInputFieldValue(FrameSkippingInput, out mFrameSkippingValue, 1);

            }
            else if (mFrameSkippingValue > 50)
            {
                vMsg = "VALUE MUST BE LESS OR EQUAL TO 50";
                SetInputFieldValue(FrameSkippingInput, out mFrameSkippingValue, 50);
            }
            mFrameLabelError = AnimationHelpers.FadeTextBoxWithMessage(vMsg, FrameSkippingLabel);
            StartCoroutine(mFrameLabelError);
        }

        /// <summary>
        /// Sets the value of the input field to be the same as the vReferenceVal
        /// </summary>
        /// <param name="vInputField"></param>
        /// <param name="vReferenceVal"></param>
        /// <param name="vValueForRef"></param>
        private void SetInputFieldValue(InputField vInputField, out int vReferenceVal, int vValueForRef)
        {
            vInputField.text = "" + vValueForRef;
            vReferenceVal = vValueForRef;
        }
        private void RegisterSettingsPanel(RecordingPlayerView vView)
        {
            FrameSkippingInput.text = vView.PbControlPanel.PlaybackSettings.FrameSkip + "";
            FrameSkippingInputMulitiplier.text = vView.PbControlPanel.PlaybackSettings.FrameSkipMultiplier + "";
        } 
        /// <summary>
          /// Apply settings 
          /// </summary>
        private void ApplySettings()
        {
            RecordingPlayerView.PbControlPanel.PlaybackSettings.FrameSkip = mFrameSkippingValue;
            RecordingPlayerView.PbControlPanel.PlaybackSettings.FrameSkipMultiplier = mFrameMultiSkippingValue;

        }
    }
}