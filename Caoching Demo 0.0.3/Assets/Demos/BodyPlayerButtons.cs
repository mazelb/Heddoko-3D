
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
                    TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.IsTrackingHeight);
                    HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateForward);
                    HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateUp);
                    InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.IsUsingInterpolation);

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
                    BodySegment.IsTrackingHeight = !BodySegment.IsTrackingHeight;
                    TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.IsTrackingHeight);
                }
            }
                );
            HipsEstForward.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsHipsEstimateForward = !BodySegment.IsHipsEstimateForward;
                    HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateForward);
                }
            }
               );

            HipsEstUp.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsHipsEstimateUp = !BodySegment.IsHipsEstimateUp;
                    HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateUp);
                }
            }
               );
            Interpolation.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsUsingInterpolation = !BodySegment.IsUsingInterpolation;
                    InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.IsUsingInterpolation);
                }
            }
              );
            Fusion.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsFusingSubSegments = !BodySegment.IsFusingSubSegments;
                    FusionTxt.text = ReturnOnOffFromBool(BodySegment.IsFusingSubSegments);
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
                BodySegment.IsTrackingHeight = !BodySegment.IsTrackingHeight;
                TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.IsTrackingHeight);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipsEstimateForward))
            {
                BodySegment.IsHipsEstimateForward = !BodySegment.IsHipsEstimateForward;
                HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateForward);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipsEstimateUp))
            {
                BodySegment.IsHipsEstimateUp = !BodySegment.IsHipsEstimateUp;
                HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateUp);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingInterpolation))
            {
                BodySegment.IsUsingInterpolation = !BodySegment.IsUsingInterpolation;
                InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.IsUsingInterpolation);
            }
 
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsFusingSubSegments)) 
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingFusionForBody))
            {
                BodySegment.IsFusingSubSegments = !BodySegment.IsFusingSubSegments;
                FusionTxt.text = ReturnOnOffFromBool(BodySegment.IsFusingSubSegments);
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
