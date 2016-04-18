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
{    /// <summary>
     /// controls a recording to fast rewind, step stepback
     /// </summary>
    public class RecordingRewindSubControl : AbstractSubControl
    {
        public Button RewindButton;
        public TextUnicode UnicodeRewindText;
        private string FontawesomeFastRewindText = "\\uf04a";
        private string FontawesomeStepRewindText = "\\uf048";
        private SubControlType mType = SubControlType.RecordingRewindSubControl;
        public PlaybackControlPanel ParentPanel;
        private bool mIsPaused;
        public bool IsPaused
        {
            get { return mIsPaused; }
            set
            {
                if (value)
                {
                    UnicodeRewindText.text = FontawesomeStepRewindText;
                }
                else
                {
                    UnicodeRewindText.text = FontawesomeFastRewindText;
                }
                mIsPaused = value;
            }
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
            get { return RewindButton.interactable; }
            set { RewindButton.interactable = value; }
        }

        public void Init(PlaybackControlPanel vParentPanel)
        {
            ParentPanel = vParentPanel;
            RewindButton.onClick.AddListener(() =>
            {
                ParentPanel.Rewind();
            });
        }
    }
}