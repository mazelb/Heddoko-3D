using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
namespace Calibration1.CalibrationTransformation
{
    public class LinearSystem
    {
        public int mNumbercolA = 0;
        public int mNumberOfSystemAdd = 0;
        static int mMaxNumbersOfSystem = 2;
        const int mMaxnumberlineA = 9;
        static int mMaxnumbercolA = mMaxNumbersOfSystem * 2;
        const int mMaxnumberlineb = 9;
        const int mMaxnumbercolb = 1;
        public Matrix<float> mA;
        public Matrix<float> mB;
        public LinearSystem()
        {
            this.mB = Matrix<float>.Build.Dense(LinearSystem.mMaxnumberlineb, LinearSystem.mMaxnumbercolb);
            this.mA = Matrix<float>.Build.Dense(LinearSystem.mMaxnumberlineA, LinearSystem.mMaxnumbercolA);
        }
        public LinearSystem(Matrix<float> vA, Matrix<float> vb)
        {
            this.mB = Matrix<float>.Build.Dense(LinearSystem.mMaxnumberlineb, LinearSystem.mMaxnumbercolb);
            this.mA = Matrix<float>.Build.Dense(LinearSystem.mMaxnumberlineA, LinearSystem.mMaxnumbercolA);
            float Sign = -1.0F;
            for (int i = 0; i < 9; i++)
            {
                this.mB[i, 0] = Sign * vb[i, 0];
                for (int j = 0; j < 2; j++)
                {
                    this.mA[i, j] = Sign * vA[i, j];
                }
            } 
            mNumbercolA = 2;
            mNumberOfSystemAdd++;
        }
        public void AddEquation(Matrix<float> vA, Matrix<float> vb)
        {
            float Sign = 1.0F;
            if (mNumberOfSystemAdd == 0)
            {
                Sign = -1.0F;
            }
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    this.mA[i, mNumbercolA + j] = Sign * vA[i, j];
                }
            }
            for (int j = 0; j < 9; j++)
            {
                this.mB[j, 0] = this.mB[j, 0] + Sign * vb[j, 0];
            }
            mNumbercolA += 2;
            mNumberOfSystemAdd++;
        }
        public Matrix<float> Solve()
        {
            Matrix<float> b = this.mB;
            Matrix<float> AT = this.mA.Transpose();
            Matrix<float> ATA = AT.Multiply(mA);
            Matrix<float> invATA = ATA.Inverse();
            Matrix<float> B = AT.Multiply(b);
            Matrix<float> vBeta = invATA.Multiply(B);
            return vBeta;
        }
    }
}
