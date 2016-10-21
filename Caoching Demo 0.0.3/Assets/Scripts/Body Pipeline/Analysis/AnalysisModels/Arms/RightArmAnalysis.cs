/** 
* @file RightArmAnalysis.cs
* @brief RightArmAnalysis the Joint class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System;
namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    /**
    * RightArmAnalysis class 
    * @brief RightArmAnalysis class 
    */
    [Serializable]
    public class RightArmAnalysis : ArmAnalysis
    {
        //Elbow Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightElbowFlexionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RElbow F/E", Order = 13)]
        public float RightElbowFlexionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RForeArm Pro/Sup", Order = 13)]
        public float RightForeArmPronationSignedAngle = 0;

        //Upper Arm Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderFlexionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RShould F/E", Order = 3)]
        public float RightShoulderFlexionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderVertAbductionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RShould V Add/Abd", Order = 5)]
        public float RightShoulderVerticalAbductionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderHorAbductionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RShould H Add/Abd", Order = 7)]
        public float RightShoulderHorizontalAbductionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RShould Rot", Order = 7)]
        public float RightShoulderRotationSignedAngle = 0;

        //Velocities and Accelerations
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightElbowFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightElbowFlexionPeakAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightElbowFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightForeArmPronationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightForeArmPronationAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderVertAbductionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderVertAbductionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderHorAbductionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderHorAbductionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderRotationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightShoulderRotationAngularAcceleration = 0;

        public RightArmAnalysis()
        {
            AddSign("LeftShoulderFlexionSignedAngle", 1);
            AddSign("LeftShoulderVerticalAbductionSignedAngle", 1);
        }
        /// <summary>
        /// Reset the metrics calculations
        /// </summary>
        public override void ResetMetrics()
        {
            RightElbowFlexionPeakAngularVelocity = 0;
        }

        /// <summary>
        /// Extract angles from orientations
        /// </summary>
        public override void AngleExtraction()
        {
            //Get necessary Axis info
            Vector3 vTrunkAxisUp, vTrunkAxisRight, vTrunkAxisForward;
            Vector3 vShoulderAxisUp, vShoulderAxisRight, vShoulderAxisForward;
            Vector3 vElbowAxisUp, vElbowAxisRight, vElbowAxisForward;

            //Get the 3D axis and angles
            vTrunkAxisUp = TorsoTransform.up;
            vTrunkAxisRight = TorsoTransform.right;
            vTrunkAxisForward = TorsoTransform.forward;

            vShoulderAxisUp = UpArTransform.up;
            vShoulderAxisRight = UpArTransform.right;
            vShoulderAxisForward = UpArTransform.forward;

            vElbowAxisUp = LoArTransform.up;
            vElbowAxisRight = LoArTransform.right;
            vElbowAxisForward = LoArTransform.forward;

            //calculate the Elbow Flexion angle
            float vAngleElbowFlexionNew = Vector3.Angle(vShoulderAxisRight, vElbowAxisRight);
            RightElbowFlexionAngle = vAngleElbowFlexionNew;
            Vector3 vCross = Vector3.Cross(vElbowAxisRight, vShoulderAxisRight);
            float vSign = Mathf.Sign(Vector3.Dot(vShoulderAxisUp, vCross));
            RightElbowFlexionSignedAngle = vSign * RightElbowFlexionAngle * GetSign("System.Single RightElbowFlexionAngle");

            //calculate the Elbow Pronation angle
            float vAngleForeArmPronationNew = 180 - Mathf.Abs(180 - LoArTransform.rotation.eulerAngles.x);
            RightForeArmPronationSignedAngle = vAngleForeArmPronationNew * GetSign("System.Single RightForeArmPronationAngle");

            //calculate the Shoulder Flexion angle
            Vector3 vShoulderRightAxisProjectedOnTrunkRight = Vector3.ProjectOnPlane(vShoulderAxisRight, vTrunkAxisRight);
            float vAngleShoulderFlexionNew; 
            if (vShoulderRightAxisProjectedOnTrunkRight == Vector3.zero)
            {
                vAngleShoulderFlexionNew = 0.0f;
            }
            else
            {
                vAngleShoulderFlexionNew = Vector3.Angle(-vTrunkAxisUp, vShoulderRightAxisProjectedOnTrunkRight);
            }
            RightShoulderFlexionAngle = vAngleShoulderFlexionNew;
            vCross = Vector3.Cross(vShoulderRightAxisProjectedOnTrunkRight, -vTrunkAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vTrunkAxisRight, vCross));
            RightShoulderFlexionSignedAngle = vSign * RightShoulderFlexionAngle * GetSign("System.Single RightShoulderFlexionAngle");
            
            //calculate the Shoulder Abduction Vertical angle
            Vector3 vVerticalShoulderAbdProjection = Vector3.ProjectOnPlane(vShoulderAxisRight, vTrunkAxisForward);
            float vAngleShoulderVertAbductionNew;
            if (vVerticalShoulderAbdProjection == Vector3.zero)
            {
                vAngleShoulderVertAbductionNew = 0.0f;
            }
            else
            {
                vAngleShoulderVertAbductionNew = Vector3.Angle(-vTrunkAxisUp, vVerticalShoulderAbdProjection);
            }
            RightShoulderVertAbductionAngle = vAngleShoulderVertAbductionNew;
            vCross = Vector3.Cross(vVerticalShoulderAbdProjection, -vTrunkAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vTrunkAxisForward, vCross));
            RightShoulderVerticalAbductionSignedAngle = vSign * RightShoulderVertAbductionAngle * GetSign("System.Single RightShoulderVertAbductionAngle");

            //calculate the Shoulder Abduction Horizontal angle
            Vector3 vHorizontalShoulderAbdProjection = Vector3.ProjectOnPlane(vShoulderAxisRight, vTrunkAxisUp);
            float vAngleShoulderHorAbductionNew;
            if (vHorizontalShoulderAbdProjection == Vector3.zero)
            {
                vAngleShoulderHorAbductionNew = 0.0f;
            }
            else
            {
                vAngleShoulderHorAbductionNew = Vector3.Angle(vTrunkAxisRight, vHorizontalShoulderAbdProjection);
            }
            RightShoulderHorAbductionAngle = vAngleShoulderHorAbductionNew;
            vCross = Vector3.Cross(vHorizontalShoulderAbdProjection, vTrunkAxisRight);
            vSign = Mathf.Sign(Vector3.Dot(vTrunkAxisUp, vCross));
            RightShoulderHorizontalAbductionSignedAngle = vSign * RightShoulderHorAbductionAngle * GetSign("System.Single RightShoulderHorAbductionAngle");

            //calculate the Shoulder Rotation angle
            float vAngleShoulderRotationNew = 180 - Mathf.Abs(180 - UpArTransform.rotation.eulerAngles.x);
            RightShoulderRotationSignedAngle = vAngleShoulderRotationNew * GetSign("System.Single RightShoulderRotationAngle");

            //Calculate the velocity and accelerations
            if (DeltaTime != 0)
            {
                VelocityAndAccelerationExtraction(vAngleShoulderRotationNew, vAngleShoulderHorAbductionNew,
                    vAngleShoulderVertAbductionNew, vAngleShoulderFlexionNew, vAngleElbowFlexionNew, vAngleForeArmPronationNew, DeltaTime);
            }

            //notify listeners that analysis on this component has been completed. 
            NotifyAnalysisCompletionListeners();
        }

        //TODO: Review all velocity and acceleration angles
        public void VelocityAndAccelerationExtraction(float vAngleShoulderRotationNew, float vAngleShoulderHorAbductionNew, float vAngleShoulderVertAbductionNew, float vAngleShoulderFlexionNew, float vAngleElbowFlexionNew, float vAngleElbowPronationNew, float vDeltaTime)
        {
            float vAngularVelocityShoulderRotationNew = (vAngleShoulderRotationNew - Mathf.Abs(RightShoulderRotationSignedAngle)) / DeltaTime;
            RightShoulderRotationAngularAcceleration = (vAngularVelocityShoulderRotationNew - RightShoulderRotationAngularVelocity) / DeltaTime;
            RightShoulderRotationAngularVelocity = vAngularVelocityShoulderRotationNew;

            float vAngularVelocityShoulderHorAbductionNew = (vAngleShoulderHorAbductionNew - Mathf.Abs(RightShoulderHorAbductionAngle)) / DeltaTime;
            RightShoulderHorAbductionAngularAcceleration = (vAngularVelocityShoulderHorAbductionNew - RightShoulderHorAbductionAngularVelocity) / DeltaTime;
            RightShoulderHorAbductionAngularVelocity = vAngularVelocityShoulderHorAbductionNew;

            float vAngularVelocityShoulderVertAbductionNew = (vAngleShoulderVertAbductionNew - Mathf.Abs(RightShoulderVertAbductionAngle)) / DeltaTime;
            RightShoulderVertAbductionAngularAcceleration = (vAngularVelocityShoulderVertAbductionNew - RightShoulderVertAbductionAngularVelocity) / DeltaTime;
            RightShoulderVertAbductionAngularVelocity = vAngularVelocityShoulderVertAbductionNew;

            float vAngularVelocityShoulderFlexionNew = (vAngleShoulderFlexionNew - Mathf.Abs(RightShoulderFlexionAngle)) / DeltaTime;
            RightShoulderFlexionAngularAcceleration = (vAngularVelocityShoulderFlexionNew - RightShoulderFlexionAngularVelocity) / DeltaTime;
            RightShoulderFlexionAngularVelocity = vAngularVelocityShoulderFlexionNew;


            float vAngularVelocityElbowFlexionNew = (vAngleElbowFlexionNew - RightElbowFlexionAngle) / DeltaTime;
            RightElbowFlexionAngularAcceleration = (vAngularVelocityElbowFlexionNew - RightElbowFlexionAngularVelocity) / DeltaTime;
            RightElbowFlexionAngularVelocity = vAngularVelocityElbowFlexionNew;
            RightElbowFlexionPeakAngularVelocity = Mathf.Max(Mathf.Abs(RightElbowFlexionAngularVelocity), RightElbowFlexionPeakAngularVelocity);
            float vAngularVelocityElbowPronationNew = (vAngleElbowPronationNew - Mathf.Abs(RightForeArmPronationSignedAngle)) / DeltaTime;
            RightForeArmPronationAngularAcceleration = (vAngularVelocityElbowPronationNew - RightForeArmPronationAngularVelocity) / DeltaTime;
            RightForeArmPronationAngularVelocity = vAngularVelocityElbowPronationNew;
        }
    }
}