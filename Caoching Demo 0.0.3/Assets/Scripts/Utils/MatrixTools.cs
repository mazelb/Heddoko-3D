/** 
* @file MatrixTools.cs
* @brief Contains the MatrixTools  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// MatrixTools, provides functions and methods to convert, calculate and execute on float[,] data types and IMUQuaternion orientations for RIGHT-HANDED systems.
    /// </summary>
    public static class MatrixTools
    {

        /// <summary>
        /// It converts a 3*3 orientation Matrix to a Quaternion.
        /// reference: @http://www.cg.info.hiroshima-cu.ac.jp/~miyazaki/knowledge/teche52.html
        /// </summary>
        /// <returns>The IMU quaternion.</returns>
        /// <param name="vRotationMatrix">the original 3*3 matrix.</param>
        public static IMUQuaternionOrientation MatToQuat1(float[,] vRotationMatrix)
        {
            IMUQuaternionOrientation vQuaternionResult;
            vQuaternionResult.w = (vRotationMatrix[0, 0] + vRotationMatrix[1, 1] + vRotationMatrix[2, 2] + 1.0f) / 4.0f;
            vQuaternionResult.x = (vRotationMatrix[0, 0] - vRotationMatrix[1, 1] - vRotationMatrix[2, 2] + 1.0f) / 4.0f;
            vQuaternionResult.y = (-vRotationMatrix[0, 0] + vRotationMatrix[1, 1] - vRotationMatrix[2, 2] + 1.0f) / 4.0f;
            vQuaternionResult.z = (-vRotationMatrix[0, 0] - vRotationMatrix[1, 1] + vRotationMatrix[2, 2] + 1.0f) / 4.0f;
            if (vQuaternionResult.w < 0.0f) vQuaternionResult.w = 0.0f;
            if (vQuaternionResult.x < 0.0f) vQuaternionResult.x = 0.0f;
            if (vQuaternionResult.y < 0.0f) vQuaternionResult.y = 0.0f;
            if (vQuaternionResult.z < 0.0f) vQuaternionResult.z = 0.0f;
            vQuaternionResult.w = (float)Math.Sqrt(vQuaternionResult.w);
            vQuaternionResult.x = (float)Math.Sqrt(vQuaternionResult.x);
            vQuaternionResult.y = (float)Math.Sqrt(vQuaternionResult.y);
            vQuaternionResult.z = (float)Math.Sqrt(vQuaternionResult.z);
            if (vQuaternionResult.w >= vQuaternionResult.x && vQuaternionResult.w >= vQuaternionResult.y && vQuaternionResult.w >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= +1.0f;
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[2, 1] - vRotationMatrix[1, 2]);
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[0, 2] - vRotationMatrix[2, 0]);
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[1, 0] - vRotationMatrix[0, 1]);
            }
            else if (vQuaternionResult.x >= vQuaternionResult.w && vQuaternionResult.x >= vQuaternionResult.y && vQuaternionResult.x >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[2, 1] - vRotationMatrix[1, 2]);
                vQuaternionResult.x *= +1.0f;
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[1, 0] + vRotationMatrix[0, 1]);
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[0, 2] + vRotationMatrix[2, 0]);
            }
            else if (vQuaternionResult.y >= vQuaternionResult.w && vQuaternionResult.y >= vQuaternionResult.x && vQuaternionResult.y >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[0, 2] - vRotationMatrix[2, 0]);
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[1, 0] + vRotationMatrix[0, 1]);
                vQuaternionResult.y *= +1.0f;
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[2, 1] + vRotationMatrix[1, 2]);
            }
            else if (vQuaternionResult.z >= vQuaternionResult.w && vQuaternionResult.z >= vQuaternionResult.x && vQuaternionResult.z >= vQuaternionResult.y)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[1, 0] - vRotationMatrix[0, 1]);
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[2, 0] + vRotationMatrix[0, 2]);
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[2, 1] + vRotationMatrix[1, 2]);
                vQuaternionResult.z *= +1.0f;
            }

            float r = (float)Math.Sqrt(vQuaternionResult.w * vQuaternionResult.w + vQuaternionResult.x * vQuaternionResult.x + vQuaternionResult.y * vQuaternionResult.y + vQuaternionResult.z * vQuaternionResult.z);
            vQuaternionResult.w /= r;
            vQuaternionResult.x /= r;
            vQuaternionResult.y /= r;
            vQuaternionResult.z /= r;
            return vQuaternionResult;
        }
        public static Quaternion MatToQuat(float[,] vRotationMatrix)
        {
            Quaternion vQuaternionResult;
            vQuaternionResult.w = (vRotationMatrix[0, 0] + vRotationMatrix[1, 1] + vRotationMatrix[2, 2] + 1.0f) / 4.0f;
            vQuaternionResult.x = (vRotationMatrix[0, 0] - vRotationMatrix[1, 1] - vRotationMatrix[2, 2] + 1.0f) / 4.0f;
            vQuaternionResult.y = (-vRotationMatrix[0, 0] + vRotationMatrix[1, 1] - vRotationMatrix[2, 2] + 1.0f) / 4.0f;
            vQuaternionResult.z = (-vRotationMatrix[0, 0] - vRotationMatrix[1, 1] + vRotationMatrix[2, 2] + 1.0f) / 4.0f;
            if (vQuaternionResult.w < 0.0f) vQuaternionResult.w = 0.0f;
            if (vQuaternionResult.x < 0.0f) vQuaternionResult.x = 0.0f;
            if (vQuaternionResult.y < 0.0f) vQuaternionResult.y = 0.0f;
            if (vQuaternionResult.z < 0.0f) vQuaternionResult.z = 0.0f;
            vQuaternionResult.w = (float)Math.Sqrt(vQuaternionResult.w);
            vQuaternionResult.x = (float)Math.Sqrt(vQuaternionResult.x);
            vQuaternionResult.y = (float)Math.Sqrt(vQuaternionResult.y);
            vQuaternionResult.z = (float)Math.Sqrt(vQuaternionResult.z);
            if (vQuaternionResult.w >= vQuaternionResult.x && vQuaternionResult.w >= vQuaternionResult.y && vQuaternionResult.w >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= +1.0f;
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[2, 1] - vRotationMatrix[1, 2]);
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[0, 2] - vRotationMatrix[2, 0]);
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[1, 0] - vRotationMatrix[0, 1]);
            }
            else if (vQuaternionResult.x >= vQuaternionResult.w && vQuaternionResult.x >= vQuaternionResult.y && vQuaternionResult.x >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[2, 1] - vRotationMatrix[1, 2]);
                vQuaternionResult.x *= +1.0f;
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[1, 0] + vRotationMatrix[0, 1]);
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[0, 2] + vRotationMatrix[2, 0]);
            }
            else if (vQuaternionResult.y >= vQuaternionResult.w && vQuaternionResult.y >= vQuaternionResult.x && vQuaternionResult.y >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[0, 2] - vRotationMatrix[2, 0]);
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[1, 0] + vRotationMatrix[0, 1]);
                vQuaternionResult.y *= +1.0f;
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[2, 1] + vRotationMatrix[1, 2]);
            }
            else if (vQuaternionResult.z >= vQuaternionResult.w && vQuaternionResult.z >= vQuaternionResult.x && vQuaternionResult.z >= vQuaternionResult.y)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[1, 0] - vRotationMatrix[0, 1]);
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[2, 0] + vRotationMatrix[0, 2]);
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[2, 1] + vRotationMatrix[1, 2]);
                vQuaternionResult.z *= +1.0f;
            }

            float r = (float)Math.Sqrt(vQuaternionResult.w * vQuaternionResult.w + vQuaternionResult.x * vQuaternionResult.x + vQuaternionResult.y * vQuaternionResult.y + vQuaternionResult.z * vQuaternionResult.z);
            vQuaternionResult.w /= r;
            vQuaternionResult.x /= r;
            vQuaternionResult.y /= r;
            vQuaternionResult.z /= r;
            return vQuaternionResult;
        }
        /// <summary>
        /// This Performs transformation From Global Coordinate System To local IMU coordinates
        /// </summary>
        /// <returns>The Global rotation matrix.</returns>
        /// <param name="vYaw">rotation in Yaw.</param>
        /// <param name="vPitch">rotation in Yaw.</param>
        /// <param name="vRoll">rotation in Yaw.</param>
        public static float[,] RotationGlobal(float vYaw, float vPitch, float vRoll)
        {
            float[,] vRotationGlobalResult = new float[3, 3];
            vRotationGlobalResult[0, 0] = Mathf.Cos(vPitch) * Mathf.Cos(vYaw);
            vRotationGlobalResult[1, 0] = (Mathf.Sin(vRoll) * Mathf.Cos(vYaw) * Mathf.Sin(vPitch) - Mathf.Sin(vYaw) * Mathf.Cos(vRoll));
            vRotationGlobalResult[2, 0] = (Mathf.Sin(vRoll) * Mathf.Sin(vYaw) + Mathf.Cos(vYaw) * Mathf.Sin(vPitch) * Mathf.Cos(vRoll));

            vRotationGlobalResult[0, 1] = Mathf.Cos(vPitch) * Mathf.Sin(vYaw);
            vRotationGlobalResult[1, 1] = Mathf.Sin(vRoll) * Mathf.Sin(vYaw) * Mathf.Sin(vPitch) + Mathf.Cos(vYaw) * Mathf.Cos(vRoll);
            vRotationGlobalResult[2, 1] = (Mathf.Cos(vRoll) * Mathf.Sin(vYaw) * Mathf.Sin(vPitch) - Mathf.Cos(vYaw) * Mathf.Sin(vRoll));

            vRotationGlobalResult[0, 2] = -Mathf.Sin(vPitch);
            vRotationGlobalResult[1, 2] = Mathf.Cos(vPitch) * Mathf.Sin(vRoll);
            vRotationGlobalResult[2, 2] = (Mathf.Cos(vPitch) * Mathf.Cos(vRoll));
            return vRotationGlobalResult;
        }

        /// <summary>
        /// This Performs transformation From IMU Local Coordinate System To global coordinates
        /// </summary>
        /// <returns>The rotation matrix in global coordinate system.</returns>
        /// <param name="vYaw">rotation in Yaw.</param>
        /// <param name="vPitch">rotation in Yaw.</param>
        /// <param name="vRoll">rotation in Yaw.</param>
        public static float[,] RotationLocal(float vYaw, float vPitch, float vRoll)
        {
            float[,] vRotationLocalResult = new float[3, 3];
            vRotationLocalResult[0, 0] = Mathf.Cos(vPitch) * Mathf.Cos(vYaw);
            vRotationLocalResult[1, 0] = Mathf.Cos(vPitch) * Mathf.Sin(vYaw);
            vRotationLocalResult[2, 0] = -Mathf.Sin(vPitch);

            vRotationLocalResult[0, 1] = -Mathf.Cos(vRoll) * Mathf.Sin(vYaw) + Mathf.Cos(vYaw) * Mathf.Sin(vPitch) * Mathf.Sin(vRoll);
            vRotationLocalResult[1, 1] = Mathf.Sin(vRoll) * Mathf.Sin(vYaw) * Mathf.Sin(vPitch) + Mathf.Cos(vYaw) * Mathf.Cos(vRoll);
            vRotationLocalResult[2, 1] = Mathf.Cos(vPitch) * Mathf.Sin(vRoll);

            vRotationLocalResult[0, 2] = (Mathf.Sin(vRoll) * Mathf.Sin(vYaw) + Mathf.Cos(vYaw) * Mathf.Sin(vPitch) * Mathf.Cos(vRoll));
            vRotationLocalResult[1, 2] = (Mathf.Cos(vRoll) * Mathf.Sin(vYaw) * Mathf.Sin(vPitch) - Mathf.Cos(vYaw) * Mathf.Sin(vRoll));
            vRotationLocalResult[2, 2] = (Mathf.Cos(vPitch) * Mathf.Cos(vRoll));
            return vRotationLocalResult;
        }

        /// <summary>
        /// This Performs matrix transpose
        /// </summary>
        /// <returns>The matrix transpose result.</returns>
        /// <param name="vMatrix">The original matrix.</param>
        public static float[,] MatrixTranspose(float[,] vMatrix)
        {
            float[,] vMatrixTransposeResult = new float[3, 3];

            vMatrixTransposeResult[0, 0] = vMatrix[0, 0];
            vMatrixTransposeResult[1, 0] = vMatrix[0, 1];
            vMatrixTransposeResult[2, 0] = vMatrix[0, 2];

            vMatrixTransposeResult[0, 1] = vMatrix[1, 0];
            vMatrixTransposeResult[1, 1] = vMatrix[1, 1];
            vMatrixTransposeResult[2, 1] = vMatrix[1, 2];

            vMatrixTransposeResult[0, 2] = vMatrix[2, 0];
            vMatrixTransposeResult[1, 2] = vMatrix[2, 1];
            vMatrixTransposeResult[2, 2] = vMatrix[2, 2];

            return vMatrixTransposeResult;
        }


        /// <summary>
        /// Eulers to quaternion transformation.
        /// </summary>
        /// <returns>The IMU quaternion.</returns>
        /// <param name="vPitch">Pitch.</param>
        /// <param name="vRoll">Roll.</param>
        /// <param name="vYaw">Yaw.</param>
        public static IMUQuaternionOrientation eulerToQuaternion(float vPitch, float vRoll, float vYaw)
        {
            float vSinHalfYaw = Mathf.Sin(vYaw / 2.0f);
            float vCosHalfYaw = Mathf.Cos(vYaw / 2.0f);
            float vSinHalfPitch = Mathf.Sin(vPitch / 2.0f);
            float vCosHalfPitch = Mathf.Cos(vPitch / 2.0f);
            float vSinHalfRoll = Mathf.Sin(vRoll / 2.0f);
            float vCosHalfRoll = Mathf.Cos(vRoll / 2.0f);

            IMUQuaternionOrientation vResult;
            vResult.x = -vCosHalfRoll * vSinHalfPitch * vSinHalfYaw
                + vCosHalfPitch * vCosHalfYaw * vSinHalfRoll;
            vResult.y = vCosHalfRoll * vCosHalfYaw * vSinHalfPitch
                + vSinHalfRoll * vCosHalfPitch * vSinHalfYaw;
            vResult.z = vCosHalfRoll * vCosHalfPitch * vSinHalfYaw
                - vSinHalfRoll * vCosHalfYaw * vSinHalfPitch;
            vResult.w = vCosHalfRoll * vCosHalfPitch * vCosHalfYaw
                + vSinHalfRoll * vSinHalfPitch * vSinHalfYaw;

            return vResult;
        }

        /// <summary>
        /// returns a 3x3 identity matrix.
        /// </summary>
        /// <returns>3*3  identity matrix.</returns>
        public static float[,] Identity3X3Matrix()
        {
            float[,] vIdentMatrix = new float[3, 3];
            vIdentMatrix[0, 0] = 0;
            vIdentMatrix[1, 1] = 0;
            vIdentMatrix[2, 2] = 0;
            return vIdentMatrix;
        }


        /// <summary>
        /// multiplication between two 3*3 matrices.
        /// </summary>
        /// <returns>the multiplication result matrix A x B.</returns>
        /// <param name="vMatA">Matrix a.</param>
        /// <param name="vMatB">Matrix b.</param>
        public static float[,] MultiplyMatrix(float[,] vMatA, float[,] vMatB)
        {
            float[,] c = new float[3, 3];
            int i, j;
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    c[i, j] = vMatA[i, 0] * vMatB[0, j] + vMatA[i, 1] * vMatB[1, j] + vMatA[i, 2] * vMatB[2, j];
                }
            }
            return c;
        }

        /// <summary>
        /// produces a rotation matrix around an arbitrary vector with desired angles.
        /// </summary>
        /// <returns>the resulting rotation matrix.</returns>
        /// <param name="vRotVector">Rotation vector.</param>
        /// <param name="vAngle">Rotation angle in Rad.</param>
        public static float[,] RVector(Vector3 vRotVector, float vAngle)
        {
            float[,] a = new float[3, 3];
            a[0, 0] = Mathf.Cos(vAngle) + vRotVector.x * vRotVector.x * (1 - Mathf.Cos(vAngle));
            a[1, 0] = vRotVector.x * vRotVector.y * (1 - Mathf.Cos(vAngle)) + vRotVector.z * Mathf.Sin(vAngle);
            a[2, 0] = vRotVector.x * vRotVector.z * (1 - Mathf.Cos(vAngle)) - vRotVector.y * Mathf.Sin(vAngle);

            a[0, 1] = vRotVector.x * vRotVector.y * (1 - Mathf.Cos(vAngle)) - vRotVector.z * Mathf.Sin(vAngle);
            a[1, 1] = Mathf.Cos(vAngle) + vRotVector.y * vRotVector.y * (1 - Mathf.Cos(vAngle)); ;
            a[2, 1] = vRotVector.z * vRotVector.y * (1 - Mathf.Cos(vAngle)) + vRotVector.x * Mathf.Sin(vAngle);

            a[0, 2] = vRotVector.x * vRotVector.z * (1 - Mathf.Cos(vAngle)) + vRotVector.y * Mathf.Sin(vAngle);
            a[1, 2] = vRotVector.z * vRotVector.y * (1 - Mathf.Cos(vAngle)) - vRotVector.x * Mathf.Sin(vAngle);
            a[2, 2] = Mathf.Cos(vAngle) + vRotVector.z * vRotVector.z * (1 - Mathf.Cos(vAngle));
            return a;
        }
    }

    /// <summary>
    /// A quaternion structure that is needed to render orientation data .
    /// </summary>
    public struct IMUQuaternionOrientation
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public IMUQuaternionOrientation(float _x, float _y, float _z, float _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }
    };
}
