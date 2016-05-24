using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Distributions;
namespace Calibration1.CalibrationTransformation
{
    public class ParkTransform
    {
        public static float K = 2.0F * Mathf.PI / 360;
        static string sTpose       = "T";
        static string sZombiepose  = "Z";
        static string sSoldierpose = "S";
        static string pose1 = sZombiepose;
        static string pose2 = sSoldierpose;
        public float TestTransfXAngle = 0.0F;
        public float TestTransfYAngle = 0.0F;
        public float TestTransfZAngle = 0.0F;
        public int NForTest = 10;
        public Matrix<float> Parkfunc(Vector<float> RawSensorsEulerAngle1, Vector<float> RawSensorsEulerAngle2)
        {
            ///Euler angles for Pose1 and Pose2
            Vector<float> Pose1ToEulerAngles = PoseToEulerAngles(pose1);
            Vector<float> Pose2ToEulerAngles = PoseToEulerAngles(pose2);
            ///Rotation matrix for Pose1  
            Matrix<float> Rx = EulerAngleToRotationMatrix(Pose1ToEulerAngles[0], "X");
            Matrix<float> Ry = EulerAngleToRotationMatrix(Pose1ToEulerAngles[1], "Y");
            Matrix<float> Rz = EulerAngleToRotationMatrix(Pose1ToEulerAngles[2], "Z");
            Matrix<float> Ra1 = Rz.Multiply(Ry.Multiply(Rx));
            ///Rotation matrix for Pose2
            Rx = EulerAngleToRotationMatrix(Pose2ToEulerAngles[0], "X");
            Ry = EulerAngleToRotationMatrix(Pose2ToEulerAngles[1], "Y");
            Rz = EulerAngleToRotationMatrix(Pose2ToEulerAngles[2], "Z");
            Matrix<float> Ra2 = Rz.Multiply(Ry.Multiply(Rx));
            ///for Sensors raw Data
            Matrix<float> Rb1 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> Rb2 = Matrix<float>.Build.Dense(3, 3, 0);
            ///for Sensors raw Data set 1
            Rx = EulerAngleToRotationMatrix(RawSensorsEulerAngle1[0], "X");
            Ry = EulerAngleToRotationMatrix(RawSensorsEulerAngle1[1], "Y");
            Rz = EulerAngleToRotationMatrix(RawSensorsEulerAngle1[2], "Z");
            Rb1 = Rz.Multiply(Ry.Multiply(Rx));
            ///for Sensors raw Data set 2
            Rx = EulerAngleToRotationMatrix(RawSensorsEulerAngle2[0], "X");
            Ry = EulerAngleToRotationMatrix(RawSensorsEulerAngle2[1], "Y");
            Rz = EulerAngleToRotationMatrix(RawSensorsEulerAngle2[2], "Z");
            Rb2 = Rz.Multiply(Ry.Multiply(Rx));
            ///LogMatrix
            Matrix<float> A1 = LogM(Ra1);
            Matrix<float> A2 = LogM(Ra2);
            Matrix<float> B1 = LogM(Rb1);
            Matrix<float> B2 = LogM(Rb2);
            ///Rotation axis
            Vector<float> a1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> a2 = Vector<float>.Build.Dense(3, 0);
            Vector<float> b1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> b2 = Vector<float>.Build.Dense(3, 0);
            ///pose  1
            a1[0] = A1[2, 1];
            a1[1] = A1[0, 2];
            a1[2] = A1[1, 0];
            ///pose  2
            a2[0] = A2[2, 1];
            a2[1] = A2[0, 2];
            a2[2] = A2[1, 0];
            ///sensor data set 1
            b1[0] = B1[2, 1];
            b1[1] = B1[1, 0];
            b1[2] = B1[0, 2];
            ///sensor data set 2
            b2[0] = B2[2, 1];
            b2[1] = B2[1, 0];
            b2[2] = B2[0, 2];
            ///Matrix formation
            Matrix<float> M = Matrix<float>.Build.Dense(3, 3, 0);
            M += RmatrixFormation(a1, b1);
            M += RmatrixFormation(a2, b2);
            ///Eigen values decomposition
            Matrix<float> MtxM = (M.Transpose()).Multiply(M);
            Evd<float> eigen = MtxM.Evd();
            var eigvec = eigen.EigenVectors;
            var eigval = eigen.EigenValues;
            return M;
        }
        public Matrix<float> TestFunction()
        {
            float randx1;
            float randy1;
            float randz1;
            float randx2;
            float randy2;
            float randz2;
            ///Euler angles for Pose1 and Pose2
            Vector<float> Pose1ToEulerAngles = PoseToEulerAngles(pose1);
            Vector<float> Pose2ToEulerAngles = PoseToEulerAngles(pose2);
            ///Fake transformation sensor data
            Matrix<float> RX = EulerAngleToRotationMatrix(TestTransfXAngle, "X");
            Matrix<float> RY = EulerAngleToRotationMatrix(TestTransfYAngle, "Y");
            Matrix<float> RZ = EulerAngleToRotationMatrix(TestTransfZAngle, "Z");
            Matrix<float> RTest = RZ.Multiply(RY.Multiply(RX));
            Matrix<float> invRTest = RTest.Transpose();
            Console.WriteLine("----------RTest:----------");
            Console.WriteLine(RTest);
            Console.WriteLine("--------------------------");
            ///Rotation axis
            Vector<float> a1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> a2 = Vector<float>.Build.Dense(3, 0);
            Vector<float> b1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> b2 = Vector<float>.Build.Dense(3, 0);
            ///Matrix to be Decompose in term of its eigen vectors 
            Matrix<float> M = Matrix<float>.Build.Dense(3, 3, 0);
            for (int i = 0; i < NForTest; i++)
            {
                ///Random perturbation
                randx1 = (float)Normal.Sample(0.0, 3.0);
                randy1 = (float)Normal.Sample(0.0, 3.0);
                randz1 = (float)Normal.Sample(0.0, 3.0);
                randx2 = (float)Normal.Sample(0.0, 3.0);
                randy2 = (float)Normal.Sample(0.0, 3.0);
                randz2 = (float)Normal.Sample(0.0, 3.0);
                ///for Pose1  
                Matrix<float> Rx = EulerAngleToRotationMatrix(Pose1ToEulerAngles[0] + randx1, "X");
                Matrix<float> Ry = EulerAngleToRotationMatrix(Pose1ToEulerAngles[1] + randy1, "Y");
                Matrix<float> Rz = EulerAngleToRotationMatrix(Pose1ToEulerAngles[2] + randz1, "Z");
                Matrix<float> Ra1 = Rz.Multiply(Ry.Multiply(Rx));
                ///for Pose2
                Rx = EulerAngleToRotationMatrix(Pose2ToEulerAngles[0] + randx2, "X");
                Ry = EulerAngleToRotationMatrix(Pose2ToEulerAngles[1] + randy2, "Y");
                Rz = EulerAngleToRotationMatrix(Pose2ToEulerAngles[2] + randz2, "Z");
                Matrix<float> Ra2 = Rz.Multiply(Ry.Multiply(Rx));
                ///Rotation matrix 
                Matrix<float> Rb1 = invRTest * Ra1 * RTest;
                Matrix<float> Rb2 = invRTest * Ra2 * RTest;
                ///LogMatrix
                Matrix<float> A1 = LogM(Ra1);
                Matrix<float> A2 = LogM(Ra2);
                Matrix<float> B1 = LogM(Rb1);
                Matrix<float> B2 = LogM(Rb2);
                ///pose  1
                a1[0] = A1[2, 1];
                a1[1] = A1[0, 2];
                a1[2] = A1[1, 0];
                ///pose  2
                a2[0] = A2[2, 1];
                a2[1] = A2[0, 2];
                a2[2] = A2[1, 0];
                ///sensor data set 1
                b1[0] = B1[2, 1];
                b1[1] = B1[1, 0];
                b1[2] = B1[0, 2];
                ///sensor data set 2
                b2[0] = B2[2, 1];
                b2[1] = B2[1, 0];
                b2[2] = B2[0, 2];
                ///Matrix formation                
                M += RmatrixFormation(a1, b1);
                M += RmatrixFormation(a2, b2);
            }
            ///Eigen values decomposition
            Matrix<float> MtxM = (M.Transpose()).Multiply(M);
            Evd<float> eigen = MtxM.Evd();
            var eigvec = eigen.EigenVectors;
            var eigval = eigen.EigenValues;
            Matrix<float> v = Matrix<float>.Build.Dense(3, 3, 0);
            v[0, 0] = 1.0F / Mathf.Sqrt((float)eigval[0].Real);
            v[1, 1] = 1.0F / Mathf.Sqrt((float)eigval[1].Real);
            v[2, 2] = 1.0F / Mathf.Sqrt((float)eigval[2].Real);
            Matrix<float> R = ((eigvec.Multiply(v)).Multiply(eigvec.Transpose())).Multiply(M.Transpose());            
            return R;
        }
        public Matrix<float> LogM(Matrix<float> R)
        {
            Matrix<float> M = Matrix<float>.Build.Dense(3, 3, 0);
            float Tr = M[0, 0] + M[1, 1] + M[2, 2];
            float theta = Mathf.Acos((Tr - 1.0F) / 2.0F);
            if (theta != 0)
            {
                M = theta * (R - R.Transpose()) / (2.0F * Mathf.Sin(theta));
            }
            else if (Mathf.Abs(Mathf.Abs(theta) - Mathf.PI) < 0.01)
            {
                Console.WriteLine("PROBLEM!!!!!!!! THETA=PI OR -PI THE FROBENIUS LogM must be implemented");
                Console.WriteLine("Checkout https://en.wikipedia.org/wiki/Axis%E2%80%93angle_representation");
            }
            return M;
        }
        public static Matrix<float> EulerAngleToRotationMatrix(float EuAn, string Axis)
        {
            Matrix<float> R = Matrix<float>.Build.DenseIdentity(3);
            if (Axis == "X")
            {
                R[0, 0] = 1.0F; R[0, 1] = 0.0F; R[0, 2] = 0.0F;
                R[1, 0] = 0.0F; R[1, 1] = 1.0F * Mathf.Cos(EuAn); R[1, 2] = 1.0F * Mathf.Sin(EuAn);
                R[2, 0] = 0.0F; R[2, 1] =-1.0F * Mathf.Sin(EuAn); R[2, 2] = 1.0F * Mathf.Cos(EuAn);
            }
            else if (Axis == "Y")
            {
                R[0, 0] = 1.0F * Mathf.Cos(EuAn); R[0, 1] = 0.0F; R[0, 2] = -1.0F * Mathf.Sin(EuAn);
                R[1, 0] = 0.0F ; R[1, 1] = 1.0F; R[1, 2] = 0.0F;
                R[2, 0] = 1.0F * Mathf.Sin(EuAn); R[2, 1] = 0.0F; R[2, 2] = 1.0F * Mathf.Cos(EuAn);
            }
            else if (Axis == "Z")
            {
                R[0, 0] = 1.0F * Mathf.Cos(EuAn); R[0, 1] = 1.0F * Mathf.Sin(EuAn); R[0, 2] = 0.0F;
                R[1, 0] = -1.0F * Mathf.Sin(EuAn); R[1, 1] = 1.0F * Mathf.Cos(EuAn); R[1, 2] = 0.0F;
                R[2, 0] = 0.0F; R[2, 1] = 0.0F; R[2, 2] = 1.0F;
            }
            return R;
        }
        public Vector<float> PoseToEulerAngles(string vPose)
        {
            Vector<float> vEulerAngles = Vector<float>.Build.Dense(3, 0);
            if (vPose == "T")
            {
                vEulerAngles[0] = 0.0F;      //X
                vEulerAngles[1] = 0.0F;      //Y
                vEulerAngles[2] = 0.0F;      //Z
            }
            else if (vPose == "Z")
            {
                vEulerAngles[0] = 90.0F * K; //X
                vEulerAngles[1] = 0.0F;      //Y
                vEulerAngles[2] = 0.0F;      //Z
            }
            else if (vPose == "S")
            {
                vEulerAngles[0] = 0.0F;      //X
                vEulerAngles[1] = 90.0F * K; //Y
                vEulerAngles[2] = 0.0F;      //Z
            }
            return vEulerAngles;
        }
        public Matrix<float> RmatrixFormation(Vector<float> a, Vector<float> b)
        {
            Matrix<float> M = Matrix<float>.Build.Dense(3, 3, 0);
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    M[i, j] = b[i] * a[j];
                }
            }
            return M;
        }
        public static void Print(Matrix<float> M, int n)
        {
            Matrix<double> Mr = Matrix<double>.Build.Dense(3, 3, 0);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Mr[i, j] = Math.Round(M[i, j], n);
                }
            }
            Console.WriteLine(Mr);
        }
    }
}
