// /**
// * @file U3DQuaternion.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date May 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.Utils.HMath.Structure
{
    public class U3DQuaternion : HQuaternion
    {
        // ReSharper disable once InconsistentNaming
        internal Quaternion mQuaternion;

        public override float X
        {
            get { return mQuaternion.x; }
            set { mQuaternion.x = value; }
        }

        public override float Y
        {
            get { return mQuaternion.y; }
            set { mQuaternion.y = value; }
        }

        public override float Z
        {
            get { return mQuaternion.z; }
            set { mQuaternion.z = value; }
        }

        public override float W
        {
            get { return mQuaternion.w; }
            set { mQuaternion.w = value; }
        }



        /// <summary>
        /// Default constructor: identity quaternion
        /// </summary>
        public U3DQuaternion()
        {
            mQuaternion = Quaternion.identity;

        }

        public U3DQuaternion(Quaternion vQuat)
        {
            mQuaternion = vQuat;

        }
        public U3DQuaternion(float vX, float vY, float vZ, float vW) : base(vX, vY, vZ, vW)
        {
        }

        public override void ToAngleAxis(out float vAngle, out HVector3 vAxis)
        {
            vAxis = new U3DVector3(0, 0, 0);
            mQuaternion.ToAngleAxis(out vAngle, out ((U3DVector3)vAxis).mVector3);
        }

        public override void SetFromToRotation(HVector3 vFromDirection, HVector3 vToDirection)
        { 
            mQuaternion.SetFromToRotation(  ((U3DVector3)vFromDirection).mVector3, ((U3DVector3)vToDirection).mVector3);
        }

        public override void SetLookRotation(HVector3 vForward)
        {
            mQuaternion.SetLookRotation(((U3DVector3)vForward).mVector3 );

        }

        public override void SetLookRotation(HVector3 vForward, HVector3 vUp)
        {
            mQuaternion.SetLookRotation(((U3DVector3)vForward).mVector3, ((U3DVector3)vUp).mVector3); 
        }

    

       
    }
}