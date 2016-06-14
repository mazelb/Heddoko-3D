 /**
* @file ImuFrame.cs
 * @brief Contains the ImuFrame class
 * @author Mohammed Haider( mohammed@heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */

using System;

namespace HeddokoLib.body_pipeline
{
    public class ImuFrame
    {
        public byte ImuId = 0;
        public float QuaternionX;
        public float QuaternionY;
        public float QuaternionZ;
        public float QuaternionW;
        public uint MagneticX;
        public UInt16 MagneticY;
        public UInt16 MagneticZ;
        public UInt16 AccelerationX;
        public UInt16 AccelerationY;
        public UInt16 AccelerationZ;
        public UInt16 RotationX;
        public UInt16 RotationY;
        public UInt16 RotationZ;
    }
}