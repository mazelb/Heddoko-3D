// /**
// * @file AnalysisFramesCollectionController.cs
// * @brief Contains the AnalysisFramesCollectionController class
// * @author Mohammed Haider( mohammed@ heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System;
using System.Linq;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Serialization;
using Assets.Scripts.Body_Pipeline.Analysis.Trunk;

namespace Assets.Scripts.Body_Pipeline.Analysis.Controller
{
    /// <summary>
    /// A controller for collecting frames. its responsiblity is to set rules on how a Body's analysis frames are collected,stored and serialized
    /// </summary>
    public class AnalysisFramesCollectionController
    {
        private const int FRAME_OFFSET = 6;
        /// <summary>
        /// Body from which to collect analysis frames from
        /// </summary>
        private Body mNotifyingBody;
        private bool mCollectingData;
        private TPosedAnalysisFrame mCurrentAnalysisFrame;
        private int mAnalysisSegmentCounter;
        private int mTotalSingleSegmentsToCount;
         internal bool MovementSet;
        private BodyFrame mCurrentBodyFrame;
        /// <summary>
        /// Frames serializer
        /// </summary>
        private IAnalysisFramesSerializer FramesSerializer { get; set; }

        /// <summary>
        /// the body 
        /// </summary>
        public Body Body
        {
            get
            {
                return mNotifyingBody;
            }
            private set
            {
                mNotifyingBody = value;
            }
        }

        /// <summary>
        /// Create an instance with a serializer
        /// </summary>
        /// <param name="vSerializer"></param>
        public AnalysisFramesCollectionController(IAnalysisFramesSerializer vSerializer)
        {
            FramesSerializer = vSerializer;
        }
        /// <summary>
        /// Set the notifying body to the current instance.
        /// </summary>
        /// <param name="vBody">the new body.</param>
        public void SetBody(Body vBody)
        {
            RemoveListeners();
            if (vBody == null)
            {
                throw new NullReferenceException("Passed body to the AnalysisFramesCollectionController is null.");
            }
            Body = vBody;
            var vAnalysisSegments = Body.AnalysisSegments;
            mTotalSingleSegmentsToCount = vAnalysisSegments.Count;
            FramesSerializer.SetAnalysisSegments(Body.AnalysisSegments.Values.ToList());
            RegisterListener();
        }

        /// <summary>
        /// Registers listeners to the Body
        /// </summary>
        private void RegisterListener()
        {
            if (Body != null)
            {
                var vView = Body.View;
                vView.BodyFrameUpdatedEvent += BodyFrameUpdatedHandler;
                vView.BodyFrameResetInitializedEvent += BodyFrameResetInitializedEvent;
                var vAnalysisSegments = Body.AnalysisSegments;
                //set up analysis listeners
                foreach (var vKeyValue in vAnalysisSegments)
                {
                    vKeyValue.Value.AnalysisCompletedEvent += AnalysisCompletedEvent;
                }
            }
        }

        /// <summary>
        /// Serializes the set 
        /// </summary>
        /// <param name="vPath">the path to serialize to</param>
        public void SerializeSet(string vPath)
        {
            FramesSerializer.Serialize(Body.AnalysisFramesSet, vPath);
        }

        /// <summary>
        /// Event listener to when the body has been iniatilized a reset
        /// </summary>
        /// <param name="vBodyFrame">the frame that has been reset to</param>
        private void BodyFrameResetInitializedEvent(BodyFrame vBodyFrame)
        {

            if (!MovementSet || !mCollectingData || mCurrentBodyFrame != null)
            {
                return;
            }
            //verify if the frame exists already in the set
            var vFrame = Body.AnalysisFramesSet.Get(mCurrentBodyFrame.Index - FRAME_OFFSET);
            if (vFrame != null)
            {
                if (mCurrentBodyFrame.Index == vBodyFrame.Index)
                {
                    mCurrentAnalysisFrame.Status = TposeStatus.Tpose;
                }
                else
                {
                    vFrame.Status = TposeStatus.Tpose;
                }
                //clear out frames after this current index
                int vStartIndexClear = mCurrentBodyFrame.Index - FRAME_OFFSET + 1;
                int vMaxCount = Body.AnalysisFramesSet.MaxFramesCount;
                for (int vI = vStartIndexClear; vI < vMaxCount; vI++)
                {
                    Body.AnalysisFramesSet.Remove(vI);
                }
            }
        }

