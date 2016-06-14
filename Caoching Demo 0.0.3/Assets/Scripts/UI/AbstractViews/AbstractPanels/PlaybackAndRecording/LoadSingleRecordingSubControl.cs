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
using Assets.Scripts.UI.AbstractViews.Permissions;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
   
    /// <summary>
    /// A subcontrol that brings up a panel to load recordings
    /// </summary>
    [UserRolePermission()]

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
        internal void SelectedRecording()
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

        /// <summary>
        /// Changes the Load recording button to the one passed in
        /// </summary>
        /// <param name="vNewRecordingButton"></param>
        public void SetNewButtonControl(Button vNewRecordingButton)
        {
            if (LoadButton != null)
            {
                //disable before switching
                LoadButton.enabled = false;
                LoadButton.onClick.RemoveAllListeners();
                //find any children with graphical components and disable them.
                MaskableGraphic[] vChildren = LoadButton.GetComponentsInChildren<MaskableGraphic>();
                foreach (var vMaskableChild in vChildren)
                {
                    vMaskableChild.enabled = false;
                }

            }
            LoadButton = vNewRecordingButton;
            //set the new event handler
            LoadButton.onClick.AddListener(SelectedRecording);
        }
    }
}