/** 
* @file RecordingProgressSubControl .cs
* @brief Contains the RecordingProgressSubControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    /// <summary>
    /// Controls the progress of a recording playback
    /// </summary>
    public class RecordingProgressSubControl : AbstractSubControl
    {
        public Text CurrentPlayTimeText;
        public Text TotalRecordingTimeText;
        public Slider PlaySlider;
        private static SubControlType sType = SubControlType.RecordingProgressSubControl;
        public PlaybackControlPanel ParentPanel;


        public void Init(PlaybackControlPanel vParentPanel)
        {
            ParentPanel = vParentPanel;

        }
        /// <summary>
        /// Formats Milliseconds to a string in the following format HH:MM:SS
        /// </summary>
        /// <param name="vTotalTime"></param>
       public static string FormatSecondsToTimeString(int vTotalTime)
        {
            //seperate the total time passed in into three ints. 
            float vTempTime = vTotalTime;
            int vHour = (int)(vTempTime / 3600f);
            vTempTime -= vHour * 3600f;
            int vMin = (int)(vTempTime / 60);
            vTempTime -= vMin * 60;
            int vSec = (int)vTempTime;

            //Format the time segements in case the values are less than 10
            string vToHour = vHour < 10 ? "0" + vHour + ":" : vHour + ":";
            string vToMin = vMin < 10 ? "0" + vMin + ":" : vMin + ":";
            string vToSec = vSec < 10 ? "0" + vSec   : vSec+""  ;

            return vToHour + vToMin + vToSec;
        }


        /// <summary>
        /// Updates the current time on CurrentPlayTimeText ui text
        /// </summary>
        /// <param name="vIndex"></param>
        public void UpdateCurrentTime(int vIndex)
        {
            //update the slider
            PlaySlider.value = vIndex;
            float vCurrTimeStamp = ParentPanel.GetTimeStampFromFrameIdx(vIndex);
            float vStartTime = ParentPanel.GetTimeStampFromFrameIdx(0);
            float vCurrentTime = vCurrTimeStamp - vStartTime;
            CurrentPlayTimeText.text = "" + FormatSecondsToTimeString((int)vCurrentTime);
        }

        /// <summary>
        /// get subcontroltype of current sub control
        /// </summary>
        public override SubControlType SubControlType
        {
            get { return sType; }
        }

        public override void Disable()
        {
            IsInteractable = false;
        }

        public override void Enable()
        {
            IsInteractable = true;
            int vMax = ParentPanel.PlaybackTask.RecordingCount;
            PlaySlider.maxValue = vMax;
            PlaySlider.wholeNumbers = true;
        }

        public bool IsInteractable
        {
            get { return PlaySlider.interactable; }
            set { PlaySlider.interactable = value; }
        }

        public void OnDragStart()
        {
            if (IsInteractable)
            {
                ParentPanel.ChangeState(PlaybackState.Pause);
            }
        }

        public void OnDragEnd()
        {
            if (IsInteractable)
            {
                int vPlaySliderValue = (int)PlaySlider.value;
                ParentPanel.SetPlayPositionAt(vPlaySliderValue);
                Debug.Log(vPlaySliderValue);
                ParentPanel.ChangeState(PlaybackState.Pause);
            }
        }

        /// <summary>
        /// Updates the max slider value and the total recording time text
        /// </summary>
        /// <param name="recordingCount"></param>
        /// <param name="totalRecordingTime"></param>
        public void UpdateMaxTimeAndMaxValue(int recordingCount, float totalRecordingTime)
        {
            PlaySlider.maxValue = recordingCount;
            TotalRecordingTimeText.text = " / "+FormatSecondsToTimeString((int)totalRecordingTime);

        }
    }
}