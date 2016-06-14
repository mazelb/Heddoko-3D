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
            //Test1_ShiuTransform();
            //Test_ShiuTransform_From_RawData();
        }
        static public void Test1_ShiuTransform()
        {
            bool Test = false;
            Vector<float> RawSensDataAngles1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> RawSensDataAngles2 = Vector<float>.Build.Dense(3, 0);
            Matrix<float> Xtmp = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> invXtmp = Matrix<float>.Build.Dense(3, 3, 0);
            Vector<float> rand1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> rand2 = Vector<float>.Build.Dense(3, 0);
            ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform();
            Matrix<float> M = Matrix<float>.Build.Dense(3, 2, 0);
            /* M = Case("Case1upperarm");
             RawSensDataAngles1 = M.Column(0);
             RawSensDataAngles2 = M.Column(1);
             Xtmp = AvatarToDataSensorsTransform.Shiufunc(RawSensDataAngles1, RawSensDataAngles2);
             Console.WriteLine("----------------------------------------------------------------");
             Console.ReadLine();
             M = Case("Case2upperarm");
             RawSensDataAngles1 = M.Column(0);
             RawSensDataAngles2 = M.Column(1);
             Xtmp = AvatarToDataSensorsTransform.Shiufunc(RawSensDataAngles1, RawSensDataAngles2);
             Console.WriteLine("----------------------------------------------------------------");
             Console.ReadLine();*/
             M = Case("Case1forearm");
             RawSensDataAngles1 = M.Column(0);
             RawSensDataAngles2 = M.Column(1);
             Xtmp = AvatarToDataSensorsTransform.Shiufunc(RawSensDataAngles1, RawSensDataAngles2);
             Console.WriteLine("----------------------------------------------------------------");
             Console.ReadLine();
            /* M = Case("Case2forearm");
             RawSensDataAngles1 = M.Column(0);
             RawSensDataAngles2 = M.Column(1);
             Xtmp = AvatarToDataSensorsTransform.Shiufunc(RawSensDataAngles1, RawSensDataAngles2);
             Console.WriteLine("----------------------------------------------------------------");
             Console.ReadLine();*/
        }
        static public void Test_ShiuTransform_From_RawData()
        {
            ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform();
            string namefileZ = "C:\\Users\\Simon\\Documents\\Sython\\RawDA\\Zpose.txt";
            string namefileS = "C:\\Users\\Simon\\Documents\\Sython\\RawDA\\Spose.txt";
            Vector<float> RawSensDataAngles1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> RawSensDataAngles2 = Vector<float>.Build.Dense(3, 0);
            Matrix<float> Angles = Program.Read_RawData(namefileZ, namefileS);
            Console.WriteLine(Angles);
            //Matrix<float> X;
            Matrix<float> R = Matrix<float>.Build.Dense(3,3,0);
            for (int i = 0; i < 3; i++)
            {
                RawSensDataAngles1 = Angles.Column(i, 0, 3);
                RawSensDataAngles2 = Angles.Column(i, 3, 3);
                R += AvatarToDataSensorsTransform.Shiufunc(RawSensDataAngles1, RawSensDataAngles2);
                Console.WriteLine("------Rmoy-------");
                ShiuTransform.Print(R/(i+1.0F));
            }            
            Console.ReadLine();
        }
        static public void Test_ShiuTransform_And_Park()
        {
            ShiuTransform s = new ShiuTransform();
            ParkTransform p = new ParkTransform();

            Matrix<float> B1 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> B2 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> B3 = Matrix<float>.Build.Dense(3, 3, 0);

            B1[0, 0] = -0.04951F;  B1[0, 1] = 0.06666F; B1[0, 2] = -0.99655F;
            B1[1, 0] =  0.06404F;  B1[1, 1] = 0.99593F; B1[1, 2] =  0.06343F;
            B1[2, 0] =  0.99672F;  B1[2, 1] =-0.06067F; B1[2, 2] = -0.05358F;

            B2[0, 0] =-0.09062F; B2[0, 1] = 0.70163F; B2[0, 2] =-0.70676F;
            B2[1, 0] =-0.90508F; B2[1, 1] = 0.23806F; B2[1, 2] = 0.35237F;
            B2[2, 0] = 0.41548F; B2[2, 1] = 0.67160F; B2[2, 2] = 0.61345F;

            /*B1[0, 0] = -0.00526F; B1[0, 1] =  0.07006F; B1[0, 2] = - 0.99753F;
            B1[1, 0] =  0.13790F; B1[1, 1] =  0.98806F; B1[1, 2] =   0.06867F;
            B1[2, 0] =  0.99043F; B1[2, 1] = -0.13720F; B1[2, 2] = - 0.01486F;  

            B2[0, 0] = 0.26827F; B2[0, 1] = 0.84552F;   B2[0, 2] =-0.46166F;
            B2[1, 0] =-0.94790F; B2[1, 1] = 0.31715F;   B2[1, 2] = 0.03002F;
            B2[2, 0] = 0.17180F; B2[2, 1] = 0.42955F;   B2[2, 2] = 0.88655F;*/


            /*B1[0, 0] = -0.04951F; B1[0, 1] =  0.06666F; B1[0, 2] = -0.99655F;
            B1[1, 0] =  0.06404F; B1[1, 1] =  0.99593F; B1[1, 2] =  0.06343F;
            B1[2, 0] =  0.99672F; B1[2, 1] = -0.06067F; B1[2, 2] = -0.05358F;

            B2[0, 0] = -0.09062F; B2[0, 1] = 0.70163F; B2[0, 2] = -0.70676F;
            B2[1, 0] = -0.90508F; B2[1, 1] = 0.23806F; B2[1, 2] =  0.35237F;
            B2[2, 0] =  0.41548F; B2[2, 1] = 0.67160F; B2[2, 2] =  0.61345F; */

            K = 2.0F * Mathf.PI / 360.0F;
           
            Vector<float> vEulerA1Pose = p.PoseToEulerAngles("Z");
            Vector<float> vEulerA2Pose = p.PoseToEulerAngles("S");
            Vector<float> vEulerA3Pose = p.PoseToEulerAngles("W");
            //Console.WriteLine(vEulerA1Pose);
            //Console.WriteLine(vEulerA2Pose);
            //Console.WriteLine(vEulerA3Pose);
            Matrix<float> Rx1 = ParkTransform.EulerAngleToRotationMatrix(vEulerA1Pose[0], "X");
            Matrix<float> Ry1 = ParkTransform.EulerAngleToRotationMatrix(vEulerA1Pose[1], "Y");
            Matrix<float> Rz1 = ParkTransform.EulerAngleToRotationMatrix(vEulerA1Pose[2], "Z");

            Matrix<float> Rx2 = ParkTransform.EulerAngleToRotationMatrix(vEulerA2Pose[0], "X");
            Matrix<float> Ry2 = ParkTransform.EulerAngleToRotationMatrix(vEulerA2Pose[1], "Y");
            Matrix<float> Rz2 = ParkTransform.EulerAngleToRotationMatrix(vEulerA2Pose[2], "Z");

            Matrix<float> Rx3 = ParkTransform.EulerAngleToRotationMatrix(vEulerA3Pose[0], "X");
            Matrix<float> Ry3 = ParkTransform.EulerAngleToRotationMatrix(vEulerA3Pose[1], "Y");
            Matrix<float> Rz3 = ParkTransform.EulerAngleToRotationMatrix(vEulerA3Pose[2], "Z");

            Matrix <float> A1 = Ry1.Multiply(Rx1.Multiply(Rz1));
            Matrix<float>  A2 = Ry2.Multiply(Rx2.Multiply(Rz2));
            Matrix<float>  A3 = Ry3.Multiply(Rx3.Multiply(Rz3));
            Console.WriteLine("-------------Id.Poses--------------------");
            ParkTransform.Print(A1, 3);
            ParkTransform.Print(A2, 3);
            ParkTransform.Print(A3, 3);
            Console.WriteLine("-----------------------------------------");
            Matrix<float> X = s.Shiufunc(B1, "Z", B2, "S");
            B3 =  X.Inverse().Multiply(A3.Multiply(X));
            Console.WriteLine("-------------B1,B2,B3--------------------");
            ParkTransform.Print(B1, 3);
            ParkTransform.Print(B2, 3);
            ParkTransform.Print(B3, 3);
            Console.WriteLine("-----------------------------------------");            
            Matrix<float> M = p.Parkfunc(B1,"Z");
            M = p.Parkfunc(B2, "S");
            M = p.Parkfunc(B3, "W");
            Matrix<float> C = Matrix<float>.Build.Dense(3, 3, 0);
            M = p.Parkfunc(C, "Eig");
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
        static public Matrix<float> Read_RawData(string NameFilePoseZ, string NameFilePoseS)
        {
            StreamReader reader = File.OpenText(NameFilePoseZ);
            string line = reader.ReadLine();
            string[] numbers = line.Split(',');
            Matrix<float> Angles = Matrix<float>.Build.Dense(6, 3, 0);
            //Zpose       
            for (int i = 0; i < 3; i++)//i:angles (X,Y,Z)
            {
                for (int j = 0; j < 3; j++)//j:angles values (start,mean,end) 
                {
                    Angles[i, j] = float.Parse(numbers[j + 3 * i]);
                }
            }
            reader = File.OpenText(NameFilePoseS);
            line = reader.ReadLine();
            numbers = line.Split(',');
            //Spose       
            for (int i = 0; i < 3; i++)//i:angles (X,Y,Z)
            {
                for (int j = 0; j < 3; j++)//j:angles values (start,mean,end) 
                {
                    Angles[3 + i, j] = float.Parse(numbers[j + 3 * i]);
                }
            }
            return Angles;
        }
        static public Matrix<float> Case(string s)
        {
            Matrix<float> V = Matrix<float>.Build.Dense(3, 2, 0);
            if (s == "Case1upperarm")
            {
                V[0, 0] = -0.070070F;
                V[1, 0] = -0.137210F;
                V[2, 0] = 1.590820F;
                V[0, 1] = -0.021360F;
                V[1, 1] = 1.246216F;
                V[2, 1] = 0.476928F;
            }
            else if (s == "Case2upperarm")
            {
                V[0, 0] = -0.074100F;
                V[1, 0] = -0.147710F;
                V[2, 0] = 1.564209F;
                V[0, 1] = -0.030640F;
                V[1, 1] = 1.247436F;
                V[2, 1] = 0.484131F;
            }
            else if (s == "Case1forearm")
            {
                V[0, 0] = -0.062500F;
                V[1, 0] = -0.058350F;
                V[2, 0] = 1.631836F;
                V[0, 1] = -0.357670F;
                V[1, 1] = 1.305542F;
                V[2, 1] = 0.889282F;
            }
            else if (s == "Case2forearm")
            {
                V[0, 0] = -0.077510F;
                V[1, 0] = -0.101810F;
                V[2, 0] = 1.586792F;
                V[0, 1] = -0.357300F;
                V[1, 1] = 1.319458F;
                V[2, 1] = 0.885010F;
            }
            return V;
        }
    }
}
