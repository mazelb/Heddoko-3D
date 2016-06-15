/**
* @file BodySegmentQuaternionFlagSetter.cs
* @brief Contains the BodySegmentQuaternionFlagSetter
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
    public class BodySegmentQuaternionFlagSetter:MonoBehaviour
    {
        public Toggle Toggle;

        void Awake()
        {
            Toggle.onValueChanged.AddListener(  SetBodySegmentFlag);
        }

        /// <summary>
        /// Sets the BodySegment's using Quaternion flag to vX
        /// </summary>
        /// <param name="vX"></param>
        void SetBodySegmentFlag(bool vX)
        {
            BodySegment.GBodyFrameUsingQuaternion = vX;
        }
    }
}