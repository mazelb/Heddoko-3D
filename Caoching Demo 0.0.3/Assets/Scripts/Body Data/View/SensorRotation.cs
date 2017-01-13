// /**
// * @file SensorRotation.cs
// * @brief Contains the SensorRotation class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Security.Policy;
using Assets.Scripts.Utils.DebugContext;
using heddoko;
using HeddokoLib.body_pipeline;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    public class SensorRotation : MonoBehaviour
    {
        public Vector3 AxisOfRotation = new Vector3(0, 180, 0);
        public Quaternion AbsoluteRotation = Quaternion.identity;
        //public Quaternion RelativeRotation = Quaternion.identity;
        public Quaternion InitialRotation = Quaternion.identity;
        public Quaternion UpAxisRotation;

        [SerializeField]
        public Vector3 OrientationCorrection;
        public bool UseCorrection = false;

        void Awake()
        {
            SetAxisOfRotation();

        }

        void SetAxisOfRotation()
        {
            UpAxisRotation = Quaternion.Euler(AxisOfRotation);
            InitialRotation = UpAxisRotation;
        }
        public void UpdateRotatation(Quaternion vNewRot)
        {
            AbsoluteRotation.x = -vNewRot.y;
            AbsoluteRotation.y = vNewRot.z;
            AbsoluteRotation.z = -vNewRot.x;
            AbsoluteRotation.w = vNewRot.w;
            //RelativeRotation = InitialRotation * AbsoluteRotation;
        }

        public void Reset()
        {
            InitialRotation = Quaternion.Inverse(AbsoluteRotation);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
                Reset();
            transform.rotation = InitialRotation * AbsoluteRotation;
        }

        void Calibrate(Packet vFramePacket, int vIndex)
        {
            var vImuFrame = vFramePacket.fullDataFrame.imuDataFrame;
            //get the acceleration. Hopefully the user is not moving at this present time
            Vector3 vAcceleration = new Vector3();
            vAcceleration.x = vImuFrame[vIndex].Mag_x;
            vAcceleration.y= vImuFrame[vIndex].Mag_y;
            vAcceleration.z = vImuFrame[vIndex].Mag_z;
            Reset();


        }

    }
}