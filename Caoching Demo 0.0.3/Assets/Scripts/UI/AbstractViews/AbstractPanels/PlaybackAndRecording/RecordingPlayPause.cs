

/* @file AbstractControlPanel.cs
* @brief Contains the ActivitiesContextViewAnalyze class
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
    /// A abstract sub control that changes a recording's play/pause state
    /// </summary>
    public class RecordingPlayPause : AbstractSubControl
    {
        public Button PlayPauseButton;
        public TextUnicode UnicodePlayText; 
        public string FontawesomePlayText = "\\uf04c";
        public string FontawesomePauseText = "\\uf04b";
        public string FontawesomeStopText = "\\uf04d";
        private bool mIsPaused;
        public PlaybackControlPanel ParentPanel;

        private SubControlType mType = SubControlType.RecordingPlayPause;
        public override SubControlType SubControlType
        {
            get { return mType; }
        }

        /// <summary>
        /// Changes the text of the button
        /// </summary>
        public bool IsPaused
        {
            get { return mIsPaused; }
            set
            { 
                mIsPaused = value;
                ChangePlayPauseText();
            }
        }

        public void Init(PlaybackControlPanel mControlPanel)
        {
            ParentPanel = mControlPanel;
            if (PlayPauseButton != null)
            {
                PlayPauseButton = GetComponent<Button>();
            }
            PlayPauseButton.onClick.AddListener(ParentPanel.SetPlayState);
        }
        /// <summary>
        /// Stops the control from being used
        /// </summary>
        private void Stop()
        {
            UnicodePlayText.text = FontawesomeStopText;
        
        }

        /// <summary>
        /// Changes the text of the play pause button according to the current pause state
        /// </summary>
        private void ChangePlayPauseText()
        {
            UnicodePlayText.text = IsPaused? FontawesomePauseText : FontawesomePlayText;
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
            get { return PlayPauseButton.interactable; }
            set
            {
                PlayPauseButton.interactable = value;

                if (!value)
                {
                    Stop();
                }

                else
                {
                    ChangePlayPauseText();
                }
            }
        }
    }
}
