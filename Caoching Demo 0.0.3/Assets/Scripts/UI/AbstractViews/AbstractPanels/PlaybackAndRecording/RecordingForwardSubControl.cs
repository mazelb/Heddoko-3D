

/* @file RecordingForwardSubControl.cs
* @brief Contains the RecordingForwardSubControl class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    /// <summary>
    /// controls a recording to fast forward, step forward
    /// </summary>
    public class RecordingForwardSubControl : AbstractSubControl
    {
        public Button ForwardButton;
        public Sprite FastForward;
        public Sprite StepForward;
        private SubControlType mType = SubControlType.RecordingForwardSubControl;
        public PlaybackControlPanel ParentPanel;

        private bool mIsPaused;
        public bool IsPaused
        {
            get { return mIsPaused; }
            set
            {
                mIsPaused = value;
            }
        }

        public void Init(PlaybackControlPanel mParentPanel)
        {
            ParentPanel = mParentPanel;
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

        /// <summary>
        /// Request resouces to be set up for the subcontrol
        /// </summary>
        public void RequestResources()
        {
            InputHandler.RegisterKeyboardAction(KeyCode.RightArrow, StepForwardAction);
        }


        /// <summary>
        /// Step forward
        /// </summary>
        private void StepForwardAction()
        {
            ParentPanel.Pause();
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                 ParentPanel.FastForward(3);
            }
            else
            {
                ParentPanel.FastForward(1);
            }
        }

        /// <summary>
        /// Releases previously held resources
        /// </summary>
        public void ReleaseResources()
        {
            InputHandler.RemoveKeybinding(KeyCode.RightArrow, StepForwardAction);
        }
    }
}