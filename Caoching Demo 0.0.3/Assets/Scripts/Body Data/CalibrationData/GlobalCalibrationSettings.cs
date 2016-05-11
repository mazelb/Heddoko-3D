
/** 
* @file GlobalCalibrationSettings.cs
* @brief Contains the GlobalCalibrationSettings class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.CalibrationData
{
    /// <summary>
    /// Global Calibration settings 
    /// </summary>
    public static class GlobalCalibrationSettings
    {
        private static bool sIsInitialized;
        private static CalibrationType sType;
        /// <summary>
        /// Transition period between the number of calibrations
        /// </summary>
        private static float sTransitionPeriod = 3f;

        /// <summary>
        /// Calibration timing
        /// </summary>
        private static float sCalibrationTimer = 3f;

        


        private static List<CalibrationType> sCalibrationTypes = new List<CalibrationType>();
        private static Dictionary<CalibrationType, float> sCalibrationTimes = new Dictionary<CalibrationType, float>();

        /// <summary>
        /// Transition period between the number of calibrations
        /// </summary>
        public static float TransitionTimer
        {
            get
            {
                CheckIfInitialized();
                return sTransitionPeriod;
            }
        }

        /// <summary>
        /// Calibration timing
        /// </summary>
        public static float CalibrationTimer
        {
            get
            {
                CheckIfInitialized();
                return sCalibrationTimer;
            }
        }

        public static List<CalibrationType> CalibrationTypes
        {
            get
            {
                CheckIfInitialized();
                return sCalibrationTypes;
            }
        }

        public static CalibrationType FinalPose
        {
            get
            {
                if (!sIsInitialized)
                {
                    Init();
                }
                return sType;
            }
            private set { sType = value; }
        }

        public static Dictionary<CalibrationType, float> CalibrationTimestamps
        {
            get
            {
                CheckIfInitialized();
                return sCalibrationTimes;
            }
        }

        /// <summary>
        /// Checks if the Gloabal Calibration settings have been initialized
        /// </summary>
        private static void CheckIfInitialized()
        {
            if (!sIsInitialized)
            {
                sIsInitialized = true;
                Init();
            }
        }
        /// <summary>
        /// initializes global settings
        /// </summary>
        private static void Init()
        {
            sIsInitialized = true;
            sCalibrationTypes = new List<CalibrationType>();
            //set calibration times according to their current order
            float vMultipler = 1;
            sCalibrationTimes.Add(CalibrationType.NullToTPose, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.Tpose, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.TPoseToArmsForward, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.ArmsForward, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.ArmsForwardToArmsDown, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.ArmsDown, CalibrationTimer * vMultipler);
            FinalPose = CalibrationType.ArmsDown;
        }
    }



    /// <summary>
    /// Calibration types
    /// </summary>
    public enum CalibrationType
    {
        Invalid = -1,
        NullToTPose = 0,
        Tpose=1,
        TPoseToArmsForward=2,
        ArmsForward = 3,
        ArmsForwardToArmsDown = 4,
        ArmsDown = 5,
        Count = 6
    }
}