        /// <summary>
        /// Event listener when the body updates its frame. 
        /// </summary>
        /// <param name="vNewFrame"></param>
        private void BodyFrameUpdatedHandler(BodyFrame vNewFrame)
        { 
            //mCurrentFrameIndex = vNewFrame.Index;
            //if (mCurrentAnalysisFrame != null)
            //{
            //    mCurrentAnalysisFrame.TimeStamp = vNewFrame.Timestamp;
            //}
            mCurrentBodyFrame = vNewFrame;
            //todo: add time stamp as uint
        }

        /// <summary>
        /// Analysis completed event
        /// </summary>
        /// <param name="vSegmentAnalysis"></param>
        private void AnalysisCompletedEvent(SegmentAnalysis vSegmentAnalysis)
        {
            if (!mCollectingData || !MovementSet)
            {
                return;
            }
            //Verify if the current set already has a an analysis frame
            if (Body.AnalysisFramesSet.ContainsFrameAt(mCurrentBodyFrame.Index - FRAME_OFFSET))
            {
                return;
            }
            if (mAnalysisSegmentCounter == 0)
            {
                mCurrentAnalysisFrame = new TPosedAnalysisFrame();
                mCurrentAnalysisFrame.Index = mCurrentBodyFrame.Index;
                mCurrentAnalysisFrame.TimeStamp = mCurrentBodyFrame.Timestamp;
            }
            SetSegmentAnalysisToCurrentAnalysisFrame(vSegmentAnalysis);
            mAnalysisSegmentCounter++;
            if (mAnalysisSegmentCounter == mTotalSingleSegmentsToCount)
            {
                mCurrentAnalysisFrame.Index = mCurrentBodyFrame.Index;
                Body.AnalysisFramesSet.Add(mCurrentBodyFrame.Index - FRAME_OFFSET, mCurrentAnalysisFrame);
                mAnalysisSegmentCounter = 0;
            }
        }

        /// <summary>
        /// resets the data collection
        /// </summary>
        public void ResetCollection()
        {
            mAnalysisSegmentCounter = 0;
            if (Body == null || Body.AnalysisFramesSet == null)
            {
                return;
            }
            Body.AnalysisFramesSet.Clear();
        }

