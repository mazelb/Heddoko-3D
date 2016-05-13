// /**
// * @file UnitySubSegmentTransform.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date April 2016   
// * Copyright Heddoko(TM) 2016, all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.Body_Data.Interfaces
{
    public class UnitySubSegmentTransform : IBodySubsegmentTransformValues
    {
        public Quaternion SubsegmentOrientation;
        public Vector3 SubSegmentPosition;
 
    }
}