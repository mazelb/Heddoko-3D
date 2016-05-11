// /**
// * @file CalibrationTrackingStructure.cs
// * @brief Contains the CalibrationTrackingStructure class
// * @author Mohammed Haider(mohammed@heddoko.com)
// * @date May 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using WebSocketSharp;

namespace Assets.Scripts.Body_Data.CalibrationData
{
    /// <summary>
    /// Provides a tracking structure for BodyFrameCalibrationContainer. 
    /// </summary>
    public class CalibrationTrackingStructure
    {
        protected List<Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>> CalibrationFramesList = new List<Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>>();

        /// <summary>
        /// Has the structure been full updated?
        /// </summary>
        protected internal bool Completed { get; set; }


        /// <summary>
        /// Getter to return FramesList
        /// </summary>
        public List<Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>> GetList
        {
            get
            {
                return CalibrationFramesList;
            }
       
    }
        /// <summary>
        /// Clears the structure
        /// </summary>
        protected internal void Clear()
        {
            CalibrationFramesList.Clear();
        }

        /// <summary>
        /// Add an item to the list
        /// </summary>
        /// <param name="vFiltered"></param>
        protected internal void Add(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFiltered)
        {
            CalibrationFramesList.Add(vFiltered);
        }
    }
}