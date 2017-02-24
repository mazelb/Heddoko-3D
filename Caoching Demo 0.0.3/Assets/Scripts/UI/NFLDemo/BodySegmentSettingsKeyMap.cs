
/** 
* @file BodySegmentSettingsKeyMap.cs
* @brief BodySegmentSettingsKeyMap class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Data;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// Sets the TradingHeight, IsHipsEstimateForward, IsUsingInterpolation flags, as well as increments and decrements
    /// interpolation speed
    /// </summary>
    public class BodySegmentSettingsKeyMap : MonoBehaviour
    {
        public PlayerStreamManager PlayerStreamManager;
        private Body mBody;

        void Awake()
        {
            PlayerStreamManager.BodyChangedEvent += SetBody;
        }

        void OnDisable()
        {
            PlayerStreamManager.BodyChangedEvent -= SetBody;
        }

        /// <summary>
        /// Setts the new body
        /// </summary>
        /// <param name="vNewBody"></param>
        private void SetBody(Body vNewBody)
        {
            mBody = vNewBody;
        }

        void Update()
        {
            if (mBody != null)
            {
                InputHandler();
            }
        }


        /// <summary>
        /// Handles input key events
        /// </summary>
        void InputHandler()
        {
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.ResetMetrics))
            {
                mBody.ResetBodyMetrics();
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.DecBodyInterpoationSp))
            {
                ChangeInterpolationValue(-0.05f);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IncBodyInterpolationSp))
            {
                ChangeInterpolationValue(0.05f);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipsEstimateForward))
            {
                BodySegment.IsHipsEstimateForward = !BodySegment.IsHipsEstimateForward;
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipsEstimateUp))
            {
                BodySegment.IsHipsEstimateUp = !BodySegment.IsHipsEstimateUp;
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsTrackingHeight))
            {
                BodySegment.IsTrackingHeight = !BodySegment.IsTrackingHeight;
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingInterpolation) ||
                Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingInterpolationForBody))
            {
                BodySegment.IsUsingInterpolation = !BodySegment.IsUsingInterpolation;
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsFusingSubSegments) ||
                Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingFusionForBody))
 
            {
                BodySegment.IsFusingSubSegments = !BodySegment.IsFusingSubSegments;
            }
        }

        /// <summary>
        /// increments/decrements interpolation value of the current body in play. The interpolation is clamped between 0-1
        /// </summary>
        /// <param name="vIncValue"></param>
        private void ChangeInterpolationValue(float vIncValue)
        {
            BodySegment.InterpolationSpeed += vIncValue;
            Mathf.Clamp01(BodySegment.InterpolationSpeed);
        }
    }
}
