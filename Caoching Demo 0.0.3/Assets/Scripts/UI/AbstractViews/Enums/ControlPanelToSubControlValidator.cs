/** 
* @file ControlPanelToSubControlValidator.cs
* @brief Contains the ControlPanelToSubControlValidator  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;

namespace Assets.Scripts.UI.AbstractViews.Enums
{
    /// <summary>
    /// Validates the correct mapping Control panels to SubControls
    ///Example: SuitConnectionSubControls should not be panelled by a DebugControlPanel
    /// </summary>
    public static class ControlPanelToSubControlValidator
    {
        private static Dictionary<ControlPanelType, List<SubControlType>> sValidControlToSubControls = new Dictionary<ControlPanelType, List<SubControlType>>();
        private static bool sIsInitialized;

        /// <summary>
        /// Initalize rules
        /// </summary>
        private static void Init()
        {
            List<SubControlType> vRecSubCtrls = new List<SubControlType>();
            List<SubControlType> vAvatarSubCtrl = new List<SubControlType>();
            List<SubControlType> vDebugSubCtrl = new List<SubControlType>();
            List<SubControlType> vSuitSubControls = new List<SubControlType>();
            List<SubControlType> vTagsSubControls = new List<SubControlType>();
            List<SubControlType> vCommentsSubControls = new List<SubControlType>();
            List<SubControlType> vCameraSubControls = new List<SubControlType>();
            List<SubControlType> vDemoKitSubControls = new List<SubControlType>();


            sValidControlToSubControls.Add(ControlPanelType.RecordingPlaybackControlPanel, vRecSubCtrls);
            sValidControlToSubControls.Add(ControlPanelType.RenderedBodyControlPanel, vAvatarSubCtrl);
            sValidControlToSubControls.Add(ControlPanelType.DebugControlPanel, vDebugSubCtrl);
            sValidControlToSubControls.Add(ControlPanelType.SuitCommunicationControlPanel, vSuitSubControls);
            sValidControlToSubControls.Add(ControlPanelType.TagControlPanel, vTagsSubControls);
            sValidControlToSubControls.Add(ControlPanelType.CommentControlPanel, vCommentsSubControls);
            sValidControlToSubControls.Add(ControlPanelType.CameraControlPanel, vCameraSubControls);
            sValidControlToSubControls.Add(ControlPanelType.DemoKit, vDemoKitSubControls);

            //register recordings subcontrols
            vRecSubCtrls.Add(SubControlType.PlaybackSliderSubcontrol);
            vRecSubCtrls.Add(SubControlType.RecordingSpeedModifierSubcontrol);
            vRecSubCtrls.Add(SubControlType.RecordingSelectionSubControl);
            vRecSubCtrls.Add(SubControlType.RecordingLoadSingleSubControl);

            //register avatar subcontrols
            vAvatarSubCtrl.Add(SubControlType.ResetAvatarSubControl);

            //register suit  subcontrols
            vSuitSubControls.Add(SubControlType.SuitsCommunicationSubControl);
            vSuitSubControls.Add(SubControlType.SuitsSelectionSubControl);
            vSuitSubControls.Add(SubControlType.SuitConnectionSubControl);
            vSuitSubControls.Add(SubControlType.SuitModificationSubControl);
            vSuitSubControls.Add(SubControlType.RenderedBodyModifierSubControl);
            vSuitSubControls.Add(SubControlType.RenderedBodySelectionSubControl);

            //register tag subcontrols
            vTagsSubControls.Add(SubControlType.TagWidgetSubControl);
            vTagsSubControls.Add(SubControlType.TagModificationSubControl);
            vTagsSubControls.Add(SubControlType.TagTextBoxSubControl);
            vTagsSubControls.Add(SubControlType.AddTagSubControl);

            //register comments subcontrols
            vCommentsSubControls.Add(SubControlType.CommentTextboxSubControl);
            vCommentsSubControls.Add(SubControlType.CommentWidgetSubControl);
            vCommentsSubControls.Add(SubControlType.AddCommentSubControl);
            vCommentsSubControls.Add(SubControlType.ModifyCommentSubControl);

            //register camera subcontrols
            vCameraSubControls.Add(SubControlType.CameraOrbitSubControl);
            vCameraSubControls.Add(SubControlType.CameraZoomSubControl);

            //Demo kit subcontrols
            vDemoKitSubControls.Add(SubControlType.PlayerLoopback);
        }

        /// <summary>
        /// Validates a control panel to subcontrols
        /// </summary>
        /// <param name="vCtrlPanelType"></param>
        /// <param name="vSubCtrlType"></param>
        /// <returns>the resulting mapping is valid or not</returns>
        public static bool Validate(ControlPanelType vCtrlPanelType, SubControlType vSubCtrlType)
        {
            if (!sIsInitialized)
            {
                sIsInitialized = true;
                Init();
            }
            bool vResult = false;

            List<SubControlType> vTypes = sValidControlToSubControls[vCtrlPanelType];
            vResult = vTypes.Contains(vSubCtrlType);

            return vResult;
        }

        /// <summary>
        ///  Validates a control panel to subcontrols
        /// </summary>
        /// <param name="vControlPanel">The Control panel to check</param>
        /// <param name="vSubControl">The subcontrol to check</param>
        /// <returns>the resulting mapping is valid or not</returns>
        public static bool Validate(AbstractControlPanel vControlPanel, AbstractSubControl vSubControl)
        {
            if (!sIsInitialized)
            {
                sIsInitialized = true;
                Init();
            }


            return Validate(vControlPanel.PanelType, vSubControl.SubControlType);
        }
    }

    public enum ControlPanelType
    {
        DebugControlPanel,
        CameraControlPanel,
        FeedbackControlPanel,
        RecordingPlaybackControlPanel,
        TagControlPanel,
        RenderedBodyControlPanel,
        SuitCommunicationControlPanel,
        CommentControlPanel,
        AnalysisControlPanel,
        MiscClientControlPanel,
        SettingControlPanel,
        DebugRecordingPanel,
        LoadingPanel,
        DemoKit,
        BrainpackControlPanel,
        LiveBPFeedView
    }

    public enum SubControlType
    {
        PlaybackSubControl,
        RecordingPlayPause,
        RecordingForwardSubControl,
        RecordingRewindSubControl,
        RecordingProgressSubControl,
        RecordingPlaySpeedModSubControl,
        RecordingLoadSingleSubControl,
        PlayerLoopback,
        TagWidgetSubControl,
        TagModificationSubControl,
        TagTextBoxSubControl,
        CommentTextboxSubControl,
        CommentWidgetSubControl,
        RecordingSpeedModifierSubcontrol,
        PlaybackSliderSubcontrol,
        RecordingSelectionSubControl,
        SuitsCommunicationSubControl,
        SuitsSelectionSubControl,
        SuitConnectionSubControl,
        SuitModificationSubControl,
        SuitStateModifierSubControl,
        SuitsCalibrationSubControl,
        RenderedBodyModifierSubControl,
        RenderedBodySelectionSubControl,
        ResetAvatarSubControl,
        AddTagSubControl,
        AddCommentSubControl,
        ModifyCommentSubControl,
        CameraOrbitSubControl,
        RecordingPlaybackSpeedDisplay,
        RightClickSubControl,
        LoginControl,
        CameraZoomSubControl
    }
}
