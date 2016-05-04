/// <summary>
/// LinearSystems is a Class used by ShiuTransform and desing to 
/// form and solve the calibation systems of equations : 
/// A1 X = X B1 and A2 X = X B2  
/// From the papers: 
/// 1)Calibration of Wrist-Mounted Robotic Sensors by Solving by
/// Solving Homogeneous Transform Equations of the Form AX = XB.(Y.C.Shiu)
/// 2) An Overview of Robot-Sensor Calibration Methods for Evaluation 
/// of Perception Systems (M.Shah).  
/// </summary>
/// <param name="vA"> Matrix<float> 9x2 : contain the matrix A of the linear systems Ax=b </param> 
/// <param name="vb"> Matrix<float> 9x1 : contain the matrix b of the linear systems Ax=b</param> 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
namespace Calibration1.CalibrationTransformation
{
    /// build 2 matrix vA and vb objects with predefined length because we 
    /// know how many set of data we are going to used: 
    /// A1 X = X B1 (first set)
    /// A2 X = X B2 (second set) 
    /// The method allow to handle more then 2 set but now it's
    /// hard coded. 
    public class LinearSystem
    {
        
        public int mNumberlineb = 0;
        public int mNumbercolb  = 0;
        public int mNumberlineA = 0;
        public int mNumbercolA  = 0;
        public int mNumberOfSystemAdd  = 0;        
        static int mMaxNumbersOfSystem = 2;
        static int mMaxnumbercolA   = mMaxNumbersOfSystem * 2;
        const  int mMaxnumberlineb  = 9;
        const  int mMaxnumbercolb   = 1;
        const  int mMaxnumberlineA  = 9;        
        public Matrix<float> vA;
        public Matrix<float> vb;

        /// <summary>
        /// Default constructor        
        /// </summary>
        public LinearSystem()
        {
            this.vb = Matrix<float>.Build.Dense(LinearSystem.mMaxnumberlineb, LinearSystem.mMaxnumbercolb);
            this.vA = Matrix<float>.Build.Dense(LinearSystem.mMaxnumberlineA, LinearSystem.mMaxnumbercolA);
        }
        /// <summary>
        /// constructor       
        /// </summary>
        public LinearSystem(Matrix<float> vA, Matrix<float> vb)
        {
            this.vb = Matrix<float>.Build.Dense(LinearSystem.mMaxnumberlineb, LinearSystem.mMaxnumbercolb);
            this.vA = Matrix<float>.Build.Dense(LinearSystem.mMaxnumberlineA, LinearSystem.mMaxnumbercolA);
            float Sign = -1.0F;
            for (int i = 0; i < 9; i++)
            {
                this.vb[i, 0] = Sign*vb[i, 0];
                for (int j = 0; j < 2; j++)
                {
                    this.vA[i, j] = Sign*vA[i, j];
                }
            }                        
            mNumberlineb = 9;
            mNumbercolb  = 1;
            mNumberlineA = 9;
            mNumbercolA  = 2;
            mNumberOfSystemAdd++;
        }
        /// <summary>
        /// Method whith allows to add a set of equations to the global linear system
        /// 1 set for each data set associated to a pose taken by the wearer .
        /// Hard coded for two sets of data (could be extended).
        /// </summary>
        /// <param name="vA"> Matrix<float> 9x2 : contain the matrix A of the linear systems Ax=b </param> 
        /// <param name="vb"> Matrix<float> 9x1 : contain the matrix b of the linear systems Ax=b </param> 
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
                    this.vA[i, mNumbercolA + j] = Sign * vA[i, j];
                }                
            }            
            for (int j = 0; j < 9; j++)
            {  
                this.vb[j,0] =  this.vb[j, 0] + Sign * vb[j, 0] ;
            }

            if ((mNumbercolA > LinearSystem.mMaxnumbercolA) || (mNumberlineA > LinearSystem.mMaxnumberlineA))
            {
                System.Console.Write("Exceed max size of A");
            }
            mNumberlineb =  9;
            mNumbercolb  =  1;
            mNumberlineA =  9;
            mNumbercolA  = +2;
            mNumberOfSystemAdd++;
        }
        /// <summary>
        /// Method who solve the internal linear system and output the solution
        /// </summary>
        /// <returns> Beta contain two solutions those two solution shoud be almost the same if
        /// the method is successful.
        /// </returns>
        public Matrix<float> Solve()
        {
            Matrix<float> b       = this.vb;
            Matrix<float> AT      = this.vA.Transpose();
            Matrix<float> ATA     = AT.Multiply(vA);
            Matrix<float> invATA  = ATA.Inverse();
            Matrix<float> B       = AT.Multiply(b);
            Matrix<float> vBeta   = invATA.Multiply(B);
            return vBeta;
        }
    }
}
