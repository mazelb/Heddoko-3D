/** 
* @file RecordingPlaySpeedModSubControl.cs
* @brief Contains the RecordingPlaySpeedModSubControl  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    /// <summary>
    /// Controls the playback speed of a recording
    /// </summary>
    public class RecordingPlaySpeedModSubControl : AbstractSubControl
    {
        public Slider PlaybackSpeedSlider;
        public PlaybackControlPanel ParentPanel;
        private static SubControlType sType = SubControlType.RecordingPlaySpeedModSubControl;
        public override SubControlType SubControlType
        {
            get { return sType; }
        }

        public override void Disable()
        {
            IsInteractable = false;
        }

        public void Init(PlaybackControlPanel vParentPanel)
        {
            ParentPanel = vParentPanel;

            //this slider should never reach 0.1f
            PlaybackSpeedSlider.minValue = 0.1f;
            PlaybackSpeedSlider.onValueChanged.AddListener(ParentPanel.ChangeSloMoSpeed);
        }


        public override void Enable()
        {
            IsInteractable = true;
        }

        public bool IsInteractable
        {
            get { return PlaybackSpeedSlider.interactable; }
            set { PlaybackSpeedSlider.interactable = value; }
        }

        public bool IsPaused { get; set; }


        /// <summary>
        /// updates the slider's valude
        /// </summary>
        /// <param name="vPlaybackSpeed"></param>
        public void UpdateCurrentPlaybackSpeed(float vPlaybackSpeed)
        {
            PlaybackSpeedSlider.value = vPlaybackSpeed;
        }
    }
}