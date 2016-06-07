/** 
* @file IBodySubSegment.cs
* @brief Contains the IBodySubSegment  interface
* @author Mohammed Haider(mohamed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
namespace Assets.Scripts.Body_Data.Interfaces
{
    public interface IBodySubSegment
    {

        object AssociatedView { get; }
        /// <summary>
        /// Resets the orientations of the associated view
        /// </summary>
        void ResetViewTransforms();
  
        void UpdateSubsegmentOrientation(object vNewOrientation, int vApplyLocal = 0, bool vResetRotation = false);
        object GetSubSegmentTransform();
        void UpdateSubsegmentPosition(object vNewDisplacement);

        void InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes vSubsegmentType);

        /// <summary>
        /// updates the current transform to the passed in parameter
        /// </summary>
        /// <param name="vSubSegmentTransform"></param>
        void UpdateSubSegmentTransform(object vSubSegmentTransform);
        /// <summary>
        /// Releases resources used by the sub segment
        /// </summary>
        void ReleaseResources();
    }
}