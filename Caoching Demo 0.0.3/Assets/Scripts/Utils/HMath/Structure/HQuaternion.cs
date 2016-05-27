// /**
// * @file HQuaternion.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 05 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;

using Assets.Scripts.Utils.HMath.Service_Provider;
using HeddokoLib.genericPatterns;

namespace Assets.Scripts.Utils.HMath.Structure
{
    public abstract class HQuaternion: IReleasableResource
    {
        public const float KEpsilon = 1E-05f;
        /// <summary>
        /// X component of a quaternion
        /// </summary>
        public abstract float X { get; set; } 
        /// <summary>
        /// Y Component of a Quaternion
        /// </summary>
        public abstract float Y { get; set; }
        /// <summary>
        /// Z Component of a Quaternion
        /// </summary>
        public abstract float Z { get; set; }
        /// <summary>
        /// W Component of a Quaternion
        /// </summary>
        public abstract float W { get; set; }

        /// <summary>
        /// Quaternion service provider
        /// </summary>
        internal static IQuaternionMathServiceProvider QuaternionMathServiceProvider { get; set; }

        /// <summary>
        /// Gets and sets the vX,y,z, or w component based on the index passed in
        /// <list type="index">
        /// <item>0 returns the X component</item>
        /// <item>1 returns the Y component</item>
        /// <item>2 returns the Z component</item>
        /// <item>3 returns the W component</item>
        /// </list>
        /// <exception cref="IndexOutOfRangeException">Thrown when the requested index is &lt; 0 or &gt; 3 </exception>
        /// </summary>
        /// <param name="vIndex">the index of the component</param>
        /// <returns>the component</returns>
        public float this[int vIndex]
        {
            get
            {
                switch (vIndex)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new IndexOutOfRangeException("Invalid HQuaternion index!");
                }
            }
            set
            {
                switch (vIndex)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        this.Y = value;
                        break;
                    case 2:
                        this.Z = value;
                        break;
                    case 3:
                        this.W = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid HQuaternion index!");
                }
            }
        }

        /// <summary>
        /// Returns an Identity Quaternion
        /// </summary>
        public static HQuaternion Identity
        {
            get { return QuaternionMathServiceProvider.Identity; }
        }

   

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="vX"></param>
        /// <param name="vY"></param>
        /// <param name="vZ"></param>
        /// <param name="vW"></param>
        public HQuaternion(float vX, float vY, float vZ, float vW)
        {
            X = vX;
            Y = vY;
            Z = vZ;
            W = vW;
        }

        public HQuaternion() { }
        /// <summary>
        /// Sets the HQuaternion with vNewX,vNewY,vNewZ,vNewW
        /// </summary>
        /// <param name="vNewX"></param>
        /// <param name="vNewY"></param>
        /// <param name="vNewZ"></param>
        /// <param name="vNewW"></param>
        public void Set(float vNewX, float vNewY, float vNewZ, float vNewW)
        {
            this.X = vNewX;
            this.Y = vNewY;
            this.Z = vNewZ;
            this.W = vNewW;
        }

        /// <summary>
        /// Get the Dot product of two quaternions
        /// </summary>
        /// <param name="vA"></param>
        /// <param name="vB"></param>
        /// <returns>The dot product</returns>
        public static float Dot(HQuaternion vA, HQuaternion vB)
        {
            return QuaternionMathServiceProvider.Dot(vA, vB);
        }

        /// <summary>
        ///  Creates a rotation which rotates vAngle degrees around the vAxis
        /// </summary>
        /// <param name="vAngle"></param>
        /// <param name="vAxis"></param>
        /// <returns></returns>
        public static HQuaternion AngleAxis(float vAngle, HVector3 vAxis)
        {
            return QuaternionMathServiceProvider.AngleAxis(vAngle,   vAxis);
        }

        /// <summary>
        /// Converts a rotation to an angle axis representation
        /// </summary>
        /// <param name="vAngle"></param>
        /// <param name="vAxis"></param>
        public abstract void ToAngleAxis(out float vAngle, out HVector3 vAxis);

        /// <summary>
        /// Creates a rotation that rotates from vFromDirection to vToDirection
        /// </summary>
        /// <param name="vFromDirection"></param>
        /// <param name="vToDirection"></param>
        /// <returns></returns>
        public static HQuaternion FromToRotation(HVector3 vFromDirection, HVector3 vToDirection)
        {
            return QuaternionMathServiceProvider.FromToRotation(vFromDirection, vToDirection);
        }

        /// <summary>
        /// Creates a rotation that rotates from vFromDirection to vToDirection
        /// </summary>
        /// <param name="vFromDirection"></param>
        /// <param name="vToDirection"></param>
        public abstract void SetFromToRotation(HVector3 vFromDirection, HVector3 vToDirection);


        /// <summary>
        /// Create a look rotation with the specified vForward and vUpwards
        /// </summary>
        /// <param name="vForward"></param>
        /// <param name="vUpwards"></param>
        /// <returns></returns>
        public static HQuaternion LookRotation(HVector3 vForward, HVector3 vUpwards)
        {
            return QuaternionMathServiceProvider.LookRotation(  vForward,  vUpwards);
        }

        /// <summary>
        /// Returns a look rotation from the given vForward. The Up vector used is the globally defined up vector
        /// </summary>
        /// <param name="vForward"></param>
        /// <returns></returns>
        public static HQuaternion LookRotation(HVector3 vForward)
        {
            HVector3 up = HVector3.Up;
            return QuaternionMathServiceProvider.LookRotation(  vForward,   up);
        }

        /// <summary>
        /// Creates and applies a look rotation with the specified vForward. The Up vector used is the globally defined up vector
        /// </summary>
        /// <param name="vForward">The forward</param>
        public abstract void SetLookRotation(HVector3 vForward);

        /// <summary>
        /// Creates and applies a look rotation with the specified vForward and VUp.  
        /// </summary>
        /// <param name="vForward">The forward</param>
        /// <param name="vUp">The up vector</param>
        public abstract void SetLookRotation(HVector3 vForward, HVector3 vUp);


        /// <summary>
        /// Spherically interpolates between vFrom  and vTo with respect to step vT
        /// </summary>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param>
        /// <param name="vT"></param>
        /// <returns></returns>
        public static HQuaternion Slerp(HQuaternion vFrom, HQuaternion vTo, float vT)
        {
            return QuaternionMathServiceProvider.Slerp(vFrom, vTo, vT);
        }

        /// <summary>
        /// Linearly interpolates between vFrom  and vTo with respect to step vT
        /// </summary>
        /// <param name="vFrom">Start Quaternion</param>
        /// <param name="vTo">End Quaternion</param>
        /// <param name="vT">Step</param>
        /// <returns>new Quaternion</returns>
        public static HQuaternion Lerp(HQuaternion vFrom, HQuaternion vTo, float vT)
        {
            return QuaternionMathServiceProvider.Lerp(vFrom, vTo, vT);
        }

        /// <summary>
        /// Rotates vfrom rotation towards vTo by an angular step of maxDegrees delta. 
        /// <remarks>Negative values of <code>vMaxDegreesDelta</code>  will move away from <code>vTo</code> until the rotation is exacly in the opposite direction</remarks>
        /// </summary>
        /// <param name="vFrom"></param>
        /// <param name="vTo"></param>
        /// <param name="vMaxDegreesDelta"></param>
        /// <returns></returns>
        public static HQuaternion RotateTowards(HQuaternion vFrom, HQuaternion vTo, float vMaxDegreesDelta)
        {
            
            return QuaternionMathServiceProvider.RotateTowards(vFrom, vTo, vMaxDegreesDelta);
        }

 
        /// <summary>
        /// Returns the inverse of vRotation
        /// </summary>
        /// <param name="vRotation"></param>
        /// <returns></returns>
        public static HQuaternion Inverse(HQuaternion vRotation)
        {
            return QuaternionMathServiceProvider.Inverse(  vRotation);
        }



        /// <summary>
        /// Retuns the angles in <b>degrees</b> between two rotation <code>vA</code> and <code>vB</code>
        /// </summary>
        /// <param name="vA"></param>
        /// <param name="vB"></param>
        /// <returns></returns>
        public static float Angle(HQuaternion vA, HQuaternion vB)
        {

            return QuaternionMathServiceProvider.Angle(vA, vB);
        }

        /// <summary>
        /// Returns a rotation that rotates around the x,y,z axis. 
        /// <remarks>Note, if being used with unity, the rotation starts with the Z component, X, and Y in that respective order</remarks>
        /// </summary>
        /// <param name="vX"></param>
        /// <param name="vY"></param>
        /// <param name="vZ"></param>
        /// <returns></returns>
        public static HQuaternion Euler(float vX, float vY, float vZ)
        {
            return QuaternionMathServiceProvider.Euler(vX, vY, vZ);
        }

        /// <summary>
        /// Returns a rotation that rotates around the x,y,z axis. 
        /// <remarks>Note, if being used with unity, the rotation starts with the HVector's Z component, X, and Y in that respective order</remarks>
        /// </summary>  
        /// <param name="vEuler">The HVector3 representation of a seto fo Euler Angles</param>
        /// <returns></returns>
        public static HQuaternion Euler(HVector3 vEuler)
        {
            return QuaternionMathServiceProvider.Euler(vEuler);
        }
 
        public override bool Equals(object vOther)
        {
            if (!(vOther is HQuaternion))
            {
                return false;
            }
            HQuaternion quaternion = (HQuaternion)vOther;
            return this.X.Equals(quaternion.X) && this.Y.Equals(quaternion.Y) && this.Z.Equals(quaternion.Z) && this.W.Equals(quaternion.W);
        }

        /// <summary>
        /// Multiplication operator
        /// </summary>
        /// <remarks>Rotating <code>vLhs * vRhs</code> is the same as applying two rotations in sequence relative to the refernece frame resulting from <code>vLhs</code> rotation. Rotations are not commutative.</remarks>
        /// <param name="vLhs">Left hand side HQuaternion</param>
        /// <param name="vRhs">Right hand side HQuaternion</param>
        /// <returns></returns>
        public static HQuaternion operator *(HQuaternion vLhs, HQuaternion vRhs)
        {
            return QuaternionMathServiceProvider.Multiply(vLhs, vRhs);
        }

        /// <summary>
        /// Rotates the <code>vPoint</code> with <code>vRotation</code>
        /// </summary> 
        /// <returns></returns>
        public static HVector3 operator *(HQuaternion vRotation, HVector3 vPoint)
        {
            return QuaternionMathServiceProvider.Multiply(vRotation, vPoint);

        }

        /// <summary>
        /// Equality operation between two HQuaternions
        /// </summary>
        /// <param name="vLhs"></param>
        /// <param name="vRhs"></param>
        /// <returns></returns>
        public static bool operator ==(HQuaternion vLhs, HQuaternion vRhs)
        {
            return QuaternionMathServiceProvider.Dot(vLhs, vRhs) > 0.999999f;
        }

        /// <summary>
        /// Inequality operator between two HQuaternion
        /// </summary>
        /// <param name="vLhs"></param>
        /// <param name="vRhs"></param>
        /// <returns></returns>
        public static bool operator !=(HQuaternion vLhs, HQuaternion vRhs)
        {
            return QuaternionMathServiceProvider.Dot(vLhs, vRhs) <= 0.999999f;
        }
     
        public void Cleanup()
        {
            X = Y = Z = 0;
        }
    }
}