// /**
// * @file UVector3MathServiceProvider.cs
// * @brief Contains the UVector3MathServiceProvider
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date May 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.Utils.HMath.Service_Provider
{
    /// <summary>
    /// A Vector3 math service provider for U3DVector3s
    /// </summary>
    public class UVector3MathServiceProvider : IVector3MathServiceProvider
    {
        public HVector3 Zero
        {
            get
            {
                return new U3DVector3(Vector3.zero);
            }
        }

        public HVector3 One
        {
            get
            {
                return new U3DVector3(Vector3.one);
            }
        }

        public HVector3 Forward
        {
            get
            {
                return new U3DVector3(Vector3.forward);
            }
        }
        public HVector3 Back
        {
            get
            {
                return new U3DVector3(Vector3.back);
            }
        }
        public HVector3 Up
        {
            get
            {
                return new U3DVector3(Vector3.up);
            }
        }
        public HVector3 Down
        {
            get
            {
                return new U3DVector3(Vector3.down);
            }
        }
        public HVector3 Left
        {
            get
            {
                return new U3DVector3(Vector3.left);
            }
        }
        public HVector3 Right
        {
            get
            {
                return new U3DVector3(Vector3.right);
            }
        }
        /// <summary>
        /// Linearly interpolates between two vectors. Note that this perform a direct cast
        /// on vFrom and vTo and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param>
        /// <param name="vT"></param>
        /// <returns></returns>
        public HVector3 Lerp(HVector3 vFrom, HVector3 vTo, float vT)
        {
            return new U3DVector3(Vector3.Lerp(((U3DVector3)vFrom).Instance, ((U3DVector3)vTo).Instance, vT));
        }
        /// <summary>
        /// returns the dot product of two vectors. Note that this perform a direct cast
        /// on vFrom and vTo and will fail if they aren't of type U3DVector3
        /// </summary>
        public float Dot(HVector3 vLhs, HVector3 vRhs)
        {
            return Vector3.Dot(((U3DVector3)vLhs).Instance, ((U3DVector3)vRhs).Instance);
        }

        /// <summary>
        /// Project one vector onto another.Note that this perform a direct cast
        /// on vFrom and vTo and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vVector"></param>
        /// <param name="vOnNormal"></param>
        /// <returns></returns>
        public HVector3 Project(HVector3 vVector, HVector3 vOnNormal)
        {
            return new U3DVector3(Vector3.Project(((U3DVector3)vVector).Instance, ((U3DVector3)vOnNormal).Instance));
        }

        /// <summary>
        /// Get a projection of a vector onto a plane normal.Note that this perform a direct cast
        /// on vFrom and vTo and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vVector3"></param>
        /// <param name="vPlaneNormal"></param>
        /// <returns></returns>
        public HVector3 ProjectOnPlane(HVector3 vVector3, HVector3 vPlaneNormal)
        {
            return new U3DVector3(Vector3.ProjectOnPlane(((U3DVector3)vVector3).Instance, ((U3DVector3)vPlaneNormal).Instance));
        }


        /// <summary>
        /// Get the positive acute angle between two planes.Note that this perform a direct cast
        /// on vFrom and vTo and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param>
        /// <returns></returns>
        public float Angle(HVector3 vFrom, HVector3 vTo)
        {
            return Vector3.Angle(((U3DVector3)vFrom).Instance, ((U3DVector3)vTo).Instance);

        }

        /// <summary>
        /// Get the distance between two vectors. Note that this perform a direct cast
        /// on vFrom and vTo and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vA"></param>
        /// <param name="vB"></param>
        /// <returns></returns>
        public float Distance(HVector3 vA, HVector3 vB)
        {
            return Vector3.Distance(((U3DVector3)vA).Instance, ((U3DVector3)vB).Instance);
        }
        /// <summary>
        /// Returns a copy of the vector with its magnitude clamped to the passed in Max Length.
        /// Note that this perform a direct cast
        /// on vFrom and vTo and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vVector">Vector to clamp</param>
        /// <param name="vMaxLength">max length to clamp to </param>
        /// <returns>Vector with a clamped magnitude</returns>
        public HVector3 ClampMagnitude(HVector3 vVector, float vMaxLength)
        {
            return new U3DVector3(Vector3.ClampMagnitude(((U3DVector3)vVector).Instance, vMaxLength));
        }


        /// <summary>
        /// Get the Magnitude of the passed in Vector
        /// Note that this perform a direct cast
        /// on a and will fail if they isn't of type U3DVector3
        /// </summary>
        /// <param name="vA">the passed in vector</param>
        /// <returns>it's corresponding magnitude</returns>
        public float Magnitude(HVector3 vA)
        {
            return Vector3.Magnitude(((U3DVector3)vA).Instance);
        }

        /// <summary>
        /// Get the Square Magnitude of the passed in Vector
        /// Note that this perform a direct cast
        /// on a and will fail if they isn't of type U3DVector3
        /// </summary>
        /// <param name="vA">the vector</param>
        /// <returns>the vector's square magnitude</returns>
        public float SqrMagnitude(HVector3 vA)
        {
            return Vector3.SqrMagnitude(((U3DVector3)vA).Instance);
        }

        /// <summary>
        /// Returns a vector that is composed of the smallest components of both passed in vectors
        /// Note that this perform a direct cast
        /// on vLhs and vRhs and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vLhs">Vector to clamp</param>
        /// <param name="vRhs">max length to clamp to </param>
        /// <returns>Vector with a clamped magnitude</returns>
        public HVector3 Min(HVector3 vLhs, HVector3 vRhs)
        {
            return new U3DVector3(Vector3.Min(((U3DVector3)vLhs).Instance, ((U3DVector3)vRhs).Instance));

        }
        /// <summary>
        /// Returns a vector that is composed of the largest components of the passed in vectors. 
        /// Note that this perform a direct cast
        /// on vLhs and vRhs and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vLhs">Vector to clamp</param>
        /// <param name="vRhs">max length to clamp to </param>
        /// <returns>Vector with a clamped magnitude</returns>
        public HVector3 Max(HVector3 vLhs, HVector3 vRhs)
        {
            return new U3DVector3(Vector3.Max(((U3DVector3)vLhs).Instance, ((U3DVector3)vRhs).Instance));
        }

        /// <summary>
        /// Makes the passed in vectors normalized and orthogonal to each other.
        // Note that this perform a direct cast
        /// on vNormal and vTangent and vBinormal and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vNormal">Will be normalized</param>
        /// <param name="vTangent">Will be orthogonal to vNormal and normalized</param>
        /// <param name="vBinormal">will normalize the binormal and will be orthogonal is vTangent and vNormal</param>
        public void OrthoNormalize(ref HVector3 vNormal, ref HVector3 vTangent, ref HVector3 vBinormal)
        {
            Vector3.OrthoNormalize(ref ((U3DVector3)vNormal).Instance, ref ((U3DVector3)vTangent).Instance, ref ((U3DVector3)vBinormal).Instance);
        }

        /// <summary>
        /// Makes the passed in vectors normalized and orthogonal to each other.
        /// Note that this perform a direct cast
        /// on vNormal and vTangent  and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vNormal">Will be normalized</param>
        /// <param name="vTangent">Will be orthogonal to vNormal and normalized</param> 
        public void OrthoNormalize(ref HVector3 vNormal, ref HVector3 vTangent)
        {
            Vector3.OrthoNormalize(ref ((U3DVector3)vNormal).Instance, ref ((U3DVector3)vTangent).Instance);
        }

        /// <summary>
        /// Speherically interpolate between two vectors.
        /// Note that this perform a direct cast
        /// on vFrom and vTo  and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param>
        /// <param name="vT"></param>
        /// <returns></returns>
        public HVector3 Slerp(HVector3 vFrom, HVector3 vTo, float vT)
        {
            return new U3DVector3(Vector3.Slerp(((U3DVector3)vFrom).Instance, ((U3DVector3)vTo).Instance, vT));
        }
        /// <summary>
        /// Moves a point current in a straight line towards a vTarget point.
        /// Note that this perform a direct cast
        /// on vCurrentVector and vTargetVector  and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vCurrentVector">the current vector</param>
        /// <param name="vTargetVector">the vTarget vector</param>
        /// <param name="vMaxDistanceDelta">the delta distance between the two vTarget vectors</param>
        public HVector3 MoveTowards(HVector3 vCurrentVector, HVector3 vTargetVector, float vMaxDistanceDelta)
        {
            return new U3DVector3(Vector3.MoveTowards(((U3DVector3)vCurrentVector).Instance, ((U3DVector3)vTargetVector).Instance, vMaxDistanceDelta));
        }

        /// <summary>
        /// Rotates a vector towards its current vTarget by an angle of max radians delta. If a negative value is provided from maxMagnitude delta, then the vector will rotate away i
        ///  the vector will rotate away from vTarget/ until it is pointing in exactly the opposite direction, then stop.
        /// Note that this perform a direct cast
        /// on vCurrentVector and vTargetVector  and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vCurrentVector"></param>
        /// <param name="vTarget"></param>
        /// <param name="vMaxRads"></param>
        /// <param name="vMaxMagDelta"></param>
        public HVector3 RotateTowards(HVector3 vCurrentVector, HVector3 vTarget, float vMaxRads, float vMaxMagDelta)
        {
            return new U3DVector3(Vector3.RotateTowards(((U3DVector3)vCurrentVector).Instance, ((U3DVector3)vTarget).Instance, vMaxRads, vMaxMagDelta));

        }

        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.   
        /// Note that this perform a direct cast
        /// on vCurrent,vTarget and vCurrVelocity  and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vCurrent">the vector</param>
        /// <param name="vTarget">the vector's target</param>
        /// <param name="vdeltaTime">delta time</param>
        /// <param name="vCurrVelocity">the vector's current velocity</param>
        /// <param name="vSmoothTime">smooth time</param>
        /// <param name="vMaxSpeed">the max speed</param>
        public HVector3 SmoothDamp(HVector3 vCurrent, HVector3 vTarget, float vdeltaTime, ref HVector3 vCurrVelocity, float vSmoothTime,
            float vMaxSpeed)
        {
            return new U3DVector3(Vector3.SmoothDamp(((U3DVector3)vCurrent).Instance, ((U3DVector3)vTarget).Instance, ref ((U3DVector3)vCurrVelocity).Instance,
             vSmoothTime, vMaxSpeed, vdeltaTime));
        }

        /// <summary>
        /// Get the cross product of two vectors
        /// Note that this perform a direct cast
        /// on vLhs,vRhs and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vLhs"></param>
        /// <param name="vRhs"></param>
        /// <returns></returns>
        public HVector3 Cross(HVector3 vLhs, HVector3 vRhs)
        {
            return new U3DVector3(Vector3.Cross(((U3DVector3)vLhs).Instance, ((U3DVector3)vRhs).Instance));
        }
        /// <summary>
        /// Normalizes this vector
        /// Note that this perform a direct cast
        /// on vVector  and will fail if they aren't of type U3DVector3
        /// </summary>
        /// <param name="vVector"></param>
        public void Normalize(HVector3 vVector)
        {
            ((U3DVector3)vVector).Normalize();
        }
        

        /// <summary>
        /// Every component in the result is a component of vA multiplied by the same component of vB
        /// Note that this perform a direct cast
        /// on vA and vB  and will fail if they aren't of type U3DVector3
        /// </summary> 
        public HVector3 Scale(HVector3 vA, HVector3 vB)
        {
            return new U3DVector3(Vector3.Scale(((U3DVector3)vA).Instance, ((U3DVector3)vB).Instance));

        }

        /// <summary>
        /// Reflects a vector off a plane defined the by the passed in normal.
        /// Note that this perform a direct cast
        /// on inDirection and inNormal  and will fail if they aren't of type U3DVector3
        /// </summary> 
        public HVector3 Reflect(HVector3 inDirection, HVector3 inNormal)
        {
            return new U3DVector3(Vector3.Reflect(((U3DVector3)inDirection).Instance, ((U3DVector3)inNormal).Instance));
        }


    }
}