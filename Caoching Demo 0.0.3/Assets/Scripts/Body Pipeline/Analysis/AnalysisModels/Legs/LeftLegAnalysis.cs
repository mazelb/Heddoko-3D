﻿/** 
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
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LKnee F/E", Order = 16)]
        public float LeftKneeFlexion;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleKneeRotation;

        //Hip Angles

        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleHipFlexion;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public bool UseGlobalReference = false;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LHip F/E", Order = 8)]
        public float LeftSignedHipFlexionAngle;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleHipAbduction;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LHip Add/Abd", Order = 10)]
        public float LeftSignedHipAbductionAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LHip Int/Ext Rot", Order = 12)]
        public float LeftHipRotationAngle;

        //Accelerations and velocities
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityKneeFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationKneeFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityKneeRotation = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationKneeRotation = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityHipFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationHipFlexion = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityHipAbduction = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationHipAbduction = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityHipRotation = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationHipRotation = 0;

        //Squats Analytics
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float NumberofSquats;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleSum;
        [AnalysisSerialization(IgnoreAttribute = true)]
        private bool mStartCountingSquats = true;

        //Detection of vertical Hip position
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LegHeight;
        [AnalysisSerialization(IgnoreAttribute = true)]
        private float mInitThighHeight = 0.475f;
        [AnalysisSerialization(IgnoreAttribute = true)]
        private float mInitTibiaHeight = 0.475f;



        /// <summary>
        /// Listens to events where squats need to be counted
        /// </summary>
        /// <param name="vFlag"></param>
        public void StartCountingSquats(bool vFlag)
        {
            mStartCountingSquats = vFlag;
        }

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

            //calculate the Knee Flexion angle (angles between axis projection in YZ plane)
            float vAngleKneeFlexionNew = Vector3.Angle(Vector3.ProjectOnPlane(vThighAxisUp, vThighAxisRight), Vector3.ProjectOnPlane(vKneeAxisUp, vThighAxisRight));
            LeftKneeFlexion = vAngleKneeFlexionNew * GetSign("System.Single LeftKneeFlexion");

            //calculate the Knee Rotation angle (angles between axis projection in XZ plane)
            float vAngleKneeRotationNew = 180 - Mathf.Abs(180 - KneeTransform.rotation.eulerAngles.y);
            AngleKneeRotation = vAngleKneeRotationNew;

            //calculate the Hip Flexion angle (angles between axis projection in YZ plane)
            float vAngleHipFlexionNew;
            Vector3 vFlexCrossPrdct = Vector3.zero;
            Vector3 vFlexLHS = Vector3.zero;
            float vFlexSign = 0;

            if (UseGlobalReference)
            {
                var vPlaneNormal = Vector3.ProjectOnPlane(vThighAxisUp, HipGlobalTransform.right);
                vAngleHipFlexionNew = Vector3.Angle(HipGlobalTransform.up, vPlaneNormal);
                vFlexCrossPrdct = Vector3.Cross(HipGlobalTransform.right, vPlaneNormal);
                vFlexLHS = HipGlobalTransform.up;
            }
            else
            {
                var vPlaneNormal = Vector3.ProjectOnPlane(vThighAxisUp, vTorsoAxisRight);
                var vLhsProjection = Vector3.ProjectOnPlane(vTorsoAxisUp, vTorsoAxisRight);
                vAngleHipFlexionNew = Vector3.Angle(vLhsProjection, vPlaneNormal);
                vFlexCrossPrdct = Vector3.Cross(vTorsoAxisRight, vPlaneNormal);
                vFlexLHS = vLhsProjection;
            }

            AngleHipFlexion = vAngleHipFlexionNew;
            vFlexSign = Mathf.Sign(Vector3.Dot(vFlexLHS, vFlexCrossPrdct));
            LeftSignedHipFlexionAngle = vFlexSign * AngleHipFlexion * GetSign("System.Single LeftSignedHipFlexionAngle");

            //calculate the Hip Abduction angle (angles between axis projection in XY plane)
            float vAngleHipAbductionNew;
            Vector3 vAbductionCrossPrdct = Vector3.zero;
            Vector3 vAbdLHS = Vector3.zero;

            if (UseGlobalReference)
            {
                var vPlaneNormal = Vector3.ProjectOnPlane(vThighAxisUp, HipGlobalTransform.forward);
                vAngleHipAbductionNew = Vector3.Angle(HipGlobalTransform.up, vPlaneNormal);
                vAbductionCrossPrdct = Vector3.Cross(HipGlobalTransform.forward, vPlaneNormal);
                vAbdLHS = HipGlobalTransform.up;
            }
            else
            {
                var vPlaneNormal = Vector3.ProjectOnPlane(vThighAxisUp, vTorsoAxisForward);
                var vLhsProjection = Vector3.ProjectOnPlane(vTorsoAxisUp, vTorsoAxisForward);
                vAbductionCrossPrdct = Vector3.Cross(vTorsoAxisForward, vPlaneNormal);
                vAbdLHS = vLhsProjection;
                vAngleHipAbductionNew = Vector3.Angle(vLhsProjection, vPlaneNormal);
            }

            AngleHipAbduction = vAngleHipAbductionNew;
            float vAbdSign = Mathf.Sign(Vector3.Dot(vAbductionCrossPrdct, vAbdLHS));
            LeftSignedHipAbductionAngle = vAbdSign * AngleHipAbduction * GetSign("System.Single LeftSignedHipAbductionAngle");

            //calculate the Hip Rotation angle (angles between axis projection in XZ plane) 
            float vAngleHipRotationNew = 180 - Mathf.Abs(180 - ThighTransform.rotation.eulerAngles.y);
            LeftHipRotationAngle = vAngleHipRotationNew * GetSign("System.Single LeftHipRotationAngle");

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

        public void VelocityAndAccelerationExtraction(float vAngleKneeFlexionNew, float vAngleHipRotationNew, float vAngleHipAbductionNew,
         float vAngleHipFlexionNew, float vAngleKneeRotationNew, float vDeltaTime)
        {
            float vAngularVelocityKneeFlexionNew = Mathf.Abs(vAngleKneeFlexionNew - LeftKneeFlexion) / vDeltaTime;
            AngularAccelerationKneeFlexion = Mathf.Abs(vAngularVelocityKneeFlexionNew - AngularVelocityKneeFlexion) / vDeltaTime;
            AngularVelocityKneeFlexion = vAngularVelocityKneeFlexionNew;

            float vAngularVelocityRHipRotationNew = Mathf.Abs(vAngleHipRotationNew - Mathf.Abs(LeftHipRotationAngle)) / vDeltaTime;
            AngularAccelerationHipRotation = Mathf.Abs(vAngularVelocityRHipRotationNew - AngularVelocityHipRotation) / vDeltaTime;
            AngularVelocityHipRotation = vAngularVelocityRHipRotationNew;

            float vAngularVelocityHipAbductionNew = Mathf.Abs(vAngleHipAbductionNew - Mathf.Abs(AngleHipAbduction)) / vDeltaTime;
            AngularAccelerationHipAbduction = Mathf.Abs(vAngularVelocityHipAbductionNew - AngularVelocityHipAbduction) / vDeltaTime;
            AngularVelocityHipAbduction = vAngularVelocityHipAbductionNew;
            float vAngularVelocityHipFlexionNew = Mathf.Abs(vAngleHipFlexionNew - Mathf.Abs(AngleHipFlexion)) / vDeltaTime;
            AngularAccelerationHipFlexion = Mathf.Abs(vAngularVelocityHipFlexionNew - AngularVelocityHipFlexion) / vDeltaTime;
            AngularVelocityHipFlexion = vAngularVelocityHipFlexionNew;
            float vAngularVelocityKneeRotationNew = Mathf.Abs(vAngleKneeRotationNew - Mathf.Abs(AngleKneeRotation)) / vDeltaTime;
            AngularAccelerationKneeRotation = Mathf.Abs(vAngularVelocityKneeRotationNew - AngularVelocityKneeRotation) / vDeltaTime;
            AngularVelocityKneeRotation = vAngularVelocityKneeRotationNew;
        }
    }
}
