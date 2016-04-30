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
    /// A component for Body Sub Segments
    /// </summary>
    public interface IBodySubsegment
    {
        BodyStructureMap.SubSegmentTypes SubSegmentType { get; }
        BodyStructureMap.SubSegmentOrientationType SubsegmentOrientationType { get; }

    }
}