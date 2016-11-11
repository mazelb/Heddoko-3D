// /**
// * @file SensorRotation.cs
// * @brief Contains the SensorRotation class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Security.Policy;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    public class SensorRotation : MonoBehaviour
    {
        public Vector3 AxisOfRotation = new Vector3(0, 180, 0);
        public Quaternion AbsoluteRotation = Quaternion.identity;
        public Quaternion RelativeRotation = Quaternion.identity;
        public Quaternion InitialRotation = Quaternion.identity;
        public Quaternion UpAxisRotation;
        [SerializeField]
        public Vector3 OrientationCorrection;

        void Awake()
        {
            SetAxisOfRotation();
            InputHandler.RegisterKeyboardAction(KeyCode.Home, Reset);
            InputHandler.RegisterKeyboardAction(KeyCode.End, SetAxisOfRotation);
        }

        void SetAxisOfRotation()
        {
            UpAxisRotation = Quaternion.Euler(AxisOfRotation);
            InitialRotation = UpAxisRotation;
        }
        public void UpdateRotatation(Quaternion vNewRot)
        {
            AbsoluteRotation.x = vNewRot.x;
            AbsoluteRotation.y = vNewRot.y;
            AbsoluteRotation.z = vNewRot.z;
            AbsoluteRotation.w = vNewRot.w;
            AbsoluteRotation *= Quaternion.Euler(OrientationCorrection);
            RelativeRotation = InitialRotation * AbsoluteRotation;
        }

        public void Reset()
        {
            InitialRotation = Quaternion.Inverse(AbsoluteRotation);
        }

        void Update()
        {
            transform.rotation = RelativeRotation;

        }

    }
}