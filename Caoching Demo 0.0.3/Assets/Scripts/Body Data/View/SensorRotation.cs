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
        public Quaternion GravityOffset = Quaternion.identity;

        public Vector3 UpVector = Vector3.up;
        public Vector3 RightVector = Vector3.right;
        public Vector3 ForwardVector = Vector3.forward;

        public Vector3 LegUpVector = new Vector3(0, 0, 1);
        public Vector3 LegRightVector = new Vector3(1, 0, 0);
        public Vector3 LegForwardVector = new Vector3(0, 1, 0);

        public Vector3 RightArmUpVector = new Vector3(0, 0, 1);
        public Vector3 RightArmRightVector = new Vector3(0, 1, 0);
        public Vector3 RightArmForwardVector = new Vector3(1, 0, 0);

        public Vector3 LeftArmUpVector = new Vector3(0, 0, 1);
        public Vector3 LeftArmRightVector = new Vector3(0, -1, 0);
        public Vector3 LeftArmForwardVector = new Vector3(-1, 0, 0);

        public Vector3 CurAccelVector = Vector3.zero;
        public Vector3 CurGyroVector = Vector3.zero;

        private bool mCalState = false;
        private bool mMagState = false;

        public int CurId = -1;

        [SerializeField]
        public Vector3 OrientationCorrection;
        public bool UseCorrection = false;
        public bool IsReset = false;

        void Awake()
        {
            IsReset = false;
            SetAxisOfRotation();
        }

        void SetAxisOfRotation()
        {
            UpAxisRotation = Quaternion.Euler(AxisOfRotation);
            InitialRotation = UpAxisRotation;
        }

        public void UpdateAccel(Vector3 vNewAccel)
        {
            //Lowpass filter fro acceleration to smooth it (in static case
            CurAccelVector = Vector3.Lerp(CurAccelVector, vNewAccel, 0.15f);
        }

        public void UpdateGyro(Vector3 vNewGyro)
        {
            CurGyroVector = vNewGyro;// Vector3.Lerp(CurGyroVector, vNewGyro, 0.85f);
        }

        public void UpdateRotatation(Quaternion vNewRot)
        {
            AbsoluteRotation.x = -vNewRot.y;
            AbsoluteRotation.y = vNewRot.z;
            AbsoluteRotation.z = -vNewRot.x;
            AbsoluteRotation.w = vNewRot.w;

            if (!IsReset)
            {
                Reset();
            }
        }

        /// <summary>
        /// Computes an offset from a given input quaternion. 
        /// </summary>
        /// <param name="vInputQuaternion">the input quaternion to extract an offset from</param>
        /// <returns>The offset rotation</returns>
        private static Quaternion ComputeTorsoOffsetFromGlobalDown(Quaternion vInputQuaternion)
        {
            //get the global down vector3 with respect to the input quaternion
            Vector3 vRelativeDown = vInputQuaternion * Vector3.down;
            //compute the angle between the two
            float vAngle = Vector3.Angle(Vector3.down, vRelativeDown);
            //get the cross between the two, giving us an orthonormal axis from which we can create a quaternion from
            Vector3 vOrthoNormalCross = Vector3.Cross(Vector3.down, vRelativeDown).normalized;
            //create a quaternion with the cross product as the axis
            var vOffset = Quaternion.AngleAxis(vAngle, vOrthoNormalCross);
            return vOffset;
        }



        void Q2HPR(Quaternion vQ, ref Vector3 ypr)
        {
            ypr[0] = Mathf.Rad2Deg * Mathf.Atan2((2 * vQ[0] * vQ[1] + 2 * vQ[3] * vQ[2]), (2 * vQ[3] * vQ[3] + 2 * vQ[0] * vQ[0] - 1));
            ypr[1] = Mathf.Rad2Deg * Mathf.Asin(-(2 * vQ[0] * vQ[2] - 2 * vQ[3] * vQ[1]));
            ypr[2] = Mathf.Rad2Deg * Mathf.Atan2((2 * vQ[1] * vQ[2] + 2 * vQ[3] * vQ[0]), (2 * vQ[3] * vQ[3] + 2 * vQ[2] * vQ[2] - 1));
            if (ypr[0] < 0)
                ypr[0] += 360;
        }

        public void Reset()
        {
            IsReset = true;

            Quaternion vExpectedRotation = Quaternion.identity;
            Vector3 vExpectedGravity = Vector3.down;
            Vector3 vCurGravity = vExpectedGravity;

            if (CurId == 5 || CurId == 7 || CurId == 6 || CurId == 8)
            {
                vExpectedRotation = Quaternion.LookRotation(LegForwardVector, LegUpVector);
            }
            else if (CurId == 1 || CurId == 2)
            {
                vExpectedRotation = Quaternion.LookRotation(RightArmForwardVector, RightArmUpVector);
            }
            else if (CurId == 3 || CurId == 4)
            {
                vExpectedRotation = Quaternion.LookRotation(LeftArmForwardVector, LeftArmUpVector);
            }
            else if (CurId == 0)
            {
                vExpectedRotation = Quaternion.LookRotation(-LegForwardVector, LegUpVector);
            }

            vExpectedGravity = vExpectedRotation * Vector3.down;
            vCurGravity = Quaternion.Inverse(vExpectedRotation) * CurAccelVector;
            GravityOffset = Quaternion.FromToRotation(vExpectedGravity, vCurGravity);
            InitialRotation = AbsoluteRotation * GravityOffset;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
                Reset();

            if (IsReset)
            {
                Quaternion vCurRotation = Quaternion.identity;
                Quaternion vExpectedRotation = Quaternion.identity;
                Quaternion vGravityOffset = Quaternion.identity;

                if (CurId == 5 || CurId == 7 || CurId == 6 || CurId == 8)
                {
                    vExpectedRotation = Quaternion.LookRotation(LegForwardVector, LegUpVector);
                }
                else if (CurId == 1 || CurId == 2)
                {
                    vExpectedRotation = Quaternion.LookRotation(RightArmForwardVector, RightArmUpVector);
                }
                else if (CurId == 3 || CurId == 4)
                {
                    vExpectedRotation = Quaternion.LookRotation(LeftArmForwardVector, LeftArmUpVector);
                }
                else if (CurId == 0)
                {
                    vExpectedRotation = Quaternion.LookRotation(-LegForwardVector, LegUpVector);
                }

                vCurRotation = Quaternion.Inverse(vExpectedRotation) * Quaternion.Inverse(InitialRotation) * (AbsoluteRotation * GravityOffset);
                Quaternion vNewRotation = Quaternion.Slerp(transform.rotation, vCurRotation, 0.3f);
                transform.rotation = vNewRotation;
            }
        }
    }
}