        /// <summary>
        /// Helper method to the analysis completion event
        /// </summary>
        /// <param name="vSegmentAnalysis"></param>
        private void SetSegmentAnalysisToCurrentAnalysisFrame(SegmentAnalysis vSegmentAnalysis)
        {
            switch (vSegmentAnalysis.SegmentType)
            {
                case BodyStructureMap.SegmentTypes.SegmentType_Trunk:
                    var vTrunkAnalysis = (TrunkAnalysis)vSegmentAnalysis;
                    mCurrentAnalysisFrame.TrunkFlexionAngle = vTrunkAnalysis.TrunkFlexionAngle;
                    mCurrentAnalysisFrame.TrunkFlexionSignedAngle = vTrunkAnalysis.TrunkFlexionSignedAngle;
                    mCurrentAnalysisFrame.TrunkLateralAngle = vTrunkAnalysis.TrunkLateralAngle;
                    mCurrentAnalysisFrame.TrunkLateralSignedAngle = vTrunkAnalysis.TrunkLateralSignedAngle;
                    mCurrentAnalysisFrame.TrunkRotationAngle = vTrunkAnalysis.TrunkRotationAngle;
                    mCurrentAnalysisFrame.TrunkRotationSignedAngle = vTrunkAnalysis.TrunkRotationSignedAngle;
                    break;
                case BodyStructureMap.SegmentTypes.SegmentType_LeftArm:
                    var vLeftArmAnalysis = (LeftArmAnalysis)vSegmentAnalysis;
                    mCurrentAnalysisFrame.LeftElbowFlexionAngle = vLeftArmAnalysis.LeftElbowFlexionAngle;
                    mCurrentAnalysisFrame.LeftElbowFlexionSignedAngle = vLeftArmAnalysis.LeftElbowFlexionSignedAngle;
                    mCurrentAnalysisFrame.LeftForeArmPronationSignedAngle = vLeftArmAnalysis.LeftForeArmPronationSignedAngle;
                    mCurrentAnalysisFrame.LeftShoulderFlexionAngle = vLeftArmAnalysis.LeftShoulderFlexionAngle;
                    mCurrentAnalysisFrame.LeftShoulderFlexionSignedAngle = vLeftArmAnalysis.LeftShoulderFlexionSignedAngle;
                    mCurrentAnalysisFrame.LeftShoulderVertAbductionAngle = vLeftArmAnalysis.LeftShoulderVertAbductionAngle;
                    mCurrentAnalysisFrame.LeftShoulderVerticalAbductionSignedAngle = vLeftArmAnalysis.LeftShoulderVerticalAbductionSignedAngle;
                    mCurrentAnalysisFrame.LeftShoulderHorAbductionAngle = vLeftArmAnalysis.LeftShoulderHorAbductionAngle;
                    mCurrentAnalysisFrame.LeftShoulderHorizontalAbductionSignedAngle = vLeftArmAnalysis.LeftShoulderHorizontalAbductionSignedAngle;
                    mCurrentAnalysisFrame.LeftShoulderRotationSignedAngle = vLeftArmAnalysis.LeftShoulderRotationSignedAngle;
                    mCurrentAnalysisFrame.LeftElbowFlexionAngularVelocity = vLeftArmAnalysis.LeftElbowFlexionAngularVelocity;
                    mCurrentAnalysisFrame.LeftElbowFlexionPeakAngularVelocity = vLeftArmAnalysis.LeftElbowFlexionPeakAngularVelocity;
                    mCurrentAnalysisFrame.LeftElbowFlexionAngularAcceleration = vLeftArmAnalysis.LeftElbowFlexionAngularAcceleration;
                    mCurrentAnalysisFrame.LeftForeArmPronationAngularVelocity = vLeftArmAnalysis.LeftForeArmPronationAngularVelocity;
                    mCurrentAnalysisFrame.LeftElbowPronationAngularAcceleration = vLeftArmAnalysis.LeftElbowPronationAngularAcceleration;
                    mCurrentAnalysisFrame.LeftShoulderFlexionAngularVelocity = vLeftArmAnalysis.LeftShoulderFlexionAngularVelocity;
                    mCurrentAnalysisFrame.LeftShoulderFlexionAngularAcceleration = vLeftArmAnalysis.LeftShoulderFlexionAngularAcceleration;
                    mCurrentAnalysisFrame.LeftShoulderVertAbductionAngularVelocity = vLeftArmAnalysis.LeftShoulderVertAbductionAngularVelocity;
                    mCurrentAnalysisFrame.LeftShoulderVertAbductionAngularAcceleration = vLeftArmAnalysis.LeftShoulderVertAbductionAngularAcceleration;
                    mCurrentAnalysisFrame.LeftShoulderHorAbductionAngularVelocity = vLeftArmAnalysis.LeftShoulderHorAbductionAngularVelocity;
                    mCurrentAnalysisFrame.LeftShoulderHorAbductionAngularAcceleration = vLeftArmAnalysis.LeftShoulderHorAbductionAngularAcceleration;
                    mCurrentAnalysisFrame.LeftShoulderRotationAngularVelocity = vLeftArmAnalysis.LeftShoulderRotationAngularVelocity;
                    mCurrentAnalysisFrame.LeftShoulderRotationAngularAcceleration = vLeftArmAnalysis.LeftShoulderRotationAngularAcceleration;
                    break;
                case BodyStructureMap.SegmentTypes.SegmentType_RightArm:
                    var vRightArmAnalysis = (RightArmAnalysis)vSegmentAnalysis;
                    mCurrentAnalysisFrame.RightElbowFlexionAngle = vRightArmAnalysis.RightElbowFlexionAngle;
                    mCurrentAnalysisFrame.RightElbowFlexionSignedAngle = vRightArmAnalysis.RightElbowFlexionSignedAngle;
                    mCurrentAnalysisFrame.RightForeArmPronationSignedAngle = vRightArmAnalysis.RightForeArmPronationSignedAngle;
                    mCurrentAnalysisFrame.RightShoulderFlexionAngle = vRightArmAnalysis.RightShoulderFlexionAngle;
                    mCurrentAnalysisFrame.RightShoulderFlexionSignedAngle = vRightArmAnalysis.RightShoulderFlexionSignedAngle;
                    mCurrentAnalysisFrame.RightShoulderVertAbductionAngle = vRightArmAnalysis.RightShoulderVertAbductionAngle;
                    mCurrentAnalysisFrame.RightShoulderVerticalAbductionSignedAngle = vRightArmAnalysis.RightShoulderVerticalAbductionSignedAngle;
                    mCurrentAnalysisFrame.RightShoulderHorAbductionAngle = vRightArmAnalysis.RightShoulderHorAbductionAngle;
                    mCurrentAnalysisFrame.RightShoulderHorizontalAbductionSignedAngle = vRightArmAnalysis.RightShoulderHorizontalAbductionSignedAngle;
                    mCurrentAnalysisFrame.RightShoulderRotationSignedAngle = vRightArmAnalysis.RightShoulderRotationSignedAngle;
                    mCurrentAnalysisFrame.RightElbowFlexionAngularVelocity = vRightArmAnalysis.RightElbowFlexionAngularVelocity;
                    mCurrentAnalysisFrame.RightElbowFlexionPeakAngularVelocity = vRightArmAnalysis.RightElbowFlexionPeakAngularVelocity;
                    mCurrentAnalysisFrame.RightElbowFlexionAngularAcceleration = vRightArmAnalysis.RightElbowFlexionAngularAcceleration;
                    mCurrentAnalysisFrame.RightForeArmPronationAngularVelocity = vRightArmAnalysis.RightForeArmPronationAngularVelocity;
                    mCurrentAnalysisFrame.RightForeArmPronationAngularAcceleration = vRightArmAnalysis.RightForeArmPronationAngularAcceleration;
                    mCurrentAnalysisFrame.RightShoulderFlexionAngularVelocity = vRightArmAnalysis.RightShoulderFlexionAngularVelocity;
                    mCurrentAnalysisFrame.RightShoulderFlexionAngularAcceleration = vRightArmAnalysis.RightShoulderFlexionAngularAcceleration;
                    mCurrentAnalysisFrame.RightShoulderVertAbductionAngularVelocity = vRightArmAnalysis.RightShoulderVertAbductionAngularVelocity;
                    mCurrentAnalysisFrame.RightShoulderVertAbductionAngularAcceleration = vRightArmAnalysis.RightShoulderVertAbductionAngularAcceleration;
                    mCurrentAnalysisFrame.RightShoulderHorAbductionAngularVelocity = vRightArmAnalysis.RightShoulderHorAbductionAngularVelocity;
                    mCurrentAnalysisFrame.RightShoulderHorAbductionAngularAcceleration = vRightArmAnalysis.RightShoulderHorAbductionAngularAcceleration;
                    mCurrentAnalysisFrame.RightShoulderRotationAngularVelocity = vRightArmAnalysis.RightShoulderRotationAngularVelocity;
                    mCurrentAnalysisFrame.RightShoulderRotationAngularAcceleration = vRightArmAnalysis.RightShoulderRotationAngularAcceleration;
                    break;
                case BodyStructureMap.SegmentTypes.SegmentType_LeftLeg:
                    var vLeftLegAnalysis = (LeftLegAnalysis)vSegmentAnalysis;
                    mCurrentAnalysisFrame.LeftKneeFlexionAngle = vLeftLegAnalysis.LeftKneeFlexionAngle;
                    mCurrentAnalysisFrame.LeftKneeFlexionSignedAngle = vLeftLegAnalysis.LeftKneeFlexionSignedAngle;
                    mCurrentAnalysisFrame.LeftKneeRotationSignedAngle = vLeftLegAnalysis.LeftKneeRotationSignedAngle;
                    mCurrentAnalysisFrame.LeftHipFlexionAngle = vLeftLegAnalysis.LeftHipFlexionAngle;
                    mCurrentAnalysisFrame.LeftHipFlexionSignedAngle = vLeftLegAnalysis.LeftHipFlexionSignedAngle;
                    mCurrentAnalysisFrame.LeftHipAbductionAngle = vLeftLegAnalysis.LeftHipAbductionAngle;
                    mCurrentAnalysisFrame.LeftHipAbductionSignedAngle = vLeftLegAnalysis.LeftHipAbductionSignedAngle;
                    mCurrentAnalysisFrame.LeftHipRotationSignedAngle = vLeftLegAnalysis.LeftHipRotationSignedAngle;
                    mCurrentAnalysisFrame.LeftKneeFlexionAngularVelocity = vLeftLegAnalysis.LeftKneeFlexionAngularVelocity;
                    mCurrentAnalysisFrame.LeftKneeFlexionAngularAcceleration = vLeftLegAnalysis.LeftKneeFlexionAngularAcceleration;
                    mCurrentAnalysisFrame.LeftKneeRotationAngularVelocity = vLeftLegAnalysis.LeftKneeRotationAngularVelocity;
                    mCurrentAnalysisFrame.LeftKneeRotationAngularAcceleration = vLeftLegAnalysis.LeftKneeRotationAngularAcceleration;
                    mCurrentAnalysisFrame.LeftHipFlexionAngularVelocity = vLeftLegAnalysis.LeftHipFlexionAngularVelocity;
                    mCurrentAnalysisFrame.LeftHipFlexionAngularAcceleration = vLeftLegAnalysis.LeftHipFlexionAngularAcceleration;
                    mCurrentAnalysisFrame.LeftHipAbductionAngularVelocity = vLeftLegAnalysis.LeftHipAbductionAngularVelocity;
                    mCurrentAnalysisFrame.LeftHipAbductionAngularAcceleration = vLeftLegAnalysis.LeftHipAbductionAngularAcceleration;
                    mCurrentAnalysisFrame.LeftHipRotationAngularVelocity = vLeftLegAnalysis.LeftHipRotationAngularVelocity;
                    mCurrentAnalysisFrame.LeftHipRotationAngularAcceleration = vLeftLegAnalysis.LeftHipRotationAngularAcceleration;
                    mCurrentAnalysisFrame.LeftLegHeight = vLeftLegAnalysis.LegHeight;
                    mCurrentAnalysisFrame.LeftInitThighHeight = vLeftLegAnalysis.mInitThighHeight;
                    mCurrentAnalysisFrame.LeftInitTibiaHeight = vLeftLegAnalysis.mInitTibiaHeight;
                    break;
                case BodyStructureMap.SegmentTypes.SegmentType_RightLeg:
                    var vRightLegtAnalysis = (RightLegAnalysis)vSegmentAnalysis;
                    mCurrentAnalysisFrame.RightKneeFlexionAngle = vRightLegtAnalysis.RightKneeFlexionAngle;
                    mCurrentAnalysisFrame.RightKneeFlexionSignedAngle = vRightLegtAnalysis.RightKneeFlexionSignedAngle;
                    mCurrentAnalysisFrame.RightKneeRotationSignedAngle = vRightLegtAnalysis.RightKneeRotationSignedAngle;
                    mCurrentAnalysisFrame.RightHipFlexionAngle = vRightLegtAnalysis.RightHipFlexionAngle;
                    mCurrentAnalysisFrame.RightHipFlexionSignedAngle = vRightLegtAnalysis.RightHipFlexionSignedAngle;
                    mCurrentAnalysisFrame.RightHipAbductionAngle = vRightLegtAnalysis.RightHipAbductionAngle;
                    mCurrentAnalysisFrame.RightHipAbductionSignedAngle = vRightLegtAnalysis.RightHipAbductionSignedAngle;
                    mCurrentAnalysisFrame.RightHipRotationSignedAngle = vRightLegtAnalysis.RightHipRotationSignedAngle;
                    mCurrentAnalysisFrame.RightKneeFlexionAngularVelocity = vRightLegtAnalysis.RightKneeFlexionAngularVelocity;
                    mCurrentAnalysisFrame.RightKneeFlexionAngularAcceleration = vRightLegtAnalysis.RightKneeFlexionAngularAcceleration;
                    mCurrentAnalysisFrame.RightKneeRotationAngularVelocity = vRightLegtAnalysis.RightKneeRotationAngularVelocity;
                    mCurrentAnalysisFrame.RightKneeRotationAngularAcceleration = vRightLegtAnalysis.RightKneeRotationAngularAcceleration;
                    mCurrentAnalysisFrame.RightHipFlexionAngularVelocity = vRightLegtAnalysis.RightHipFlexionAngularVelocity;
                    mCurrentAnalysisFrame.RightHipFlexionAngularAcceleration = vRightLegtAnalysis.RightHipFlexionAngularAcceleration;
                    mCurrentAnalysisFrame.RightHipAbductionAngularVelocity = vRightLegtAnalysis.RightHipAbductionAngularVelocity;
                    mCurrentAnalysisFrame.RightHipAbductionAngularAcceleration = vRightLegtAnalysis.RightHipAbductionAngularAcceleration;
                    mCurrentAnalysisFrame.RightHipRotationAngularVelocity = vRightLegtAnalysis.RightHipRotationAngularVelocity;
                    mCurrentAnalysisFrame.RightHipRotationAngularAcceleration = vRightLegtAnalysis.RightHipRotationAngularAcceleration;
                    mCurrentAnalysisFrame.RightLegHeight = vRightLegtAnalysis.LegHeight;
                    mCurrentAnalysisFrame.RightInitThighHeight = vRightLegtAnalysis.mInitThighHeight;
                    mCurrentAnalysisFrame.RightInitTibiaHeight = vRightLegtAnalysis.mInitTibiaHeight;
                    break;
            }
        }

