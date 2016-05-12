// /**
// * @file ISubSegmentModelObserver.cs
// * @brief Contains the ISubSegmentModelObserver interface
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date April 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
namespace Assets.Scripts.Body_Data.Interfaces
{
    /// <summary>
    /// An interface for a SubSegment Model observer
    /// </summary>
    public interface ISubSegmentModelObserver
    {
        /// <summary>
        /// a body subsegment  transform values are updated
        /// </summary>
        /// <param name="vSubsegmentType">The subsegment type that was updated</param>
        /// <param name="vTransformValues">The subsegment type's values</param>
        void SubSegmentUpdated(BodyStructureMap.SubSegmentTypes vSubsegmentType, ref IBodySubsegmentTransformValues vTransformValues);
    }
}