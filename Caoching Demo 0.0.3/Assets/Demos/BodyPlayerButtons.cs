
/** 
* @file BodyPlayerButtons.cs
* @brief Contains the BodyPlayerButtons class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    /// <summary>
    /// 
    /// </summary>
    public class BodyPlayerButtons : MonoBehaviour
    {
        public BodyPlayer BodyPlayer;

        public Button ResetMetrics;

        public Button TrackingHeight;
        public Text TrackHeightTxt;

        public Button TrackingHips;
        public Text TrackHipsTxt;

        public Button ArmAdjustment;
        public Text ArmAdjustmentTxt;

        public Button HipsEstForward;
        public Text HipsEstForwardTxt;

        public Button HipsEstUp;
        public Text HipsEstUpTxt;

        public Button Interpolation;
        public Text InterpolationTxt;

        public Button Fusion;
        public Text FusionTxt;

        public Button ProjectionXZ;
        public Text ProjectionXZTxt;
        public Button ProjectionXY;
        public Text ProjectionXYTxt;
        public Button ProjectionYZ;
        public Text ProjectionYZTxt;
        

        public Text HideText;

        private bool mBodySet;
        public Animator Animator;
        [SerializeField] private bool mIsHidden;

        void Update()
        {
            if (BodyPlayer.CurrentBodyInPlay != null)
            {

                if (!mBodySet)
                {
                    TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsTrackingHeight);
                    TrackHipsTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsTrackingHips);
                    ArmAdjustmentTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsAdjustingSegmentAxis);
                    HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsHipsEstimateForward);
                    HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsHipsEstimateUp);
                    InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsUsingInterpolation);
                    FusionTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsFusingSubSegments);

                    ProjectionXZTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingXZ);
                    ProjectionXYTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingXY);
                    ProjectionYZTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingYZ);

                    mBodySet = true;
                }
                InputHandler();
            }

            if (Input.GetKeyDown(HeddokoDebugKeyMappings.HideSegmentFlagPanel))
            {
                mIsHidden = !mIsHidden;
                Animator.SetBool("mIsHidden", mIsHidden);
            }
        }

        void Awake()
        {
            HideText.text = "Press " + HeddokoDebugKeyMappings.HideSegmentFlagPanel + "  to hide/show ";
            ResetMetrics.onClick.AddListener(() =>
            {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodyPlayer.CurrentBodyInPlay.ResetBodyMetrics();
                }
            }
                );
            TrackingHeight.onClick.AddListener(() =>
            {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsTrackingHeight = !BodySegment.Flags.IsTrackingHeight;
                    TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsTrackingHeight);
                }
            }
                );
            TrackingHips.onClick.AddListener(() =>
            {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsTrackingHips = !BodySegment.Flags.IsTrackingHips;
                    TrackHipsTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsTrackingHips);
                }
            }
                );
            ArmAdjustment.onClick.AddListener( ()=> { 
                 if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsAdjustingSegmentAxis = !BodySegment.Flags.IsAdjustingSegmentAxis;
                    ArmAdjustmentTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsAdjustingSegmentAxis);
                }
            }
                );
            HipsEstForward.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsHipsEstimateForward = !BodySegment.Flags.IsHipsEstimateForward;
                    HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsHipsEstimateForward);
                }
            }
               );

            HipsEstUp.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsHipsEstimateUp = !BodySegment.Flags.IsHipsEstimateUp;
                    HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsHipsEstimateUp);
                }
            }
               );
            Interpolation.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsUsingInterpolation = !BodySegment.Flags.IsUsingInterpolation;
                    InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsUsingInterpolation);
                }
            }
              );
            Fusion.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsFusingSubSegments = !BodySegment.Flags.IsFusingSubSegments;
                    FusionTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsFusingSubSegments);
                }
            }
              );

            ProjectionXZ.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsProjectingXZ = !BodySegment.Flags.IsProjectingXZ;
                    ProjectionXZTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingXZ);
                }
            }
              );
            ProjectionXY.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsProjectingXY = !BodySegment.Flags.IsProjectingXY;
                    ProjectionXYTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingXY);
                }
            }
              );
            ProjectionYZ.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.Flags.IsProjectingYZ = !BodySegment.Flags.IsProjectingYZ;
                    ProjectionYZTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingYZ);
                }
            }
              );
        }
        /// <summary>
        /// Handles input from keyboard
        /// </summary>
        void InputHandler()
        {
          
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsTrackingHeight))
            {
                BodySegment.Flags.IsTrackingHeight = !BodySegment.Flags.IsTrackingHeight;
                TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsTrackingHeight);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsTrackingHips))
            {
                BodySegment.Flags.IsTrackingHips = !BodySegment.Flags.IsTrackingHips;
                TrackHipsTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsTrackingHips);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsAdjustingSegmentAxis))
            {
                BodySegment.Flags.IsAdjustingSegmentAxis = !BodySegment.Flags.IsAdjustingSegmentAxis;
                ArmAdjustmentTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsAdjustingSegmentAxis);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipsEstimateForward))
            {
                BodySegment.Flags.IsHipsEstimateForward = !BodySegment.Flags.IsHipsEstimateForward;
                HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsHipsEstimateForward);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipsEstimateUp))
            {
                BodySegment.Flags.IsHipsEstimateUp = !BodySegment.Flags.IsHipsEstimateUp;
                HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsHipsEstimateUp);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingInterpolation))
            {
                BodySegment.Flags.IsUsingInterpolation = !BodySegment.Flags.IsUsingInterpolation;
                InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsUsingInterpolation);
            }
 
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsFusingSubSegments)) 
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingFusionForBody))
            {
                BodySegment.Flags.IsFusingSubSegments = !BodySegment.Flags.IsFusingSubSegments;
                FusionTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsFusingSubSegments);
            }

            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsProjectingXZ))
            {
                BodySegment.Flags.IsProjectingXZ = !BodySegment.Flags.IsProjectingXZ;
                ProjectionXZTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingXZ);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsProjectingXY))
            {
                BodySegment.Flags.IsProjectingXY = !BodySegment.Flags.IsProjectingXY;
                ProjectionXYTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingXY);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsProjectingYZ))
            {
                BodySegment.Flags.IsProjectingYZ = !BodySegment.Flags.IsProjectingYZ;
                ProjectionYZTxt.text = ReturnOnOffFromBool(BodySegment.Flags.IsProjectingYZ);
            }
        }

        /// <summary>
        ///from the bool passed in, returns either On or Off
        /// </summary>
        /// <returns></returns>
        public static string ReturnOnOffFromBool(bool vValue)
        {
            return vValue ? "ON" : "OFF";
        }


    }
}
