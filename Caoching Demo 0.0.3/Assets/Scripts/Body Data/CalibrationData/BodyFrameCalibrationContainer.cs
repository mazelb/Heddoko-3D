
/** 
* @file BodyFrameCalibrationContainer.cs
* @brief Contains the BodyFrameCalibrationContainer class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.CalibrationData
{
    /// <summary>
    /// Calibration settings and times of a body calibration
    /// </summary>
    public class BodyFrameCalibrationContainer
    {
        private float mStartTime;
        private float mTime;
        private bool mFirstUpdateLoop = true;
        private bool mHasEnteredFinalPose = false;
        private CalibrationType mCurrentPos = CalibrationType.Invalid;

        /// <summary>
        /// Cette structure de donnees nous permet de retirer des points de mouvement  contigues de nature equivalent
        /// </summary>
        private Dictionary<CalibrationType, List<Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>>> mCalibrationFrames
            = new Dictionary<CalibrationType, List<Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>>>();

  
        /// <summary>
        /// default constructor
        /// </summary>
        public BodyFrameCalibrationContainer()
        {
            mStartTime = mTime = 0;
        }
        /// <summary>
        /// update the current calibration setting time from a given body frame
        /// </summary>
        /// <param name="vFrame"></param>      

        public void UpdateCalibrationContainer(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFiltered, float vCurrentFrameTime)
        {
            if (ValidationStateMachine())
            {
                CurrentTime = vCurrentFrameTime - StartTime;
                if (HasPassedCalibrationTypeEvent(CalibrationType.Tpose))
                {
                    mCalibrationFrames[CalibrationType.Tpose].Add(vFiltered);
                    mCurrentPos = CalibrationType.Tpose;
                }
                else if (HasPassedCalibrationTypeEvent(CalibrationType.ArmsForward))
                {
                    mCalibrationFrames[CalibrationType.ArmsForward].Add(vFiltered);
                    mCurrentPos = CalibrationType.ArmsForward;
                }
                else if (HasPassedCalibrationTypeEvent(CalibrationType.ArmsDown))
                {
                    mCalibrationFrames[CalibrationType.ArmsDown].Add(vFiltered);
                    mCurrentPos = CalibrationType.ArmsDown;
                }
                else
                {
                    mCurrentPos = CalibrationType.Invalid;
                }
            }
        }
        public CalibrationType GetCurrentPos
        {
            get { return mCurrentPos; }
          
        }
        /// <summary>
        /// Will return a list of calibration movements according to the Calibration movement type 
        /// Note: if note found, will return null.
        /// </summary>
        /// <param name="vType"></param>
        /// <returns>return a list of calibration movements </returns>

        public List<Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>> GetListOfCalibrationMovement(CalibrationType vType)
        {
            if (mCalibrationFrames.ContainsKey(vType))
            {
                return mCalibrationFrames[vType];
            }
            return null;
        }

        /// <summary>
        /// set a new start time from body frame's timestamp
        /// </summary> 
        public void SetNewStartTime(float vStartTime)
        {
            StartTime = vStartTime;
        }

        /// <summary>
        /// Clear and reset container
        /// </summary>
        public void Reset(BodyFrame vFirstBodyFrame )
        {
            mCalibrationFrames.Clear();
            mFirstUpdateLoop = true;
            if(vFirstBodyFrame != null)
            {
                mStartTime = vFirstBodyFrame.Timestamp;
            }
        }


        /// <summary>
        /// Calibration Type event has been passed
        /// </summary>
        /// <param name="vType">the calibration type</param>
        /// <returns>a valid calibration type has been passed</returns>
        public bool HasPassedCalibrationTypeEvent(CalibrationType vType)
        {
            bool vHasPassedTime = false;
            if (vType != GlobalCalibrationSettings.FinalPose && vType != CalibrationType.Invalid)
            {
                CalibrationType vNextCalibrationType = vType + 1;
                vHasPassedTime = CurrentTime >= GlobalCalibrationSettings.CalibrationTimes[vType] && CurrentTime < GlobalCalibrationSettings.CalibrationTimes[vNextCalibrationType];
            }
            else if(vType == GlobalCalibrationSettings.FinalPose)
            {
                vHasPassedTime = CurrentTime >= GlobalCalibrationSettings.CalibrationTimes[vType];
            }

            return vHasPassedTime;
        }

        /// <summary>
        /// A state machine that validates if a container can add anything else
        /// </summary>
        /// <returns></returns>

        private bool ValidationStateMachine()
        {
            bool vResult = false;
            if (mFirstUpdateLoop)
            {
                vResult = true;
                if (HasPassedCalibrationTypeEvent(GlobalCalibrationSettings.FinalPose))
                {
                    mHasEnteredFinalPose = true;
                }
                else
                {
                    if (mHasEnteredFinalPose)
                    {
                        mFirstUpdateLoop = false;
                    }
                }


            }
            return vResult;
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

        public float CurrentTime
        {
            get { return mTime; }
            private set { mTime = value; }
        }
    }
}