/** 
* @file LoadSingleRecordingSubControl.cs
* @brief Contains the AbstractSubControl class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Tests;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    /// <summary>
    /// A subcontrol that brings up a panel to load recordings
    /// </summary>
    public class LoadSingleRecordingSubControl : AbstractSubControl
    {
        public Button LoadButton;
        public PlaybackControlPanel ParentPanel;

        /// <summary>
        /// Initialize with the parent playback control panel
        /// </summary>
        /// <param name="mParentPanel"></param>
        public void Init(PlaybackControlPanel mParentPanel)
        {
            ParentPanel = mParentPanel;
            LoadButton.onClick.AddListener(SelectedRecording);
        }

        /// <summary>
        /// Recording selected
        /// </summary>
        private void SelectedRecording()
        {
            ParentPanel.ChangeState(PlaybackState.Pause);
            SingleRecordingSelection.Instance.OpenFileBrowseDialog(ParentPanel.NewRecordingSelected);

        }
        public override SubControlType SubControlType
        {
            get { return SubControlType.RecordingLoadSingleSubControl; }
        }

        public override void Disable()
        {

        }

        public override void Enable()
        {

        }
    }
}