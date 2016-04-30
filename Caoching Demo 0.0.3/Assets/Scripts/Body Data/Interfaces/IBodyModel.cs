/** 
* @file IBodyModel.cs
* @brief Contains the IBodyModel interface
* @author Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.Interfaces
{
    /// <summary>
    /// An interface to support Body models
    /// </summary>
    public interface IBodyModel
    {
        /// <summary>
        /// A collection of Body Segments
        /// </summary>
        IDictionary<BodyStructureMap.SubSegmentTypes, IBodySegment> BodySegmentCollection { get; }
        
    }
}