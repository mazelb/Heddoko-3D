/* @file RecordingForwardSubControl.cs
* @brief Contains the RecordingForwardSubControl class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{    /// <summary>
     /// controls a recording to fast rewind, step stepback
     /// </summary>
    [UserRolePermission()]

    public class RecordingRewindSubControl : AbstractSubControl
    {
        public Button RewindButton;
        public Sprite FastRewind;
        public Sprite StepRewind;
        private SubControlType mType = SubControlType.RecordingRewindSubControl;
        public PlaybackControlPanel ParentPanel;
        private bool mIsPaused;
         public bool IsEnabled=true;
        public bool IsPaused
        {
            get { return mIsPaused; }
            set
            {
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
           
        }

        /// <summary>
        /// Request resouces to be set up for the subcontrol
        /// </summary>
        public void RequestResources()
        {
            InputHandler.RegisterKeyboardAction(KeyCode.LeftArrow, StepbackAction);
        }

        /// <summary>
        /// Step forward
        /// </summary>
        private void StepbackAction()
        {
            if (!IsEnabled)
            {
                return;
            }
            ParentPanel.Pause();
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                ParentPanel.Rewind(3);
            }
            else
            {
                ParentPanel.Rewind(1);
            }
        }

        /// <summary>
        /// Releases previously held resources
        /// </summary>
        public void ReleaseResources()
        {
            InputHandler.RemoveKeybinding(KeyCode.LeftArrow, StepbackAction);
        }
    }
}