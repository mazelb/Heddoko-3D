
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
            float vDeltaTime = Time.time - mLastTimeCalled;
            mLastTimeCalled = Time.time;

            //Get necessary Axis info
            Vector3 vTorsoAxisUp, vTorsoAxisRight, vTorsoAxisForward;
            Vector3 vThighAxisUp, vThighAxisRight, vThighAxisForward;
            Vector3 vKneeAxisUp, vKneeAxisRight, vKneeAxisForward;
            Vector3 vHipAxisUp, vHipAxisRight, vHipAxisForward;

            //Get the 3D axis and angles
            vTorsoAxisUp = TorsoTransform.up;
            vTorsoAxisRight = TorsoTransform.right;
            vTorsoAxisForward = TorsoTransform.forward;

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
            float vAngleHipFlexionNew;
            Vector3 vFlexCrossPrdct = Vector3.zero;
            Vector3 vFlexLHS = Vector3.zero;
            float vFlexSign = 0;
            var vPlaneNormal = Vector3.ProjectOnPlane(vThighAxisUp, HipGlobalTransform.right);
            vAngleHipFlexionNew = Vector3.Angle(HipGlobalTransform.up, vPlaneNormal);
            vFlexCrossPrdct = Vector3.Cross(HipGlobalTransform.right, vPlaneNormal);
            vFlexLHS = HipGlobalTransform.up;
            vFlexSign = Mathf.Sign(Vector3.Dot(vFlexLHS, vFlexCrossPrdct));
            RightHipFlexionSignedAngle = vFlexSign * RightHipFlexionAngle;
            RightHipFlexionAngle = vAngleHipFlexionNew;

            //calculate the Hip Abduction angle
            float vAngleHipAbductionNew;
            Vector3 vAbductionCrossPrdct = Vector3.zero;
            Vector3 vAbdLHS = Vector3.zero;
            vPlaneNormal = Vector3.ProjectOnPlane(vThighAxisUp, HipGlobalTransform.forward);
            vAngleHipAbductionNew = Vector3.Angle(HipGlobalTransform.up, vPlaneNormal);
            vAbductionCrossPrdct = Vector3.Cross(HipGlobalTransform.forward, vPlaneNormal);
            vAbdLHS = HipGlobalTransform.up;
            RightHipAbductionAngle = vAngleHipAbductionNew * GetSign("System.Single SignedRightHipAbductionAngle");
            float vAbdSign = Mathf.Sign(Vector3.Dot(vAbdLHS, vAbductionCrossPrdct));
            RightHipAbductionSignedAngle = vAbdSign * RightHipAbductionAngle * GetSign("System.Single SignedRightHipAbductionAngle");

            //calculate the Hip Rotation angle
            float vAngleHipRotationNew = 180 - Mathf.Abs(180 - ThighTransform.rotation.eulerAngles.y);
            RightHipRotationSignedAngle = vAngleHipRotationNew * GetSign("System.Single SignedRightHipRotation");

            //Calculate Leg height 
            float vThighHeight = mInitThighHeight * Mathf.Abs(Vector3.Dot(vThighAxisUp, Vector3.up));
            float vTibiaHeight = mInitTibiaHeight * Mathf.Abs(Vector3.Dot(vKneeAxisUp, Vector3.up));
            LegHeight = vThighHeight + vTibiaHeight;

            float vThighStride = Mathf.Sqrt((mInitThighHeight * mInitThighHeight) - (vThighHeight * vThighHeight));
            float vTibiaStride = Mathf.Sqrt((mInitTibiaHeight * mInitTibiaHeight) - (vTibiaHeight * vTibiaHeight));

            Vector3 vThighDirection = -vThighAxisUp.normalized;
            Vector3 vTibiaDirection = -vKneeAxisUp.normalized;

            RightLegStride = Vector3.ProjectOnPlane((vThighStride * vThighDirection), Vector3.up) + Vector3.ProjectOnPlane((vTibiaStride * vTibiaDirection), Vector3.up);

            if (vDeltaTime > 0)
            {
                VelocityAndAccelerationExtraction(vAngleKneeFlexionNew, vAngleHipRotationNew, vAngleHipAbductionNew,
                    vAngleKneeRotationNew, vAngleHipFlexionNew, vDeltaTime);

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
