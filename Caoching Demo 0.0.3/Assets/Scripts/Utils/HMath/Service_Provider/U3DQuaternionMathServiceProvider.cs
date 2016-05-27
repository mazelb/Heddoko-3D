// /**
// * @file U3DQuaternionMathServiceProvider.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 05 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using Assets.Scripts.Utils.HMath.Structure;
using HeddokoLib.genericPatterns;
using UnityEngine;

namespace Assets.Scripts.Utils.HMath.Service_Provider
{
    /// <summary>
    /// A math service provider for components within the unity3d environment.
    /// <remarks>Note that arguments that use HQuaternions and HVector3s will be casted as U3DQuaternion and HVector3 respectivily. 
    /// Consider appropriate measures if you are using this service provider with a non U3DQuaternion</remarks> 
    /// </summary>
    public class U3DQuaternionMathServiceProvider : IQuaternionMathServiceProvider
    {

        public U3DQuaternionMathServiceProvider()
        {

        }


        public HQuaternion Identity
        {
            get
            {
                return new U3DQuaternion(); ;
            }
        }

        /// <summary>
        /// Get the Dot product of two quaternions
        /// <remarks>Note that these two HQuaternions will be cast as U3DQuaternions, consider appropriate measures 
        /// if you are using this service provider with a non U3DQuaternion</remarks>
        /// </summary>
        /// <param name="vHQuaternion">LHS quaternion</param>
        /// <param name="vHQuaternion1">RHS quaternion</param>
        /// <returns>The dot product</returns>
        public float Dot(HQuaternion vHQuaternion, HQuaternion vHQuaternion1)
        {
            U3DQuaternion vLhs = (U3DQuaternion)vHQuaternion;
            U3DQuaternion vRhs = (U3DQuaternion)vHQuaternion1;
            return Quaternion.Dot(vLhs.mQuaternion, vRhs.mQuaternion);
        }


        /// <summary>
        ///  Creates a rotation which rotates vAngle degrees around the vAxis
        /// </summary>
        /// <param name="vAngle"></param>
        /// <param name="vAxis"></param>
        /// <returns></returns>
        public HQuaternion AngleAxis(float vAngle, HVector3 vAxis)
        {
            U3DVector3 vV3 = (U3DVector3)vAxis;
            var vQuat = Quaternion.AngleAxis(vAngle, vV3.mVector3);
            return new U3DQuaternion(vQuat);
        }


        /// <summary>
        /// Creates a rotation that rotates from vFromDirection to vToDirection 
        /// </summary> 
        /// <param name="vFromDirection"></param>
        /// <param name="vToDirection"></param>
        /// <returns></returns>
        public HQuaternion FromToRotation(HVector3 vFromDirection, HVector3 vToDirection)
        {
            Vector3 v1 = ((U3DVector3)vFromDirection).mVector3;
            Vector3 v2 = ((U3DVector3)vToDirection).mVector3;
            Quaternion vQuat = Quaternion.FromToRotation(v1, v2);
            return new U3DQuaternion(vQuat);
        }
        /// <summary>
        ///  Create a look rotation with the specified vForward and vUpwards 
        /// </summary>  
        /// <returns></returns>
        public HQuaternion LookRotation(HVector3 vForward, HVector3 vUpwards)
        {
            Vector3 v1 = ((U3DVector3)vForward).mVector3;
            Vector3 v2 = ((U3DVector3)vUpwards).mVector3;
            Quaternion vQuat = Quaternion.LookRotation(v1, v2);
            return new U3DQuaternion(vQuat);
        }

        /// <summary>
        /// Spherically interpolates between vFrom  and vTo with respect to step vT
        /// </summary>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param> 
        /// <param name="vF"></param>
        public HQuaternion Slerp(HQuaternion vFrom, HQuaternion vTo, float vF)
        {
            U3DQuaternion vQ1 = ((U3DQuaternion)vFrom);
            U3DQuaternion vQ2 = ((U3DQuaternion)vTo);
            Quaternion vQuat = Quaternion.Slerp(vQ1.mQuaternion, vQ2.mQuaternion, vF);
            return new U3DQuaternion(vQuat);
        }
        /// <summary>
        /// Linearly interpolates between vFrom  and vTo with respect to step vT
        /// </summary>
        /// <param name="vFrom">Start Quaternion</param>
        /// <param name="vTo">End Quaternion</param>
        /// <param name="vF">Step</param>
        /// <returns>new Quaternion</returns>
        public HQuaternion Lerp(HQuaternion vFrom, HQuaternion vTo, float vF)
        {
            U3DQuaternion vQ1 = ((U3DQuaternion)vFrom);
            U3DQuaternion vQ2 = ((U3DQuaternion)vTo);
            Quaternion vQuat = Quaternion.Lerp(vQ1.mQuaternion, vQ2.mQuaternion, vF);
            return new U3DQuaternion(vQuat);
        }

