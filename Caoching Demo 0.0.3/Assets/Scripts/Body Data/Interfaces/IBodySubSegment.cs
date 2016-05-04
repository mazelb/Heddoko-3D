// /**
// * @file IBodySubsegment.cs
// * @brief Contains the IBodySegment interface
// * @author Mohammed Haider(mohammed@heddoko.com)
// * @date April 2016
// * Copyright Heddoko(TM) 2016, all rights reserved
// */
namespace Assets.Scripts.Body_Data.Interfaces
{
    /// <summary>
    /// A component for Body SubSegments
    /// </summary>
    public interface IBodySubsegment
    {
        BodyStructureMap.SubSegmentTypes SubSegmentType { get; }
        BodyStructureMap.SubSegmentOrientationType SubsegmentOrientationType { get; }

        /// <summary>
        /// The current subsegment transforms values
        /// </summary> 
        IBodySubsegmentTransformValues SubsegmentTransformValues { get; }

        /// <summary>
        /// Reset's the Subsegment's transform values 
        /// </summary>
        void Reset();

        /// <summary>
        ///  Updates the Subsegment's orientation with respect to its quaternion values
        /// </summary>
        /// <param name="vX">X component of the quaternion</param>
        /// <param name="vY">Y component of the quaternion</param>
        /// <param name="vZ">Z component of the quaternion</param>
        /// <param name="vW">W component of the quaternion</param>
        /// <param name="vApplyLocal">apply the rotation on a local level or global level?</param>
        /// <param name="vResetRotation">reset the rotation?</param>
        void UpdateSubsegmentOrientation(float vX, float vY, float vZ, float vW, int vApplyLocal = 0,
            bool vResetRotation = false);

        
 
    }
}