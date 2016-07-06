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
        public static float K = 2.0F * Mathf.PI / 360;
        public static Matrix<float> sA = Matrix<float>.Build.Dense(9, 2);
        public static Matrix<float> sb = Matrix<float>.Build.Dense(9, 1);
        static string sTpose       = "T";
        static string sZombiepose  = "Z";
        static string sSoldierpose = "S";
        public static string pose1 = sZombiepose;
        public static string pose2 = sSoldierpose;
        /////////////////////////--------Principal Method---------------//////////////////////////////////
        public Matrix<float> Shiufunc(Matrix<float> B1, string pose1, Matrix<float> B2, string pose2)
        {
            Vector<float> vEulerA1Pose = PoseToEulerAngles(pose1);
            Vector<float> vEulerA2Pose = PoseToEulerAngles(pose2);
            Matrix<float> SX1 = EulerAngleToRotationMatrix(vEulerA1Pose[0], "X");
            Matrix<float> SY1 = EulerAngleToRotationMatrix(vEulerA1Pose[1], "Y");
            Matrix<float> SZ1 = EulerAngleToRotationMatrix(vEulerA1Pose[2], "Z");

            Matrix<float> SX2 = EulerAngleToRotationMatrix(vEulerA2Pose[0], "X");
            Matrix<float> SY2 = EulerAngleToRotationMatrix(vEulerA2Pose[1], "Y");
            Matrix<float> SZ2 = EulerAngleToRotationMatrix(vEulerA2Pose[2], "Z");         
              
            Matrix<float> vAMatrixPose1 = SY1.Multiply(SX1.Multiply(SZ1));
            Matrix<float> vAMatrixPose2 = SY2.Multiply(SX2.Multiply(SZ2));
            Matrix<float> vBMatrixSensor1 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> vBMatrixSensor2 = Matrix<float>.Build.Dense(3, 3, 0);
            vBMatrixSensor1 = B1;
            vBMatrixSensor2 = B2;

            Matrix<float> vX = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> Xp1 = Matrix<float>.Build.DenseIdentity(3, 3);
            Matrix<float> Xp2 = Matrix<float>.Build.DenseIdentity(3, 3);
            Matrix<float> Ra = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> Beta = Matrix<float>.Build.Dense(4, 1);
                
            Vector<float> vRotationAxisA1 = RotationAxis(vAMatrixPose1);
            Vector<float> vRotationAxisA2 = RotationAxis(vAMatrixPose2);
            Vector<float> vRotationAxisB1 = RotationAxis(vBMatrixSensor1);
            Vector<float> vRotationAxisB2 = RotationAxis(vBMatrixSensor2);
            Vector<float> ka1 = vRotationAxisA1.Normalize(2);
            Vector<float> ka2 = vRotationAxisA2.Normalize(2);

            Vector<float> vVectorPerpenToa1b1Plan = CrossProduct(vRotationAxisB1, vRotationAxisA1);
            Vector<float> vVectorPerpenToa2b2Plan = CrossProduct(vRotationAxisB2, vRotationAxisA2);
            Vector<float> k1 = vVectorPerpenToa1b1Plan.Normalize(2);
            Vector<float> k2 = vVectorPerpenToa2b2Plan.Normalize(2);
            float ww1 = Mathf.Sqrt(vVectorPerpenToa1b1Plan.DotProduct(vVectorPerpenToa1b1Plan));
            float ww2 = Mathf.Sqrt(vVectorPerpenToa2b2Plan.DotProduct(vVectorPerpenToa2b2Plan));
            float W1 = Mathf.Atan2(ww1, vRotationAxisA1.DotProduct(vRotationAxisB1));
            float W2 = Mathf.Atan2(ww2, vRotationAxisA2.DotProduct(vRotationAxisB2));
            Xp1 = Xpreliminairy(W1, k1);
            Xp2 = Xpreliminairy(W2, k2);
            LinearSystem Axb = new LinearSystem();

            ShiuMatrix(ka1, Xp1);
            Axb.AddEquation(sA, sb);        
            ShiuMatrix(ka2, Xp2);
            Axb.AddEquation(sA, sb);     
            Beta = Axb.Solve();
            float theta1 = Mathf.Atan2(Beta[1, 0], Beta[0, 0]);
            float theta2 = Mathf.Atan2(Beta[3, 0], Beta[2, 0]);
            Matrix<float> RA1 = Xpreliminairy(theta1, ka1);
            Matrix<float> RA2 = Xpreliminairy(theta2, ka2);
            Matrix<float> R1  = RA1 * Xp1;
            Matrix<float> R2  = RA2 * Xp2;
            Matrix<float> R   = R1;
            return R;
        }
        /////////////////////////--------linear System Constructions Methods----------////////////////////
        static void ShiuMatrix(Vector<float> k, Matrix<float> Xp)
        {
            Vector<float> vPrelimVector1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> vPrelimVector2 = Vector<float>.Build.Dense(3, 0);
            Vector<float> vPrelimVector3 = Vector<float>.Build.Dense(3, 0);
            vPrelimVector1[0] = Xp[0, 0]; vPrelimVector1[1] = Xp[1, 0]; vPrelimVector1[2] = Xp[2, 0];
            vPrelimVector2[0] = Xp[0, 1]; vPrelimVector2[1] = Xp[1, 1]; vPrelimVector2[2] = Xp[2, 1];
            vPrelimVector3[0] = Xp[0, 2]; vPrelimVector3[1] = Xp[1, 2]; vPrelimVector3[2] = Xp[2, 2];

            Vector<float> n = CrossProduct(vPrelimVector1, k);
            Vector<float> o = CrossProduct(vPrelimVector2, k);
            Vector<float> a = CrossProduct(vPrelimVector3, k);

            sA[0, 0] = Xp[0, 0] - k[0] * vPrelimVector1.DotProduct(k);
            sA[1, 0] = Xp[0, 1] - k[0] * vPrelimVector2.DotProduct(k);
            sA[2, 0] = Xp[0, 2] - k[0] * vPrelimVector3.DotProduct(k);

            sA[3, 0] = Xp[1, 0] - k[1] * vPrelimVector1.DotProduct(k);
            sA[4, 0] = Xp[1, 1] - k[1] * vPrelimVector2.DotProduct(k);
            sA[5, 0] = Xp[1, 2] - k[1] * vPrelimVector3.DotProduct(k);

            sA[6, 0] = Xp[2, 0] - k[2] * vPrelimVector1.DotProduct(k);
            sA[7, 0] = Xp[2, 1] - k[2] * vPrelimVector2.DotProduct(k);
            sA[8, 0] = Xp[2, 2] - k[2] * vPrelimVector3.DotProduct(k);

            sA[0, 1] = -n[0];
            sA[1, 1] = -o[0];
            sA[2, 1] = -a[0];

            sA[3, 1] = -n[1];
            sA[4, 1] = -o[1];
            sA[5, 1] = -a[1];

            sA[6, 1] = -n[2];
            sA[7, 1] = -o[2];
            sA[8, 1] = -a[2];

            sb[0, 0] = -k[0] * vPrelimVector1.DotProduct(k);
            sb[1, 0] = -k[0] * vPrelimVector2.DotProduct(k);
            sb[2, 0] = -k[0] * vPrelimVector3.DotProduct(k);

            sb[3, 0] = -k[1] * vPrelimVector1.DotProduct(k);
            sb[4, 0] = -k[1] * vPrelimVector2.DotProduct(k);
            sb[5, 0] = -k[1] * vPrelimVector3.DotProduct(k);

            sb[6, 0] = -k[2] * vPrelimVector1.DotProduct(k);
            sb[7, 0] = -k[2] * vPrelimVector2.DotProduct(k);
            sb[8, 0] = -k[2] * vPrelimVector3.DotProduct(k);
        }
        public Matrix<float> Xpreliminairy(float AngleOfRotation, Vector<float> AxisOfRotation)
        {
            Matrix<float> Xp = Matrix<float>.Build.Dense(3,3,0);
            //Skew part
            Xp = Skew(AxisOfRotation) * Mathf.Sin(AngleOfRotation) +
                 Matrix<float>.Build.DenseIdentity(3, 3) * Mathf.Cos(AngleOfRotation) +
                 (1.0F - Mathf.Cos(AngleOfRotation)) * VV(AxisOfRotation);
             return Xp;
        }
        public Vector<float> RotationAxis(Matrix<float> vRotationMatrix)
        {
            Vector<float> vRotationAxis = Vector<float>.Build.Dense(3, 0);
            Matrix<float> vScalarxLogRotationMatrix = Matrix<float>.Build.Dense(3, 3, 0);            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    vScalarxLogRotationMatrix[i, j] = (vRotationMatrix[i, j] - vRotationMatrix[j, i]); 
                }
            }
            vRotationAxis[0] = vScalarxLogRotationMatrix[2, 1];
            vRotationAxis[1] = vScalarxLogRotationMatrix[0, 2];
            vRotationAxis[2] = vScalarxLogRotationMatrix[1, 0];
            vRotationAxis    = vRotationAxis.Normalize(2);
            return vRotationAxis;
        }
        /////////////////////////-------Euler and Matrix Methods--------////////////////////////////////
        public Vector<float> PoseToEulerAngles(string vPose)
        {
            Vector<float> vEulerAngles = Vector<float>.Build.Dense(3, 0);
            if (vPose == "T")
            {
                vEulerAngles[0] = 0.0F * K;//X
                vEulerAngles[1] = 0.0F * K;//Y
                vEulerAngles[2] = 0.0F * K;//Z
            }
            else if (vPose == "Z")
            {
                vEulerAngles[0] =  0.0F * K;//X    90.0F*K
                vEulerAngles[1] =-90.0F * K;//Y
                vEulerAngles[2] =  0.0F * K;//Z
            }
            else if (vPose == "S")
            {
                vEulerAngles[0] =   0.0F  * K;//X
                vEulerAngles[1] =   0.0F  * K;//Y
                vEulerAngles[2] = -90.0F  * K;//Z
            }
            else if (vPose == "W")
            {
                vEulerAngles[0] =   0.0F * K; //X
                vEulerAngles[1] = -45.0F * K; //Y
                vEulerAngles[2] =   0.0F * K; //Z
            }
            return vEulerAngles;
        }
        public static Matrix<float> EulerAngleToRotationMatrix(float EuAn, string Axis)
        {            
            Matrix<float> R = Matrix<float>.Build.DenseIdentity(3);
            if (Axis == "X")
            {
                R[0, 0] = 1.0F; R[0, 1] =  0.0F;                    R[0, 2] =  0.0F;
                R[1, 0] = 0.0F; R[1, 1] =  1.0F * Mathf.Cos(EuAn) ; R[1, 2] = -1.0F * Mathf.Sin(EuAn);
                R[2, 0] = 0.0F; R[2, 1] =  1.0F * Mathf.Sin(EuAn) ; R[2, 2] =  1.0F * Mathf.Cos(EuAn);
            }
            else if (Axis == "Y")
            {
                R[0, 0] = 1.0F * Mathf.Cos(EuAn);   R[0, 1] = 0.0F; R[0, 2] =  1.0F * Mathf.Sin(EuAn);
                R[1, 0] = 0.0F;                     R[1, 1] = 1.0F; R[1, 2] =  0.0F;
                R[2, 0] =-1.0F * Mathf.Sin(EuAn);   R[2, 1] = 0.0F; R[2, 2] =  1.0F * Mathf.Cos(EuAn);
            }
            else if (Axis == "Z")
            {
                R[0, 0] =  1.0F * Mathf.Cos(EuAn); R[0, 1] = -1.0F * Mathf.Sin(EuAn); R[0, 2] = 0.0F;
                R[1, 0] =  1.0F * Mathf.Sin(EuAn); R[1, 1] =  1.0F * Mathf.Cos(EuAn); R[1, 2] = 0.0F;
                R[2, 0] =  0.0F;                   R[2, 1] =  0.0F;                   R[2, 2] = 1.0F;
            }
            return R;
        }
        ////////////////////////--------Helping Methods--------////////////////////////////////////////
        public Matrix<float> UtoM(Matrix4x4 U)
        {
            Matrix<float> M = Matrix<float>.Build.Dense(4, 4);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    M[i, j] = U[i, j];
                }
            }
            return M;
        }
        public Matrix4x4 MtoU(Matrix<float> M)
        {           
            Matrix4x4 U = Matrix4x4.zero;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    U[i, j] = M[i, j];
                }
            }
            U[3, 3] = 1F;
            return U;
        }
        public static Vector<float> CrossProduct(Vector<float> u, Vector<float> v)
        {         
            Vector<float> w = Vector<float>.Build.Dense(3, 0);
            w[0] = (v[2] * u[1] - v[1] * u[2]);
            w[1] = -(v[2] * u[0] - v[0] * u[2]);
            w[2] = (v[1] * u[0] - v[0] * u[1]);
            return w;
        }
        public static void Print(Matrix<float> M)
        {
            Matrix<double> Mr = Matrix<double>.Build.Dense(3, 3, 0);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Mr[i, j] = Math.Round(M[i, j], 3);
                }
            }
            Console.WriteLine(Mr);
        }
        public Matrix<float> Skew(Vector<float> k)
        {
            Matrix < float > sk= Matrix<float>.Build.Dense(3, 3, 0);
            sk[0, 1] = -k[2];
            sk[0, 2] =  k[1];
            sk[1, 0] =  k[2];
            sk[1, 2] = -k[0];  
            sk[2, 0] = -k[1];
            sk[2, 1] =  k[0]; 
            return sk;
        }
        public Matrix<float> VV(Vector<float> k)
        {
            Matrix<float> vv = Matrix<float>.Build.Dense(3, 3, 0);
            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    vv[i, j] = k[i] * k[j];
                }
            }
            return vv;
        }
    }
}