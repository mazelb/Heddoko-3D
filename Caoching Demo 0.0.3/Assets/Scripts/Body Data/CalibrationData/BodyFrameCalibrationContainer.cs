
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
    public delegate void CompletedCollectionEvent();
    /// <summary>
    /// Calibration settings and times of a body calibration
    /// </summary>
    public class BodyFrameCalibrationContainer
    {
        private float mStartTime;
        private float mTime;
        private bool mFirstUpdateLoop = true;
        private bool mHasEnteredFinalPose;
        private CalibrationType mCurrentPos = CalibrationType.NULL;
        public event CompletedCollectionEvent CollectionCompletedNotification;
        /// <summary>
        /// Cette structure de donnees nous permet de retirer des points de mouvement  contigues de nature equivalent
        /// </summary>
        private Dictionary<CalibrationType, CalibrationTrackingStructure> mCalibrationFrames;

        private Dictionary<CalibrationType, CalibrationTrackingStructure> CalibrationFrames
        {
            get
            {
                if (mCalibrationFrames == null)
                {
                    mCalibrationFrames = new Dictionary<CalibrationType, CalibrationTrackingStructure>();
                    Initialize();
                }
                return mCalibrationFrames;
            }
        }

        /// <summary>
        /// Initializes container
        /// </summary>
        private void Initialize()
        {
            mCalibrationFrames.Add(CalibrationType.Tpose, new CalibrationTrackingStructure());
            mCalibrationFrames.Add(CalibrationType.ArmsForward, new CalibrationTrackingStructure());
            mCalibrationFrames.Add(CalibrationType.ArmsDown, new CalibrationTrackingStructure());
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public BodyFrameCalibrationContainer()
        {
            mStartTime = mTime = 0;
        }


        /// <summary>
        /// updates the BodyFrameCalibration container with movement data. 
        /// </summary>
        /// <param name="vFiltered">the filtered movement data </param>
        /// <param name="vCurrentFrameTime">Current Time frame of the movement data</param>
        public void UpdateCalibrationContainer(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFiltered, float vCurrentFrameTime)
        {
            CurrentTime = vCurrentFrameTime - StartTime;
            if (ValidationStateMachine())
            {
                if (HasPassedCalibrationTypeEvent(CalibrationType.Tpose))
                {
                    mCalibrationFrames[CalibrationType.Tpose].Add(vFiltered);
                    mCurrentPos = CalibrationType.Tpose;
                }
                else if (HasPassedCalibrationTypeEvent(CalibrationType.ArmsForward))
                {
                    mCalibrationFrames[CalibrationType.ArmsForward].Completed = false;
                    mCalibrationFrames[CalibrationType.ArmsForward].Add(vFiltered);
                    mCurrentPos = CalibrationType.ArmsForward;
                    //Set the Tpose to being completed
                    mCalibrationFrames[CalibrationType.Tpose].Completed = true;
                }
                else if (HasPassedCalibrationTypeEvent(CalibrationType.ArmsDown))
                {
                    mCalibrationFrames[CalibrationType.ArmsDown].Completed = false;
                    mCalibrationFrames[CalibrationType.ArmsDown].Add(vFiltered);
                    mCurrentPos = CalibrationType.ArmsDown;
                    mCalibrationFrames[CalibrationType.ArmsForward].Completed = true;
                }
                else
                {
                    mCurrentPos = CalibrationType.NULL;
                }
            }
        }

        /// <summary>
        ///  Returns the current pointer of the movement position that the calibration container is in. 
        /// </summary>
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
            if (CalibrationFrames.ContainsKey(vType))
            {
                if (CalibrationFrames[vType].Completed)
                {
                    return CalibrationFrames[vType].GetList;
                }
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
        public void Reset(BodyFrame vFirstBodyFrame)
        {
            Clear();
            mFirstUpdateLoop = true;
            if (vFirstBodyFrame != null)
            {
                mStartTime = vFirstBodyFrame.Timestamp;
            }
        }


        /// <summary>
        /// Clears out the content of the calibration container.
        /// </summary>
        public void Clear()
        {
            foreach (var vCalibrationFrames in CalibrationFrames)
            {
                vCalibrationFrames.Value.Clear();
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
            CalibrationType vPrevCalibrationType = vType - 1;
            if (vType != CalibrationType.NULL && vPrevCalibrationType != CalibrationType.NULL)
            {
                vHasPassedTime = CurrentTime >= GlobalCalibrationSettings.CalibrationTimestamps[vPrevCalibrationType] && CurrentTime <= GlobalCalibrationSettings.CalibrationTimestamps[vType];
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
                //first verify if the Frame container has recieved a timestamp that has exited the final pose
                if (CurrentTime > GlobalCalibrationSettings.CalibrationTimestamps[GlobalCalibrationSettings.FinalPose])
                {
                    mHasEnteredFinalPose = true;
                }
                else
                {
                    if (mHasEnteredFinalPose)
                    {
                        mFirstUpdateLoop = false;
                        mCalibrationFrames[GlobalCalibrationSettings.FinalPose].Completed = true;
                        //notify this on 
                        if (CollectionCompletedNotification != null)
                        {
                            CollectionCompletedNotification();
                        }
                    }
                }
            }
            return vResult;
        }

        /// <summary>
        /// Get the calibration type's timer
        /// </summary>
        /// <param name="vType"> The calibration type</param>
        /// <returns></returns>
        public float GetCalibrationTypeTimer(CalibrationType vType)
        {
            if (GlobalCalibrationSettings.CalibrationTimestamps.ContainsKey(vType))
            {
                return GlobalCalibrationSettings.CalibrationTimestamps[vType];
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