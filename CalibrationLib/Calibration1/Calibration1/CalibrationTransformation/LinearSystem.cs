using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
namespace Calibration1.CalibrationTransformation
{
    public class LinearSystem
    {
        public int numberlineb = 0;
        public int numbercolb = 0;
        public int numberlineA = 0;
        public int numbercolA = 0;
        const int NumbersofSubSys = 2;
        const int Maxnumberlineb = NumbersofSubSys * 9;
        const int Maxnumbercolb = 1;
        const int MaxnumberlineA = 9;
        const int MaxnumbercolA = NumbersofSubSys * 9;
        public Matrix<float> A;
        public Matrix<float> b;
        public LinearSystem()
        {
            this.b = Matrix<float>.Build.Dense(LinearSystem.Maxnumberlineb, LinearSystem.Maxnumbercolb);
            this.A = Matrix<float>.Build.Dense(LinearSystem.MaxnumberlineA, LinearSystem.MaxnumbercolA);
        }
        public LinearSystem(Matrix<float> A, Matrix<float> b)
        {
            this.b = Matrix<float>.Build.Dense(LinearSystem.Maxnumberlineb, LinearSystem.Maxnumbercolb);
            this.A = Matrix<float>.Build.Dense(LinearSystem.MaxnumberlineA, LinearSystem.MaxnumbercolA);
            for (int i = 0; i < 9; i++)
            {
                this.b[i, 0] = b[i, 0];
                for (int j = 0; j < 9; j++)
                {
                    this.A[i, j] = A[i, j];
                }
            }
            this.numberlineb = 9;
            this.numbercolb = 1;
            this.numberlineA = 9;
            this.numbercolA = 2;
        }
        public void Addequation(Matrix<float> A, Matrix<float> b)
        {
            for (int i = 0; i < 2; i++)
            { this.A.InsertColumn(this.numbercolA + i, A.Column(i)); }
            this.numberlineA = 9;
            this.numbercolA = +2;
            for (int j = 0; j < 9; j++)
            { this.b.InsertRow(this.numberlineb + j, b.Row(j)); }
            this.numberlineb = +9;
            this.numbercolb = 1;
            if ((this.numbercolA > LinearSystem.MaxnumbercolA) || (this.numberlineA > LinearSystem.MaxnumberlineA))
            { System.Console.Write("Exceed max size of A"); }
            if ((this.numbercolb > LinearSystem.Maxnumbercolb) || (this.numberlineb > LinearSystem.Maxnumberlineb))
            { System.Console.Write("Exceed max size of b"); }
        }
    }
}
