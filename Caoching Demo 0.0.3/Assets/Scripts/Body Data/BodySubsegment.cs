/** 
* @file BodySubSegment.cs
* @brief Contains the BodySubSegment  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using Assets.Scripts.Body_Data.view;
using UnityEngine;

/**
* BodySubSegment class 
* @brief BodySubSegment class (represents one abstracted reprensentation of a body subsegment)
*/
namespace Assets.Scripts.Body_Data
{
    [Serializable]
    public class BodySubSegment
    {
        //TODO: Sub Segment Orientation Type (Raw-Tracked-fused-mapped)
        public BodyStructureMap.SubSegmentTypes SubsegmentType;
        public Quaternion SubsegmentOrientation = Quaternion.identity;
        public Vector3 SubSegmentPosition;
        public Vector3 SubSegmentGravity = Vector3.zero;
        public BodyStructureMap.SubSegmentOrientationType SubsegmentOrientationType;
        public BodySubsegmentView AssociatedView;

        /// <summary>
        /// Resets the orientations of the associated view
        /// </summary>
        public void ResetViewTransforms()
        {
            AssociatedView.ResetTransforms();
        }

        /// <summary>
        /// Updates the subsegments Orientation
        /// <summary>
        /// <param name="vNewOrientation">the new SubSegment orientation</param>
        /// <param name="vNewDisplacement">Apply it locally or globaly ?</param>
        /// <param name="vNewDisplacement">reset the current orientation before applying new one or cumulate?</param>
        public void UpdateSubsegmentOrientation(Quaternion vNewOrientation, int vApplyLocal = 0, bool vResetRotation = false)
        {
            //update the view
            SubsegmentOrientation = vNewOrientation;
            AssociatedView.UpdateOrientation(vNewOrientation, vApplyLocal, vResetRotation);
        }

        /// <summary>
        /// Updates the subsegments position
        /// <summary>
        /// <param name="vNewDisplacement">the new SubSegment displacement vector</param>
        public void UpdateSubsegmentPosition(Vector3 vNewDisplacement)
        {
            //update the view
            SubSegmentPosition = vNewDisplacement;
            AssociatedView.UpdatePosition(vNewDisplacement);
        }

        /// <summary>
        /// Update the acceleration data for each segment (mainly to get the gravity vector).
        /// </summary>
        /// <param name="vNewAccelData">new acceleration value.</param>
        public void UpdateSubSegmentGravity(Vector3 vNewAccelData)
        {
            if (SubSegmentGravity.Equals(Vector3.zero))
            {
                SubSegmentGravity = vNewAccelData;
            }
            else
            {
                //Use lowpass filter to extract the gravity vector from cumulative acceleration data
                SubSegmentGravity = Vector3.Lerp(SubSegmentGravity, vNewAccelData, 0.15f);
            }
        }


        /// <summary>
        /// Initializes a new  subsegment structure's internal properties with the desired subsegment Type s
        /// <summary>
        /// <param name="vSubsegmentType">the desired SubSegment Type</param>
        internal void InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes vSubsegmentType)
        {
            GameObject go = new GameObject(vSubsegmentType.GetName());
            AssociatedView = go.AddComponent<BodySubsegmentView>();
            AssociatedView.AssociatedSubSegment = this;
            SubsegmentType = vSubsegmentType;
        }

        /// <summary>
        /// updates the current transform to the passed in parameter
        /// </summary>
        /// <param name="vSubSegmentTransform"></param>
        public void UpdateSubSegmentTransform(Transform vSubSegmentTransform)
        {
            AssociatedView.AssignTransforms(vSubSegmentTransform);
        }

        /// <summary>
        /// Releases resources used by the sub segment
        /// </summary>
        internal void ReleaseResources()
        {
            AssociatedView.Clear();
        }
    }
}
