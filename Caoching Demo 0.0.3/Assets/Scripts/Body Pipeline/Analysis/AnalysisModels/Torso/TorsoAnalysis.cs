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
        //current torso orientation
        public Transform TorsoTransform;
        public Transform HipGlobalTransform;
        public Transform HipTransform;
        public Transform KneeTransform;

        //Angles extracted
        public float AngleTorsoFlexion;
        public float AngleTorsoLateral;
        public float AngleTorsoRotation;
        public float SignedTorsoFlexion;
         
        //Accelerations and velocities
        public float AngularAccelerationTorsoFlexion;
        public float AngularVelocityTorsoFlexion;
        public float AngularAccelerationTorsoLateral;
        public float AngularVelocityTorsoLateral;
        public float AngularAccelerationTorsoRotation;
        public float AngularVelocityTorsoRotation;

        //Flips and turns detections
        public float AngleIntegrationTurns;
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
                return;
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
            float vAngleTorsoFlexionNew = Vector3.Angle(HipGlobalTransform.up, Vector3.ProjectOnPlane(vTorsoAxisUp, HipGlobalTransform.right));
            float vAngularVelocityTorsoFlexionNew = (vAngleTorsoFlexionNew - Math.Abs(AngleTorsoFlexion)) / vTimeDifference;
            AngularAccelerationTorsoFlexion = (vAngularVelocityTorsoFlexionNew - AngularVelocityTorsoFlexion) / vTimeDifference;
            AngularVelocityTorsoFlexion = vAngularVelocityTorsoFlexionNew;
            AngleTorsoFlexion = vAngleTorsoFlexionNew;
  
            Vector3 vCross = Vector3.Cross(vHipAxisUp, Vector3.ProjectOnPlane(vTorsoAxisUp, HipGlobalTransform.right));
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
            float vAngleTorsoLateralNew = Vector3.Angle(HipGlobalTransform.up, Vector3.ProjectOnPlane(vTorsoAxisUp, HipGlobalTransform.forward)); 
            float vAngularVelocityTorsoLateralNew = (vAngleTorsoLateralNew - Math.Abs(AngleTorsoLateral)) / vTimeDifference;
            AngularAccelerationTorsoLateral = (vAngularVelocityTorsoLateralNew - AngularVelocityTorsoLateral) / vTimeDifference;
            AngularVelocityTorsoLateral = vAngularVelocityTorsoLateralNew;
            AngleTorsoLateral = vAngleTorsoLateralNew;

            // calculate the Torso Rotational angle 
            float vAngleTorsoRotationNew = Vector3.Angle(HipGlobalTransform.right, Vector3.ProjectOnPlane(vTorsoAxisRight, HipGlobalTransform.up));
            float vAngularVelocityTorsoRotationNew = (vAngleTorsoRotationNew - Mathf.Abs(AngleTorsoRotation)) / vTimeDifference;
            AngularAccelerationTorsoRotation = (vAngularVelocityTorsoRotationNew - AngularVelocityTorsoRotation) / vTimeDifference;
            AngularVelocityTorsoRotation = vAngularVelocityTorsoRotationNew;
            AngleTorsoRotation = vAngleTorsoRotationNew;

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
