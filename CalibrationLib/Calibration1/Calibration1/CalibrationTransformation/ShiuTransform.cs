using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

namespace Calibration1.CalibrationTransformation
{
    public class ShiuTransform
    {
        public static Matrix<float> A = Matrix<float>.Build.Dense(9, 2, 0);
        public static Matrix<float> b = Matrix<float>.Build.Dense(9, 1, 0);
        public Matrix4x4 Shiufunc(float phi1, float psy1, float theta1, float phi2, float psy2, float theta2)
        {
            string pose1 = "T";
            string pose2 = "Z";
            //string pose3 = "S";

            Vector3 EulerA1 = AngleEulerPose(pose1); //?????
            Vector3 EulerA2 = AngleEulerPose(pose2); //?????

            Matrix4x4 A1 = RotationMatrix(EulerA1[0], EulerA1[1], EulerA1[2]);
            Matrix4x4 A2 = RotationMatrix(EulerA2[0], EulerA2[1], EulerA2[2]);

            Matrix4x4 B1 = RotationMatrix(phi1, psy1, theta1);
            Matrix4x4 B2 = RotationMatrix(phi2, psy2, theta2);

            Matrix4x4 TransformationMatrixX = Matrix4x4.identity;
            Matrix4x4 Xp1 = Matrix4x4.identity;
            Matrix4x4 Xp2 = Matrix4x4.identity;

            Vector3 RotationaxiA1 = RotationAxis(A1);
            Vector3 RotationaxiA2 = RotationAxis(A2);
            Vector3 RotationaxiB1 = RotationAxis(B1);
            Vector3 RotationaxiB2 = RotationAxis(B2);

            Vector3 Ka1crosskb1 = Vector3.Cross(RotationaxiA1, RotationaxiB1);
            Vector3 Ka2crosskb2 = Vector3.Cross(RotationaxiA2, RotationaxiB2);
            Vector3 k1 = Vector3.Normalize(Ka1crosskb1);
            Vector3 k2 = Vector3.Normalize(Ka2crosskb2);

            float W1 = Mathf.Atan2(Vector3.Magnitude(Ka1crosskb1), Vector3.Dot(RotationaxiA1, RotationaxiB1));
            float W2 = Mathf.Atan2(Vector3.Magnitude(Ka2crosskb2), Vector3.Dot(RotationaxiA2, RotationaxiB2));

            Xp1 = Xpreliminairy(W1, k1);
            Xp2 = Xpreliminairy(W2, k2);

            Matrix<float> A = Matrix<float>.Build.Dense(9, 2);
            Matrix<float> b = Matrix<float>.Build.Dense(9, 1);

            LinearSystem Axb = new LinearSystem(b, A);

            ShiuMatrix(k1, Xp1);
            Axb.Addequation(A, b);

            ShiuMatrix(k2, Xp2);
            Axb.Addequation(A, b);

            return TransformationMatrixX;
        }
        public Vector3 AngleEulerPose(string pose)
        {
            Vector3 E = Vector3.zero;
            if (pose.CompareTo("T") == 1)
            {
                E[0] = 0.0F;
                E[1] = 0.0F;
                E[2] = 0.0F;
            }
            else if (pose.CompareTo("Z") == 1)
            {
                E[0] = 0.0F;
                E[1] = 90.0F;
                E[2] = 0.0F;
            }
            else if (pose.CompareTo("S") == 1)
            {
                E[0] = 0.0F;
                E[1] = 0.0F;
                E[2] = 90.0F;
            }
            return E;
        }
        public Matrix4x4 RotationMatrix(float phi, float psy, float theta)
        {
            Matrix4x4 M = Matrix4x4.zero;
            M[0, 0] = Mathf.Cos(phi) * Mathf.Cos(psy) - Mathf.Sin(phi) * Mathf.Cos(theta) * Mathf.Sin(psy);
            M[0, 1] = Mathf.Cos(phi) * Mathf.Sin(psy) + Mathf.Sin(phi) * Mathf.Cos(theta) * Mathf.Sin(psy);
            M[0, 2] = Mathf.Cos(phi) * Mathf.Sin(theta);
            M[1, 0] = -Mathf.Sin(phi) * Mathf.Cos(psy) - Mathf.Cos(phi) * Mathf.Cos(theta) * Mathf.Sin(psy);
            M[1, 1] = -Mathf.Sin(phi) * Mathf.Sin(psy) + Mathf.Cos(phi) * Mathf.Cos(theta) * Mathf.Cos(psy);
            M[1, 2] = Mathf.Cos(phi) * Mathf.Sin(theta);
            M[2, 0] = Mathf.Sin(theta) * Mathf.Sin(psy);
            M[2, 1] = -Mathf.Sin(theta) * Mathf.Cos(psy);
            M[2, 2] = Mathf.Cos(theta);
            M[3, 3] = 1.0F;
            return M;
        }
        public Vector3 RotationAxis(Matrix4x4 A)
        {
            Vector3 k = Vector3.zero;
            Matrix4x4 ScalarxLogA = Matrix4x4.zero;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    ScalarxLogA[i, j] = A[i, j] - A[j, i];
                }
            }
            k[0] = ScalarxLogA[2, 1];
            k[1] = ScalarxLogA[0, 2];
            k[2] = ScalarxLogA[1, 0];
            k = Vector3.Normalize(k);
            return k;
        }
        public Matrix4x4 Xpreliminairy(float w, Vector3 k)
        {
            float A = 0.0F;
            Matrix4x4 Xp = Matrix4x4.zero;
            //Skew part
            Xp[1, 0] = k[2] * Mathf.Sin(w);
            Xp[0, 1] = -k[2] * Mathf.Sin(w);
            Xp[2, 0] = -k[1] * Mathf.Sin(w);
            Xp[0, 2] = k[1] * Mathf.Sin(w);
            Xp[2, 1] = k[0] * Mathf.Sin(w);
            Xp[1, 2] = -k[0] * Mathf.Sin(w);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    A = 0.0F;
                    if (i == j)
                    { A = 1.0F; } //just for Diagonal element
                    Xp[i, j] = Xp[i, j] + A * Mathf.Cos(w) + (1.0F - Mathf.Cos(w)) * k[i] * k[j];
                }
            }
            return Xp;
        }
        static void ShiuMatrix(Vector3 k,Matrix4x4 Xp)
        {
            Vector3 xx1 = new Vector3(Xp[0, 0], Xp[1, 0], Xp[2, 0]);
            Vector3 xx2 = new Vector3(Xp[0, 1], Xp[1, 1], Xp[2, 1]);
            Vector3 xx3 = new Vector3(Xp[0, 2], Xp[1, 2], Xp[2, 2]);

            Vector3 n = new Vector3(0f, 0f, 0f);
            Vector3 o = new Vector3(0f, 0f, 0f);
            Vector3 a = new Vector3(0f, 0f, 0f);

            n = Vector3.Cross(xx1, k);
            o = Vector3.Cross(xx2, k);
            a = Vector3.Cross(xx3, k);

            A[0, 0] = Xp[0, 0] - k[0] * Vector3.Dot(xx1, k);
            A[1, 0] = Xp[0, 1] - k[0] * Vector3.Dot(xx2, k);
            A[2, 0] = Xp[0, 2] - k[0] * Vector3.Dot(xx3, k);

            A[3, 0] = Xp[1, 0] - k[1] * Vector3.Dot(xx1, k);
            A[4, 0] = Xp[1, 1] - k[1] * Vector3.Dot(xx2, k);
            A[5, 0] = Xp[1, 2] - k[1] * Vector3.Dot(xx3, k);

            A[6, 0] = Xp[2, 0] - k[2] * Vector3.Dot(xx1, k);
            A[7, 0] = Xp[2, 1] - k[2] * Vector3.Dot(xx2, k);
            A[8, 0] = Xp[2, 2] - k[2] * Vector3.Dot(xx3, k);

            A[0, 1] = -n[0];
            A[1, 1] = -o[0];
            A[2, 1] = -a[0];

            A[3, 1] = -n[1];
            A[4, 1] = -o[1];
            A[5, 1] = -a[1];

            A[6, 1] = -n[2];
            A[7, 1] = -o[2];
            A[8, 1] = -a[2];

            //n = X[:, 1];
            //o = X[:, 2];
            //a = X[:, 3];

            b[0, 0] = -k[0] * Vector3.Dot(xx1, k);
            b[1, 0] = -k[0] * Vector3.Dot(xx2, k);
            b[2, 0] = -k[0] * Vector3.Dot(xx3, k);

            b[3, 0] = -k[1] * Vector3.Dot(xx1, k);
            b[4, 0] = -k[1] * Vector3.Dot(xx2, k);
            b[5, 0] = -k[1] * Vector3.Dot(xx3, k);

            b[6, 0] = -k[2] * Vector3.Dot(xx1, k);
            b[7, 0] = -k[2] * Vector3.Dot(xx2, k);
            b[8, 0] = -k[2] * Vector3.Dot(xx3, k);

        }
    }
}
