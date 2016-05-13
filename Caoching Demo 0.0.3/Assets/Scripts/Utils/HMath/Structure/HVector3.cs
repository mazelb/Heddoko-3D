/** 
* @file HVector3.cs
* @brief Contains the HVector3  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Utils.HMath.Service_Provider;

namespace Assets.Scripts.Utils.HMath
{
    /// <summary>
    /// A HVector3 class holding X,y,z components of type floats
    /// </summary>
  public abstract class HVector3
    {
        public const float KEpsilon = 1E-05f;
        public abstract float X { get; set; }
        public abstract float Y { get; set; }
        public abstract float Z { get; set; }

        internal static IVector3MathServiceProvider Vector3MathServiceProvider { get; set; }

        /// <summary>
        /// Operator overload.
        /// Note: will throw an IndexOutOfRangeException if index doesn't fall between 0-2
        /// </summary>
        /// <param name="index"> the index of the HVector3 Component. 
        /// </param>
        /// <returns></returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new IndexOutOfRangeException("Invalid HVector3 index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid HVector3 index!");
                }
            }
        }

      
 
        public abstract float Magnitude { get; }
        public abstract float SqrMagnitude { get; }
        

        protected HVector3(float x, float y, float z)
        {
             X = x;
            Y = y;
            Z = z;
        }
        protected HVector3(float x, float y)
        {
             X = x;
            Y = y;
             Z = 0f;
        }

        public static HVector3 Lerp(HVector3 vFrom, HVector3 vTo, float vT)
        {
            return Vector3MathServiceProvider.Lerp(vFrom, vTo, vT);
        }
        public static HVector3 Slerp(HVector3 vFrom, HVector3 vTo, float vT)
        {
            return Vector3MathServiceProvider.Slerp(vFrom, vTo, vT);
        }

        public static void OrthoNormalize(ref HVector3 vNormal, ref HVector3 vTangent)
        {
            Vector3MathServiceProvider.OrthoNormalize(ref vNormal,ref vTangent);
        }


        public static void OrthoNormalize(ref HVector3 vNormal, ref HVector3 vTangent, ref HVector3 vBinormal)
        {
            Vector3MathServiceProvider.OrthoNormalize(ref vNormal, ref vTangent, ref vBinormal);

        }


        public static HVector3 MoveTowards(HVector3 current, HVector3 target, float maxDistanceDelta)
        {
          return   Vector3MathServiceProvider.MoveTowards(current, target, maxDistanceDelta);
        }

        public static HVector3 RotateTowards(HVector3 current, HVector3 target, float maxRadiansDelta,
            float maxMagnitudeDelta)
        {
         return   Vector3MathServiceProvider.RotateTowards(current, target, maxRadiansDelta, maxMagnitudeDelta);

        }

        public static HVector3 SmoothDamp(HVector3 current, HVector3 target, ref HVector3 currentVelocity,
            float smoothTime, float deltaTime, float maxSpeed = float.PositiveInfinity)
        {
            return Vector3MathServiceProvider.SmoothDamp(current, target,deltaTime, ref currentVelocity, smoothTime, maxSpeed);

        }
        /// <summary>
        /// Set the new passed in values
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        /// <param name="newZ"></param>
        public void Set(float newX, float newY, float newZ)
        {
            X = newX;
           Y = newY;
            Z = newZ;
        }
        /// <summary>
        /// Every component in the result is a component of vA multiplied by the same component of vB
        /// </summary>
        public static HVector3 Scale(HVector3 vA, HVector3 vB)
        {
            return Vector3MathServiceProvider.Scale(vA, vB);
        }
        public void Scale(HVector3 scale)
        {
            this.X *= scale.X;
            this.Y *= scale.Y;
            this.Z *= scale.Z;
        }

        public static HVector3 Cross(HVector3 lhs, HVector3 rhs)
        {
            return Vector3MathServiceProvider.Cross(lhs, rhs);
        }
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2;
        }
        public override bool Equals(object other)
        {
            if (!(other is HVector3))
            {
                return false;
            }
            HVector3 vector = (HVector3)other;
            return this.X.Equals(vector.X) && this.Y.Equals(vector.Y) && this.Z.Equals(vector.Z);
        }

        public static HVector3 Reflect(HVector3 inDirection, HVector3 inNormal)
        {
          return  Vector3MathServiceProvider.Reflect(inDirection, inNormal);
        }
        public abstract void Normalize();

        public static void Normalize(HVector3 vVector)
        {
            Vector3MathServiceProvider.Normalize(vVector);
        }
        public abstract override string ToString();
        public abstract string ToString(string format);

        public static float Dot(HVector3 lhs, HVector3 rhs)
        {
            return Vector3MathServiceProvider.Dot(lhs, rhs);
        }

        public static HVector3 Project(HVector3 vector, HVector3 onNormal)
        {
            return Vector3MathServiceProvider.Project(vector, onNormal);
        }


        public static HVector3 ProjectOnPlane(HVector3 vector, HVector3 planeNormal)
        {
            return Vector3MathServiceProvider.ProjectOnPlane(vector, planeNormal);
        }

        public static float Angle(HVector3 from, HVector3 to)
        {
          return Vector3MathServiceProvider.Angle(from, to);
        }

        public static float Distance(HVector3 a, HVector3 b)
        {
            return Vector3MathServiceProvider.Distance(a, b);
        }
        public static HVector3 ClampMagnitude(HVector3 vector, float maxLength)
        {
            return Vector3MathServiceProvider.ClampMagnitude(vector, maxLength);
        }
        public static HVector3 Min(HVector3 lhs, HVector3 rhs)
        {
            return Vector3MathServiceProvider.Min(lhs, rhs);
        }
        public static HVector3 Max(HVector3 lhs, HVector3 rhs)
        {
            return Vector3MathServiceProvider.Max(lhs, rhs);
        }
 

        /// <summary>
        /// Adds an input HVector to the current vector
        /// </summary>
        /// <param name="b"></param>
        /// <returns>two added vectors</returns>
        protected abstract HVector3 Plus(HVector3 b);
        public static HVector3 operator +(HVector3 a, HVector3 b)
        {
            return a.Plus(b);
        }

        /// <summary>
        /// substracts input HVector from the current vector
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected abstract HVector3 Minus(HVector3 b);

        public static HVector3 operator -(HVector3 a, HVector3 b)
        {
            return a.Minus(b);
        }


        public static HVector3 operator -(HVector3 a)
        {
            return a.ScalarMultiply(-1);
        }

        protected abstract HVector3 ScalarMultiply(float d);


        public static HVector3 operator *(HVector3 a, float d)
        {
            return a.ScalarMultiply(d);
        }
        public static HVector3 operator *(float d, HVector3 a)
        {
            return a.ScalarMultiply(d);
        }
        public static HVector3 operator /(HVector3 a, float d)
        {
            return a.ScalarMultiply((1f)/d);
        }
        public static bool operator ==(HVector3 lhs, HVector3 rhs)
        {
            return Vector3MathServiceProvider.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
        }
         
        public static bool operator !=(HVector3 lhs, HVector3 rhs)
        {
            return Vector3MathServiceProvider.SqrMagnitude(lhs- rhs) >= 9.99999944E-11f;
        }

    }
}
