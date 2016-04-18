

/* @file RecordingForwardSubControl.cs
* @brief Contains the RecordingForwardSubControl class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    /// <summary>
    /// controls a recording to fast forward, step forward
    /// </summary>
    public class RecordingForwardSubControl: AbstractSubControl
    {
        public Button ForwardButton;
        public TextUnicode UnicodeFowardText; 
        public string FontawesomeStepForwardText = "\\uf051";
        public string FontawesomeFastForwardText = "\\uf04e";
        private SubControlType mType = SubControlType.RecordingForwardSubControl;
        public PlaybackControlPanel ParentPanel;

        private bool mIsPaused;
        public bool IsPaused
        {
            get { return mIsPaused; }
            set
            {
                if (value)
                {
                    UnicodeFowardText.text = FontawesomeStepForwardText;
                }
                else
                {
                    UnicodeFowardText.text = FontawesomeFastForwardText;
                }
                mIsPaused = value;
            }
        }

        public void Init(PlaybackControlPanel mParentPanel)
        {
            ParentPanel = mParentPanel;
            ForwardButton.onClick.AddListener(ParentPanel.FastForward);
        }
        public override SubControlType SubControlType
        {
            get { return mType; }
        }

        public override void Disable()
        {
            Interactable = false;
        }

        public override void Enable()
        {
            Interactable = true;
        }

        /// <summary>
        /// Sets the interaction of the Button
        /// </summary>
        public bool Interactable
        {
            get { return ForwardButton.interactable; }
            set { ForwardButton.interactable = value; }
        }
    }
}