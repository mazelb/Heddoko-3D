/** 
* @file LeftArmAnalysis.cs
* @brief LeftArmAnalysis the Joint class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System;
namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    /**
    * LeftArmAnalysis class 
    * @brief LeftArmAnalysis class 
    */
    [Serializable]
    public class LeftArmAnalysis : ArmAnalysis
    {
        //Elbow Angles
        public float AngleElbowFlexion = 0;
        public float SignedAngleElbowFlexion = 0;
        public float AngleElbowPronation = 0;
        public float SignedAngleElbowAdduction = 0;
        public float SignedAngleShoulderFlexion = 0;

        //Upper Arm Angles
        public float AngleShoulderFlexion = 0;
        public float AngleShoulderVertAbduction = 0;
        public float AngleShoulderHorAbduction = 0;
        public float AngleShoulderRotation = 0;

        //Velocities and Accelerations
        public float AngularVelocityElbowFlexion = 0;
        public float PeakAngularVelocityElbowFlexion = 0;
        public float AngularAccelerationElbowFlexion = 0;
        public float AngularVelocityPronation = 0;
        public float AngularAccelerationElbowPronation = 0;
        public float AngularVelocityShoulderFlexion = 0;
        public float AngularAccelerationShoulderFlexion = 0;
        public float AngularVelocityShoulderVertAbduction = 0;
        public float AngularAccelerationShoulderVertAbduction = 0;
        public float AngularVelocityShoulderHorAbduction = 0;
        public float AngularAccelerationShoulderHorAbduction = 0;
        public float AngularVelocityShoulderRotation = 0;
        public float AngularAccelerationShoulderRotation = 0;

        /// <summary>
        /// Reset the metrics calculations
        /// </summary>
        public override void ResetMetrics()
        {
            PeakAngularVelocityElbowFlexion = 0;
        }
 

        /// <summary>
        /// Extract angles from orientations
        /// </summary>
        public override void AngleExtraction()
        {
            //float vDeltaTime = Time.deltaTime;
            // Time.time - mLastTimeCalled;
            if (DeltaTime == 0)
            {
                return;
            }
            //mLastTimeCalled = Time.time;

            //Get necessary Axis info
            Vector3 vTorsoAxisUp, vTorsoAxisRight, vTorsoAxisForward;
            Vector3 vShoulderAxisUp, vShoulderAxisRight, vShoulderAxisForward;
            Vector3 vElbowAxisUp, vElbowAxisRight, vElbowAxisForward;

            //Get the 3D axis and angles
            vTorsoAxisUp = TorsoTransform.up;
            vTorsoAxisRight = TorsoTransform.right;
            vTorsoAxisForward = TorsoTransform.forward;

            vShoulderAxisUp = UpArTransform.up;
            vShoulderAxisRight = -UpArTransform.right;
            vShoulderAxisForward = UpArTransform.forward;

            vElbowAxisUp = LoArTransform.up;
            vElbowAxisRight = -LoArTransform.right;
            vElbowAxisForward = LoArTransform.forward;


            //calculate the Elbow Flexion angle

            Vector3 vProjectedShoulderAxisRight = Vector3.ProjectOnPlane(vShoulderAxisRight, vShoulderAxisForward);
            Vector3 vProjectedElbowAxisRight = Vector3.ProjectOnPlane(vElbowAxisRight, vShoulderAxisForward);
            float vAngleElbowFlexionNew = Vector3.Angle(vElbowAxisRight, vProjectedElbowAxisRight);
            float vAngularVelocityElbowFlexionNew = (vAngleElbowFlexionNew - AngleElbowFlexion) / DeltaTime;
            AngularAccelerationElbowFlexion = (vAngularVelocityElbowFlexionNew - AngularVelocityElbowFlexion) / DeltaTime;
            AngularVelocityElbowFlexion = vAngularVelocityElbowFlexionNew;
            PeakAngularVelocityElbowFlexion = Mathf.Max(Mathf.Abs(AngularVelocityElbowFlexion), PeakAngularVelocityElbowFlexion);
            AngleElbowFlexion = vAngleElbowFlexionNew;


            //   Vector3 vProjection = Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisRight);
            float vAngleSignedAngleShoulderFlexion = Vector3.Angle(-vTorsoAxisUp, vShoulderAxisRight);// Vector3.Angle(-vTorsoAxisUp, vProjection);
            //Vector3 vCross = Vector3.Cross(-vTorsoAxisUp, vProjection);
            float vSignShoulderFlexion = 1;//Mathf.Sign(Vector3.Dot(vTorsoAxisRight, vCross));
            SignedAngleShoulderFlexion = vSignShoulderFlexion * vAngleSignedAngleShoulderFlexion;


            Vector3 vProjection = Vector3.ProjectOnPlane(vShoulderAxisRight, -vTorsoAxisRight);
            float vAngle = Vector3.Angle(-vTorsoAxisUp, vProjection);
            Vector3 vCross = Vector3.Cross(vProjection ,-vTorsoAxisUp);
            float vSign = Mathf.Sign(Vector3.Dot(vTorsoAxisRight, vCross));
            SignedAngleElbowFlexion = vSign * vAngle;


            //Signed angle adduction calculation
            Vector3 vAductionprojection = Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisForward);
            float vAductionAngle = Vector3.Angle(-vTorsoAxisUp, vAductionprojection);
            Vector3 vAductionCross = Vector3.Cross(vAductionprojection  ,- vTorsoAxisUp);
            float vAductionSign = Mathf.Sign(Vector3.Dot(vTorsoAxisForward, vAductionCross));
            SignedAngleElbowAdduction = vAductionSign * vAductionAngle;
            //  SignedAngleElbowFlexion = GetSignedAngle( vShoulderAxisRight, vElbowAxisRight,vElbowAxisUp.normalized);

            //= GetSignedAngle(vShoulderAxisRight, vElbowAxisRight, vElbowAxisUp.normalized);

            //calculate the Elbow Pronation angle
            float vAngleElbowPronationNew = 180 - Mathf.Abs(180 - LoArTransform.rotation.eulerAngles.x);
            float vAngularVelocityElbowPronationNew = (vAngleElbowPronationNew - Mathf.Abs(AngleElbowPronation)) / DeltaTime;
            AngularAccelerationElbowPronation = (vAngularVelocityElbowPronationNew - AngularVelocityPronation) / DeltaTime;
            AngularVelocityPronation = vAngularVelocityElbowPronationNew;
            AngleElbowPronation = vAngleElbowPronationNew;


            //calculate the Shoulder Flexion angle
            float vAngleShoulderFlexionNew = Vector3.Angle(-vTorsoAxisUp, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisRight));
            float vAngularVelocityShoulderFlexionNew = (vAngleShoulderFlexionNew - Mathf.Abs(AngleShoulderFlexion)) / DeltaTime;
            AngularAccelerationShoulderFlexion = (vAngularVelocityShoulderFlexionNew - AngularVelocityShoulderFlexion) / DeltaTime;
            AngularVelocityShoulderFlexion = vAngularVelocityShoulderFlexionNew;
            AngleShoulderFlexion = vAngleShoulderFlexionNew;

            //calculate the Shoulder Abduction Vertical angle
            float vAngleShoulderVertAbductionNew = Vector3.Angle(-vTorsoAxisUp, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisForward));
            float vAngularVelocityShoulderVertAbductionNew = (vAngleShoulderVertAbductionNew - Mathf.Abs(AngleShoulderVertAbduction)) / DeltaTime;
            AngularAccelerationShoulderVertAbduction = (vAngularVelocityShoulderVertAbductionNew - AngularVelocityShoulderVertAbduction) / DeltaTime;
            AngularVelocityShoulderVertAbduction = vAngularVelocityShoulderVertAbductionNew;
            AngleShoulderVertAbduction = vAngleShoulderVertAbductionNew;

            //calculate the Shoulder Abduction Horizontal angle
            float vAngleShoulderHorAbductionNew = Vector3.Angle(vTorsoAxisForward, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisUp));
            float vAngularVelocityShoulderHorAbductionNew = (vAngleShoulderHorAbductionNew - Mathf.Abs(AngleShoulderHorAbduction)) / DeltaTime;
            AngularAccelerationShoulderHorAbduction = (vAngularVelocityShoulderHorAbductionNew - AngularVelocityShoulderHorAbduction) / DeltaTime;
            AngularVelocityShoulderHorAbduction = vAngularVelocityShoulderHorAbductionNew;
            AngleShoulderHorAbduction = vAngleShoulderHorAbductionNew;

            //calculate the Shoulder Rotation angle
            float vAngleShoulderRotationNew = 180 - Mathf.Abs(180 - UpArTransform.rotation.eulerAngles.x);
            float vAngularVelocityShoulderRotationNew = (vAngleShoulderRotationNew - Mathf.Abs(AngleShoulderRotation)) / DeltaTime;
            AngularAccelerationShoulderRotation = (vAngularVelocityShoulderRotationNew - AngularVelocityShoulderRotation) / DeltaTime;
            AngularVelocityShoulderRotation = vAngularVelocityShoulderRotationNew;
            AngleShoulderRotation = vAngleShoulderRotationNew;
        }
    }
}
