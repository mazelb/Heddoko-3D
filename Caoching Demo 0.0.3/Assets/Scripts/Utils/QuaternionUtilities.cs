// /**
// * @file QuaternionUtilities.cs
// * @brief Contains the QuaternionUtilities class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// A utility class for Quaternions
    /// </summary>
    public static class QuaternionUtilities
    {
        /// <summary>
        /// Returns an average of the given quaternions as described in <see cref="https://ntrs.nasa.gov/archive/nasa/casi.ntrs.nasa.gov/20070017872.pdf"/>
        /// This function on works if the quaternions are relatively close together
        /// </summary>
        /// <param name="vQuaternionList">the list of quaternions to iterate over</param>
        /// <returns>the averaged quaternion.</returns>
        public static Quaternion AverageQuaternion(List<Quaternion> vQuaternionList)
        {
            float vW = 0.0f;
            float vX = 0.0f;
            float vY = 0.0f;
            float vZ = 0.0f;
            int vTotal = vQuaternionList.Count;
            if (vTotal <= 1)
            {
                throw new InvalidOperationException("The given number of quaternions is invalid. Need to be at least of length 2");
            }
            Quaternion vFirst = vQuaternionList[0];
            Quaternion vAveraged = Quaternion.identity;
            float vDet = 1f / (float)vTotal;
            
            for (int vI = 0; vI < vTotal; vI++)
            {
                Quaternion vCurr = vQuaternionList[vI];
                //before we add a new quaternion to the average, we have to check whether the quaternion has to be inverted(vQ and -vQ are the same rotation).
                if (AreQuaternionsClose(vFirst, vCurr) && vI != 0)
                {
                    vCurr = InverseSigns(vCurr);
                }
                //average the values
                vAveraged.w += vCurr.w;
                vW = vAveraged.w * vDet;
                vAveraged.x += vCurr.x;
                vX = vAveraged.x * vDet;
                vAveraged.y += vCurr.y;
                vY = vAveraged.y * vDet;
                vAveraged.z += vCurr.z;
                vZ = vAveraged.z * vDet;
            }
            return NormalizeQuaternion(vX, vY, vZ, vW);
        }

        /// <summary>
        /// Normalizes a quaternion 
        /// </summary>
        /// <param name="vX">the x component</param>
        /// <param name="vY">The y component of a quaternion</param>
        /// <param name="vZ">The z component of a quaternion</param>
        /// <param name="vW">the w component of a quaternion</param>
        /// <returns></returns>
        public static Quaternion NormalizeQuaternion(float vX, float vY, float vZ, float vW)
        {

            float vLengthD = 1.0f / (vW * vW + vX * vX + vY * vY + vZ * vZ);
            vW *= vLengthD;
            vX *= vLengthD;
            vY *= vLengthD;
            vZ *= vLengthD;

            return new Quaternion(vX, vY, vZ, vW);
        }

        /// <summary>
        /// Inverses the signs of the quaternions
        /// </summary>
        /// <param name="vQ">the quaternion whose sings need to be inverted</param>
        /// <returns>the original quaternion vQ with its signs inverted.</returns>
        public static Quaternion InverseSigns(Quaternion vQ)
        {
            return new Quaternion(-vQ.x, -vQ.y, -vQ.z, -vQ.w);

        }
        /// <summary>
        /// Normalizes a quaternion 
        /// </summary> 
        /// <param name="vInput">the quaternion to be normalized</param>
        /// <returns>A normalized quaternion</returns>
        public static Quaternion NormalizeQuaternion(Quaternion vInput)
        {
            return NormalizeQuaternion(vInput.x, vInput.y, vInput.z, vInput.w);
        }

        /// <summary>
        /// Returns if the two input quaternions are close to each other.
        ///  This can be used to check whether or not one of the two quaternions which are supposed to be very similar
        ///  but has its components signs reversed( -vQ is equal to vQ)
        /// </summary>
        /// <param name="vQ1">First quaternion</param>
        /// <param name="vQ2">second input quaternion</param>
        /// <returns></returns>
        public static bool AreQuaternionsClose(Quaternion vQ1, Quaternion vQ2)
        {

            float vDot = Quaternion.Dot(vQ1, vQ2);

            if (vDot < 0.0f)
            {

                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns a quaternion from a given raw value
        /// </summary>
        /// <param name="vRawValue">The raw value</param>
        /// <returns></returns>
        public static Quaternion ToQuaternion(BodyFrame.Vect4 vRawValue)
        {
            return new Quaternion(vRawValue.x, vRawValue.y, vRawValue.z, vRawValue.w);
        }
    }
}