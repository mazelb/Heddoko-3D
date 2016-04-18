namespace Assets.Scripts.Body_Data.CalibrationData
{
    /// <summary>
    /// Calibration settings and times of a body calibration
    /// </summary>
    public class BodyCalibrationSetting
    {
        private float mStartTime;
        private float mTime;
        public BodyCalibrationSetting(BodyFrame vFirstBodyFrame)
        {
            StartTime = vFirstBodyFrame.Timestamp;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public BodyCalibrationSetting( )
        {
            mStartTime = mTime = 0;
        }
        /// <summary>
        /// update the current calibration setting time from a given body frame
        /// </summary>
        /// <param name="vFrame"></param>
        public void UpdateTimeFromBodyFrame(BodyFrame vFrame)
        {
            Time = vFrame.Timestamp - StartTime;
        }

        /// <summary>
        /// set a new start time from body frame's timestamp
        /// </summary>
        /// <param name="vBodyFrame"></param>
        public void SetNewStartTimeFromBodyFrame(BodyFrame vBodyFrame)
        {
            StartTime = vBodyFrame.Timestamp;
        }

        /// <summary>
        /// Calibration Type event has been passed
        /// </summary>
        /// <param name="vType"></param>
        /// <returns></returns>
        public bool HasPassedCalibrationTypeEvent(CalibrationType vType)
        {
            bool vHasPassedTime = false;
            if (vType != GlobalCalibrationSettings.FinalPose)
            {
                CalibrationType vNextCalibrationType = vType + 1;
                vHasPassedTime = Time >= GlobalCalibrationSettings.CalibrationTimes[vType] && Time < GlobalCalibrationSettings.CalibrationTimes[vNextCalibrationType];
            }
            else
            {
                vHasPassedTime = Time >= GlobalCalibrationSettings.CalibrationTimes[vType] ;
            }

            return vHasPassedTime;
        }

        /// <summary>
        /// Get the calibration type's timer
        /// </summary>
        /// <param name="vType"></param>
        /// <returns></returns>
        public float GetCalibrationTypeTimer(CalibrationType vType)
        {
            if (GlobalCalibrationSettings.CalibrationTimes.ContainsKey(vType))
            {
                return GlobalCalibrationSettings.CalibrationTimes[vType];
            }
            return -1;
        }
        public float StartTime
        {
            get { return mStartTime; }
            private set { mStartTime = value; }
        }

        public float Time
        {
            get { return mTime; }
            private set { mTime = value; }
        }
    }
}