        /// <summary>
        /// Rotates vfrom rotation towards vTo by an angular step of maxDegrees delta. 
        /// <remarks>Negative values of <code>vMaxDegreesDelta</code>  will move away from <code>vTo</code> until the rotation is exacly in the opposite direction</remarks>

        /// </summary>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param>
        /// <param name="vMaxDegreesDelta"></param>
        /// <returns></returns>
        public HQuaternion RotateTowards(HQuaternion vFrom, HQuaternion vTo, float vMaxDegreesDelta)
        {
            U3DQuaternion vQ1 = ((U3DQuaternion)vFrom);
            U3DQuaternion vQ2 = ((U3DQuaternion)vTo);
            Quaternion vQuat = Quaternion.RotateTowards(vQ1.mQuaternion, vQ2.mQuaternion, vMaxDegreesDelta);
            return new U3DQuaternion(vQuat);
        }

        /// <summary>
        /// Returns the inverse of vRotation
        /// </summary>
        /// <param name="vRotation"></param>
        /// <returns></returns>
        public HQuaternion Inverse(HQuaternion vRotation)
        {
            U3DQuaternion vQ1 = ((U3DQuaternion)vRotation);
            Quaternion vQuat = Quaternion.Inverse(vQ1.mQuaternion);
            return new U3DQuaternion(vQuat);
        }


        /// <summary>
        /// Retuns the angles in <b>degrees</b> between two rotation <code>vA</code> and <code>vB</code>
        /// </summary>
        /// <param name="vHQuaternion"></param>
        /// <param name="vHQuaternion1"></param>
        /// <returns></returns>
        public float Angle(HQuaternion vHQuaternion, HQuaternion vHQuaternion1)
        {
            U3DQuaternion vQ1 = ((U3DQuaternion)vHQuaternion);
            U3DQuaternion vQ2 = ((U3DQuaternion)vHQuaternion1);
            return Quaternion.Angle(vQ1.mQuaternion, vQ2.mQuaternion); 
        }

        /// <summary>
        /// Returns a rotation that rotates around the x,y,z axis. 
        /// <remarks>The rotation starts with the Z component, X, and Y in that respective order</remarks>
        /// </summary>
        /// <returns></returns>
        public HQuaternion Euler(float vX, float vY, float vZ)
        {
            Quaternion vQuat = Quaternion.Euler(vX, vY, vZ);
            return new U3DQuaternion(vQuat);
        }
        /// <summary>
        /// Returns a rotation that rotates around the x,y,z axis from the given HVector3. 
        /// <remarks>The rotation starts with the Z component, X, and Y in that respective order</remarks>
        /// </summary>
        /// <returns></returns>
        public HQuaternion Euler(HVector3 vEuler)
        {
            return Euler(vEuler.X, vEuler.Y, vEuler.Z);
        }

        /// <summary>
        /// Multiplication operator
        /// </summary>
        /// <remarks>Rotating <code>vLhs * vRhs</code> is the same as applying two rotations in sequence relative to the refernece frame resulting from <code>vLhs</code> rotation. Rotations are not commutative.</remarks>
        /// <param name="vLhs">Left hand side HQuaternion</param>
        /// <param name="vRhs">Right hand side HQuaternion</param>
        /// <returns></returns>
        public HQuaternion Multiply(HQuaternion vLhs, HQuaternion vRhs)
        {
            U3DQuaternion vQ1 = ((U3DQuaternion)vLhs);
            U3DQuaternion vQ2 = ((U3DQuaternion)vRhs);
            return new U3DQuaternion(vQ1.mQuaternion * vQ2.mQuaternion);
        }

        /// <summary>
        /// Rotates the <code>vPoint</code> with <code>vRotation</code>
        /// </summary> 
        /// <returns></returns>
        public HVector3 Multiply(HQuaternion vRotation, HVector3 vPoint)
        {
            U3DQuaternion vQ1 = ((U3DQuaternion)vRotation);
            U3DVector3 vVector3 = ((U3DVector3)vPoint);
            return new U3DVector3(vQ1.mQuaternion * vVector3.mVector3);
        }
    }
}