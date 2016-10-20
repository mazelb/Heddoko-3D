/** 
* @file TorsoAnalysis.cs
* @brief TorsoAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Torso
{
    [Serializable]
    public class TorsoAnalysis : SegmentAnalysis
    {
        //Angles extracted 
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleTorsoFlexion;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleTorsoLateral;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk LatBend R/L", Order = 0)]
        public float SignedAngleTorsoLateral;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleTorsoRotation;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk Rot R/L", Order = 2)]
        public float SignedAngleTorsoRotation;
        //current torso orientation
        [AnalysisSerialization(IgnoreAttribute = true)]
        public Transform TorsoTransform;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public Transform HipGlobalTransform;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public Transform HipTransform;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public Transform KneeTransform;


        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk F/E", Order = 1)]
        public float SignedTorsoFlexion;

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

        //Flips and turns detections
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleIntegrationTurns;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float AngleIntegrationFlips;
        public int NumberOfTurns;
        public int NumberOfFlips;

        /// <summary>
        /// Extract angles of torso
        /// </summary>
        public override void AngleExtraction()
        {
            float vTimeDifference = Time.time - mLastTimeCalled;
            mLastTimeCalled = Time.time;

            //Get necessary Axis info
            Vector3 vTorsoAxisUp, vTorsoAxisRight, vTorsoAxisForward;
            Vector3 vHipAxisUp, vHipAxisRight, vHipAxisForward;

            //Get the 3D axis and angles
            vTorsoAxisUp = TorsoTransform.up;
            vTorsoAxisRight = TorsoTransform.right;
            vTorsoAxisForward = TorsoTransform.forward;

            vHipAxisUp = HipGlobalTransform.up;
            vHipAxisRight = HipGlobalTransform.right;
            vHipAxisForward = HipGlobalTransform.forward;

            // calculate the Torso Flexion angle 
            float vAngleTorsoFlexionNew = Vector3.Angle(HipGlobalTransform.up, Vector3.ProjectOnPlane(vTorsoAxisUp, HipGlobalTransform.right));
            AngleTorsoFlexion = vAngleTorsoFlexionNew;

            Vector3 vCross = Vector3.Cross(vHipAxisUp, Vector3.ProjectOnPlane(vTorsoAxisUp, HipGlobalTransform.right));
            float vSign = Mathf.Sign(Vector3.Dot(vHipAxisRight, vCross));
            SignedTorsoFlexion = vSign * AngleTorsoFlexion * GetSign("System.Single SignedTorsoFlexion");

            //  calculate the Torso lateral angle 
            Vector3 vAngleTorsoPlaneProjection = Vector3.ProjectOnPlane(vTorsoAxisUp, HipGlobalTransform.forward);
            float vAngleTorsoLateralNew = Vector3.Angle(HipGlobalTransform.up, vAngleTorsoPlaneProjection);
      
            Vector3 vCrossTorsoLateral = Vector3.Cross(HipGlobalTransform.forward, vAngleTorsoPlaneProjection);
            AngleTorsoLateral = vAngleTorsoLateralNew;
            SignedAngleTorsoLateral =
                Mathf.Sign(Vector3.Dot(HipGlobalTransform.up, vCrossTorsoLateral)) * AngleTorsoLateral * GetSign("System.Single SignedAngleTorsoLateral");

            // calculate the Torso Rotational angle 
            Vector3 vAngleTorsoRotationPlaneProjection = Vector3.ProjectOnPlane(vTorsoAxisRight, HipGlobalTransform.up);
            float vAngleTorsoRotationNew = Vector3.Angle(HipGlobalTransform.right, vAngleTorsoRotationPlaneProjection);

            Vector3 vCrossTorsoRotation = Vector3.Cross(vTorsoAxisRight, vAngleTorsoRotationPlaneProjection);
            AngleTorsoRotation = vAngleTorsoRotationNew;
            SignedAngleTorsoRotation = Mathf.Sign(Vector3.Dot(HipGlobalTransform.forward, vCrossTorsoRotation)) * AngleTorsoRotation
                * GetSign("System.Single SignedAngleTorsoRotation");

            if (vTimeDifference != 0f)
            {
                VelocityAndAccelerationExtraction(  vAngleTorsoFlexionNew,   vAngleTorsoLateralNew,   vAngleTorsoRotationNew,   vTimeDifference);
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
            float vAngularVelocityTorsoFlexionNew = (vAngleTorsoFlexionNew - Math.Abs(AngleTorsoFlexion)) / vDeltaTime;
            AngularAccelerationTorsoFlexion = (vAngularVelocityTorsoFlexionNew - AngularVelocityTorsoFlexion) / vDeltaTime;
            AngularVelocityTorsoFlexion = vAngularVelocityTorsoFlexionNew;
            float vAngularVelocityTorsoLateralNew = (vAngleTorsoLateralNew - Math.Abs(AngleTorsoLateral)) / vDeltaTime;
            AngularAccelerationTorsoLateral = (vAngularVelocityTorsoLateralNew - AngularVelocityTorsoLateral) / vDeltaTime;
            AngularVelocityTorsoLateral = vAngularVelocityTorsoLateralNew;
            float vAngularVelocityTorsoRotationNew = (vAngleTorsoRotationNew - Mathf.Abs(AngleTorsoRotation)) / vDeltaTime;
            AngularAccelerationTorsoRotation = (vAngularVelocityTorsoRotationNew - AngularVelocityTorsoRotation) / vDeltaTime;
            AngularVelocityTorsoRotation = vAngularVelocityTorsoRotationNew;
        }
    }
}
