// /**
// * @file BodySegmentCalibration.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System;
using HeddokoLib.body_pipeline;
using UnityEngine;

namespace Assets.Scripts.Body_Data.CalibrationData
{
    /// <summary>
    /// Class to calibrate a sensor to a body segment
    /// </summary>
    public class BodySegmentCalibration
    {
        /// <summary>
        /// Getter/setter of the main BodySegment component of the object
        /// </summary>
        private BodySegment mSegment { get; set; }
        /// <summary>
        /// Getter/setter of the quaterion  that will calibration the BodySegment
        /// </summary>
        private Quaternion mOffsetRotation { get; set; }

        /// <summary>
        ///The primary calibration action. Needs an imuframe  from which to fetch spatial data from, and a callback action to update the offset rotation
        /// </summary>
        internal Action<ImuFrame, Action<Quaternion>> CalibrationFunction { get; set; }
        /// <summary>
        /// Instantiates a BodySegmentCalibration instance with respect to a BodySegment
        /// </summary>
        /// <param name="vSegment">the bodysegment whose sensor will be calibrated</param>

        public BodySegmentCalibration(BodySegment vSegment)
        {
            mSegment = vSegment;
        }

        /// <summary>
        /// Initiate a calibration routine 
        /// </summary>
        /// <param name="vFrame"></param>
        public void InitiateCalibrationRoutine(ImuFrame vFrame)
        {
            if (CalibrationFunction != null)
            {
                CalibrationFunction(vFrame, UpdateOffsetRotation);
            }
            else
            {
                throw new NullReferenceException("The calibration function has not been initialized");
            }
        }

        /// <summary>
        /// Updates the offset rotation. 
        /// </summary>
        /// <param name="vQuaterion">the new offset rotation</param>
        private void UpdateOffsetRotation(Quaternion vQuaterion)
        {
            mOffsetRotation = vQuaterion;
        }
    }
}