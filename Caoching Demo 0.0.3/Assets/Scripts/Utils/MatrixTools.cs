/** 
* @file MatrixTools.cs
* @brief Contains the MatrixTools  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using MathNet.Numerics.LinearAlgebra;
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
            if (vQuaternionResult.w >= vQuaternionResult.x && vQuaternionResult.w >= vQuaternionResult.y &&
                vQuaternionResult.w >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= +1.0f;
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[2, 1] - vRotationMatrix[1, 2]);
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[0, 2] - vRotationMatrix[2, 0]);
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[1, 0] - vRotationMatrix[0, 1]);
            }
            else if (vQuaternionResult.x >= vQuaternionResult.w && vQuaternionResult.x >= vQuaternionResult.y &&
                     vQuaternionResult.x >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[2, 1] - vRotationMatrix[1, 2]);
                vQuaternionResult.x *= +1.0f;
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[1, 0] + vRotationMatrix[0, 1]);
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[0, 2] + vRotationMatrix[2, 0]);
            }
            else if (vQuaternionResult.y >= vQuaternionResult.w && vQuaternionResult.y >= vQuaternionResult.x &&
                     vQuaternionResult.y >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[0, 2] - vRotationMatrix[2, 0]);
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[1, 0] + vRotationMatrix[0, 1]);
                vQuaternionResult.y *= +1.0f;
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[2, 1] + vRotationMatrix[1, 2]);
            }
            else if (vQuaternionResult.z >= vQuaternionResult.w && vQuaternionResult.z >= vQuaternionResult.x &&
                     vQuaternionResult.z >= vQuaternionResult.y)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[1, 0] - vRotationMatrix[0, 1]);
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[2, 0] + vRotationMatrix[0, 2]);
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[2, 1] + vRotationMatrix[1, 2]);
                vQuaternionResult.z *= +1.0f;
            }

            float r =
                (float)
                    Math.Sqrt(vQuaternionResult.w * vQuaternionResult.w + vQuaternionResult.x * vQuaternionResult.x +
                              vQuaternionResult.y * vQuaternionResult.y + vQuaternionResult.z * vQuaternionResult.z);
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
            if (vQuaternionResult.w >= vQuaternionResult.x && vQuaternionResult.w >= vQuaternionResult.y &&
                vQuaternionResult.w >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= +1.0f;
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[2, 1] - vRotationMatrix[1, 2]);
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[0, 2] - vRotationMatrix[2, 0]);
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[1, 0] - vRotationMatrix[0, 1]);
            }
            else if (vQuaternionResult.x >= vQuaternionResult.w && vQuaternionResult.x >= vQuaternionResult.y &&
                     vQuaternionResult.x >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[2, 1] - vRotationMatrix[1, 2]);
                vQuaternionResult.x *= +1.0f;
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[1, 0] + vRotationMatrix[0, 1]);
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[0, 2] + vRotationMatrix[2, 0]);
            }
            else if (vQuaternionResult.y >= vQuaternionResult.w && vQuaternionResult.y >= vQuaternionResult.x &&
                     vQuaternionResult.y >= vQuaternionResult.z)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[0, 2] - vRotationMatrix[2, 0]);
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[1, 0] + vRotationMatrix[0, 1]);
                vQuaternionResult.y *= +1.0f;
                vQuaternionResult.z *= Mathf.Sign(vRotationMatrix[2, 1] + vRotationMatrix[1, 2]);
            }
            else if (vQuaternionResult.z >= vQuaternionResult.w && vQuaternionResult.z >= vQuaternionResult.x &&
                     vQuaternionResult.z >= vQuaternionResult.y)
            {
                vQuaternionResult.w *= Mathf.Sign(vRotationMatrix[1, 0] - vRotationMatrix[0, 1]);
                vQuaternionResult.x *= Mathf.Sign(vRotationMatrix[2, 0] + vRotationMatrix[0, 2]);
                vQuaternionResult.y *= Mathf.Sign(vRotationMatrix[2, 1] + vRotationMatrix[1, 2]);
                vQuaternionResult.z *= +1.0f;
            }

            float r =
                (float)
                    Math.Sqrt(vQuaternionResult.w * vQuaternionResult.w + vQuaternionResult.x * vQuaternionResult.x +
                              vQuaternionResult.y * vQuaternionResult.y + vQuaternionResult.z * vQuaternionResult.z);
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
            vRotationGlobalResult[1, 0] = (Mathf.Sin(vRoll) * Mathf.Cos(vYaw) * Mathf.Sin(vPitch) -
                                           Mathf.Sin(vYaw) * Mathf.Cos(vRoll));
            vRotationGlobalResult[2, 0] = (Mathf.Sin(vRoll) * Mathf.Sin(vYaw) +
                                           Mathf.Cos(vYaw) * Mathf.Sin(vPitch) * Mathf.Cos(vRoll));

            vRotationGlobalResult[0, 1] = Mathf.Cos(vPitch) * Mathf.Sin(vYaw);
            vRotationGlobalResult[1, 1] = Mathf.Sin(vRoll) * Mathf.Sin(vYaw) * Mathf.Sin(vPitch) +
                                          Mathf.Cos(vYaw) * Mathf.Cos(vRoll);
            vRotationGlobalResult[2, 1] = (Mathf.Cos(vRoll) * Mathf.Sin(vYaw) * Mathf.Sin(vPitch) -
                                           Mathf.Cos(vYaw) * Mathf.Sin(vRoll));

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

            vRotationLocalResult[0, 1] = -Mathf.Cos(vRoll) * Mathf.Sin(vYaw) +
                                         Mathf.Cos(vYaw) * Mathf.Sin(vPitch) * Mathf.Sin(vRoll);
            vRotationLocalResult[1, 1] = Mathf.Sin(vRoll) * Mathf.Sin(vYaw) * Mathf.Sin(vPitch) +
                                         Mathf.Cos(vYaw) * Mathf.Cos(vRoll);
            vRotationLocalResult[2, 1] = Mathf.Cos(vPitch) * Mathf.Sin(vRoll);

            vRotationLocalResult[0, 2] = (Mathf.Sin(vRoll) * Mathf.Sin(vYaw) +
                                          Mathf.Cos(vYaw) * Mathf.Sin(vPitch) * Mathf.Cos(vRoll));
            vRotationLocalResult[1, 2] = (Mathf.Cos(vRoll) * Mathf.Sin(vYaw) * Mathf.Sin(vPitch) -
                                          Mathf.Cos(vYaw) * Mathf.Sin(vRoll));
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
        /// Converts a Vect4 to a PNI specified DCM. This incoming Quaternion needs to be in NED coordinates
        /// </summary>
        /// <param name="vInput"></param>
        /// <returns></returns>
        internal static Matrix<float> PniDcmConversion(Quaternion vInput)
        {
            Matrix<float> vDCM = Matrix<float>.Build.Dense(3, 3);
            float vSqw = vInput.w * vInput.w;
            float vSqx = vInput.x * vInput.x;
            float vSqy = vInput.y * vInput.y;
            float vSqz = vInput.z * vInput.z;
            //set diagonals for legibility first
            vDCM[0, 0] = (vSqx - vSqy - vSqz + vSqw);
            vDCM[1, 1] = (-vSqx + vSqy - vSqz + vSqw);
            vDCM[2, 2] = (-vSqx - vSqy + vSqz + vSqw);

            float vXZ = vInput.x * vInput.z;
            float vXY = vInput.x * vInput.y;
            float vYZ = vInput.y * vInput.z;
            float vYW = vInput.y * vInput.w;
            float vZW = vInput.z * vInput.w;

            vDCM[0, 1] = 2.0f * (vXY + vZW);
            vDCM[0, 2] = 2.0f * (vXZ - vYW);
            vDCM[1, 0] = 2.0f * (vXY - vZW);
            vDCM[1, 2] = 2.0f * (vYZ + vYW);
            vDCM[2, 0] = 2.0f * (vXZ + vYW);
            vDCM[2, 1] = 2.0f * (vYZ - vYW);

            return vDCM;
        }


        /// <summary>
        /// Converts a Direction Cosine Matrix to a quaternion <see cref="http://www.vectornav.com/docs/default-source/documentation/vn-100-documentation/AN002.pdf?sfvrsn=10"/>
        /// </summary>
        /// <param name="vDCM">the direction cosine matrix</param>
        /// <returns>The converted quaternion</returns>
        internal static Quaternion DCMToQuaternion(Matrix<float> vDCM)
        {
            Quaternion vReturn = Quaternion.identity;
            vReturn.w = 0.5f * Mathf.Sqrt(vDCM[0, 0] + vDCM[1, 1] + vDCM[2, 2] + 1);
            float vWFactor = (4f * vReturn.w);
            vReturn.x = (vDCM[1, 2] + vDCM[2, 1]) / vWFactor;
            vReturn.y = (vDCM[2, 0] + vDCM[0, 2]) / vWFactor;
            vReturn.z = (vDCM[0, 1] + vDCM[1, 0]) / vWFactor;
            return vReturn;
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
            a[1, 1] = Mathf.Cos(vAngle) + vRotVector.y * vRotVector.y * (1 - Mathf.Cos(vAngle));
            ;
            a[2, 1] = vRotVector.z * vRotVector.y * (1 - Mathf.Cos(vAngle)) + vRotVector.x * Mathf.Sin(vAngle);

            a[0, 2] = vRotVector.x * vRotVector.z * (1 - Mathf.Cos(vAngle)) + vRotVector.y * Mathf.Sin(vAngle);
            a[1, 2] = vRotVector.z * vRotVector.y * (1 - Mathf.Cos(vAngle)) - vRotVector.x * Mathf.Sin(vAngle);
            a[2, 2] = Mathf.Cos(vAngle) + vRotVector.z * vRotVector.z * (1 - Mathf.Cos(vAngle));
            return a;
        }

        public static void FromMatrix4x4(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.GetScale();
            transform.rotation = matrix.GetRotation();
            transform.position = matrix.GetPosition();
        }

        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
            var qw = Mathf.Sqrt(1f + matrix.m00 + matrix.m11 + matrix.m22) / 2;
            var w = 4 * qw;
            var qx = (matrix.m21 - matrix.m12) / w;
            var qy = (matrix.m02 - matrix.m20) / w;
            var qz = (matrix.m10 - matrix.m01) / w;

            return new Quaternion(qx, qy, qz, qw);
        }

        public static Vector3 GetPosition(this Matrix4x4 matrix)
        {
            var x = matrix.m03;
            var y = matrix.m13;
            var z = matrix.m23;

            return new Vector3(x, y, z);
        }

        public static Vector3 GetScale(this Matrix4x4 m)
        {
            var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
            var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
            var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Converts a Vett4 to a rotation matrix
        ///<seealso cref="http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/"/>> 
        /// </summary>
        /// <param name="vInput"></param>
        /// <returns></returns>
        public static Matrix<float> QuatToMatrix(BodyFrame.Vect4 vInput)
        {
            Matrix<float> vReturnMatrix = Matrix<float>.Build.Dense(3, 3);
            float vSqw = vInput.w * vInput.w;
            float vSqx = vInput.x * vInput.x;
            float vSqy = vInput.y * vInput.y;
            float vSqz = vInput.z * vInput.z;

            // invs (inverse square length) is only required if quaternion is not already normalised
            float vInvs = 1 / (vSqx + vSqy + vSqz + vSqw);
            // since sqw + sqx + sqy + sqz =1/invs*invs
            vReturnMatrix[0, 0] = (vSqx - vSqy - vSqz + vSqw) * vInvs;
            vReturnMatrix[1, 1] = (-vSqx + vSqy - vSqz + vSqw) * vInvs;
            vReturnMatrix[2, 2] = (-vSqx - vSqy + vSqz + vSqw) * vInvs;

            float vTmp1 = vInput.x * vInput.y;
            float vTmp2 = vInput.z * vInput.w;
            vReturnMatrix[1, 0] = 2.0f * (vTmp1 + vTmp2) * vInvs;
            vReturnMatrix[0, 1] = 2.0f * (vTmp1 - vTmp2) * vInvs;

            vTmp1 = vInput.x * vInput.z;
            vTmp2 = vInput.y * vInput.w;
            vReturnMatrix[2, 0] = 2.0f * (vTmp1 - vTmp2) * vInvs;
            vReturnMatrix[0, 2] = 2.0f * (vTmp1 + vTmp2) * vInvs;
            vTmp1 = vInput.y * vInput.z;
            vTmp2 = vInput.x * vInput.w;
            vReturnMatrix[2, 1] = 2.0f * (vTmp1 + vTmp2) * vInvs;
            vReturnMatrix[1, 2] = 2.0f * (vTmp1 - vTmp2) * vInvs;
            return vReturnMatrix;
        }
        public static Matrix<float> NonNormalizationQuatToMatrix(BodyFrame.Vect4 vInput)
        {
            Matrix<float> vReturnMatrix = Matrix<float>.Build.Dense(3, 3);
            float vSqw = vInput.w * vInput.w;
            float vSqx = vInput.x * vInput.x;
            float vSqy = vInput.y * vInput.y;
            float vSqz = vInput.z * vInput.z;

            // since sqw + sqx + sqy + sqz =1/invs*invs
            vReturnMatrix[0, 0] = (vSqx - vSqy - vSqz + vSqw);
            vReturnMatrix[1, 1] = (-vSqx + vSqy - vSqz + vSqw);
            vReturnMatrix[2, 2] = (-vSqx - vSqy + vSqz + vSqw);

            float vXY = vInput.x * vInput.y;
            float vWZ = vInput.z * vInput.w;
            float vWY = vInput.w * vInput.y;
            float vXZ = vInput.x * vInput.z;
            float vYZ = vInput.y * vInput.z;
            float vWX = vInput.x * vInput.w;
            vReturnMatrix[1, 0] = 2.0f * (vXY + vWZ);
            vReturnMatrix[0, 1] = 2.0f * (vXY - vWZ);

            vReturnMatrix[2, 0] = 2.0f * (vXZ - vWY);
            vReturnMatrix[0, 2] = 2.0f * (vXZ + vWY);

            vReturnMatrix[2, 1] = 2.0f * (vYZ + vWX);
            vReturnMatrix[1, 2] = 2.0f * (vYZ - vWX);
            return vReturnMatrix;
        }

        public static Matrix<float> QuatToMatrix(Quaternion vInput)
        {
            Matrix<float> vReturnMatrix = Matrix<float>.Build.Dense(3, 3);
            float vSqw = vInput.w * vInput.w;
            float vSqx = vInput.x * vInput.x;
            float vSqy = vInput.y * vInput.y;
            float vSqz = vInput.z * vInput.z;

            // invs (inverse square length) is only required if quaternion is not already normalised
            float vInvs = 1 / (vSqx + vSqy + vSqz + vSqw);
            // since sqw + sqx + sqy + sqz =1/invs*invs
            vReturnMatrix[0, 0] = (vSqx - vSqy - vSqz + vSqw) * vInvs;
            vReturnMatrix[1, 1] = (-vSqx + vSqy - vSqz + vSqw) * vInvs;
            vReturnMatrix[2, 2] = (-vSqx - vSqy + vSqz + vSqw) * vInvs;

            float vTmp1 = vInput.x * vInput.y;
            float vTmp2 = vInput.z * vInput.w;
            vReturnMatrix[1, 0] = 2.0f * (vTmp1 + vTmp2) * vInvs;
            vReturnMatrix[0, 1] = 2.0f * (vTmp1 - vTmp2) * vInvs;

            vTmp1 = vInput.x * vInput.z;
            vTmp2 = vInput.y * vInput.w;
            vReturnMatrix[2, 0] = 2.0f * (vTmp1 - vTmp2) * vInvs;
            vReturnMatrix[0, 2] = 2.0f * (vTmp1 + vTmp2) * vInvs;
            vTmp1 = vInput.y * vInput.z;
            vTmp2 = vInput.x * vInput.w;
            vReturnMatrix[2, 1] = 2.0f * (vTmp1 + vTmp2) * vInvs;
            vReturnMatrix[1, 2] = 2.0f * (vTmp1 - vTmp2) * vInvs;
            return vReturnMatrix;
        }

        /// <summary>
        /// Returns a new instance of a Vect4 from a given input
        /// <seealso cref="http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm"/>
        /// </summary>
        /// <param name="vInput"></param>
        /// <returns></returns>
        public static BodyFrame.Vect4 Matrix3X3ToQuat(Matrix<float> vInput)
        {
            BodyFrame.Vect4 vReturnVect4;
            vReturnVect4.w = Mathf.Sqrt(1.0f + vInput[0, 0] + vInput[1, 1] + vInput[2, 2]) / 2.0f;
            float vW4 = (4.0f * vReturnVect4.w);
            vReturnVect4.x = (vInput[2, 1] - vInput[1, 2]) / vW4;
            vReturnVect4.y = (vInput[0, 2] - vInput[2, 0]) / vW4;
            vReturnVect4.z = (vInput[1, 0] - vInput[0, 1]) / vW4;
            return vReturnVect4;
        }

        /// <summary>
        /// Returns a new instance of a Vect4 from a given input
        /// <seealso cref="http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm"/>
        /// </summary>
        /// <param name="vInput"></param>
        /// <returns></returns>
        public static Quaternion Matrix3X3ToUnityQuat(Matrix<float> vInput)
        {
            Quaternion vReturnVect4;
            vReturnVect4.w = Mathf.Sqrt(1.0f + vInput[0, 0] + vInput[1, 1] + vInput[2, 2]) / 2.0f;
            float vW4 = (4.0f * vReturnVect4.w);
            vReturnVect4.x = (vInput[2, 1] - vInput[1, 2]) / vW4;
            vReturnVect4.y = (vInput[0, 2] - vInput[2, 0]) / vW4;
            vReturnVect4.z = (vInput[1, 0] - vInput[0, 1]) / vW4;
            return vReturnVect4;
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

        /// <summary>
        /// Converts a Vect4 to a PNI specified DCM. This incoming Vect4 needs to be in NED coordinates
        /// </summary>
        /// <param name="vInput"></param>
        /// <returns></returns>
        public static Matrix<float> PniDcmConversion(BodyFrame.Vect4 vInput)
        {
            Matrix<float> vDCM = Matrix<float>.Build.Dense(3, 3);
            float vSqw = vInput.w * vInput.w;
            float vSqx = vInput.x * vInput.x;
            float vSqy = vInput.y * vInput.y;
            float vSqz = vInput.z * vInput.z;
            //set diagonals for legibility first
            vDCM[0, 0] = (vSqx - vSqy - vSqz + vSqw);
            vDCM[1, 1] = (-vSqx + vSqy - vSqz + vSqw);
            vDCM[2, 2] = (-vSqx - vSqy + vSqz + vSqw);

            float vXZ = vInput.x * vInput.z;
            float vXY = vInput.x * vInput.y;
            float vYZ = vInput.y * vInput.z;
            float vYW = vInput.y * vInput.w;
            float vZW = vInput.z * vInput.w;

            vDCM[0, 1] = 2.0f * (vXY + vZW);
            vDCM[0, 2] = 2.0f * (vXZ - vYW);
            vDCM[1, 0] = 2.0f * (vXY - vZW);
            vDCM[1, 2] = 2.0f * (vYZ + vYW);
            vDCM[2, 0] = 2.0f * (vXZ + vYW);
            vDCM[2, 1] = 2.0f * (vYZ - vYW);

            return vDCM;
        }
    }
}
