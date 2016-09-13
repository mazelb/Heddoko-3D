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
    public class TorsoAnalysis: SegmentAnalysis
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
            if(  vTimeDifference == 0)
            {
             //   return;
            }
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
			Vector3 vTorsoUpNorm = Vector3.ProjectOnPlane(vTorsoAxisUp, vHipAxisRight);
			float vAngleTorsoFlexionNew = Vector3.Angle(vHipAxisUp, vTorsoUpNorm);
			
			// calculate the Torso Flexion velocity and acceleration 
            float vAngularVelocityTorsoFlexionNew = (vAngleTorsoFlexionNew - Math.Abs(AngleTorsoFlexion)) / vTimeDifference;
            AngularAccelerationTorsoFlexion = (vAngularVelocityTorsoFlexionNew - AngularVelocityTorsoFlexion) / vTimeDifference;

			//update torso Flexion velocity & angle
            AngleTorsoFlexion = vAngleTorsoFlexionNew;
			AngularVelocityTorsoFlexion = vAngularVelocityTorsoFlexionNew;
			
			Vector3 vCross = Vector3.Cross(vHipAxisUp, vTorsoUpNorm);
            float vSign = Mathf.Sign(Vector3.Dot(vHipAxisRight, vCross));
            SignedTorsoFlexion = vSign*AngleTorsoFlexion;
            /*
            
              Vector3 vUpVector = vAnalysis.CenteredObject.right;

            //Get the projection of the perfect Vector
            Vector3 vPerfectVectProjection = Vector3.ProjectOnPlane(vAnalysis.TransformComparison.up, vUpVector);
            float vAngle = Vector3.Angle(vAnalysis.CenteredObject.up, vPerfectVectProjection);
            Quaternion vRot = Quaternion.LookRotation(vAnalysis.TransformComparison.right,
                vAnalysis.TransformComparison.up);



            Vector3 vOffset = (0.2f * vAnalysis.CenteredObject.up);
            vAnalysis.PieGraph.transform.position = vAnalysis.CenteredObject.position + vOffset;
            vAnalysis.MaskingImage.transform.position = vAnalysis.CenteredObject.position + vOffset;


            Vector3 vCross = Vector3.Cross(vAnalysis.CenteredObject.up, vPerfectVectProjection);
            float vSign = Mathf.Sign(Vector3.Dot(vUpVector, vCross));
            
            **/



            //  calculate the Torso lateral angle 
			Vector3 vAngleTorsoPlaneProjection = Vector3.ProjectOnPlane(vTorsoAxisUp, vHipAxisForward);
			float vAngleTorsoLateralNew = Vector3.Angle(vHipAxisUp, vAngleTorsoPlaneProjection); 
            float vAngularVelocityTorsoLateralNew = (vAngleTorsoLateralNew - Math.Abs(AngleTorsoLateral)) / vTimeDifference;
			Vector3 vCrossTorsoLateral = Vector3.Cross(vHipAxisForward, vAngleTorsoPlaneProjection);

            AngularAccelerationTorsoLateral = (vAngularVelocityTorsoLateralNew - AngularVelocityTorsoLateral) / vTimeDifference;
            AngularVelocityTorsoLateral = vAngularVelocityTorsoLateralNew;
            AngleTorsoLateral = vAngleTorsoLateralNew;
            SignedAngleTorsoLateral =
				Mathf.Sign(Vector3.Dot(vHipAxisUp, vCrossTorsoLateral)) * AngleTorsoLateral;

            // calculate the Torso Rotational angle 
			Vector3 vAngleTorsoRotationPlaneProjection = Vector3.ProjectOnPlane(vTorsoAxisRight, vHipAxisUp);
			float vAngleTorsoRotationNew = Vector3.Angle(vHipAxisRight, vAngleTorsoRotationPlaneProjection);
            float vAngularVelocityTorsoRotationNew = (vAngleTorsoRotationNew - Mathf.Abs(AngleTorsoRotation)) / vTimeDifference;
            AngularAccelerationTorsoRotation = (vAngularVelocityTorsoRotationNew - AngularVelocityTorsoRotation) / vTimeDifference;
            AngularVelocityTorsoRotation = vAngularVelocityTorsoRotationNew;

            Vector3 vCrossTorsoRotation = Vector3.Cross(vTorsoAxisRight, vAngleTorsoRotationPlaneProjection);
            AngleTorsoRotation = vAngleTorsoRotationNew;
			SignedAngleTorsoRotation = Mathf.Sign(Vector3.Dot(vHipAxisForward, vCrossTorsoRotation)) * AngleTorsoRotation;
            /*// Turn detection 
            if (Math.Abs(vAngleTorsoRotationNew) < 3)
            {
                AngleIntegrationTurns = 0;
            }
            else
            {
                AngleIntegrationTurns += (vAngularVelocityTorsoRotationNew * vTimeDifference);
            } 
            if (Math.Abs(AngleIntegrationTurns) > 330)
            { 
                AngleIntegrationTurns = 0;
                NumberOfTurns++; 
            }

            // Flip detection 
            if (Math.Abs(vAngularVelocityTorsoFlexionNew) < 3)
            {
                AngleIntegrationFlips = 0;
            }
            else
            {
                AngleIntegrationFlips += (AngularVelocityTorsoFlexion * vTimeDifference);
            }

            if (Math.Abs(AngleIntegrationFlips) > 330)
            { 
                NumberOfFlips++;
                AngleIntegrationFlips = 0; 
            }//*/
            NotifyAnalysisCompletionListeners();
        }

        /*Transform EstimateHipsOrientation()
        {
            // angle in [0,180]
            float angle = Vector3.Angle(a,b);
            float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

            // angle in [-179,180]
            float signed_angle = angle * sign;

            // angle in [0,360] (not used but included here for completeness)
            float angle360 =  (signed_angle + 180) % 360;

            return angle360;    
        }//*/
    }
}
