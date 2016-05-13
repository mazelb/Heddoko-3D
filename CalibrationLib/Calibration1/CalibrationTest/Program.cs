﻿using System;
using System.Collections.Generic;
using System.Text;
using Calibration1.CalibrationTransformation;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
namespace CalibrationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            bool Test = true;
            Vector<float> RawSensDataAngles1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> RawSensDataAngles2 = Vector<float>.Build.Dense(3, 0);
            Matrix<float> Xtmp = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> invXtmp = Matrix<float>.Build.Dense(3, 3, 0);
            ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(Test);
            float K = 2.0F * Mathf.PI / 360;
            float x = 45.0F;
            float y = 55.0F;
            float z = 15.0F;
            AvatarToDataSensorsTransform.TestFunction("CleanRotation", x * K, y * K, z * K);
            Xtmp = AvatarToDataSensorsTransform.Shiufunc(RawSensDataAngles1, RawSensDataAngles2);
            invXtmp = Xtmp.Inverse();
            Matrix<float> B1 = AvatarToDataSensorsTransform.TestB1;
            Matrix<float> B2 = AvatarToDataSensorsTransform.TestB2;
            Matrix<float> A1 = (Xtmp.Multiply(B1)).Multiply(invXtmp);
            Matrix<float> A2 = (Xtmp.Multiply(B2)).Multiply(invXtmp);
            Console.WriteLine("-------------------Fake sensor data (B1 and B2):----------------");
            ShiuTransform.Print(B1);
            ShiuTransform.Print(B2);
            Console.WriteLine("--------------Recoved initial values A1 and A2:-----------------");
            ShiuTransform.Print(A1);
            ShiuTransform.Print(A2);
            Console.WriteLine("----------------------------------------------------------------");
            Vector<float> u = Vector<float>.Build.Dense(3, 0);
            Vector<float> v = Vector<float>.Build.Dense(3, 0);
            u[0] = 4;
            u[1] = -2;
            u[2] = 1;
            v[0] = 1;
            v[1] = -1;
            v[2] = 3;
            Vector<float> w = Vector<float>.Build.Dense(3, 0);
            w = ShiuTransform.CrossProduct(u, v);
            Console.WriteLine(w);
            Console.ReadLine();
        }
    }
}

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
