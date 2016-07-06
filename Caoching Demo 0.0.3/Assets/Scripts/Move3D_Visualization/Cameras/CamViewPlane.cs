/** 
* @file CamViewPlane.cs
* @brief Contains the CamViewPlane  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.Body_Data.View;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.camera
{
    public delegate void CameraPositionChanged(Vector3 vNewPosition);
    /// <summary>
    /// Set a position for a camera to a plane of view, referencing a body segment
    /// </summary>
    public class CamViewPlane
    {
        private RenderedBody mRefBody;
        private Dictionary<CameraViewPlane, Vector3> mPositionMaps;
        private BodyStructureMap.SubSegmentTypes mReferenceSubSegment = BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine;
        private CameraViewPlane mCameraViewPlane;
        private float mDistanceFromRefTarget = 5f;
        internal event CameraPositionChanged CameraPositionChangedEvent;
        internal RenderedBody TargetBody
        {
            get { return mRefBody; }
            set
            {
                if (mRefBody != value)
                {
                    mRefBody = value;
                    InitCamPositions();
                }
            }
        }

        /// <summary>
        /// Getter and setter method of the referenced subsegment
        /// </summary>
        public BodyStructureMap.SubSegmentTypes ReferenceSubSegment
        {
            get
            {
                return mReferenceSubSegment;
            }
            set
            {
                if (mReferenceSubSegment != value)
                {
                    mReferenceSubSegment = value;
                    InitCamPositions();
                }
            }
        }

        public CameraViewPlane ViewPlane
        {
            get { return mCameraViewPlane; }
            set
            {
                mCameraViewPlane = value;
                InitCamPositions();
            }
        }

        /// <summary>
        /// Default constructor. 
        /// </summary>
        public CamViewPlane()
        {
            mPositionMaps = new Dictionary<CameraViewPlane, Vector3>(3);
            mPositionMaps.Add(CameraViewPlane.Frontal, Vector3.forward);
            mPositionMaps.Add(CameraViewPlane.Sagital, Vector3.right);
            mPositionMaps.Add(CameraViewPlane.Transverse, Vector3.up);
        }
        /// <summary>
        /// Initialize camera positions with respect to the referenced body type
        /// Triggers a CameraPositionChangedEvent 
        /// </summary>
        public void InitCamPositions()
        {
            Transform vTransform = mRefBody.GetSubSegment(ReferenceSubSegment);
            Vector3 vNormal = mPositionMaps[ViewPlane];

            Vector3 vNewPos = vTransform.position + vNormal * mDistanceFromRefTarget;
            if (CameraPositionChangedEvent != null)
            {
                CameraPositionChangedEvent(vNewPos);
            }
        }




    }
    public enum CameraViewPlane
    {
        /// <summary>
        /// YZ plane  normal is (1,0,0)
        /// </summary>
        Sagital,
        /// <summary>
        /// XZ plane , normal is  (0,0,1)
        /// </summary>
        Frontal,
        /// <summary>
        /// XY plane, normal is (0,1,0)
        /// </summary>
        Transverse
    }
}