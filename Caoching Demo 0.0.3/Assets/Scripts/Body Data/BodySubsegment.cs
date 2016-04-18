/** 
* @file BodySubSegment.cs
* @brief Contains the BodySubSegment  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.view;
using UnityEngine; 

/**
* BodySubSegment class 
* @brief BodySubSegment class (represents one abstracted reprensentation of a body subsegment)
*/
[Serializable]
public class BodySubSegment
{
    //TODO: Sub Segment Orientation Type (Raw-Tracked-fused-mapped)
    public BodyStructureMap.SubSegmentTypes subsegmentType;
    public Quaternion SubsegmentOrientation = Quaternion.identity;
    public Vector3 SubSegmentPosition;
    public SubSegmentOrientationType SubsegmentOrientationType;
    public BodySubsegmentView AssociatedView;

    public BodySubSegment()
    {
        
    }
   
    /**
    * SubSegmentOrientationType enum
    * @brief The type of subsegment's orientation
    */
    public enum SubSegmentOrientationType
    {
        Fused = 0,
        NonFused = 1,
        MappedTransformation
    }

    /// <summary>
    /// Resets the orientations of the associated view
    /// </summary>
    public void ResetViewTransforms()
    {
        AssociatedView.ResetTransforms();
    }

    /**
    *  UpdateSubsegmentOrientation(float[,] vOrientationMatrix)
    * @param  vOrientation: the orientation that will update the subsegment's orientation
    * @brief  Updates the subsegments orientation
    */
    public void UpdateSubsegmentOrientation(Quaternion vNewOrientation, int vApplyLocal = 0, bool vResetRotation = false)
    {
        //update the view
        SubsegmentOrientation = vNewOrientation;
        AssociatedView.UpdateOrientation(vNewOrientation, vApplyLocal, vResetRotation);
    }

    public Transform GetSubSegmentTransform()
    {
        return AssociatedView.SubsegmentTransform;
    }


    /**
    * UpdateSubsegmentPosition(Vector3 vNewPosition)
    * @param  vOrientation: the orientation that will update the subsegment's orientation
    * @brief  Updates the subsegments orientation
    */
    public void UpdateSubsegmentPosition(Vector3 vNewDisplacement)
    {
        //update the view
        SubSegmentPosition = vNewDisplacement;
        AssociatedView.UpdatePosition(vNewDisplacement);
    }

    /**
    *  InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes sstype)
    * @param  sstype: the desired SubSegment Type
    * @brief Initializes a new  subsegment structure's internal properties with the desired subsegment Type 
    */
    internal void InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes vSubsegmentType)
    {
        GameObject go = new GameObject(vSubsegmentType.GetName());
        AssociatedView = go.AddComponent<BodySubsegmentView>();
        AssociatedView.AssociatedSubSegment = this;
        subsegmentType = vSubsegmentType;
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
