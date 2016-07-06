using System;
using System.Collections.Generic;
using System.Text;
using Calibration1.CalibrationTransformation;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using UnityEngine;
using System.IO;
namespace CalibrationTest
{
    class Program
    {
        public static float K;
        static void Main(string[] args)
        {
            Test_ShiuTransform_And_Park();
        }
        static public void Test_ShiuTransform_And_Park()
        {
            ShiuTransform s = new ShiuTransform();
            ParkTransform p = new ParkTransform();
            K = 2.0F * Mathf.PI / 360.0F;
            Matrix<float> B1 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> B2 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> B3 = Matrix<float>.Build.Dense(3, 3, 0);

            B1[0, 0] = -0.04951F; B1[0, 1] = 0.06666F; B1[0, 2] = -0.99655F;
            B1[1, 0] = 0.06404F; B1[1, 1] = 0.99593F; B1[1, 2] = 0.06343F;
            B1[2, 0] = 0.99672F; B1[2, 1] = -0.06067F; B1[2, 2] = -0.05358F;
            B2[0, 0] = -0.09062F; B2[0, 1] = 0.70163F; B2[0, 2] = -0.70676F;
            B2[1, 0] = -0.90508F; B2[1, 1] = 0.23806F; B2[1, 2] = 0.35237F;
            B2[2, 0] = 0.41548F; B2[2, 1] = 0.67160F; B2[2, 2] = 0.61345F;


            Vector<float> vEulerA1Pose = p.PoseToEulerAngles("Z");
            Vector<float> vEulerA2Pose = p.PoseToEulerAngles("S");
            Vector<float> vEulerA3Pose = p.PoseToEulerAngles("W");

            Matrix<float> Rx1 = ParkTransform.EulerAngleToRotationMatrix(vEulerA1Pose[0], "X");
            Matrix<float> Ry1 = ParkTransform.EulerAngleToRotationMatrix(vEulerA1Pose[1], "Y");
            Matrix<float> Rz1 = ParkTransform.EulerAngleToRotationMatrix(vEulerA1Pose[2], "Z");
            Matrix<float> Rx2 = ParkTransform.EulerAngleToRotationMatrix(vEulerA2Pose[0], "X");
            Matrix<float> Ry2 = ParkTransform.EulerAngleToRotationMatrix(vEulerA2Pose[1], "Y");
            Matrix<float> Rz2 = ParkTransform.EulerAngleToRotationMatrix(vEulerA2Pose[2], "Z");
            Matrix<float> Rx3 = ParkTransform.EulerAngleToRotationMatrix(vEulerA3Pose[0], "X");
            Matrix<float> Ry3 = ParkTransform.EulerAngleToRotationMatrix(vEulerA3Pose[1], "Y");
            Matrix<float> Rz3 = ParkTransform.EulerAngleToRotationMatrix(vEulerA3Pose[2], "Z");

            Matrix<float> A1 = Ry1.Multiply(Rx1.Multiply(Rz1));
            Matrix<float> A2 = Ry2.Multiply(Rx2.Multiply(Rz2));
            Matrix<float> A3 = Ry3.Multiply(Rx3.Multiply(Rz3));
            Console.WriteLine("-------------Id.Poses--------------------");
            ParkTransform.Print(A1, 3);
            ParkTransform.Print(A2, 3);
            ParkTransform.Print(A3, 3);
            Console.WriteLine("-----------------------------------------");
            Matrix<float> X = s.Shiufunc(B1, "Z", B2, "S");
            B3 = X.Inverse().Multiply(A3.Multiply(X));
            Console.WriteLine("-------------B1,B2,B3--------------------");
            ParkTransform.Print(B1, 3);
            ParkTransform.Print(B2, 3);
            ParkTransform.Print(B3, 3);
            Console.WriteLine("-----------------------------------------");
            Matrix<float> M = p.Parkfunc(B1, A1, "Add");
            M = p.Parkfunc(B2, A2, "Add");
            M = p.Parkfunc(B3, A3, "Add");
            Matrix<float> C = Matrix<float>.Build.Dense(3, 3, 0);
            M = p.Parkfunc(C,C, "Eig");
            Console.WriteLine("-------------X and M---------------------");
            ParkTransform.Print(X, 3);
            ParkTransform.Print(M, 3);
            Console.WriteLine("--------------A1 A2 and A3 (Rx)----------");
            Matrix<float> A1rx = X.Multiply(B1.Multiply(X.Inverse()));
            Matrix<float> A2rx = X.Multiply(B2.Multiply(X.Inverse()));
            Matrix<float> A3rx = X.Multiply(B3.Multiply(X.Inverse()));
            ParkTransform.Print(A1rx, 3);
            ParkTransform.Print(A2rx, 3);
            ParkTransform.Print(A3rx, 3);
            Console.WriteLine("--------------A1 A2 and A3 (Rm)----------");
            Matrix<float> A1rm = M.Multiply(B1.Multiply(M.Inverse()));
            Matrix<float> A2rm = M.Multiply(B2.Multiply(M.Inverse()));
            Matrix<float> A3rm = M.Multiply(B3.Multiply(M.Inverse()));
            ParkTransform.Print(A1rm, 3);
            ParkTransform.Print(A2rm, 3);
            ParkTransform.Print(A3rm, 3);
            Console.WriteLine("-----------------------------------------");
            Console.ReadLine();
        }
    }
}
