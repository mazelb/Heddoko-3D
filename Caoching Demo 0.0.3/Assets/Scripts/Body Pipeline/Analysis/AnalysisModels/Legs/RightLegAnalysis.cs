
/** 
* @file RightLegAnalysis.cs
* @brief RightLegAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs
{
    /// <summary>
    /// Represents the anaylsis of the right leg segment
    /// </summary>
    [Serializable]
    public class RightLegAnalysis : LegAnalysis
    {
        //Knee Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightKneeFlexionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RKnee F/E", Order = 15)]
        public float RightKneeFlexionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RKnee Rot", Order = 13)]
        public float RightKneeRotationSignedAngle;

        //Hip Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightHipFlexionAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RHip F/E", Order = 7)]
        public float RightHipFlexionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightHipAbductionAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RHip Add/Abd", Order = 9)]
        public float RightHipAbductionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "RHip Int/Ext Rot", Order = 11)]
        public float RightHipRotationSignedAngle;

        //Accelerations and velocities
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightKneeFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightKneeFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightKneeRotationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightKneeRotationAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightHipFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightHipFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightHipAbductionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightHipAbductionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightHipRotationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float RightHipRotationAngularAcceleration = 0;

        //Detection of vertical Hip position
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LegHeight;
        [AnalysisSerialization(IgnoreAttribute = true)]
        private float mInitThighHeight = 0.475f;
        [AnalysisSerialization(IgnoreAttribute = true)]
        private float mInitTibiaHeight = 0.475f;

        /// <summary>
        /// Extract angles from orientations for the right leg
        /// </summary>
        public override void AngleExtraction()
        {
            //Get necessary Axis info
            Vector3 vThighAxisUp, vThighAxisRight, vThighAxisForward;
            Vector3 vKneeAxisUp, vKneeAxisRight, vKneeAxisForward;
            Vector3 vHipAxisUp, vHipAxisRight, vHipAxisForward;

            //Get the 3D axis and angles
            vThighAxisUp = ThighTransform.up;
            vThighAxisRight = ThighTransform.right;
            vThighAxisForward = ThighTransform.forward;

            vKneeAxisUp = KneeTransform.up;
            vKneeAxisRight = KneeTransform.right;
            vKneeAxisForward = KneeTransform.forward;

            vHipAxisUp = HipGlobalTransform.up;
            vHipAxisRight = HipGlobalTransform.right;
            vHipAxisForward = HipGlobalTransform.forward;

            //calculate the Knee Flexion angle
            float vAngleKneeFlexionNew = Vector3.Angle(-vThighAxisUp, -vKneeAxisUp);
            RightKneeFlexionAngle = vAngleKneeFlexionNew;
            Vector3 vCross = Vector3.Cross(-vThighAxisUp, -vKneeAxisUp);
            float vSign = Mathf.Sign(Vector3.Dot(vThighAxisRight, vCross));
            RightKneeFlexionSignedAngle = vSign * vAngleKneeFlexionNew * GetSign("System.Single RightKneeFlexion");

            //calculate the Knee Rotation angle
            float vAngleKneeRotationNew = 180 - Mathf.Abs(180 - KneeTransform.rotation.eulerAngles.y);
            RightKneeRotationSignedAngle = vAngleKneeRotationNew * GetSign("System.Single RightKneeRotationAngle");

            //calculate the Hip Flexion angle
            Vector3 vThighUpAxisProjectedOnHipRight = Vector3.ProjectOnPlane(-vThighAxisUp, vHipAxisRight);
            float vAngleHipFlexionNew;
            if (vThighUpAxisProjectedOnHipRight == Vector3.zero)
            {
                vAngleHipFlexionNew = 0.0f;
            }
            else
            {
                vAngleHipFlexionNew = Vector3.Angle(-vHipAxisUp, vThighUpAxisProjectedOnHipRight);
            }
            RightHipFlexionAngle = vAngleHipFlexionNew;
            vCross = Vector3.Cross(vThighUpAxisProjectedOnHipRight, -vHipAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vHipAxisRight, vCross));
            RightHipFlexionSignedAngle = vSign * RightHipFlexionAngle * GetSign("System.Single RightHipFlexionAngle");

            //calculate the Hip Abduction angle
            Vector3 vThighUpAxisProjectedOnHipForward = Vector3.ProjectOnPlane(-vThighAxisUp, vHipAxisRight);
            float vAngleHipAbductionNew;
            if (vThighUpAxisProjectedOnHipForward == Vector3.zero)
            {
                vAngleHipAbductionNew = 0.0f;
            }
            else
            {
                vAngleHipAbductionNew = Vector3.Angle(-vHipAxisUp, vThighUpAxisProjectedOnHipForward);
            }
            RightHipAbductionAngle = vAngleHipAbductionNew;
            vCross = Vector3.Cross(vThighUpAxisProjectedOnHipForward, -vHipAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vHipAxisRight, vCross));
            RightHipAbductionSignedAngle = vSign * RightHipAbductionAngle * GetSign("System.Single RightHipAbductionAngle");

            //calculate the Hip Rotation angle
            float vAngleHipRotationNew = 180 - Mathf.Abs(180 - ThighTransform.rotation.eulerAngles.y);
            RightHipRotationSignedAngle = vAngleHipRotationNew * GetSign("System.Single RightHipRotationAngle");

            //Calculate Leg height 
            float vThighHeight = mInitThighHeight * Mathf.Abs(Vector3.Dot(vThighAxisUp, Vector3.up));
            float vTibiaHeight = mInitTibiaHeight * Mathf.Abs(Vector3.Dot(vKneeAxisUp, Vector3.up));
            LegHeight = vThighHeight + vTibiaHeight;
            float vThighStride = Mathf.Sqrt((mInitThighHeight * mInitThighHeight) - (vThighHeight * vThighHeight));
            float vTibiaStride = Mathf.Sqrt((mInitTibiaHeight * mInitTibiaHeight) - (vTibiaHeight * vTibiaHeight));
            Vector3 vThighDirection = -vThighAxisUp.normalized;
            Vector3 vTibiaDirection = -vKneeAxisUp.normalized;
            RightLegStride = Vector3.ProjectOnPlane((vThighStride * vThighDirection), Vector3.up) + Vector3.ProjectOnPlane((vTibiaStride * vTibiaDirection), Vector3.up);

            //Calculate the velocity and accelerations
            if (DeltaTime > 0)
            {
                VelocityAndAccelerationExtraction(vAngleKneeFlexionNew, vAngleHipRotationNew, vAngleHipAbductionNew,
                    vAngleKneeRotationNew, vAngleHipFlexionNew, DeltaTime);
            }

            //notify listeners that analysis on this component has been completed. 
            NotifyLegAnalysisCompletion();
        }

        //TODO: Review all velocity and acceleration angles
        public void VelocityAndAccelerationExtraction(float vAngleKneeFlexionNew, float vAngleHipRotationNew, float vAngleHipAbductionNew, float vAngleKneeRotationNew, float vAngleHipFlexionNew, float vDeltaTime)
        {
            float vAngularVelocityKneeFlexionNew = Mathf.Abs(vAngleKneeFlexionNew - RightKneeFlexionSignedAngle) / vDeltaTime;
            RightKneeFlexionAngularAcceleration = Mathf.Abs(vAngularVelocityKneeFlexionNew - RightKneeFlexionAngularVelocity) / vDeltaTime;
            RightKneeFlexionAngularVelocity = vAngularVelocityKneeFlexionNew;

            float vAngularVelocityRHipRotationNew = Mathf.Abs(vAngleHipRotationNew - Mathf.Abs(RightHipRotationSignedAngle)) / vDeltaTime;
            RightHipRotationAngularAcceleration = Mathf.Abs(vAngularVelocityRHipRotationNew - RightHipRotationAngularVelocity) / vDeltaTime;
            RightHipRotationAngularVelocity = vAngularVelocityRHipRotationNew;
            float vAngularVelocityHipAbductionNew = Mathf.Abs(vAngleHipAbductionNew - Mathf.Abs(RightHipAbductionAngle)) / vDeltaTime;
            RightHipAbductionAngularAcceleration = Mathf.Abs(vAngularVelocityHipAbductionNew - RightHipAbductionAngularVelocity) / vDeltaTime;
            RightHipAbductionAngularVelocity = vAngularVelocityHipAbductionNew;
            float vAngularVelocityKneeRotationNew = Mathf.Abs(vAngleKneeRotationNew - Mathf.Abs(RightKneeRotationSignedAngle)) / vDeltaTime;
            RightKneeRotationAngularAcceleration = Mathf.Abs(vAngularVelocityKneeRotationNew - RightKneeRotationAngularVelocity) / vDeltaTime;
            RightKneeRotationAngularVelocity = vAngularVelocityKneeRotationNew;

            float vAngularVelocityHipFlexionNew = Mathf.Abs(vAngleHipFlexionNew - Mathf.Abs(RightHipFlexionAngle)) / vDeltaTime;
            RightHipFlexionAngularAcceleration = Mathf.Abs(vAngularVelocityHipFlexionNew - RightHipFlexionAngularVelocity) / vDeltaTime;
            RightHipFlexionAngularVelocity = vAngularVelocityHipFlexionNew;
        }

    }
}
