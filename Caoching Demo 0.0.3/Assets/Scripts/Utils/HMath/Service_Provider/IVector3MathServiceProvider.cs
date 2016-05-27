// /**
// * @file IVector3MathServiceProvider.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 05 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */


using Assets.Scripts.Utils.HMath.Structure;

namespace Assets.Scripts.Utils.HMath.Service_Provider
{
    interface IVector3MathServiceProvider
    {

        /// <summary>
        /// Returns a Zero vector3
        /// </summary>
        HVector3 Zero { get; }

        /// <summary>
        /// Returns a HVector3 of the form (1,1,1)
        /// </summary>
        HVector3 One { get; }

        /// <summary>
        /// Returns a HVector3 in the global positive forward
        /// </summary>
        HVector3 Forward { get; }

        /// <summary>
        /// Retunrs an HVector3 in the global back direction
        /// </summary>
        HVector3 Back { get; }


        /// <summary>
        /// Returns an HVector3 in the global up direction
        /// </summary>
        HVector3 Up { get; }


        /// <summary>
        /// Returns an HVector3 in the global down direction
        /// </summary>
        HVector3 Down { get; }

        /// <summary>
        /// Returns a HVector3 in the global left direction
        /// </summary>
        HVector3 Left { get; }

        /// <summary>
        /// Returns a HVector3 in the global right direction
        /// </summary>
        HVector3 Right { get; }

        /// <summary>
        /// Linearly interpolate between two vectors.
        /// </summary>
        HVector3 Lerp(HVector3 vFrom, HVector3 vTo, float vT);
        /// <summary>
        /// Returns the dot product of two vectors
        /// </summary>
        /// <param name="vLhs"></param>
        /// <param name="vRhs"></param>
        /// <returns></returns>
        float Dot(HVector3 vLhs, HVector3 vRhs);

        /// <summary>
        /// Project a vector onto another vector
        /// </summary>
        HVector3 Project(HVector3 vVector, HVector3 vOnNormal);

        /// <summary>
        /// Project a vector onto a plane normal.  
        /// </summary>
        HVector3 ProjectOnPlane(HVector3 vVector3, HVector3 vPlaneNormal);
        /// <summary>
        /// Get an acute angle between the two passed in vectors
        /// </summary> 
        float Angle(HVector3 vFrom, HVector3 vTo);
        /// <summary>
        /// Get the distance between the two vectors
        /// </summary> 
        float Distance(HVector3 vA, HVector3 vB);

        /// <summary>
        /// Returns a copy of the vector with its magnitude clamped to the passed in Max Length
        /// </summary>
        /// <param name="vVector">Vector to clamp</param>
        /// <param name="vMaxLength">max length to clamp to </param>
        /// <returns>Vector with a clamped magnitude</returns>
        HVector3 ClampMagnitude(HVector3 vVector, float vMaxLength);

        /// <summary>
        /// Get the Magnitude of the passed in Vector
        /// </summary>
        /// <param name="a">the passed in vector</param>
        /// <returns>it's corresponding magnitude</returns>
        float Magnitude(HVector3 a);

        /// <summary>
        /// Get the Square Magnitude of the passed in Vector
        /// </summary>
        /// <param name="a">the vector</param>
        /// <returns>the vector's square magnitude</returns>
        float SqrMagnitude(HVector3 a);

        /// <summary>
        /// Returns a vector that is composed of the smallest components of both passed in vectors
        /// </summary>  
        HVector3 Min(HVector3 vLhs, HVector3 vRhs);

        /// <summary>
        /// Returns a vector that is composed of the largest components of the passed in vectors. 
        /// </summary>  
        HVector3 Max(HVector3 vLhs, HVector3 vRhs);

        /// <summary>
        /// Makes the passed in vectors normalized and orthogonal to each other.
        /// </summary>
        /// <param name="vNormal">Will normalized</param>
        /// <param name="vTangent">Will be orthogonal to vNormal and normalized</param>
        /// <param name="vBinormal">will normalize the binormal and will be orthogonal is vTangent and vNormal</param>
        void OrthoNormalize(  HVector3 vNormal,   HVector3 vTangent,   HVector3 vBinormal);

        /// <summary>
        /// Makes the passed in vectors normalized and orthogonal to each other.
        /// </summary> 
        void OrthoNormalize(  HVector3 vNormal,   HVector3 vTangent);
        /// <summary>
        /// Speherically interpolate between two vectors.
        /// </summary>
        HVector3 Slerp(HVector3 vFrom, HVector3 vTo, float vT);

        /// <summary>
        /// Moves a point current in a straight line towards a vTarget point.
        /// </summary>
        /// <param name="vCurrentVector">the current vector</param>
        /// <param name="vTargetVector">the vTarget vector</param>
        /// <param name="vMaxDistanceDelta">the delta distance between the two vTarget vectors</param>
        HVector3 MoveTowards(HVector3 vCurrentVector, HVector3 vTargetVector, float vMaxDistanceDelta);

        /// <summary>
        /// Rotates a vector towards its current vTarget by an angle of max radians delta. If a negative value is provided from maxMagnitude delta, then the vector will rotate away i
        ///  the vector will rotate away from vTarget/ until it is pointing in exactly the opposite direction, then stop.
        /// </summary>
        /// <param name="vCurrent"></param>
        /// <param name="vTarget"></param>
        /// <param name="vMaxRads"></param>
        /// <param name="vMaxMagDelta"></param>
        HVector3 RotateTowards(HVector3 vCurrent, HVector3 vTarget, float vMaxRads, float vMaxMagDelta);

        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.   
        /// </summary>
        /// <param name="vCurrent">the vector</param>
        /// <param name="vTarget">the vector's target</param>
        /// <param name="vdeltaTime">delta time</param>
        /// <param name="vCurrVelocity">the vector's current velocity</param>
        /// <param name="vSmoothTime">smooth time</param>
        /// <param name="vMaxSpeed">the max speed</param>
        /// <returns></returns>
        HVector3 SmoothDamp(HVector3 vCurrent, HVector3 vTarget, float vdeltaTime, ref HVector3 vCurrVelocity, float vSmoothTime, float vMaxSpeed);

        /// <summary>
        /// Get the cross product of two vectors
        /// </summary>
        /// <param name="vLhs"></param>
        /// <param name="vRhs"></param>
        /// <returns></returns>
        HVector3 Cross(HVector3 vLhs, HVector3 vRhs);

        /// <summary>
        /// Normalizes this vector
        /// </summary>
        /// <param name="vVector"></param>
        void Normalize(HVector3 vVector);

        /// <summary>
        /// Every component in the result is a component of vA multiplied by the same component of vB
        /// </summary> 
        HVector3 Scale(HVector3 vA, HVector3 vB);
        /// <summary>
        /// Reflects a vector off a plane defined the by the passed in normal.
        /// </summary> 
        HVector3 Reflect(HVector3 inDirection, HVector3 inNormal);


    }
}