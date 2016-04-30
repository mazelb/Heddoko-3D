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
        static string pose1 = sTpose     ;
        static string pose2 = sZombiepose;

        public bool Test = false;
        public Matrix4x4 TestB1 = Matrix4x4.zero;
        public Matrix4x4 TestB2 = Matrix4x4.zero;

        public ShiuTransform(bool test=false)
        {
            this.Test   = test;
        }
        /// <summary>
        /// Method which compute the required transformation X 
        /// linking sensors data and avatar movement. 
        /// A = (vX)*B*inv(vX)
        /// </summary>
        /// <param name="vEulerXAngle1Sen">sensor Euler's angles around X axis associated with first calibration poses</param>
        /// <param name="vEulerYAngle1Sen">sensor Euler's angles around Y axis associated with first calibration poses</param>
        /// <param name="vEulerZAngle1Sen">sensor Euler's angles around Z axis associated with first calibration poses</param>
        /// <param name="vEulerXAngle2Sen">sensor Euler's angles around X axis associated with second calibration poses</param>
        /// <param name="vEulerYAngle2Sen">sensor Euler's angles around Y axis associated with second calibration poses</param>
        /// <param name="vEulerZAngle2Sen">sensor Euler's angles around Z axis associated with second calibration poses</param>
        /// <returns name="vX">Calibration transformation Matrix X </returns>
        public Matrix4x4 Shiufunc(
                                  float vEulerXAngle1Sen = 0.0F, float vEulerYAngle1Sen = 0.0F, float vEulerZAngle1Sen = 0.0F, 
                                  float vEulerXAngle2Sen = 0.0F, float vEulerYAngle2Sen = 0.0F, float vEulerZAngle2Sen = 0.0F)
        {

            ///association between predifined poses and Euler angle 
            Vector3 vEulerA1Pose = PoseToEulerAngles(pose1); 
            Vector3 vEulerA2Pose = PoseToEulerAngles(pose2);

            ///transformation from Euler angle to rotation matrix (for pose 1 and 2)
            Matrix4x4 vAMatrixPose1 = EulerToRotationMatrix(vEulerA1Pose[0], vEulerA1Pose[1], vEulerA1Pose[2]);
            Matrix4x4 vAMatrixPose2 = EulerToRotationMatrix(vEulerA2Pose[0], vEulerA2Pose[1], vEulerA2Pose[2]);

            ///transformation from Euler angle to rotation matrix (for data set sensor 1 and 2)
            Matrix4x4 vBMatrixSensor1;
            Matrix4x4 vBMatrixSensor2;
            //First if to Test Fakesensors data 
            //Else in normal state
            if (this.Test)
            {
                vBMatrixSensor1 = this.TestB1;
                vBMatrixSensor2 = this.TestB2;               
            }
            else
            {
                vBMatrixSensor1 = EulerToRotationMatrix(vEulerXAngle1Sen, vEulerYAngle1Sen, vEulerZAngle1Sen);
                vBMatrixSensor2 = EulerToRotationMatrix(vEulerXAngle2Sen, vEulerYAngle2Sen, vEulerZAngle2Sen);
            }

            ///Declaration and initialization of Final Transformation (vX), the 
            ///first (Xp1 and Xp2) and second part of the solution and finally 
            ///the resulting solution of solving the linear System Axb
            Matrix4x4 vX  = Matrix4x4.zero;
            Matrix4x4 Xp1 = Matrix4x4.identity;
            Matrix4x4 Xp2 = Matrix4x4.identity;
            Matrix4x4 Ra  = Matrix4x4.zero;
            Matrix<float> Beta = Matrix<float>.Build.Dense(4, 1);

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
            vBMatrixSensor2[3, 0] = 0.0F; vBMatrixSensor2[3, 1] = 0.0F; vBMatrixSensor2[3, 2] = 0.0F; vBMatrixSensor2[3, 3] = 1.0F;*/
            

            ///From rotation matrix we obtain Rotation axis vector
            Vector3 vRotationAxisA1 = RotationAxis(vAMatrixPose1);
            Vector3 vRotationAxisA2 = RotationAxis(vAMatrixPose2);
            Vector3 vRotationAxisB1 = RotationAxis(vBMatrixSensor1);
            Vector3 vRotationAxisB2 = RotationAxis(vBMatrixSensor2);
            Vector3 ka1 = Vector3.Normalize(vRotationAxisA1);
            Vector3 ka2 = Vector3.Normalize(vRotationAxisA2);
            //Vector3 kb1 = Vector3.Normalize(vRotationAxisB1);
            //Vector3 kb2 = Vector3.Normalize(vRotationAxisB2);
           

            ///From rotation axis vectors we obtain a rotation axis perpendicular to the plan containing the rotation 
            ///axis (vRotationAxisA) of Real movement (vAMatrixPose) and the rotation axis (vRotationAxisB) of sensor data vBMatrixSensor
            Vector3 vVectorPerpenToa1b1Plan = Vector3.Cross(vRotationAxisA1, vRotationAxisB1);
            Vector3 vVectorPerpenToa2b2Plan = Vector3.Cross(vRotationAxisA2, vRotationAxisB2);

            ///Normalisation of vector vVectorPerpenToa1b1Plan and vVectorPerpenToa2b2Plan 
            Vector3 k1 = Vector3.Normalize(vVectorPerpenToa1b1Plan);
            Vector3 k2 = Vector3.Normalize(vVectorPerpenToa2b2Plan);
            

            ///Rotation angle around the two new rotation axis k1 and k2
            float W1 = Mathf.Atan2(Vector3.Magnitude(vVectorPerpenToa1b1Plan), Vector3.Dot(vRotationAxisA1, vRotationAxisB1));
            float W2 = Mathf.Atan2(Vector3.Magnitude(vVectorPerpenToa2b2Plan), Vector3.Dot(vRotationAxisA2, vRotationAxisB2));

            ///Formation a the first part of the full solution transformation vX
            /// the general solution is of the form vX = R(kai,theta)*R(ki,wi), here Xp1 is the R(k1,W1) and Xp2 is the R(k2,W2)
            Xp1 = Xpreliminairy(W1, k1);
            Xp2 = Xpreliminairy(W2, k2);

            ///instantiation of contain for the linear system to be form
            LinearSystem Axb = new LinearSystem();

            ///Formation of ShiuMatrix and update of the Matrices sA and sb
            ///by using the informations of the first pose
            ShiuMatrix(ka1, Xp1);
            
            ///Update of the linear Systeme
            Axb.AddEquation(sA,sb);

            ///Formation of ShiuMatrix and update of the Matrices sA and sb
            ///by using the informations of the second pose                 
            ShiuMatrix(ka2, Xp2);
           
            ///Update of the linear Systeme
            Axb.AddEquation(sA,sb);

            ///Solving Axb to obtain the second part of the solution            
            Beta = Axb.Solve();
            float theta1 = Mathf.Atan2(Beta[1,0], Beta[0,0]);
            float theta2 = Mathf.Atan2(Beta[3,0], Beta[2,0]);
            Matrix4x4 RA1 = Xpreliminairy(theta1, ka1);
            Matrix4x4 RA2 = Xpreliminairy(theta2, ka2);
            Matrix4x4 R1 = RA1 * Xp1;
            Matrix4x4 R2 = RA2 * Xp2;

            Console.WriteLine("------------------");
            Console.WriteLine("R1:");
            Console.WriteLine(R1);
            Console.WriteLine("R2:");
            Console.WriteLine(R2);
            Console.WriteLine("------------------");
            Console.ReadLine();            
            return vX;
        }
        /// <summary>
        /// Method who returns for a definite pose the Euler's angles 
        /// expected to be realize by a human wearing the suit.
        /// These are the angles supposed to be seen on Avatar.
        /// </summary>
        /// <param name="pose">String caracterising a definite pose</param>
        /// <returns> 3 Euler's angles contained in Vector3 </returns>
        public Vector3 PoseToEulerAngles(string vPose)
        {
            Vector3 vEulerAngles = Vector3.zero;
            if (vPose.CompareTo("T") == 1)
            {
                vEulerAngles[0] = 0.0F;
                vEulerAngles[1] = 0.0F;
                vEulerAngles[2] = 0.0F;
            }
            else if (vPose.CompareTo("Z") == 1)
            {
                vEulerAngles[0] =  0.0F;
                vEulerAngles[1] =  0.0F;
                vEulerAngles[2] = 90.0F;
            }
            else if (vPose.CompareTo("S") == 1)
            {
                vEulerAngles[0] =  0.0F;
                vEulerAngles[1] = 90.0F;
                vEulerAngles[2] =  0.0F;
            }
            return vEulerAngles;
        }
        /// <summary>
        ///  Method transforming Euler's angles in rotation matrix
        /// </summary>
        /// <param name="vEulerXAngle">Euler angle around X axis</param>
        /// <param name="vEulerYAngle">Euler angle around Y axis</param>
        /// <param name="vEulerZAngle">Euler angle around Z axis</param>
        /// <returns> Rotation matrix: Matrix4x4 </returns>
        public Matrix4x4 EulerToRotationMatrix(float vEulerXAngle, float vEulerYAngle, float vEulerZAngle)
        {
            Matrix4x4 M = Matrix4x4.zero;
            M[0, 0] =  Mathf.Cos(vEulerXAngle) * Mathf.Cos(vEulerYAngle) - Mathf.Sin(vEulerXAngle) * Mathf.Cos(vEulerZAngle) * Mathf.Sin(vEulerYAngle);
            M[0, 1] =  Mathf.Cos(vEulerXAngle) * Mathf.Sin(vEulerYAngle) + Mathf.Sin(vEulerXAngle) * Mathf.Cos(vEulerZAngle) * Mathf.Sin(vEulerYAngle);
            M[0, 2] =  Mathf.Cos(vEulerXAngle) * Mathf.Sin(vEulerZAngle);
            M[1, 0] = -Mathf.Sin(vEulerXAngle) * Mathf.Cos(vEulerYAngle) - Mathf.Cos(vEulerXAngle) * Mathf.Cos(vEulerZAngle) * Mathf.Sin(vEulerYAngle);
            M[1, 1] = -Mathf.Sin(vEulerXAngle) * Mathf.Sin(vEulerYAngle) + Mathf.Cos(vEulerXAngle) * Mathf.Cos(vEulerZAngle) * Mathf.Cos(vEulerYAngle);
            M[1, 2] =  Mathf.Cos(vEulerXAngle) * Mathf.Sin(vEulerZAngle);
            M[2, 0] =  Mathf.Sin(vEulerZAngle) * Mathf.Sin(vEulerYAngle);
            M[2, 1] = -Mathf.Sin(vEulerZAngle) * Mathf.Cos(vEulerYAngle);
            M[2, 2] =  Mathf.Cos(vEulerZAngle);
            M[3, 3] =  1.0F;
            return M;
        }
        /// <summary>
        /// Method using Rotation matrix and returning the associated axis of rotation.
        /// the forth line and colonne are not used. 
        /// The Method use a matrix (vScalarxLogRotationMatrix) proportionnal 
        /// to the Rotation generator (Lie SO3 generator)  to obtain the component of the Rotation axis.
        /// </summary>
        /// <param name="vRotationMatrix"> Rotation matrix : Matrix4x4 </param>
        /// <returns></returns>
        public Vector3 RotationAxis(Matrix4x4 vRotationMatrix)
        {
            Vector3 vRotationAxis = Vector3.zero;
            Matrix4x4 vScalarxLogRotationMatrix  = Matrix4x4.zero;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    vScalarxLogRotationMatrix[i, j] = vRotationMatrix[i, j] - vRotationMatrix[j, i];
                }
            }
            vRotationAxis[0] = vScalarxLogRotationMatrix[2, 1];
            vRotationAxis[1] = vScalarxLogRotationMatrix[0, 2];
            vRotationAxis[2] = vScalarxLogRotationMatrix[1, 0];
            vRotationAxis = Vector3.Normalize(vRotationAxis);
            return vRotationAxis;
        }
        /// <summary>
        /// Method forming the first part of the expected transformation vX
        /// Used : Xp = Identity3x3*cos(RotationAngle) + 
        ///             sin(RotationAngle)*skew(AxisOfRotation) + 
        ///             (1-cos(RotationAngle))*AxisOfRotation*Tranpose(AxisOfRotation)
        /// </summary>
        /// <param name="AngleOfRotation">Rotation angle around the axis of rotation</param>
        /// <param name="AxisOfRotation">Rotation normalize axis</param>
        /// <returns></returns>
        public Matrix4x4 Xpreliminairy(float AngleOfRotation, Vector3 AxisOfRotation)
        {
            float A = 0.0F;
            Matrix4x4 Xp = Matrix4x4.zero;
            //Skew part
            Xp[1, 0] =  AxisOfRotation[2] * Mathf.Sin(AngleOfRotation);
            Xp[0, 1] = -AxisOfRotation[2] * Mathf.Sin(AngleOfRotation);
            Xp[2, 0] = -AxisOfRotation[1] * Mathf.Sin(AngleOfRotation);
            Xp[0, 2] =  AxisOfRotation[1] * Mathf.Sin(AngleOfRotation);
            Xp[2, 1] = -AxisOfRotation[0] * Mathf.Sin(AngleOfRotation);
            Xp[1, 2] =  AxisOfRotation[0] * Mathf.Sin(AngleOfRotation);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    A = 0.0F;
                    if (i == j)
                    { A = 1.0F; } //just for Diagonal element 
                    Xp[i, j] = Xp[i, j] + 
                               A*Mathf.Cos(AngleOfRotation) +
                               (1.0F - Mathf.Cos(AngleOfRotation))*AxisOfRotation[i]*AxisOfRotation[j];
                                                           //AxisOfRotation[i]*AxisOfRotation[j] is a matrix.
                }
            }
            return Xp;
        }
        /// <summary>
        /// Method who update the matrix sA and sb to form the 
        /// Linear System which will be contained and solved by an intantiation (Axb)
        /// of the Class LinearSystem. An update is done for each pose.
        /// </summary>
        /// <param name="k"> k is the axis of rotation </param>
        /// <param name="Xp"> Xp is the first part of the solution</param>
        static void ShiuMatrix(Vector3 k,Matrix4x4 Xp)
        {
            Vector3 xx1 = new Vector3(Xp[0, 0], Xp[1, 0], Xp[2, 0]);
            Vector3 xx2 = new Vector3(Xp[0, 1], Xp[1, 1], Xp[2, 1]);
            Vector3 xx3 = new Vector3(Xp[0, 2], Xp[1, 2], Xp[2, 2]);
            
            Vector3 n = new Vector3(0f, 0f, 0f);
            Vector3 o = new Vector3(0f, 0f, 0f);
            Vector3 a = new Vector3(0f, 0f, 0f);

           
            n = Vector3.Cross(xx1, k);
            o = Vector3.Cross(xx2, k);
            a = Vector3.Cross(xx3, k);

            sA[0, 0] = Xp[0, 0] - k[0] * Vector3.Dot(xx1, k);
            sA[1, 0] = Xp[0, 1] - k[0] * Vector3.Dot(xx2, k);
            sA[2, 0] = Xp[0, 2] - k[0] * Vector3.Dot(xx3, k);

            sA[3, 0] = Xp[1, 0] - k[1] * Vector3.Dot(xx1, k);
            sA[4, 0] = Xp[1, 1] - k[1] * Vector3.Dot(xx2, k);
            sA[5, 0] = Xp[1, 2] - k[1] * Vector3.Dot(xx3, k);

            sA[6, 0] = Xp[2, 0] - k[2] * Vector3.Dot(xx1, k);
            sA[7, 0] = Xp[2, 1] - k[2] * Vector3.Dot(xx2, k);
            sA[8, 0] = Xp[2, 2] - k[2] * Vector3.Dot(xx3, k);

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

            sb[0, 0] = -k[0] * Vector3.Dot(xx1, k);
            sb[1, 0] = -k[0] * Vector3.Dot(xx2, k);
            sb[2, 0] = -k[0] * Vector3.Dot(xx3, k);

            sb[3, 0] = -k[1] * Vector3.Dot(xx1, k);
            sb[4, 0] = -k[1] * Vector3.Dot(xx2, k);
            sb[5, 0] = -k[1] * Vector3.Dot(xx3, k);

            sb[6, 0] = -k[2] * Vector3.Dot(xx1, k);
            sb[7, 0] = -k[2] * Vector3.Dot(xx2, k);
            sb[8, 0] = -k[2] * Vector3.Dot(xx3, k);
        }
        ///Function to test the algorithm with fake sensors data
 /*       public Matrix4x4 QuaterniontoRigthHanded(Quaternion Q)
        {
            Matrix4x4 RigthHandedRotation;

            n = w * w + x * x + y * y + z * z
s = if n == 0 then 0 else 2 / n
wx = s * w * x, wy = s * w * y, wz = s * w * z
xx = s * x * x, xy = s * x * y, xz = s * x * z
yy = s * y * y, yz = s * y * z, zz = s * z * z

[1 - (yy + zz)         xy - wz          xz + wy]
[xy + wz     1 - (xx + zz)         yz - wx]
[xz - wy          yz + wx     1 - (xx + yy)]


            return RigthHandedRotation;
        }*/
        public void TestFunction(string mode,float XRotEulerSensorSys, float YRotEulerSensorSys, float ZRotEulerSensorSys)
        {
            Vector3 vEulerA1Pose = PoseToEulerAngles(ShiuTransform.pose1);
            Vector3 vEulerA2Pose = PoseToEulerAngles(ShiuTransform.pose1);
            Matrix4x4 vAMatrixPose1 = EulerToRotationMatrix(vEulerA1Pose[0], vEulerA1Pose[1], vEulerA1Pose[2]);
            Matrix4x4 vAMatrixPose2 = EulerToRotationMatrix(vEulerA2Pose[0], vEulerA2Pose[1], vEulerA2Pose[2]);
            Matrix4x4 FakeOrientationSensorSys    = Matrix4x4.identity ;
            Matrix4x4 invFakeOrientationSensorSys = Matrix4x4.identity;
            if (    mode == "CleanRotation")
            {
                FakeOrientationSensorSys = EulerToRotationMatrix(XRotEulerSensorSys, XRotEulerSensorSys, XRotEulerSensorSys);
            }
            else if(mode == "NoisyRotation")
            {
                FakeOrientationSensorSys = EulerToRotationMatrix(XRotEulerSensorSys, XRotEulerSensorSys, XRotEulerSensorSys);
            }

            Matrix<float> M = Matrix<float>.Build.Dense(4, 4);
            M = UtoM(FakeOrientationSensorSys);
            Matrix<float> invM = M.Inverse();
            invFakeOrientationSensorSys = MtoU(invM);
            this.TestB1 = invFakeOrientationSensorSys*vAMatrixPose1*FakeOrientationSensorSys;
            this.TestB2 = invFakeOrientationSensorSys*vAMatrixPose2*FakeOrientationSensorSys;                        
        }
        public Matrix<float> UtoM(Matrix4x4 U)
        {
            Matrix<float> M = Matrix<float>.Build.Dense(4,4);
            for(int i=0;i<4;i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    M[i, j] = U[i, j];
                }
            }
            return M;
        }
        public Matrix4x4  MtoU(Matrix<float> M)
        {
            Matrix4x4 U = Matrix4x4.zero;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    U[i, j] = M[i, j];
                }
            }
            return U;
        }
        public void SeeYou(string s="I'm Here !")
        {
            Console.WriteLine(s);
            Console.ReadLine();
        }
        public void SeeYou(Matrix4x4 s)
        {
            Console.WriteLine(s);
            Console.ReadLine();
        }
        public void SeeYou(Matrix<float> s)
        {
            Console.WriteLine(s);
            Console.ReadLine();
        }
        public void SeeYou(Vector3 s)
        {
            Console.WriteLine(s);
            Console.ReadLine();
        }        
        public void SeeYou(float s)
        {
            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
