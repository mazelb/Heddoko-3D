/** 
* @file TorsoAnalysis.cs
* @brief TorsoAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Trunk
{
    [Serializable]
    public class TrunkAnalysis : SegmentAnalysis
    {
        //Angles extracted 
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleTrunkFlexion;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleTrunkLateral;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleTrunkRotation;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk LatBend R/L", Order = 0)]
        public float SignedAngleTrunkLateral;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk Rot R/L", Order = 2)]
        public float SignedAngleTrunkRotation;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk F/E", Order = 1)]
        public float SignedTrunkFlexion;

        //current transforms
        [AnalysisSerialization(IgnoreAttribute = true)]
        public Transform TrunkTransform;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public Transform HipGlobalTransform;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public Transform HipTransform;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public Transform KneeTransform;

        //Accelerations and velocities
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationTorsoFlexion;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityTorsoFlexion;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationTorsoLateral;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityTorsoLateral;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularAccelerationTorsoRotation;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngularVelocityTorsoRotation;

        /// <summary>
        /// Extract angles of torso
        /// </summary>
        public override void AngleExtraction()
        {
            float vTimeDifference = Time.time - mLastTimeCalled;
            mLastTimeCalled = Time.time;

            //Get necessary Axis info
            Vector3 vTrunkAxisUp, vTrunkAxisRight, vTrunkAxisForward;
            Vector3 vHipAxisUp, vHipAxisRight, vHipAxisForward;

            //Get the 3D axis and angles
            vTrunkAxisUp = TrunkTransform.up;
            vTrunkAxisRight = TrunkTransform.right;
            vTrunkAxisForward = TrunkTransform.forward;

            vHipAxisUp = HipGlobalTransform.up;
            vHipAxisRight = HipGlobalTransform.right;
            vHipAxisForward = HipGlobalTransform.forward;

            // calculate the Trunk Flexion angle
            Vector3 vTrunkAxisUpProjectedOnHipRight = Vector3.ProjectOnPlane(vTrunkAxisUp, vHipAxisRight);
            float vAngleTorsoFlexionNew = Vector3.Angle(vHipAxisUp, vTrunkAxisUpProjectedOnHipRight);
            AngleTrunkFlexion = vAngleTorsoFlexionNew;
            Vector3 vCross = Vector3.Cross(vTrunkAxisUpProjectedOnHipRight, vHipAxisUp);
            float vSign = Mathf.Sign(Vector3.Dot(vHipAxisRight, vCross));
            SignedTrunkFlexion = vSign * AngleTrunkFlexion * GetSign("System.Single SignedTorsoFlexion");

            // calculate the Trunk lateral angle 
            Vector3 vTrunkAxisUpProjectedOnHipForward = Vector3.ProjectOnPlane(vTrunkAxisUp, vHipAxisForward);
            float vAngleTrunkLateralNew = Vector3.Angle(vHipAxisUp, vTrunkAxisUpProjectedOnHipForward);
            AngleTrunkLateral = vAngleTrunkLateralNew;
            vCross = Vector3.Cross(vTrunkAxisUpProjectedOnHipForward, vHipAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vHipAxisForward, vCross));
            SignedAngleTrunkLateral = vSign * AngleTrunkLateral * GetSign("System.Single SignedAngleTorsoLateral");

            // calculate the Trunk Rotational angle 
            Vector3 vTrunkAxisForwardProjectedOnHipUp = Vector3.ProjectOnPlane(vTrunkAxisForward, vHipAxisUp);
            float vAngleTorsoRotationNew = Vector3.Angle(vHipAxisForward, vTrunkAxisForwardProjectedOnHipUp);
            AngleTrunkRotation = vAngleTorsoRotationNew;
            vCross = Vector3.Cross(vTrunkAxisForwardProjectedOnHipUp, vHipAxisForward);
            vSign = Mathf.Sign(Vector3.Dot(vHipAxisUp, vCross));
            SignedAngleTrunkRotation = vSign * AngleTrunkRotation * GetSign("System.Single SignedAngleTorsoRotation");

            //Calculate the velocity and accelerations
            if (vTimeDifference != 0f)
            {
                VelocityAndAccelerationExtraction( vAngleTorsoFlexionNew,   vAngleTrunkLateralNew,   vAngleTorsoRotationNew,   vTimeDifference);
            }

            //notify listeners that analysis on this component has been completed. 
            NotifyAnalysisCompletionListeners();
        }

        /// <summary>
        /// Extract velocity and acceleration
        /// </summary>
        /// <param name="vAngleTorsoFlexionNew"></param>
        /// <param name="vDeltaTime"></param>
        public void VelocityAndAccelerationExtraction(float vAngleTorsoFlexionNew,float vAngleTorsoLateralNew, float vAngleTorsoRotationNew,  float vDeltaTime)
        {
            float vAngularVelocityTorsoFlexionNew = (vAngleTorsoFlexionNew - Math.Abs(AngleTrunkFlexion)) / vDeltaTime;
            AngularAccelerationTorsoFlexion = (vAngularVelocityTorsoFlexionNew - AngularVelocityTorsoFlexion) / vDeltaTime;
            AngularVelocityTorsoFlexion = vAngularVelocityTorsoFlexionNew;
            float vAngularVelocityTorsoLateralNew = (vAngleTorsoLateralNew - Math.Abs(AngleTrunkLateral)) / vDeltaTime;
            AngularAccelerationTorsoLateral = (vAngularVelocityTorsoLateralNew - AngularVelocityTorsoLateral) / vDeltaTime;
            AngularVelocityTorsoLateral = vAngularVelocityTorsoLateralNew;
            float vAngularVelocityTorsoRotationNew = (vAngleTorsoRotationNew - Mathf.Abs(AngleTrunkRotation)) / vDeltaTime;
            AngularAccelerationTorsoRotation = (vAngularVelocityTorsoRotationNew - AngularVelocityTorsoRotation) / vDeltaTime;
            AngularVelocityTorsoRotation = vAngularVelocityTorsoRotationNew;
        }
    }
}
