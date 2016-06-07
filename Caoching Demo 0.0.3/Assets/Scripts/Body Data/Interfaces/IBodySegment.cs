/** 
* @file IBodySegment.cs
* @brief Contains the IBodySegment interface
* @author Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.Interfaces
{
    /// <summary>
    /// Provides an interface for a Body Segment component. 
    /// </summary>
    public interface IBodySegment
    {
        IDictionary<BodyStructureMap.SubSegmentTypes, IBodySubSegment> SubSegmentCollection { get; }

    }
}