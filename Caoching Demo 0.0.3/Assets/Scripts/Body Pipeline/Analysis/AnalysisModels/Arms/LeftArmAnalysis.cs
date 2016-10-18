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
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LElbow F/E", Order = 13)]
        public float LeftElbowFlexionAngle = 0;

        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould F/E", Order = 4)]
        public float LeftShoulderFlexionSignedAngle = 0;
        public float SignedAngleElbowFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleElbowPronation = 0;

        //Upper Arm Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleShoulderFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleShoulderVertAbduction = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould Add/Abd", Order = 6)]
        public float LeftShoulderVerticalAbductionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleShoulderHorAbduction = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleShoulderRotation = 0;

        //Velocities and Accelerations
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityElbowFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float PeakAngularVelocityElbowFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationElbowFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityPronation = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationElbowPronation = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityShoulderFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationShoulderFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityShoulderVertAbduction = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationShoulderVertAbduction = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityShoulderHorAbduction = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationShoulderHorAbduction = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityShoulderRotation = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
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
            vShoulderAxisRight = UpArTransform.right;
            vShoulderAxisForward = UpArTransform.forward;

            vElbowAxisUp = LoArTransform.up;
            vElbowAxisRight = LoArTransform.right;
            vElbowAxisForward = LoArTransform.forward;

            //calculate the Elbow Flexion angle
            Vector3 vProjectedShoulderAxisRight = Vector3.ProjectOnPlane(vShoulderAxisRight, vShoulderAxisForward);
            Vector3 vProjectedElbowAxisRight = Vector3.ProjectOnPlane(vElbowAxisRight, vShoulderAxisForward);
            float vAngleElbowFlexionNew = Vector3.Angle(vProjectedShoulderAxisRight, vProjectedElbowAxisRight);
            float vAngularVelocityElbowFlexionNew = (vAngleElbowFlexionNew - LeftElbowFlexionAngle) / DeltaTime;
            AngularAccelerationElbowFlexion = (vAngularVelocityElbowFlexionNew - AngularVelocityElbowFlexion) / DeltaTime;
            AngularVelocityElbowFlexion = vAngularVelocityElbowFlexionNew;
            PeakAngularVelocityElbowFlexion = Mathf.Max(Mathf.Abs(AngularVelocityElbowFlexion), PeakAngularVelocityElbowFlexion);
            LeftElbowFlexionAngle = vAngleElbowFlexionNew * GetSign("System.Single LeftElbowFlexionAngle");
            SignedAngleElbowFlexion = GetSignedAngle(vElbowAxisRight, vShoulderAxisRight, vElbowAxisUp.normalized);

            //calculate the Elbow Pronation angle
            float vAngleElbowPronationNew = 180 - Mathf.Abs(180 - LoArTransform.rotation.eulerAngles.x);
            float vAngularVelocityElbowPronationNew = (vAngleElbowPronationNew - Mathf.Abs(AngleElbowPronation)) / DeltaTime;
            AngularAccelerationElbowPronation = (vAngularVelocityElbowPronationNew - AngularVelocityPronation) / DeltaTime;
            AngularVelocityPronation = vAngularVelocityElbowPronationNew;
            AngleElbowPronation = vAngleElbowPronationNew;

            //calculate the Shoulder Flexion angle
            Vector3 vShoulderProjectionOntoTorsoRight = Vector3.ProjectOnPlane(-vShoulderAxisRight, vTorsoAxisRight);

            float vAngleShoulderFlexionNew = Vector3.Angle(-vTorsoAxisUp, vShoulderProjectionOntoTorsoRight);
            float vAngularVelocityShoulderFlexionNew = (vAngleShoulderFlexionNew - Mathf.Abs(AngleShoulderFlexion)) / DeltaTime;
            AngularAccelerationShoulderFlexion = (vAngularVelocityShoulderFlexionNew - AngularVelocityShoulderFlexion) / DeltaTime;
            AngularVelocityShoulderFlexion = vAngularVelocityShoulderFlexionNew;
            AngleShoulderFlexion = vAngleShoulderFlexionNew;
            //set the the signed component
            Vector3 vFlexCrossPrdct = Vector3.Cross(vTorsoAxisRight, vShoulderProjectionOntoTorsoRight);
            float vFlexSign = Mathf.Sign(Vector3.Dot(-vTorsoAxisUp, vFlexCrossPrdct));
            LeftShoulderFlexionSignedAngle = vFlexSign * AngleShoulderFlexion * GetSign("System.Single LeftShoulderFlexionSignedAngle");

            //calculate the Shoulder Abduction Vertical angle
            Vector3 vVerticalShoulderAbdProjection = Vector3.ProjectOnPlane(-vShoulderAxisRight, vTorsoAxisForward);
            float vAngleShoulderVertAbductionNew = Vector3.Angle(-vTorsoAxisUp, vVerticalShoulderAbdProjection);
            float vAngularVelocityShoulderVertAbductionNew = (vAngleShoulderVertAbductionNew - Mathf.Abs(AngleShoulderVertAbduction)) / DeltaTime;
            AngularAccelerationShoulderVertAbduction = (vAngularVelocityShoulderVertAbductionNew - AngularVelocityShoulderVertAbduction) / DeltaTime;
            AngularVelocityShoulderVertAbduction = vAngularVelocityShoulderVertAbductionNew;
            AngleShoulderVertAbduction = vAngleShoulderVertAbductionNew;
            Vector3 vVertAbductionCrossPrdct = Vector3.Cross( vVerticalShoulderAbdProjection,vTorsoAxisForward);
            float vVertAbductionSign = Mathf.Sign(Vector3.Dot(-vTorsoAxisUp, vVertAbductionCrossPrdct));
            LeftShoulderVerticalAbductionSignedAngle = vVertAbductionSign * AngleShoulderVertAbduction * GetSign("System.Single LeftShoulderVerticalAbductionSignedAngle");

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

            //notify listeners that analysis on this component has been completed. 
            NotifyAnalysisCompletionListeners();
        }


    }
}