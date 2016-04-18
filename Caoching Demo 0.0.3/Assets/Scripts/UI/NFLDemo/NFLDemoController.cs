/** 
* @file NFLDemoController.cs
* @brief Contains the NFLDemoController class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System; 
using Assets.Scripts.Cameras;
using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// Controller used for the NFL demo tech crunch
    /// </summary>
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class NFLDemoController : MonoBehaviour
    {
        public CameraController CameraController;
        public MoveCameraToPositon MoveCameraToPositon;

        [SerializeField]
        public GroupFadeEffect GroupFadeEffect;
        //the state of the cam look at will be saved once the event is triggered
        private bool mMoveCamState;
        public ActivitiesContextController ActivitiesContextController;
        public CameraMotionBlur CamBlur;
        public CameraLookAt CamLookAt;
        public PlayerStreamManager PlayerStreamManager;
        [SerializeField]
        public NFLCameraController NflCamController;
        [SerializeField]
        private bool mMainEventStarted; 

 

        public ArcAngleFill ArcAngleFill;
        public AnalysisContentPanel AnalysisContentPanel;

 
        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            //is the correct context
            bool vCorrectContext = ActivitiesContextController.CurrentState ==
                                   ActivitiesContextController.ActivitiesContextViewState.LearnByRecording
                                   ||
                                   ActivitiesContextController.CurrentState ==
                                   ActivitiesContextController.ActivitiesContextViewState.Train;
            if (!mMainEventStarted && !ActivitiesContextController.UsingSquats && vCorrectContext)
            {
                //start the event
                if (Input.GetKeyDown(HeddokoDebugKeyMappings.Pause))
                {
                    mMainEventStarted = true;

                    PlayerStreamManager.ChangePauseState();

                    NflCamController.Reset();
                    mMoveCamState = MoveCameraToPositon.isActiveAndEnabled;
                    CameraController.enabled = false;
                    MoveCameraToPositon.enabled = false;
                    CamLookAt.enabled = true;
                    CamBlur.enabled = true;
                    NflCamController.enabled = true;
                    GroupFadeEffect.Hide();
                    NflCamController.Curve.SetDirty();

                    //set the look at target
                    CameraMovementPointSetting vPointParameters = NflCamController.GetPointAt(0);
                    CamLookAt.TargetPos = vPointParameters.LookAtTarget.position;

                    NflCamController.MoveTowardsStartPos();
                    ArcAngleFill.Show();
                    AnalysisContentPanel.Show();
                    ArcAngleFill.SetParametersFromPoint(vPointParameters);
                    AnalysisContentPanel.UpdateFeedbackText(vPointParameters);
                }
            }
            else if (mMainEventStarted)
            {
                //stop the event
                if (Input.GetKeyDown(HeddokoDebugKeyMappings.Pause))
                {
                    PlayerStreamManager.ClearBuffer();
                    Reset();
                }
                else
                {
                    //get the current index and next index
                    int vCurrentCamIdx = NflCamController.CurrCamIndex;

                    //Is the group effect still animating?
                    if (GroupFadeEffect.FinishedAnimating)
                    {
                        //Is the camera still moving?
                        if (NflCamController.FinishedMovingCam)
                        {
                            //Check if the AnalysisView is in display
                            AnalysisView vCurrAnalysisView = NflCamController.GetAnalysisView(vCurrentCamIdx);
                            if (vCurrAnalysisView != null && vCurrAnalysisView.InView)
                            {
                                //Check if the user is requesting to go to next position
                                if (Input.GetKeyDown(HeddokoDebugKeyMappings.MoveNext))
                                {
                                    vCurrAnalysisView.Hide();
                                    NflCamController.MoveToNextPos();
                                    CameraMovementPointSetting vPointParameters = NflCamController.GetPointAt(NflCamController.NextCamIndex);
                                    ArcAngleFill.SetParametersFromPoint(vPointParameters);
                                    AnalysisContentPanel.UpdateFeedbackText(vPointParameters);
                                }
                            }

                            // there is no analysis view attached to the current index
                            else if (vCurrAnalysisView == null)
                            {
                                if (Input.GetKeyDown(HeddokoDebugKeyMappings.MoveNext))
                                {
                                    NflCamController.MoveToNextPos();
                                    CameraMovementPointSetting vPointParameters = NflCamController.GetPointAt(NflCamController.NextCamIndex);
                                    ArcAngleFill.SetParametersFromPoint(vPointParameters);
                                    AnalysisContentPanel.UpdateFeedbackText(vPointParameters);
                                }

                            }
                            if (vCurrAnalysisView != null && !vCurrAnalysisView.InView)
                            {
                                vCurrAnalysisView.Show();
                            }
                        }

                        /*  //check if next pos is 0, go back into training view
                          if (NFLCamController.NextCamIndex == 0)
                          {
                              Reset();
                          }*/
                    }
                }
            }

            if (Input.GetKeyDown(HeddokoDebugKeyMappings.SwitchToRecordingFromLive) &&
                (ActivitiesContextController.CurrentState ==
                                   ActivitiesContextController.ActivitiesContextViewState.Train)

                )
            {
                BackButtonPressed();
                ActivitiesContextController.NonSquatHookFunction();
                PlayerStreamManager.ResumeFromPauseState();

            }

            if (Input.GetKeyDown(HeddokoDebugKeyMappings.SkipToLiveViewFromRecordingView) &&
                (ActivitiesContextController.CurrentState ==
                                   ActivitiesContextController.ActivitiesContextViewState.LearnByRecording)
                )
            {
                BackButtonPressed();
                ActivitiesContextController.SwitchtoTrainingViewState();
                PlayerStreamManager.ResumeFromPauseState();
            }

            /*    if (Input.GetKeyDown(HeddokoDebugKeyMappings.SkipToLiveViewFromRecordingView))
                {
                    Reset();
                    ActivitiesContextController.SwitchtoTrainingViewState();
                    PlayerStreamManager.ResumeFromPauseState();
                }
                if (Input.GetKeyDown(HeddokoDebugKeyMappings.SwitchToRecordingFromLive))
                {
                    Reset();
                    ActivitiesContextController.NonSquatHookFunction();
                    PlayerStreamManager.ResumeFromPauseState();
                }*/
        }

        /// <summary>
        /// Resets the view back to training
        /// </summary>
        public void Reset()
        {
            mMainEventStarted = false;

            /*#if  UNITY_EDITOR
                        vIsDebugBuild = true;

            #elif DEVELOPMENT_BUILD
                        vIsDebugBuild = true;
            #endif*/

            PlayerStreamManager.ChangePauseState();
            PlayerStreamManager.ClearBuffer();
            CamLookAt.enabled = false;
            MoveCameraToPositon.gameObject.SetActive(mMoveCamState);
            CameraController.enabled = true;
            MoveCameraToPositon.enabled = true;
            CamBlur.enabled = false;
            NflCamController.Reset();
            NflCamController.enabled = false;
            GroupFadeEffect.Show();
            CameraMovementPointSetting vPointParameters = NflCamController.GetPointAt(0);

            CamLookAt.TargetPos = vPointParameters.LookAtTarget.position;
            ArcAngleFill.SetParametersFromPoint(vPointParameters);
            AnalysisContentPanel.Hide();
            ArcAngleFill.Hide();

        }

        /// <summary>
        /// In the event that the back button was pressed, reset the camera back to its original position
        /// </summary>
        public void BackButtonPressed()
        {
            AnalysisContentPanel.Hide();
            GroupFadeEffect.ForceShow();
            mMainEventStarted = false;
            PlayerStreamManager.ResumeFromPauseState();
            CamLookAt.enabled = false;
            MoveCameraToPositon.gameObject.SetActive(mMoveCamState);
            CameraController.enabled = true;
            MoveCameraToPositon.enabled = true;
            CamBlur.enabled = false;
            NflCamController.Reset();
            NflCamController.enabled = false;

            ArcAngleFill.Hide();
        }
 


    }
}
