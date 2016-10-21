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
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LKnee F/E", Order = 16)]
        public float LeftKneeFlexionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LKnee Rot", Order = 13)]
        public float LeftKneeRotationSignedAngle;

        //Hip Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipFlexionAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LHip F/E", Order = 8)]
        public float LeftHipFlexionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftHipAbductionAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LHip Add/Abd", Order = 10)]
        public float LeftHipAbductionSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LHip Int/Ext Rot", Order = 12)]
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

            //calculate the Knee Flexion angle
            float vAngleKneeFlexionNew = Vector3.Angle(Vector3.ProjectOnPlane(vThighAxisUp, vThighAxisRight), Vector3.ProjectOnPlane(vKneeAxisUp, vThighAxisRight));
            LeftKneeFlexionSignedAngle = vAngleKneeFlexionNew * GetSign("System.Single LeftKneeFlexion");

            //calculate the Knee Rotation angle
            float vAngleKneeRotationNew = 180 - Mathf.Abs(180 - KneeTransform.rotation.eulerAngles.y);
            LeftKneeRotationSignedAngle = vAngleKneeRotationNew;

            //calculate the Hip Flexion angle
            float vAngleHipFlexionNew;
            Vector3 vFlexCrossPrdct = Vector3.zero;
            Vector3 vFlexLHS = Vector3.zero;
            float vFlexSign = 0;
            var vPlaneNormal = Vector3.ProjectOnPlane(vThighAxisUp, HipGlobalTransform.right);
            vAngleHipFlexionNew = Vector3.Angle(HipGlobalTransform.up, vPlaneNormal);
            vFlexCrossPrdct = Vector3.Cross(HipGlobalTransform.right, vPlaneNormal);
            vFlexLHS = HipGlobalTransform.up;
            LeftHipFlexionAngle = vAngleHipFlexionNew;
            vFlexSign = Mathf.Sign(Vector3.Dot(vFlexLHS, vFlexCrossPrdct));
            LeftHipFlexionSignedAngle = vFlexSign * LeftHipFlexionAngle * GetSign("System.Single LeftSignedHipFlexionAngle");

            //calculate the Hip Abduction angle
            float vAngleHipAbductionNew;
            Vector3 vAbductionCrossPrdct = Vector3.zero;
            Vector3 vAbdLHS = Vector3.zero;
            vPlaneNormal = Vector3.ProjectOnPlane(vThighAxisUp, HipGlobalTransform.forward);
            vAngleHipAbductionNew = Vector3.Angle(HipGlobalTransform.up, vPlaneNormal);
            vAbductionCrossPrdct = Vector3.Cross(HipGlobalTransform.forward, vPlaneNormal);
            vAbdLHS = HipGlobalTransform.up;
            LeftHipAbductionAngle = vAngleHipAbductionNew;
            float vAbdSign = Mathf.Sign(Vector3.Dot(vAbductionCrossPrdct, vAbdLHS));
            LeftHipAbductionSignedAngle = vAbdSign * LeftHipAbductionAngle * GetSign("System.Single LeftSignedHipAbductionAngle");

            //calculate the Hip Rotation angle
            float vAngleHipRotationNew = 180 - Mathf.Abs(180 - ThighTransform.rotation.eulerAngles.y);
            LeftHipRotationSignedAngle = vAngleHipRotationNew * GetSign("System.Single LeftHipRotationAngle");

            //Calculate Leg height 
            float vThighHeight = mInitThighHeight * Mathf.Abs(Vector3.Dot(vThighAxisUp, Vector3.up));
            float vTibiaHeight = mInitTibiaHeight * Mathf.Abs(Vector3.Dot(vKneeAxisUp, Vector3.up));
            LegHeight = vThighHeight + vTibiaHeight;

            float vThighStride = Mathf.Sqrt((mInitThighHeight * mInitThighHeight) - (vThighHeight * vThighHeight));
            float vTibiaStride = Mathf.Sqrt((mInitTibiaHeight * mInitTibiaHeight) - (vTibiaHeight * vTibiaHeight));

            Vector3 vThighDirection = -vThighAxisUp.normalized;
            Vector3 vTibiaDirection = -vKneeAxisUp.normalized;

            LeftLegStride = Vector3.ProjectOnPlane((vThighStride * vThighDirection), Vector3.up) + Vector3.ProjectOnPlane((vTibiaStride * vTibiaDirection), Vector3.up);

            if (mLastTimeCalled != 0)
            {
                VelocityAndAccelerationExtraction(vAngleKneeFlexionNew, vAngleHipRotationNew, vAngleHipAbductionNew,
                    vAngleHipFlexionNew, vAngleKneeRotationNew, mLastTimeCalled);
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
