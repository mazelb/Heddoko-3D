/** 
* @file HVector3.cs
* @brief Contains the HVector3  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Utils.HMath.Service_Provider;
// ReSharper disable VirtualMemberCallInContructor

namespace Assets.Scripts.Utils.HMath.Structure
{
    /// <summary>
    /// A HVector3 class holding X,Y,z components of type floats
    /// </summary>
    public abstract class HVector3
    {
        public const float KEpsilon = 1E-05f;
        public abstract float x { get; set; }
        public abstract float y { get; set; }
        public abstract float z { get; set; }

        internal static IVector3MathServiceProvider Vector3MathServiceProvider { get; set; }

        /// <summary>
        /// Operator overload.
        /// Note: will throw an IndexOutOfRangeException if index doesn't fall between 0-2
        /// </summary>
        /// <param name="vIndex"> the index of the HVector3 Component. 
        /// </param>
        /// <returns></returns>
        public float this[int vIndex]
        {
            get
            {
                switch (vIndex)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid HVector3 index!");
                }
            }
            set
            {
                switch (vIndex)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid HVector3 index!");
                }
            }
        }



        public static float Magnitude(HVector3 vVector3)
        {
            return Vector3MathServiceProvider.Magnitude(vVector3);
        }


        public static float SqrMagnitude(HVector3 vVector3)
        {
            return Vector3MathServiceProvider.SqrMagnitude(vVector3);
        }


        public HVector3(float vX, float vY, float vZ)
        {
            x = vX;
            y = vY;
            z = vZ;
        }
        public HVector3(float vX, float vY)
        {
            x = vX;
            y = vY;
            z = 0f;
        }

        public static HVector3 Lerp(HVector3 vFrom, HVector3 vTo, float vT)
        {
            return Vector3MathServiceProvider.Lerp(vFrom, vTo, vT);
        }
        public static HVector3 Slerp(HVector3 vFrom, HVector3 vTo, float vT)
        {
            return Vector3MathServiceProvider.Slerp(vFrom, vTo, vT);
        }

        public static void OrthoNormalize(HVector3 vNormal, HVector3 vTangent)
        {
            Vector3MathServiceProvider.OrthoNormalize(vNormal, vTangent);
        }


        public static void OrthoNormalize(HVector3 vNormal, HVector3 vTangent, HVector3 vBinormal)
        {
            Vector3MathServiceProvider.OrthoNormalize(vNormal, vTangent, vBinormal);

        }


        public static HVector3 MoveTowards(HVector3 vCurrent, HVector3 vTarget, float vMaxDistanceDelta)
        {
            return Vector3MathServiceProvider.MoveTowards(vCurrent, vTarget, vMaxDistanceDelta);
        }

        public static HVector3 RotateTowards(HVector3 vCurrent, HVector3 vTarget, float vMaxRadiansDelta,
            float vMaxMagnitudeDelta)
        {
            return Vector3MathServiceProvider.RotateTowards(vCurrent, vTarget, vMaxRadiansDelta, vMaxMagnitudeDelta);

        }

        public static HVector3 SmoothDamp(HVector3 vCurrent, HVector3 vTarget, ref HVector3 vCurrentVelocity,
            float vSmoothTime, float vDeltaTime, float vMaxSpeed = float.PositiveInfinity)
        {
            return Vector3MathServiceProvider.SmoothDamp(vCurrent, vTarget, vDeltaTime, ref vCurrentVelocity, vSmoothTime, vMaxSpeed);

        }
        /// <summary>
        /// Set the new passed in values
        /// </summary>
        /// <param name="vNewX"></param>
        /// <param name="vNewY"></param>
        /// <param name="vNewZ"></param>
        public void Set(float vNewX, float vNewY, float vNewZ)
        {
            x = vNewX;
            y = vNewY;
            z = vNewZ;
        }
        /// <summary>
        /// Every component in the result is a component of vA multiplied by the same component of vB
        /// </summary>
        public static HVector3 Scale(HVector3 vA, HVector3 vB)
        {
            return Vector3MathServiceProvider.Scale(vA, vB);
        }
        public void Scale(HVector3 vScale)
        {
            x *= vScale.x;
            y *= vScale.y;
            z *= vScale.z;
        }

        public abstract void Cross(HVector3 vRhs);

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
        }

        public override bool Equals(object vOther)
        {
            if (!(vOther is HVector3))
            {
                return false;
            }
            HVector3 vector = (HVector3)vOther;
            return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z);
        }



        public static HVector3 Reflect(HVector3 vInDirection, HVector3 vInNormal)
        {
            return Vector3MathServiceProvider.Reflect(vInDirection, vInNormal);
        }
        public abstract void Normalize();

        public static void Normalize(HVector3 vVector)
        {
            Vector3MathServiceProvider.Normalize(vVector);
        }
        public abstract override string ToString();
        public abstract string ToString(string vFormat);


        /// <summary>
        /// Compute the scalar dot product between two vectors vLhs and vRhs
        /// </summary>
        /// <param name="vLhs"></param>
        /// <param name="vRhs"></param>
        /// <returns></returns>
        public static float Dot(HVector3 vLhs, HVector3 vRhs)
        {
            return Vector3MathServiceProvider.Dot(vLhs, vRhs);
        }

        /// <summary>
        /// Project a vector onto another
        /// </summary>
        /// <param name="vVector"></param>
        /// <param name="vOnNormal"></param>
        /// <returns></returns>
        public static HVector3 Project(HVector3 vVector, HVector3 vOnNormal)
        {
            return Vector3MathServiceProvider.Project(vVector, vOnNormal);
        }

        /// <summary>
        /// Projects a vector vVector onto a plane whose normal is vPlaneNormal.
        /// </summary>
        /// <param name="vVector"></param>
        /// <param name="vPlaneNormal"></param>
        /// <returns></returns>
        public static HVector3 ProjectOnPlane(HVector3 vVector, HVector3 vPlaneNormal)
        {
            return Vector3MathServiceProvider.ProjectOnPlane(vVector, vPlaneNormal);
        }

        /// <summary>
        /// Returns the acute angle between two vectors vFrom and vTo
        /// </summary>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param>
        /// <returns></returns>
        public static float Angle(HVector3 vFrom, HVector3 vTo)
        {
            return Vector3MathServiceProvider.Angle(vFrom, vTo);
        }

        /// <summary>
        /// Provides a distance between the two vectors vA and vB
        /// </summary>
        /// <param name="vA"></param>
        /// <param name="vB"></param>
        /// <returns></returns>
        public static float Distance(HVector3 vA, HVector3 vB)
        {
            return Vector3MathServiceProvider.Distance(vA, vB);
        }
        public static HVector3 ClampMagnitude(HVector3 vEctor, float vMaxLength)
        {
            return Vector3MathServiceProvider.ClampMagnitude(vEctor, vMaxLength);
        }
        public static HVector3 Min(HVector3 vLhs, HVector3 vRhs)
        {
            return Vector3MathServiceProvider.Min(vLhs, vRhs);
        }
        public static HVector3 Max(HVector3 vLhs, HVector3 vRhs)
        {
            return Vector3MathServiceProvider.Max(vLhs, vRhs);
        }


        /// <summary>
        /// Adds an input HVector to the current vector
        /// </summary>
        /// <param name="vB"></param>
        /// <returns>two added vectors</returns>
        protected abstract HVector3 Plus(HVector3 vB);
        public static HVector3 operator +(HVector3 vA, HVector3 vB)
        {
            return vA.Plus(vB);
        }

        /// <summary>
        /// substracts input HVector from the current vector
        /// </summary>
        /// <param name="vB"></param>
        /// <returns></returns>
        protected abstract HVector3 Minus(HVector3 vB);

        public static HVector3 operator -(HVector3 vA, HVector3 vB)
        {
            return vA.Minus(vB);
        }

        public static HVector3 Up
        {
            get { return Vector3MathServiceProvider.Up; }
        }

        public static HVector3 Zero
        {
            get { return Vector3MathServiceProvider.Zero; }
        }
        public static HVector3 operator -(HVector3 vA)
        {
            return vA.ScalarMultiply(-1);
        }

        protected abstract HVector3 ScalarMultiply(float vD);


        public static HVector3 operator *(HVector3 vA, float vD)
        {
            return vA.ScalarMultiply(vD);
        }
        public static HVector3 operator *(float vD, HVector3 vA)
        {
            return vA.ScalarMultiply(vD);
        }
        public static HVector3 operator /(HVector3 vA, float vD)
        {
            return vA.ScalarMultiply((1f) / vD);
        }
        public static bool operator ==(HVector3 vLhs, HVector3 vRhs)
        {
            return Vector3MathServiceProvider.SqrMagnitude(vLhs - vRhs) < 9.99999944E-11f;
        }

        public static bool operator !=(HVector3 vLhs, HVector3 vRhs)
        {
            return Vector3MathServiceProvider.SqrMagnitude(vLhs - vRhs) >= 9.99999944E-11f;
        }
    
    }
}
