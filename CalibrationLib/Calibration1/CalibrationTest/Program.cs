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
        static void Main(string[] args)
        {
            Test1_ShiuTransform(); //from raw data;
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

/*
RawSensDataAngles1 = AvatarToDataSensorsTransform.PoseToEulerAngles(ShiuTransform.pose1);
            RawSensDataAngles2 = AvatarToDataSensorsTransform.PoseToEulerAngles(ShiuTransform.pose2);
            Console.WriteLine(RawSensDataAngles1);
            Console.WriteLine(RawSensDataAngles2);
            float p = 7.0F;
float rand = (float)Normal.Sample(0.0, p);
Console.WriteLine(rand);
            RawSensDataAngles1[0] = RawSensDataAngles1[0] + rand* ShiuTransform.K;
rand = (float)Normal.Sample(0.0, p);
            Console.WriteLine(rand);
            RawSensDataAngles1[1] = RawSensDataAngles1[1] + rand* ShiuTransform.K;
rand = (float)Normal.Sample(0.0, p);
            Console.WriteLine(rand);
            RawSensDataAngles1[2] = RawSensDataAngles1[2] + rand* ShiuTransform.K;

rand = (float)Normal.Sample(0.0, p);
            Console.WriteLine(rand);
            RawSensDataAngles2[0] = RawSensDataAngles2[0] + rand* ShiuTransform.K;
rand = (float)Normal.Sample(0.0, p);
            Console.WriteLine(rand);
            RawSensDataAngles2[1] = RawSensDataAngles2[1] + rand* ShiuTransform.K;
rand = (float)Normal.Sample(0.0, p);
            Console.WriteLine(rand);
            RawSensDataAngles2[2] = RawSensDataAngles2[2] + rand* ShiuTransform.K;*/
/*float K = 2.0F * Mathf.PI / 360;
            float x = 45.0F;
            float y = 55.0F;
            float z = 15.0F;
            AvatarToDataSensorsTransform.TestFunction("CleanRotation", x * K, y * K, z * K);  
            Console.WriteLine("-------------------Fake sensor data (B1 and B2):----------------");
            ShiuTransform.Print(B1);
            ShiuTransform.Print(B2);   
            Console.WriteLine("--------------Recoved initial values A1 and A2:-----------------");
            ShiuTransform.Print(A1);
            ShiuTransform.Print(A2);
            invXtmp = Xtmp.Inverse();
            Matrix<float> A1 = (Xtmp.Multiply(B1)).Multiply(invXtmp);
            Matrix<float> A2 = (Xtmp.Multiply(B2)).Multiply(invXtmp);       
            Matrix<float> B1 = AvatarToDataSensorsTransform.TestB1;
            Matrix<float> B2 = AvatarToDataSensorsTransform.TestB2;    
     */
/*
            Matrix<float> X    = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> invX = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> FakeDataSensors    = Matrix<float>.Build.Dense(6, 1);
            Vector<float> RawSensDataAngles1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> RawSensDataAngles2 = Vector<float>.Build.Dense(3, 0);
            //-----------------------------------------------------------------
            //Testing case Angles         
            ParkTransform AvatarToDataSensorsTransform = new ParkTransform();
            AvatarToDataSensorsTransform.TestTransfXAngle = 10.0F;
            AvatarToDataSensorsTransform.TestTransfYAngle = 70.0F;
            AvatarToDataSensorsTransform.TestTransfZAngle = 90.0F;

            //-----------------------------------------------------------------
            //Transformation computation
            X = AvatarToDataSensorsTransform.TestFunction();
            Console.WriteLine("Resulting estimation X transform");
            Console.WriteLine(X);
            Console.WriteLine("--------------------------");
            Console.ReadLine();   */
// X = AvatarToDataSensorsTransform.Parkfunc(RawSensDataAngles1, RawSensDataAngles2);
/*ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(Test);
float K  = 2.0F * Mathf.PI / 360;
float x  = 1.0F;
float y  = 1.0F;
float z  = 1.0F;
AvatarToDataSensorsTransform.TestFunction("CleanRotation", x*K, y*K, z*K);
Xtmp = AvatarToDataSensorsTransform.Shiufunc(RawSensDataAngles1, RawSensDataAngles2);
invXtmp = Xtmp.Inverse();
Matrix<float> B1 = AvatarToDataSensorsTransform.TestB1;
Matrix<float> B2 = AvatarToDataSensorsTransform.TestB2;
Matrix<float> A1 = (invXtmp.Multiply(B1)).Multiply(Xtmp);
Matrix<float> A2 = (invXtmp.Multiply(B2)).Multiply(Xtmp);
Console.WriteLine("-------------------Fake sensor data (B1 and B2):----------------");
ShiuTransform.Print(B1);
ShiuTransform.Print(B2);
Console.WriteLine("--------------Recoved initial values A1 and A2:-----------------");            
ShiuTransform.Print(A1);
ShiuTransform.Print(A2);
Console.WriteLine("----------------------------------------------------------------");
Console.ReadLine();*/
