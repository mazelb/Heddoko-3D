/// <summary>
/// ShiuTransform is a Class designed to compute the Matrix transformation (vX) 
/// needed to map preperly the sensor data (B matrix) to the Avatar (A matrix).
/// Now it's consided that all these matrix are Rotation only, the the Shiu method can handle Rotation
/// plus Translatiion (now considered for now, to be code if it's need)
/// It use Shiu method described in 
/// From the papers: 
/// 1)Calibration of Wrist-Mounted Robotic Sensors by Solving by
/// Solving Homogeneous Transform Equations of the Form AX = XB.(Y.C.Shiu)
/// 2) An Overview of Robot-Sensor Calibration Methods for Evaluation 
/// of Perception Systems (M.Shah).  
/// </summary>
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
        /// <summary>
        /// sA and sb are 
        /// Matrices A and b of a linear system for a given pose
        /// sA and sb are used by LinearSystems instantiation (vAxb) 
        /// and they are change by the function ShiuMatrix for each calibration pose
        /// </summary>
        public static Matrix<float> sA = Matrix<float>.Build.Dense(9, 2);
        public static Matrix<float> sb = Matrix<float>.Build.Dense(9, 1);
        /// <summary>
        /// Predified calibration poses identifiers.
        /// </summary>
        static string sTpose       = "T";
        static string sZombiepose  = "Z";
        static string sSoldierpose = "S";
        public static string pose1 = sZombiepose;
        public static string pose2 = sSoldierpose;
        public bool Test = false;
        public Matrix<float> TestB1 = Matrix<float>.Build.Dense(3, 3, 0);
        public Matrix<float> TestB2 = Matrix<float>.Build.Dense(3, 3, 0);
 
        public ShiuTransform(bool test = false)
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="test">describing if ours intention is to test the class(test=true) or not(test=false)</param>
            this.Test = test;
        }
       
        /////////////////////////--------Principal Method---------------//////////////////////////////////
        public Matrix<float> Shiufunc(Vector<float> RawSensorsEulerAngle1, Vector<float> RawSensorsEulerAngle2)
        {
            /// <summary>
            /// Method which compute the required transformation X 
            /// linking sensors data and avatar movement. 
            /// A = (vX)*B*inv(vX)
            /// </summary>
            /// <param name="Q1">Quaternion describing orientation comming from the first  set of calibration sensor data (calibration pose1)</param>
            /// <param name="Q2">Quaternion describing orientation comming from the second set of calibration sensor data (calibration pose2)</param>
            /// <returns></returns>
            /// <returns name="vX">Calibration transformation Matrix X </returns>
            ///association between predifined poses and Euler angles 
            Vector<float> vEulerA1Pose = PoseToEulerAngles(pose1);
            Vector<float> vEulerA2Pose = PoseToEulerAngles(pose2);
            ///transformation from Euler angle to rotation matrix (for pose 1 and 2)
            Matrix<float> SX1 = EulerAngleToRotationMatrix(vEulerA1Pose[0], "X");
            Matrix<float> SY1 = EulerAngleToRotationMatrix(vEulerA1Pose[1], "Y");
            Matrix<float> SZ1 = EulerAngleToRotationMatrix(vEulerA1Pose[2], "Z");
            Matrix<float> SX2 = EulerAngleToRotationMatrix(vEulerA2Pose[0], "X");
            Matrix<float> SY2 = EulerAngleToRotationMatrix(vEulerA2Pose[1], "Y");
            Matrix<float> SZ2 = EulerAngleToRotationMatrix(vEulerA2Pose[2], "Z");
            ///Resulting Rotation matrix for the poses
            Matrix<float> vAMatrixPose1 = SZ1.Multiply(SY1.Multiply(SX1));   //resulting rotation matrix = R(Z)xR(Y)xR(X)
            Matrix<float> vAMatrixPose2 = SZ2.Multiply(SY2.Multiply(SX2));
            Console.WriteLine("----------Ideal rotations Matrix of Pose1 et Pose2--------------");
            Print(vAMatrixPose1);
            Print(vAMatrixPose2);
            Console.WriteLine("----------------------------------------------------------------");
            ///transformation from Euler angle to rotation matrix (for data set sensor 1 and 2)
            Matrix<float> vBMatrixSensor1 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> vBMatrixSensor2 = Matrix<float>.Build.Dense(3, 3, 0);
            ///First if to Test Fakesensors data 
            ///Else in normal state
            if (this.Test)
            {
                vBMatrixSensor1 = this.TestB1;
                vBMatrixSensor2 = this.TestB2;
            }
            else
            {
                ///transformation from Euler angle to rotation matrix Sensor data associated to pose 1 and 2
                Matrix<float> Rx1 = EulerAngleToRotationMatrix(RawSensorsEulerAngle1[0], "X");
                Matrix<float> Ry1 = EulerAngleToRotationMatrix(RawSensorsEulerAngle1[1], "Y");
                Matrix<float> Rz1 = EulerAngleToRotationMatrix(RawSensorsEulerAngle1[2], "Z");
                Matrix<float> Rx2 = EulerAngleToRotationMatrix(RawSensorsEulerAngle2[0], "X");
                Matrix<float> Ry2 = EulerAngleToRotationMatrix(RawSensorsEulerAngle2[1], "Y");
                Matrix<float> Rz2 = EulerAngleToRotationMatrix(RawSensorsEulerAngle2[2], "Z");
                ///Resulting Rotation matrix 
                vBMatrixSensor1 = Rz1.Multiply(Ry1.Multiply(Rx1));
                vBMatrixSensor2 = Rz2.Multiply(Ry2.Multiply(Rx2));
            }
            ///Declaration and initialization of Final Transformation (vX), the 
            ///first (Xp1 and Xp2) and second part of the solution and finally 
            ///the resulting solution of solving the linear System Axb
            Matrix<float> vX = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> Xp1 = Matrix<float>.Build.DenseIdentity(3, 3);
            Matrix<float> Xp2 = Matrix<float>.Build.DenseIdentity(3, 3);
            Matrix<float> Ra = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> Beta = Matrix<float>.Build.Dense(4, 1);
            ///From rotation matrix we obtain Rotation axis vector            
            Vector<float> vRotationAxisA1 = RotationAxis(vAMatrixPose1);
            Vector<float> vRotationAxisA2 = RotationAxis(vAMatrixPose2);
            Vector<float> vRotationAxisB1 = RotationAxis(vBMatrixSensor1);
            Vector<float> vRotationAxisB2 = RotationAxis(vBMatrixSensor2);
            Vector<float> ka1 = vRotationAxisA1.Normalize(2);
            Vector<float> ka2 = vRotationAxisA2.Normalize(2);

            ///From rotation axis vectors we obtain a rotation axis perpendicular to the plan containing the rotation 
            ///axis (vRotationAxisA) of Real movement (vAMatrixPose) and the rotation axis (vRotationAxisB) of sensor data vBMatrixSensor
            Vector<float> vVectorPerpenToa1b1Plan = CrossProduct(vRotationAxisB1, vRotationAxisA1);
            Vector<float> vVectorPerpenToa2b2Plan = CrossProduct(vRotationAxisB2, vRotationAxisA2);
            ///Normalisation of vector vVectorPerpenToa1b1Plan and vVectorPerpenToa2b2Plan             
            Vector<float> k1 = vVectorPerpenToa1b1Plan.Normalize(2);
            Vector<float> k2 = vVectorPerpenToa2b2Plan.Normalize(2);
            float ww1 = Mathf.Sqrt(vVectorPerpenToa1b1Plan.DotProduct(vVectorPerpenToa1b1Plan));
            float ww2 = Mathf.Sqrt(vVectorPerpenToa2b2Plan.DotProduct(vVectorPerpenToa2b2Plan));
            ///Rotation angle around the two new rotation axis k1 and k2
            float W1 = Mathf.Atan2(ww1, vRotationAxisA1.DotProduct(vRotationAxisB1));
            float W2 = Mathf.Atan2(ww2, vRotationAxisA2.DotProduct(vRotationAxisB2));

            ///Formation a the first part of the full solution transformation vX
            /// the general solution is of the form vX = R(kai,theta)*R(ki,wi) (product of 2 rotations), here Xp1 is the R(k1,W1) and Xp2 is the R(k2,W2)
            /// kai (ka1 and ka2) is the same rotation axis of the initiales A1 and A2 matrix
            Xp1 = Xpreliminairy(W1, k1);
            Xp2 = Xpreliminairy(W2, k2);
            ///instantiation of containter for the linear system to be form
            LinearSystem Axb = new LinearSystem();
            ///Formation of ShiuMatrix and update of the Matrices sA and sb
            ///by using the informations of the first pose
            ShiuMatrix(ka1, Xp1);
            ///Update of the linear Systeme
            Axb.AddEquation(sA, sb);   
            ///Formation of ShiuMatrix and update of the Matrices sA and sb
            ///by using the informations of the second pose                 
            ShiuMatrix(ka2, Xp2);
            ///Update of the linear Systeme
            Axb.AddEquation(sA, sb);
            ///Solving Axb to obtain the second part of the solution            
            Beta = Axb.Solve();
            float theta1 = Mathf.Atan2(Beta[1, 0], Beta[0, 0]);
            float theta2 = Mathf.Atan2(Beta[3, 0], Beta[2, 0]);
            Matrix<float> RA1 = Xpreliminairy(theta1, ka1);
            Matrix<float> RA2 = Xpreliminairy(theta2, ka2);
            Matrix<float> R1 = RA1 * Xp1;
            Matrix<float> R2 = RA2 * Xp2;

            Matrix<float> R = (R1+R2)/2;
            Console.WriteLine("-----------R1 and R2-------------");
            Print(R1);
            Print(R2);
            Console.WriteLine("-----------R-------------");
            Print(R);
            Console.WriteLine("------A1 and A2----------");
            Print(vAMatrixPose1);
            Print(vAMatrixPose2);
            Console.WriteLine("------B1 and B2----------");
            Print(vBMatrixSensor1);
            Print(vBMatrixSensor2);            
            Console.WriteLine("------A1 and A2(R)-------");
            Matrix<float> A1 = R * vBMatrixSensor1 * R.Transpose();
            Matrix<float> A2 = R * vBMatrixSensor2 * R.Transpose();            
            Print(A1);
            Print(A2);
            Console.WriteLine("-------------------------");
            Console.ReadLine();
            return R;
        }

        /////////////////////////--------linear System Constructions Methods----------////////////////////
        static void ShiuMatrix(Vector<float> k, Matrix<float> Xp)
        {
            /// <summary>
            /// Method who update the matrix sA and sb to form the 
            /// Linear System which will be contained and solved by an intantiation (Axb)
            /// of the Class LinearSystem. An update is done for each pose.
            /// </summary>
            /// <param name="k">  k is the axis of rotation </param>
            /// <param name="Xp"> Xp is the first part of the solution</param>

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

            //n = X[:, 1];
            //o = X[:, 2];
            //a = X[:, 3];

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
            /// <summary>
            /// Method forming the first part of the expected transformation vX
            /// Used : Xp = Identity3x3*cos(RotationAngle) + 
            ///             sin(RotationAngle)*skew(AxisOfRotation) + 
            ///             (1-cos(RotationAngle))*AxisOfRotation*Tranpose(AxisOfRotation)
            /// </summary>
            /// <param name="AngleOfRotation">Rotation angle around the axis of rotation</param>
            /// <param name="AxisOfRotation">Rotation normalize axis</param>
            /// <returns>Matrix<float> : first part of the solution</returns>
            Matrix<float> Xp = Matrix<float>.Build.Dense(3,3,0);
            //Skew part
            Xp = Skew(AxisOfRotation) * Mathf.Sin(AngleOfRotation) +
                 Matrix<float>.Build.DenseIdentity(3, 3) * Mathf.Cos(AngleOfRotation) +
                 (1.0F - Mathf.Cos(AngleOfRotation)) * VV(AxisOfRotation);
             return Xp;
        }
        public Vector<float> RotationAxis(Matrix<float> vRotationMatrix)
        {
            /// <summary>
            /// Method using Rotation matrix and returning the associated axis of rotation.
            /// the forth line and colonne are not used. 
            /// The Method use a matrix (vScalarxLogRotationMatrix) proportionnal 
            /// to the Rotation generator (Lie SO3 generator)  to obtain the component of the Rotation axis.
            /// </summary>
            /// <param name="vRotationMatrix"> Rotation matrix : Matrix4x4 </param>
            /// <returns>Vector<float> : axis of rotation</float>/returns> 
            Vector<float> vRotationAxis = Vector<float>.Build.Dense(3, 0);
            Matrix<float> vScalarxLogRotationMatrix = Matrix<float>.Build.Dense(3, 3, 0);            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    vScalarxLogRotationMatrix[i, j] = (vRotationMatrix[i, j] - vRotationMatrix[j, i]); ////vRotationMatrix[j, i] - vRotationMatrix[i, j]
                }
            }
            vRotationAxis[0] = vScalarxLogRotationMatrix[2, 1];
            vRotationAxis[1] = vScalarxLogRotationMatrix[0, 2];
            vRotationAxis[2] = vScalarxLogRotationMatrix[1, 0];
            vRotationAxis    = vRotationAxis.Normalize(2);
            return vRotationAxis;
        }
 
        /////////////////////////--------Change from Left to Rigth Handed convention--------/////////////
        public static Vector<float> LeftHQuatToRigthHEulerAngle(Quaternion Q)
        {
            /// <summary>
            /// Convert LeftHanded Quaternion into RigthHanded Euler Angles
            /// </summary>
            /// <param name="Q"></param>
            /// <returns>Vector<float> : Euler angles 1) Around X axis 2) Around Y axis 3)  Around Z axis </returns>
            Matrix<float> RigthHandedRotation = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> R = Matrix<float>.Build.Dense(3, 3, 0);
            Vector3 LeftHandedEulerAngle = Q.eulerAngles;
            Vector<float> RigthHandedEulerAngle = Vector<float>.Build.Dense(3, 0);
            ///Transformation Left to Rigth Coor System
            RigthHandedEulerAngle[0] = -1.0F * LeftHandedEulerAngle[0];  //pitch LH , pitch RH
            RigthHandedEulerAngle[1] = -1.0F * LeftHandedEulerAngle[2];  //roll  LH , roll  RH
            RigthHandedEulerAngle[2] = -1.0F * LeftHandedEulerAngle[1];  //yaw   LH , yaw   RH
            return RigthHandedEulerAngle;
        }

        /////////////////////////-------Euler and Matrix Methods--------////////////////////////////////
        public Vector<float> PoseToEulerAngles(string vPose)
        {
            /// <summary>
            /// Method who returns for a definite pose the Euler's angles 
            /// expected to be realize by a human wearing the suit.
            /// These are the angles supposed to be seen on Avatar.
            /// </summary>
            /// <param name="pose">String caracterising a definite pose</param>
            /// <returns> 3 Euler's angles contained in Vector<float> </float> </returns>
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
                vEulerAngles[1] =  0.0F * K;//Y
                vEulerAngles[2] = 90.0F * K;//Z
            }
            else if (vPose == "S")
            {
                vEulerAngles[0] = 0.0F  * K;//X
                vEulerAngles[1] = 90.0F * K;//Y
                vEulerAngles[2] = 0.0F  * K;//Z
            }
            return vEulerAngles;
        }

        public static Matrix<float> EulerAngleToRotationMatrix(float EuAn, string Axis)
        {
            /// <summary>
            /// Transformation of Euler Angle into rotation matrix
            /// </summary>
            /// <param name="EuAn">Euler angle LH or RH doesn't matter</param>
            /// <param name="Axis">String that can be Pitch, Roll or Yaw </param>
            /// <returns>Matrix<float> of rotation</returns>

            Matrix<float> R = Matrix<float>.Build.DenseIdentity(3);
            if (Axis == "X")
            {
                R[0, 0] = 1.0F; R[0, 1] =  0.0F;                    R[0, 2] = 0.0F;
                R[1, 0] = 0.0F; R[1, 1] =  1.0F * Mathf.Cos(EuAn) ; R[1, 2] = 1.0F * Mathf.Sin(EuAn);
                R[2, 0] = 0.0F; R[2, 1] = -1.0F * Mathf.Sin(EuAn) ; R[2, 2] = 1.0F * Mathf.Cos(EuAn);
            }
            else if (Axis == "Y")
            {
                R[0, 0] = 1.0F * Mathf.Cos(EuAn);  R[0, 1] = 0.0F; R[0, 2] =  -1.0F * Mathf.Sin(EuAn);
                R[1, 0] = 0.0F;                    R[1, 1] = 1.0F; R[1, 2] =  0.0F;
                R[2, 0] = 1.0F * Mathf.Sin(EuAn); R[2, 1] = 0.0F; R[2, 2] =  1.0F * Mathf.Cos(EuAn);
            }
            else if (Axis == "Z")
            {
                R[0, 0] = 1.0F * Mathf.Cos(EuAn); R[0, 1] = 1.0F * Mathf.Sin(EuAn); R[0, 2] = 0.0F;
                R[1, 0] =-1.0F * Mathf.Sin(EuAn); R[1, 1] = 1.0F * Mathf.Cos(EuAn); R[1, 2] = 0.0F;
                R[2, 0] = 0.0F;                   R[2, 1] = 0.0F;                   R[2, 2] = 1.0F;
            }
            return R;
        }
        ////////////////////////--------Testing Methods--------////////////////////////////////////////
        public void TestFunction(string mode, float Xf, float Yf, float Zf)
        {
            Vector<float> vEulerA1Pose = PoseToEulerAngles(ShiuTransform.pose1);//rad
            Vector<float> vEulerA2Pose = PoseToEulerAngles(ShiuTransform.pose2);//rad
            //A
            Matrix<float> Rx1 = EulerAngleToRotationMatrix(vEulerA1Pose[0], "X");
            Matrix<float> Ry1 = EulerAngleToRotationMatrix(vEulerA1Pose[1], "Y");
            Matrix<float> Rz1 = EulerAngleToRotationMatrix(vEulerA1Pose[2], "Z");
            Matrix<float> Rx2 = EulerAngleToRotationMatrix(vEulerA2Pose[0], "X");
            Matrix<float> Ry2 = EulerAngleToRotationMatrix(vEulerA2Pose[1], "Y");
            Matrix<float> Rz2 = EulerAngleToRotationMatrix(vEulerA2Pose[2], "Z");

            ///Resulting Rotation matrix for the poses
            Matrix<float> vAMatrixPose1 = Rz1.Multiply(Ry1.Multiply(Rx1));
            Matrix<float> vAMatrixPose2 = Rz2.Multiply(Ry2.Multiply(Rx2));

            Matrix<float> FakeOrientationSensorSys = Matrix<float>.Build.DenseIdentity(3, 3);
            Matrix<float> invFakeOrientationSensorSys = Matrix<float>.Build.DenseIdentity(3, 3);
            if (mode == "CleanRotation")
            {
                //X
                Matrix<float> RotX = EulerAngleToRotationMatrix(Xf, "X");
                Matrix<float> RotY = EulerAngleToRotationMatrix(Yf, "Y");
                Matrix<float> RotZ = EulerAngleToRotationMatrix(Zf, "Z");
                FakeOrientationSensorSys = RotZ.Multiply(RotY.Multiply(RotX));
            }
            else if (mode == "NoisyRotation")
            {
                /*Matrix<float> RotPitch = EulerAngleToRotationMatrix(PitchRotEulerSensorSys, "Pitch");
                Matrix<float> RotRoll    = EulerAngleToRotationMatrix(RollRotEulerSensorSys,   "Roll");
                Matrix<float> RotYaw     = EulerAngleToRotationMatrix(YawRotEulerSensorSys,     "Yaw");
                FakeOrientationSensorSys = RotYaw.Multiply(RotRoll.Multiply(RotPitch));*/
            }
            invFakeOrientationSensorSys = FakeOrientationSensorSys.Inverse();
            this.TestB1 = invFakeOrientationSensorSys * vAMatrixPose1 * FakeOrientationSensorSys;
            this.TestB2 = invFakeOrientationSensorSys * vAMatrixPose2 * FakeOrientationSensorSys;
        }

        ////////////////////////--------Helping Methods--------////////////////////////////////////////
        public float Performance(Matrix<float> A, Matrix<float> Aapprox)
        {
            /// <summary>
            /// Evaluation the root mean square of the difference between two matrix
            /// </summary>
            /// <param name="A">matrix of ref</param>
            /// <param name="Aapprox">matrix compared</param>
            /// <returns>Error</returns>
            float Error = 0.0F;
            for(int i=0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Error = Error + Mathf.Pow(A[i, j] - Aapprox[i, j], 2.0F);                   
                }
            }
            Error = Mathf.Sqrt(Error);
            return Error;
        }
        public Matrix<float> UtoM(Matrix4x4 U)
        {
            /// <summary>
            /// Allow to transform a Unity Matrix4x4 into a MathDotNet Matrix<float>(3,3)
            /// </summary>
            /// <param name="U">Unity Matrix4x4</param>
            /// <returns> MathDotNet Matrix<float>(4,4) </returns>
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
            /// <summary>
            /// Allow to transform a MathDotNet Matrix<float>(4,4) into a  Unity Matrix4x4 
            /// </summary>
            /// <param name="M">MathDotNet Matrix<float>(4,4)</param>
            /// <returns>Unity Matrix4x4</returns>
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
            /// <summary>
            /// Crossproduct uxv = w  (3D)
            /// </summary>
            /// <param name="u">Vector<float>(3)</param>
            /// <param name="v">Vector<float>(3)</param>
            /// <returns> Vector<float> </returns>
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

/*Console.WriteLine("------Initial differences between Raw and Ideal motion:----------");
            Console.WriteLine("------A1 - B1:-------------");
            Print(vAMatrixPose1 - vBMatrixSensor1);
            Console.WriteLine("------ErrA1B1:-------------");
            float ErrA1B1 = Performance(vAMatrixPose1, vBMatrixSensor1);
            Console.WriteLine(ErrA1B1);
            Console.ReadLine();

            Console.WriteLine("------A2 - B2:-------------");
            Print(vAMatrixPose2 - vBMatrixSensor2);
            Console.WriteLine("------ErrA2B2:-------------");
            float ErrA2B2 = Performance(vAMatrixPose2, vBMatrixSensor2);
            Console.WriteLine(ErrA2B2);
            Console.ReadLine();

            Console.WriteLine("------A1 ans B1:-------------");
            Print(vAMatrixPose1);
            Print(vBMatrixSensor1);
            Console.WriteLine("------A2 ans B2:-------------");
            Print(vAMatrixPose2);
            Print(vBMatrixSensor2);
            Console.ReadLine();

            Console.WriteLine("------Transformations(Solution of Algo1):R1 and R2:-------------");
            Console.WriteLine("------R1 - R2:-------------");
            Print(R1 - R2);
            Console.WriteLine("------ErrR1R2:-------------");
            float ErrR1R2 = Performance(R1, R2);
            Console.WriteLine(ErrR1R2);
            Console.WriteLine("-----------------------A1 and A2 (R1)---------------------------");           
            Matrix<float> A1 = R1 * vBMatrixSensor1 * R1.Transpose();
            Matrix<float> A2 = R1 * vBMatrixSensor2 * R1.Transpose();
            Console.WriteLine("------A1 - Aapprox1:-------------");
            Print(vAMatrixPose1 - A1);
            Console.WriteLine("------A2 - Aapprox2:-------------");
            Print(vAMatrixPose2 - A2);
            Console.WriteLine("------ErrA1Aa1:-------------");
            float ErrA1Aa1 = Performance(A1, vAMatrixPose1);
            Console.WriteLine(ErrA1Aa1);
            Console.WriteLine("------ErrA2Aa2:-------------");
            float ErrA2Aa2 = Performance(A2, vAMatrixPose2);
            Console.WriteLine(ErrA2Aa2);
            Console.ReadLine();

            Matrix<float> A1moy = A1;
            Matrix<float> A2moy = A2;

            Console.WriteLine("-----------------------A1 and A2 (R2)---------------------------");
            A1 = R2 * vBMatrixSensor1 * R2.Transpose();
            A2 = R2 * vBMatrixSensor2 * R2.Transpose();
            Console.WriteLine("------A1 - Aapprox1:-------------");
            Print(vAMatrixPose1 - A1);
            Console.WriteLine("------A2 - Aapprox2:-------------");
            Print(vAMatrixPose2 - A2);
            Console.WriteLine("------ErrA1Aa1:-------------");
            ErrA1Aa1 = Performance(A1, vAMatrixPose1);
            Console.WriteLine(ErrA1Aa1);
            Console.WriteLine("------ErrA2Aa2:-------------");
            ErrA2Aa2 = Performance(A2, vAMatrixPose2);
            Console.WriteLine(ErrA2Aa2);

            A1moy = A1moy + A1;
            A2moy = A2moy + A2;
            A1moy = A1moy/2.0F;
            A2moy = A2moy/2.0F;
            Console.WriteLine("------ErrA1Amoy1:-------------");
            float ErrA1A1moy = Performance(A1moy, vAMatrixPose1);
            Console.WriteLine(ErrA1A1moy);
            Console.WriteLine("------ErrA2Amoy2:-------------");
            float ErrA2A2moy = Performance(A2moy, vAMatrixPose2);
            Console.WriteLine(ErrA2A2moy);
            Console.ReadLine();*/
/*Console.WriteLine("Xp1 Xp2");
Console.WriteLine(Xp1);
Console.WriteLine(Xp2);
Console.Read();*/
/*Console.WriteLine("------Transformations(Solution of Algo1):RA1 and RA2:-------------");
            Print(RA1);
            Print(RA2);
            Console.WriteLine("------Transformations(Solution of Algo1):Xp1 and Xp2:-------------");
            Print(Xp1);
            Print(Xp2);*/
///Matrix example from the paper : Calibration of Wriste-Mounted Robotic Sensors by 
///Solving Homogeneous Transform Equations of the Form AX = XB (Y.C. Shiu, IEEE Trans. on Robotics And Automation, Feb. 1989)
/*vAMatrixPose1[0, 0] = -0.989992F; vAMatrixPose1[0, 1] = -0.141120F; vAMatrixPose1[0, 2] = 0.0F; vAMatrixPose1[0, 3] = 0.0F;
vAMatrixPose1[1, 0] = 0.141120F; vAMatrixPose1[1, 1] = -0.989992F; vAMatrixPose1[1, 2] = 0.0F; vAMatrixPose1[1, 3] = 0.0F;
vAMatrixPose1[2, 0] = 0.0F; vAMatrixPose1[2, 1] = 0.0F; vAMatrixPose1[2, 2] = 1.0F; vAMatrixPose1[2, 3] = 0.0F;
vAMatrixPose1[3, 0] = 0.0F; vAMatrixPose1[3, 1] = 0.0F; vAMatrixPose1[3, 2] = 0.0F; vAMatrixPose1[3, 3] = 1.0F;

vBMatrixSensor1[0, 0] = -0.989992F; vBMatrixSensor1[0, 1] = -0.138307F; vBMatrixSensor1[0, 2] = 0.028036F; vBMatrixSensor1[0, 3] = 0.0F;
vBMatrixSensor1[1, 0] = 0.138307F; vBMatrixSensor1[1, 1] = -0.911449F; vBMatrixSensor1[1, 2] = 0.387470F; vBMatrixSensor1[1, 3] = 0.0F;
vBMatrixSensor1[2, 0] = -0.028036F; vBMatrixSensor1[2, 1] = 0.387470F; vBMatrixSensor1[2, 2] = 0.921456F; vBMatrixSensor1[2, 3] = 0.0F;
vBMatrixSensor1[3, 0] = 0.0F; vBMatrixSensor1[3, 1] = 0.0F; vBMatrixSensor1[3, 2] = 0.0F; vBMatrixSensor1[3, 3] = 1.0F;

vAMatrixPose2[0, 0] = 0.070737F; vAMatrixPose2[0, 1] = 0.0F; vAMatrixPose2[0, 2] = 0.997495F; vAMatrixPose2[0, 3] = 0.0F;
vAMatrixPose2[1, 0] = 0.0F; vAMatrixPose2[1, 1] = 1.0F; vAMatrixPose2[1, 2] = 0.0F; vAMatrixPose2[1, 3] = 0.0F;
vAMatrixPose2[2, 0] = -0.997495F; vAMatrixPose2[2, 1] = 0.0F; vAMatrixPose2[2, 2] = 0.070737F; vAMatrixPose2[2, 3] = 0.0F;
vAMatrixPose2[3, 0] = 0.0F; vAMatrixPose2[3, 1] = 0.0F; vAMatrixPose2[3, 2] = 0.0F; vAMatrixPose2[3, 3] = 1.0F;

vBMatrixSensor2[0, 0] = 0.070737F; vBMatrixSensor2[0, 1] = 0.198172F; vBMatrixSensor2[0, 2] = 0.977612F; vBMatrixSensor2[0, 3] = 0.0F;
vBMatrixSensor2[1, 0] = -0.198172F; vBMatrixSensor2[1, 1] = 0.963323F; vBMatrixSensor2[1, 2] = -0.180936F; vBMatrixSensor2[1, 3] = 0.0F;
vBMatrixSensor2[2, 0] = -0.977612F; vBMatrixSensor2[2, 1] = -0.180936F; vBMatrixSensor2[2, 2] = 0.107415F; vBMatrixSensor2[2, 3] = 0.0F;
vBMatrixSensor2[3, 0] = 0.0F; vBMatrixSensor2[3, 1] = 0.0F; vBMatrixSensor2[3, 2] = 0.0F; vBMatrixSensor2[3, 3] = 1.0F; */
//float A = 0.0F;
//for (int i = 0; i < 3; i++)
//{
//    for (int j = 0; j < 3; j++)
//    {
//        A = 0.0F;
//        //just for Diagonal elements
//        if (i == j)
//        {
//            A = 1.0F;
//        }
//        Xp[i, j] = Xp[i, j] +
//                  (A * Mathf.Cos(AngleOfRotation)) +
//                  ((1.0F - Mathf.Cos(AngleOfRotation)) * AxisOfRotation[i] * AxisOfRotation[j]);
//                  ///AxisOfRotation[i]*AxisOfRotation[j] is a matrix.
//    }
//}




//Skew part
/*Xp[1, 0] =  AxisOfRotation[2] * Mathf.Sin(AngleOfRotation);
Xp[0, 1] = -AxisOfRotation[2] * Mathf.Sin(AngleOfRotation);
Xp[2, 0] = -AxisOfRotation[1] * Mathf.Sin(AngleOfRotation);
Xp[0, 2] =  AxisOfRotation[1] * Mathf.Sin(AngleOfRotation);
Xp[2, 1] =  AxisOfRotation[0] * Mathf.Sin(AngleOfRotation);
Xp[1, 2] = -AxisOfRotation[0] * Mathf.Sin(AngleOfRotation);*/
/*
          Xp[1, 0] = AxisOfRotation[2] * Mathf.Sin(AngleOfRotation);
          Xp[0, 1] = AxisOfRotation[2] * Mathf.Sin(AngleOfRotation);
          Xp[2, 0] = AxisOfRotation[1] * Mathf.Sin(AngleOfRotation);
          Xp[0, 2] = AxisOfRotation[1] * Mathf.Sin(AngleOfRotation);
          Xp[2, 1] = AxisOfRotation[0] * Mathf.Sin(AngleOfRotation);
          Xp[1, 2] = AxisOfRotation[0] * Mathf.Sin(AngleOfRotation);
          float Tr    = 0F;
          float theta = 0F;
          Tr    = vRotationMatrix.Trace();
          theta = Mathf.Acos((Tr - 1.0F)/2.0F);
          Console.WriteLine("Matrix, TRACE and Angle");
          Print(vRotationMatrix);
          Console.WriteLine(Tr);
          Console.WriteLine(theta);
          /*vRotationAxis[0] = (vRotationMatrix[2, 1] - vRotationMatrix[1, 2]) / (2.0F * Mathf.Sin(theta));
          vRotationAxis[1] = (vRotationMatrix[0, 2] - vRotationMatrix[2, 0]) / (2.0F * Mathf.Sin(theta));
          vRotationAxis[2] = (vRotationMatrix[1, 0] - vRotationMatrix[0, 1]) / (2.0F * Mathf.Sin(theta));
          */

