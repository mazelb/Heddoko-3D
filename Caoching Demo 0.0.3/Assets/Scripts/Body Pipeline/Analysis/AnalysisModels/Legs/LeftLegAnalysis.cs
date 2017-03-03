/** 
* @file LeftLegAnalysis.cs
* @brief LeftArmAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
    /// <summary>
    /// Analysis to be performed on the left leg 
    /// </summary>
    [Serializable]
    public class LeftLegAnalysis : LegAnalysis
    {
        //Knee Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftKneeFlexionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LKnee F/E", Order = 22)]
        public float LeftKneeFlexionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = true, AttributeName = "LKnee Rot", Order = 24)]
        public float LeftKneeRotationSignedAngle;

        //Hip Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipFlexionAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LHip F/E", Order = 12)]
        public float LeftHipFlexionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipAbductionAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LHip Add/Abd", Order = 14)]
        public float LeftHipAbductionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = true, AttributeName = "LHip Int/Ext Rot", Order = 16)]
        public float LeftHipRotationSignedAngle;

        //Accelerations and velocities
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftKneeFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftKneeFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftKneeRotationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftKneeRotationAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipAbductionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipAbductionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipRotationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipRotationAngularAcceleration = 0;

        //Detection of vertical Hip position
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LegHeight;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float mInitThighHeight = 0.475f;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float mInitTibiaHeight = 0.475f;

        /// <summary>
        /// Extract angles from orientations for the right leg
        /// </summary>
        public override void AngleExtraction()
        { //Get necessary Axis info
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
            LeftKneeFlexionAngle = vAngleKneeFlexionNew;
            Vector3 vCross = Vector3.Cross(-vThighAxisUp, -vKneeAxisUp);
            float vSign = Mathf.Sign(Vector3.Dot(vThighAxisRight, vCross));
            LeftKneeFlexionSignedAngle = vSign * vAngleKneeFlexionNew * GetSign("System.Single LeftKneeFlexion");

            //calculate the Knee Rotation angle
            float vAngleKneeRotationNew = 180 - Mathf.Abs(180 - KneeTransform.rotation.eulerAngles.y);
            LeftKneeRotationSignedAngle = vAngleKneeRotationNew * GetSign("System.Single LeftKneeRotationAngle");

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
            LeftHipFlexionAngle = vAngleHipFlexionNew;
            vCross = Vector3.Cross(vThighUpAxisProjectedOnHipRight, -vHipAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vHipAxisRight, vCross));
            LeftHipFlexionSignedAngle = vSign * LeftHipFlexionAngle * GetSign("System.Single RightHipFlexionAngle");

            //calculate the Hip Abduction angle
            Vector3 vThighUpAxisProjectedOnHipForward = Vector3.ProjectOnPlane(-vThighAxisUp, vHipAxisForward);
            float vAngleHipAbductionNew;
            if (vThighUpAxisProjectedOnHipForward == Vector3.zero)
            {
                vAngleHipAbductionNew = 0.0f;
            }
            else
            {
                vAngleHipAbductionNew = Vector3.Angle(-vHipAxisUp, vThighUpAxisProjectedOnHipForward);
            }
            LeftHipAbductionAngle = vAngleHipAbductionNew;
            vCross = Vector3.Cross(vThighUpAxisProjectedOnHipForward, -vHipAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vHipAxisForward, vCross));
            LeftHipAbductionSignedAngle = vSign * LeftHipAbductionAngle * -1f; 

            //calculate the Hip Rotation angle
            float vAngleHipRotationNew = ThighTransform.rotation.eulerAngles.y; 
            if (vAngleHipRotationNew > 180)
            {
                vAngleHipRotationNew = ThighTransform.rotation.eulerAngles.y- 360f ;
            }
           

            LeftHipRotationSignedAngle = vAngleHipRotationNew * GetSign("System.Single RightHipRotationAngle");

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
        public void VelocityAndAccelerationExtraction(float vAngleKneeFlexionNew, float vAngleHipRotationNew, float vAngleHipAbductionNew,
         float vAngleHipFlexionNew, float vAngleKneeRotationNew, float vDeltaTime)
        {
            float vAngularVelocityKneeFlexionNew = Mathf.Abs(vAngleKneeFlexionNew - LeftKneeFlexionSignedAngle) / vDeltaTime;
            LeftKneeFlexionAngularAcceleration = Mathf.Abs(vAngularVelocityKneeFlexionNew - LeftKneeFlexionAngularVelocity) / vDeltaTime;
            LeftKneeFlexionAngularVelocity = vAngularVelocityKneeFlexionNew;

            float vAngularVelocityRHipRotationNew = Mathf.Abs(vAngleHipRotationNew - Mathf.Abs(LeftHipRotationSignedAngle)) / vDeltaTime;
            LeftHipRotationAngularAcceleration = Mathf.Abs(vAngularVelocityRHipRotationNew - LeftHipRotationAngularVelocity) / vDeltaTime;
            LeftHipRotationAngularVelocity = vAngularVelocityRHipRotationNew;

            float vAngularVelocityHipAbductionNew = Mathf.Abs(vAngleHipAbductionNew - Mathf.Abs(LeftHipAbductionAngle)) / vDeltaTime;
            LeftHipAbductionAngularAcceleration = Mathf.Abs(vAngularVelocityHipAbductionNew - LeftHipAbductionAngularVelocity) / vDeltaTime;
            LeftHipAbductionAngularVelocity = vAngularVelocityHipAbductionNew;
            float vAngularVelocityHipFlexionNew = Mathf.Abs(vAngleHipFlexionNew - Mathf.Abs(LeftHipFlexionAngle)) / vDeltaTime;
            LeftHipFlexionAngularAcceleration = Mathf.Abs(vAngularVelocityHipFlexionNew - LeftHipFlexionAngularVelocity) / vDeltaTime;
            LeftHipFlexionAngularVelocity = vAngularVelocityHipFlexionNew;
            float vAngularVelocityKneeRotationNew = Mathf.Abs(vAngleKneeRotationNew - Mathf.Abs(LeftKneeRotationSignedAngle)) / vDeltaTime;
            LeftKneeRotationAngularAcceleration = Mathf.Abs(vAngularVelocityKneeRotationNew - LeftKneeRotationAngularVelocity) / vDeltaTime;
            LeftKneeRotationAngularVelocity = vAngularVelocityKneeRotationNew;
        }
    }
}
