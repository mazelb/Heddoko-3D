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
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowFlexionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LElbow F/E", Order = 18)]
        public float LeftElbowFlexionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LForeArm Pro/Sup", Order = 20)]
        public float LeftForeArmPronationSignedAngle = 0;

        //Upper Arm Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderFlexionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould F/E", Order = 4)]
        public float LeftShoulderFlexionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderVertAbductionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould V Add/Abd", Order = 6)]
        public float LeftShoulderVerticalAbductionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderHorAbductionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould H Add/Abd", Order = 8)]
        public float LeftShoulderHorizontalAbductionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould Rot", Order = 10)]
        public float LeftShoulderRotationSignedAngle = 0;

        //Velocities and Accelerations
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowFlexionPeakAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftForeArmPronationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowPronationAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderVertAbductionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderVertAbductionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderHorAbductionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderHorAbductionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderRotationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderRotationAngularAcceleration = 0;


        /// <summary>
        /// Reset the metrics calculations
        /// </summary>
        public override void ResetMetrics()
        {
            LeftElbowFlexionPeakAngularVelocity = 0;
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
            LeftElbowFlexionAngle = vAngleElbowFlexionNew;
            Vector3 vCross = Vector3.Cross(vElbowAxisRight, vShoulderAxisRight);
            float vSign = Mathf.Sign(Vector3.Dot(vShoulderAxisUp, vCross));
            LeftElbowFlexionSignedAngle = vSign * LeftElbowFlexionAngle * GetSign("System.Single LeftElbowFlexionAngle");

            //calculate the Elbow Pronation angle
            float vAngleForeArmPronationNew = 180 - Mathf.Abs(180 - LoArTransform.rotation.eulerAngles.x);
            LeftForeArmPronationSignedAngle = vAngleForeArmPronationNew * GetSign("System.Single LeftForeArmPronationAngle");

            //calculate the Shoulder Flexion angle
            Vector3 vShoulderRightAxisProjectedOnTrunkRight = Vector3.ProjectOnPlane(-vShoulderAxisRight, vTrunkAxisRight);
             float vAngleShoulderFlexionNew;
            var vShoulderProjSqrMag = vShoulderRightAxisProjectedOnTrunkRight.sqrMagnitude;
            //check if the projection's square magnitude is under a certain tolerance. 
            if (Math.Abs(vShoulderProjSqrMag) < 0.001f) 
            {
                vAngleShoulderFlexionNew = 0.0f;
            }
            else
            {
                vAngleShoulderFlexionNew = Vector3.Angle(-vTrunkAxisUp, vShoulderRightAxisProjectedOnTrunkRight);
            }
            LeftShoulderFlexionAngle = vAngleShoulderFlexionNew;
            vCross = Vector3.Cross(vShoulderRightAxisProjectedOnTrunkRight, -vTrunkAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(-vTrunkAxisRight, vCross));
            LeftShoulderFlexionSignedAngle = vSign * LeftShoulderFlexionAngle * GetSign("System.Single LeftShoulderFlexionAngle");

            //calculate the Shoulder Abduction Vertical angle
            Vector3 vVerticalShoulderAbdProjection = Vector3.ProjectOnPlane(-vShoulderAxisRight, vTrunkAxisForward);
            float vAngleShoulderVertAbductionNew;
            if (vVerticalShoulderAbdProjection == Vector3.zero)
            {
                vAngleShoulderVertAbductionNew = 0.0f;
            }
            else
            {
                vAngleShoulderVertAbductionNew = Vector3.Angle(-vTrunkAxisUp, vVerticalShoulderAbdProjection);
            }
            LeftShoulderVertAbductionAngle = vAngleShoulderVertAbductionNew;
            vCross = Vector3.Cross(vVerticalShoulderAbdProjection, -vTrunkAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vTrunkAxisForward, vCross));
            LeftShoulderVerticalAbductionSignedAngle = vSign * LeftShoulderVertAbductionAngle * GetSign("System.Single LeftShoulderVertAbductionAngle");
            Vector3 vHorizontalShoulderAbdProjection = Vector3.ProjectOnPlane(-vShoulderAxisRight, vTrunkAxisUp);

            //check if the arm is above the head 
            if (vAngleShoulderVertAbductionNew >= 165 || LeftShoulderVerticalAbductionSignedAngle <= 0)
            {
               vHorizontalShoulderAbdProjection = Vector3.ProjectOnPlane(-vShoulderAxisRight, -vTrunkAxisRight);
            }
            //calculate the Shoulder Abduction Horizontal angle
            float vAngleShoulderHorAbductionNew;
            if (vHorizontalShoulderAbdProjection == Vector3.zero)
            {
                vAngleShoulderHorAbductionNew = 0.0f;
            }
            else
            {
               
                if (vAngleShoulderVertAbductionNew >= 165)
                {
                  vAngleShoulderHorAbductionNew = Vector3.Angle(vTrunkAxisUp, vHorizontalShoulderAbdProjection);
                }
               else  if (LeftShoulderVerticalAbductionSignedAngle <= 0)
                {
                    Debug.Log(
                        "here");
                    vAngleShoulderHorAbductionNew = Vector3.Angle(-vTrunkAxisUp, vHorizontalShoulderAbdProjection);
                }

                else
                {
                 vAngleShoulderHorAbductionNew = Vector3.Angle(-vTrunkAxisRight, vHorizontalShoulderAbdProjection);
                }
            }
          
            LeftShoulderHorAbductionAngle = vAngleShoulderHorAbductionNew;
            vCross = Vector3.Cross( -vTrunkAxisRight,vHorizontalShoulderAbdProjection);
            vSign = Mathf.Sign(Vector3.Dot(vTrunkAxisUp, vCross));
            LeftShoulderHorizontalAbductionSignedAngle = vSign * LeftShoulderHorAbductionAngle * GetSign("System.Single LeftShoulderHorAbductionAngle");

            //calculate the Shoulder Rotation angle
            float vAngleShoulderRotationNew = 180 - Mathf.Abs(180 - UpArTransform.rotation.eulerAngles.x);
            LeftShoulderRotationSignedAngle = vAngleShoulderRotationNew * GetSign("System.Single LeftShoulderRotationAngle");

            //Calculate the velocity and accelerations
            if (DeltaTime != 0)
            {
                VelocityAndAccelerationExtraction(vAngleElbowFlexionNew, vAngleShoulderRotationNew,
                    vAngleShoulderHorAbductionNew,
                    vAngleShoulderVertAbductionNew, vAngleShoulderFlexionNew, vAngleForeArmPronationNew, DeltaTime);
            }

            //notify listeners that analysis on this component has been completed. 
            NotifyAnalysisCompletionListeners();
        }

        //TODO: Review all velocity and acceleration angles
        public void VelocityAndAccelerationExtraction(float vAngleElbowFlexionNew, float vAngleShoulderRotationNew, float vAngleShoulderHorAbductionNew,
            float vAngleShoulderVertAbductionNew, float vAngleShoulderFlexionNew, float vAngleElbowPronationNew, float vDeltaTime)
        {
            float vAngularVelocityElbowFlexionNew = (vAngleElbowFlexionNew - LeftElbowFlexionAngle) / DeltaTime;
            LeftElbowFlexionAngularAcceleration = (vAngularVelocityElbowFlexionNew - LeftElbowFlexionAngularVelocity) / DeltaTime;
            LeftElbowFlexionAngularVelocity = vAngularVelocityElbowFlexionNew;
            float vAngularVelocityShoulderRotationNew = (vAngleShoulderRotationNew - Mathf.Abs(LeftShoulderRotationSignedAngle)) / DeltaTime;
            LeftShoulderRotationAngularAcceleration = (vAngularVelocityShoulderRotationNew - LeftShoulderRotationAngularVelocity) / DeltaTime;
            LeftShoulderRotationAngularVelocity = vAngularVelocityShoulderRotationNew;
            float vAngularVelocityShoulderHorAbductionNew = (vAngleShoulderHorAbductionNew - Mathf.Abs(LeftShoulderHorAbductionAngle)) / DeltaTime;
            LeftShoulderHorAbductionAngularAcceleration = (vAngularVelocityShoulderHorAbductionNew - LeftShoulderHorAbductionAngularVelocity) / DeltaTime;
            LeftShoulderHorAbductionAngularVelocity = vAngularVelocityShoulderHorAbductionNew;
            float vAngularVelocityShoulderVertAbductionNew = (vAngleShoulderVertAbductionNew - Mathf.Abs(LeftShoulderVertAbductionAngle)) / DeltaTime;
            LeftShoulderVertAbductionAngularAcceleration = (vAngularVelocityShoulderVertAbductionNew - LeftShoulderVertAbductionAngularVelocity) / DeltaTime;
            LeftShoulderVertAbductionAngularVelocity = vAngularVelocityShoulderVertAbductionNew;
            float vAngularVelocityShoulderFlexionNew = (vAngleShoulderFlexionNew - Mathf.Abs(LeftShoulderFlexionAngle)) / DeltaTime;
            LeftShoulderFlexionAngularAcceleration = (vAngularVelocityShoulderFlexionNew - LeftShoulderFlexionAngularVelocity) / DeltaTime;
            LeftShoulderFlexionAngularVelocity = vAngularVelocityShoulderFlexionNew;
            float vAngularVelocityElbowPronationNew = (vAngleElbowPronationNew - Mathf.Abs(LeftForeArmPronationSignedAngle)) / DeltaTime;
            LeftElbowPronationAngularAcceleration = (vAngularVelocityElbowPronationNew - LeftForeArmPronationAngularVelocity) / DeltaTime;
            LeftForeArmPronationAngularVelocity = vAngularVelocityElbowPronationNew;
            LeftElbowFlexionPeakAngularVelocity = Mathf.Max(Mathf.Abs(LeftElbowFlexionAngularVelocity), LeftElbowFlexionPeakAngularVelocity);

        }
    }
}