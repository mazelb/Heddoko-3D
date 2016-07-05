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
        static string sArbipose    = "W";
        static string pose1 = sZombiepose;
        static string pose2 = sSoldierpose;
        static string pose3 = sArbipose;
        public Matrix<float> Eigvec;
        public Vector<float> Eigval = Vector<float>.Build.Dense(3,0);
        public Matrix<float> vec;
        Matrix<float> Mth = Matrix<float>.Build.Dense(3, 3, 0);
        //public int NForTest = 10;
        public Matrix<float> Parkfunc(Matrix<float> Rb, Matrix<float> Ra, string s)
        {
            Vector<float> a = Vector<float>.Build.Dense(3, 0);
            Vector<float> b = Vector<float>.Build.Dense(3, 0);
            Matrix<float> M = Matrix<float>.Build.Dense(3, 3, 0);
            if (s == "Add")
            {                
                Matrix<float> A = LogM(Ra);
                Matrix<float> B = LogM(Rb);
                a[0] = A[2, 1];
                a[1] = A[0, 2];
                a[2] = A[1, 0];
                b[0] = B[2, 1];
                b[1] = B[0, 2];
                b[2] = B[1, 0];
                Mth += RmatrixFormation(a, b);
                M = Mth;
                Console.WriteLine("M:   " + M);
            }
            else if (s == "Eig")
            {
                Matrix<float> MtxM = (Mth.Transpose()).Multiply(Mth);
                Evd<float> eigen = MtxM.Evd();
                var eigvec = eigen.EigenVectors;
                var eigval = eigen.EigenValues;
                Matrix<float> v = Matrix<float>.Build.Dense(3, 3, 0);
                v[0, 0] = 1.0F / Mathf.Sqrt((float)eigval[0].Real);
                v[1, 1] = 1.0F / Mathf.Sqrt((float)eigval[1].Real);
                v[2, 2] = 1.0F / Mathf.Sqrt((float)eigval[2].Real);
                M = ((eigvec.Multiply(v)).Multiply(eigvec.Transpose())).Multiply(Mth.Transpose());
                Eigvec = eigvec;
                vec = v;
                Eigval[0] = (float)eigval[0].Real;
                Eigval[1] = (float)eigval[1].Real;
                Eigval[2] = (float)eigval[2].Real;
            }
            return M;
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
                R[0, 0] = 1.0F; R[0, 1] = 0.0F                  ; R[0, 2] = 0.0F                  ;
                R[1, 0] = 0.0F; R[1, 1] = 1.0F * Mathf.Cos(EuAn); R[1, 2] =-1.0F * Mathf.Sin(EuAn);
                R[2, 0] = 0.0F; R[2, 1] = 1.0F * Mathf.Sin(EuAn); R[2, 2] = 1.0F * Mathf.Cos(EuAn);
            }
            else if (Axis == "Y")
            {
                R[0, 0] =  1.0F * Mathf.Cos(EuAn); R[0, 1] = 0.0F; R[0, 2] =  1.0F * Mathf.Sin(EuAn);
                R[1, 0] =  0.0F                  ; R[1, 1] = 1.0F; R[1, 2] =  0.0F                  ;
                R[2, 0] = -1.0F * Mathf.Sin(EuAn); R[2, 1] = 0.0F; R[2, 2] =  1.0F * Mathf.Cos(EuAn);
            }
            else if (Axis == "Z")
            {
                R[0, 0] = 1.0F * Mathf.Cos(EuAn); R[0, 1] =-1.0F * Mathf.Sin(EuAn); R[0, 2] = 0.0F;
                R[1, 0] = 1.0F * Mathf.Sin(EuAn); R[1, 1] = 1.0F * Mathf.Cos(EuAn); R[1, 2] = 0.0F;
                R[2, 0] = 0.0F                  ; R[2, 1] = 0.0F                  ; R[2, 2] = 1.0F;
            }
            return R;
        }
        public Vector<float> PoseToEulerAngles(string vPose)
        {
            Vector<float> vEulerAngles = Vector<float>.Build.Dense(3, 0);
            if (vPose == "T")
            {
                vEulerAngles[0] = 0.0F * K;      //X
                vEulerAngles[1] = 0.0F * K;      //Y
                vEulerAngles[2] = 0.0F * K;      //Z
            }
            else if (vPose == "Z")
            {
                vEulerAngles[0] =   0.0F * K; //X
                vEulerAngles[1] = -90.0F * K; //Y
                vEulerAngles[2] =   0.0F * K; //Z
            }
            else if (vPose == "S")
            {
                vEulerAngles[0] =  0.0F * K; //X
                vEulerAngles[1] =  0.0F * K; //Y
                vEulerAngles[2] =-90.0F * K; //Z
            }
            else if (vPose == "W")
            {
                vEulerAngles[0] = 45.0F * K; //X
                vEulerAngles[1] = 45.0F * K; //Y
                vEulerAngles[2] = 45.0F * K; //Z
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
