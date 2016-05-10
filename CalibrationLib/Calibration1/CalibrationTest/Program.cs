using System;
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
            bool  Test   = true;              
            Matrix4x4 t  = Matrix4x4.zero;
            Matrix4x4 X  = Matrix4x4.zero;
            Matrix<float> Xtmp    = Matrix<float>.Build.Dense(4, 4);
            Matrix<float> invXtmp = Matrix<float>.Build.Dense(4, 4);
            Matrix<float> FakeDataSensors    = Matrix<float>.Build.Dense(6, 1);
            Vector<float> RawSensDataAngles1 = Vector<float>.Build.Dense(3, 0);
            Vector<float> RawSensDataAngles2 = Vector<float>.Build.Dense(3, 0);
            ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(Test);
            float K  = 2.0F * Mathf.PI / 360;
            float x  = 45.0F; float y = 45.0F; float z = 45.0F;
            AvatarToDataSensorsTransform.TestFunction("CleanRotation", x*K, y*K, z*K);
            Xtmp = AvatarToDataSensorsTransform.Shiufunc(RawSensDataAngles1, RawSensDataAngles2);
            invXtmp = Xtmp.Transpose();
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
            Console.ReadLine();
        }
    }
}