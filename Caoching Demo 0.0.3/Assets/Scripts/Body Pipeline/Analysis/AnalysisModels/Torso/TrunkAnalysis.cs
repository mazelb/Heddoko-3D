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
        public float TrunkFlexionAngle;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float TrunkLateralAngle;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float TrunkRotationAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk LatBend R/L", Order = 1)]
        public float TrunkLateralSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk Rot R/L", Order = 2)]
        public float TrunkRotationSignedAngle;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "Trunk F/E", Order = 0)]
        public float TrunkFlexionSignedAngle;

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
        public float TorsoFlexionAngularAcceleration;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float TorsoFlexionAngularVelocity;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float TorsoLateralAngularAcceleration;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float TorsoLateralAngularVelocity;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float TorsoRotationAngularAcceleration;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float TorsoRotationAngularVelocity;

        /// <summary>
        /// Extract angles of torso
        /// </summary>
        public override void AngleExtraction()
        {
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
            float vAngleTorsoFlexionNew;
            if (vTrunkAxisUpProjectedOnHipRight == Vector3.zero)
            {
                vAngleTorsoFlexionNew = 0.0f;
            }
            else
            {
                vAngleTorsoFlexionNew = Vector3.Angle(vHipAxisUp, vTrunkAxisUpProjectedOnHipRight);
            }
            TrunkFlexionAngle = vAngleTorsoFlexionNew;
            Vector3 vCross = Vector3.Cross(vTrunkAxisUpProjectedOnHipRight, vHipAxisUp);
            float vSign = Mathf.Sign(Vector3.Dot(vHipAxisRight, vCross));
            TrunkFlexionSignedAngle = vSign * TrunkFlexionAngle * GetSign("System.Single TrunkFlexionAngle");

            // calculate the Trunk lateral angle 
            Vector3 vTrunkAxisUpProjectedOnHipForward = Vector3.ProjectOnPlane(vTrunkAxisUp, vHipAxisForward);
            float vAngleTrunkLateralNew;
            if (vTrunkAxisUpProjectedOnHipForward == Vector3.zero)
            {
                vAngleTrunkLateralNew = 0.0f;
            }
            else
            {
                vAngleTrunkLateralNew = Vector3.Angle(vHipAxisUp, vTrunkAxisUpProjectedOnHipForward);
            }
            TrunkLateralAngle = vAngleTrunkLateralNew;
            vCross = Vector3.Cross(vTrunkAxisUpProjectedOnHipForward, vHipAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vHipAxisForward, vCross));
            TrunkLateralSignedAngle = vSign * TrunkLateralAngle * GetSign("System.Single TrunkLateralAngle");

            // calculate the Trunk Rotational angle 
            Vector3 vTrunkAxisForwardProjectedOnHipUp = Vector3.ProjectOnPlane(vTrunkAxisForward, vHipAxisUp);
            float vAngleTorsoRotationNew;
            if (vTrunkAxisForwardProjectedOnHipUp == Vector3.zero)
            {
                vAngleTorsoRotationNew = 0.0f;
            }
            else
            {
                vAngleTorsoRotationNew = Vector3.Angle(vHipAxisForward, vTrunkAxisForwardProjectedOnHipUp);
            }
            TrunkRotationAngle = vAngleTorsoRotationNew;
            vCross = Vector3.Cross(vTrunkAxisForwardProjectedOnHipUp, vHipAxisForward);
            vSign = Mathf.Sign(Vector3.Dot(vHipAxisUp, vCross));
            TrunkRotationSignedAngle = vSign * TrunkRotationAngle * GetSign("System.Single TrunkRotationAngle");

            //Calculate the velocity and accelerations
            if (DeltaTime != 0f)
            {
                VelocityAndAccelerationExtraction( vAngleTorsoFlexionNew,   vAngleTrunkLateralNew,   vAngleTorsoRotationNew, DeltaTime);
            }

            //notify listeners that analysis on this component has been completed. 
            NotifyAnalysisCompletionListeners();
        }

        //TODO: Review all velocity and acceleration angles
        /// <summary>
        /// Extract velocity and acceleration
        /// </summary>
        /// <param name="vAngleTorsoFlexionNew"></param>
        /// <param name="vDeltaTime"></param>
        public void VelocityAndAccelerationExtraction(float vAngleTorsoFlexionNew,float vAngleTorsoLateralNew, float vAngleTorsoRotationNew,  float vDeltaTime)
        {
            float vAngularVelocityTorsoFlexionNew = (vAngleTorsoFlexionNew - Math.Abs(TrunkFlexionAngle)) / vDeltaTime;
            TorsoFlexionAngularAcceleration = (vAngularVelocityTorsoFlexionNew - TorsoFlexionAngularVelocity) / vDeltaTime;
            TorsoFlexionAngularVelocity = vAngularVelocityTorsoFlexionNew;
            float vAngularVelocityTorsoLateralNew = (vAngleTorsoLateralNew - Math.Abs(TrunkLateralAngle)) / vDeltaTime;
            TorsoLateralAngularAcceleration = (vAngularVelocityTorsoLateralNew - TorsoLateralAngularVelocity) / vDeltaTime;
            TorsoLateralAngularVelocity = vAngularVelocityTorsoLateralNew;
            float vAngularVelocityTorsoRotationNew = (vAngleTorsoRotationNew - Mathf.Abs(TrunkRotationAngle)) / vDeltaTime;
            TorsoRotationAngularAcceleration = (vAngularVelocityTorsoRotationNew - TorsoRotationAngularVelocity) / vDeltaTime;
            TorsoRotationAngularVelocity = vAngularVelocityTorsoRotationNew;
        }
    }
}