        /// <summary>
        /// Remove all listeners from the body.
        /// </summary>
        private void RemoveListeners()
        {
            if (Body != null)
            {
                var vView = Body.View;
                vView.BodyFrameUpdatedEvent -= BodyFrameUpdatedHandler;
                vView.BodyFrameResetInitializedEvent -= BodyFrameResetInitializedEvent;
                var vAnalysisSegments = Body.AnalysisSegments;
                //set up analysis listeners
                foreach (var vKeyValue in vAnalysisSegments)
                {
                    vKeyValue.Value.AnalysisCompletedEvent -= AnalysisCompletedEvent;
                }
            }
        }

        /// <summary>
        /// Start data collection
        /// </summary>
        public void Start()
        {
            mCollectingData = true;
        }

        public void Stop()
        {
            mCollectingData = false;
        }




        /// <summary>
        /// Returns an analysis frame at the given index
        /// </summary>
        /// <param name="vIndex"></param>
        /// <returns></returns>
        public TPosedAnalysisFrame GetAnalysisFrame(int vIndex)
        {
            if (Body == null || Body.AnalysisFramesSet == null)
            {
                return null;
            }
            else
            {
                return Body.AnalysisFramesSet.Get(vIndex - FRAME_OFFSET);
            }
        }
    }
}
