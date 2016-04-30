// /**
// * @file UnitySubSegmentTransform.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 04 2016  04
// * Copyright Heddoko(TM) 2016, all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.Body_Data.Interfaces
{
    public struct UnitySubSegmentTransform : IBodySubsegmentTransformValues
    {
        public Quaternion SubsegmentOrientation;
        public Vector3 SubSegmentPosition;
 
    }
}