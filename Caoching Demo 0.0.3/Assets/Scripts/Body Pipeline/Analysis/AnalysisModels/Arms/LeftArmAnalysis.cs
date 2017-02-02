/** 
* @file LeftArmAnalysis.cs
* @brief LeftArmAnalysis the Joint class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System;

namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    /**
    * LeftArmAnalysis class 
    * @brief LeftArmAnalysis class 
    */
    [Serializable]
    public class LeftArmAnalysis : ArmAnalysis
    {
        //Elbow Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowFlexionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LElbow F/E", Order = 18)]
        public float LeftElbowFlexionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LForeArm Pro/Sup", Order = 20)]
        public float LeftForeArmPronationSignedAngle = 0;

        //Upper Arm Angles
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderFlexionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould F/E", Order = 4)]
        public float LeftShoulderFlexionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderVertAbductionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould V Add/Abd", Order = 6)]
        public float LeftShoulderVerticalAbductionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderHorAbductionAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould H Add/Abd", Order = 8)]
        public float LeftShoulderHorizontalAbductionSignedAngle = 0;
        [AnalysisSerialization(IgnoreAttribute = false, AttributeName = "LShould Rot", Order = 10)]
        public float LeftShoulderRotationSignedAngle = 0;

        //Velocities and Accelerations
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowFlexionPeakAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftForeArmPronationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftElbowPronationAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderFlexionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderFlexionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderVertAbductionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderVertAbductionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderHorAbductionAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderHorAbductionAngularAcceleration = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderRotationAngularVelocity = 0;
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float LeftShoulderRotationAngularAcceleration = 0;


        /// <summary>
        /// Reset the metrics calculations
        /// </summary>
        public override void ResetMetrics()
        {
            LeftElbowFlexionPeakAngularVelocity = 0;
        }

        /// <summary>
        /// Extract angles from orientations
        /// </summary>
        public override void AngleExtraction()
        {
            //Get necessary Axis info
            Vector3 vTrunkAxisUp, vTrunkAxisRight, vTrunkAxisForward;
            Vector3 vShoulderAxisUp, vShoulderAxisRight, vShoulderAxisForward;
            Vector3 vElbowAxisUp, vElbowAxisRight, vElbowAxisForward;

            //Get the 3D axis and angles
            vTrunkAxisUp = TorsoTransform.up;
            vTrunkAxisRight = TorsoTransform.right;
            vTrunkAxisForward = TorsoTransform.forward;

            vShoulderAxisUp = UpArTransform.up;
            vShoulderAxisRight = UpArTransform.right;
            vShoulderAxisForward = UpArTransform.forward;

            vElbowAxisUp = LoArTransform.up;
            vElbowAxisRight = LoArTransform.right;
            vElbowAxisForward = LoArTransform.forward;

            //calculate the Elbow Flexion angle
            float vAngleElbowFlexionNew = Vector3.Angle( vElbowAxisRight, -vShoulderAxisRight);
            LeftElbowFlexionAngle = vAngleElbowFlexionNew;
            Vector3 vCross = Vector3.Cross(vElbowAxisRight, -vShoulderAxisRight);
            Debug.DrawLine(Vector3.zero, vCross,Color.red,0.1f);
            float vSign = Mathf.Sign(Vector3.Dot(vShoulderAxisUp, vCross));
            LeftElbowFlexionSignedAngle = vSign * LeftElbowFlexionAngle * GetSign("System.Single LeftElbowFlexionAngle");

            //calculate the Elbow Pronation angle
            float vAngleForeArmPronationNew = LoArTransform.rotation.eulerAngles.x;//  -UpArTransform.rotation.eulerAngles.x -360f);

            if (vAngleForeArmPronationNew > 180)
            {
                vAngleForeArmPronationNew =  vAngleForeArmPronationNew -360f;
            } 
            LeftForeArmPronationSignedAngle = vAngleForeArmPronationNew * GetSign("System.Single LeftForeArmPronationAngle");

            //calculate the Shoulder Flexion angle
            Vector3 vShoulderRightAxisProjectedOnTrunkRight = Vector3.ProjectOnPlane(-vShoulderAxisRight, vTrunkAxisRight);
            float vAngleShoulderFlexionNew;
            var vShoulderProjSqrMag = vShoulderRightAxisProjectedOnTrunkRight.sqrMagnitude;
            //check if the projection's square magnitude is under a certain tolerance. 
            if (Math.Abs(vShoulderProjSqrMag) < 0.001f)
            {
                vAngleShoulderFlexionNew = 0.0f;
            }
            else
            {
                vAngleShoulderFlexionNew = Vector3.Angle(-vTrunkAxisUp, vShoulderRightAxisProjectedOnTrunkRight);
            }
            LeftShoulderFlexionAngle = vAngleShoulderFlexionNew;
            vCross = Vector3.Cross(-vTrunkAxisUp, vShoulderRightAxisProjectedOnTrunkRight);
            vSign = Mathf.Sign(Vector3.Dot(-vTrunkAxisRight, vCross));
            LeftShoulderFlexionSignedAngle = vSign * LeftShoulderFlexionAngle * GetSign("System.Single LeftShoulderFlexionAngle");

            //calculate the Shoulder Abduction Vertical angle
            Vector3 vVerticalShoulderAbdProjection = Vector3.ProjectOnPlane(-vShoulderAxisRight, vTrunkAxisForward);
            float vAngleShoulderVertAbductionNew;
            var vShoulderVertAbductionProjSqrMag = vVerticalShoulderAbdProjection.sqrMagnitude;
            if (vShoulderVertAbductionProjSqrMag < 0.001f)
            {
                vAngleShoulderVertAbductionNew = 0.0f;
            }
            else
            {
                vAngleShoulderVertAbductionNew = Vector3.Angle(-vTrunkAxisUp, vVerticalShoulderAbdProjection);
            }
            LeftShoulderVertAbductionAngle = vAngleShoulderVertAbductionNew;
            vCross = Vector3.Cross(vVerticalShoulderAbdProjection, -vTrunkAxisUp);
            vSign = Mathf.Sign(Vector3.Dot(vTrunkAxisForward, vCross));
            LeftShoulderVerticalAbductionSignedAngle = vSign * LeftShoulderVertAbductionAngle * GetSign("System.Single LeftShoulderVertAbductionAngle");
            Vector3 vHorizontalShoulderAbdProjection = Vector3.ProjectOnPlane(-vShoulderAxisRight, vTrunkAxisUp);

            //check if the arm is above the head 
            if (vAngleShoulderVertAbductionNew >= 165 || LeftShoulderVerticalAbductionSignedAngle <= 0)
            {
                vHorizontalShoulderAbdProjection = Vector3.ProjectOnPlane(-vShoulderAxisRight, -vTrunkAxisRight);
            }
            //calculate the Shoulder Abduction Horizontal angle
            float vAngleShoulderHorAbductionNew;
            if (vHorizontalShoulderAbdProjection == Vector3.zero)
            {
                vAngleShoulderHorAbductionNew = 0.0f;
            }
            else
            {

                if (vAngleShoulderVertAbductionNew >= 165)
                {
                    vAngleShoulderHorAbductionNew = Vector3.Angle(vTrunkAxisUp, vHorizontalShoulderAbdProjection);
                }
                else if (LeftShoulderVerticalAbductionSignedAngle <= 0)
                {
                    vAngleShoulderHorAbductionNew = Vector3.Angle(-vTrunkAxisUp, vHorizontalShoulderAbdProjection);
                }

                else
                {
                    vAngleShoulderHorAbductionNew = Vector3.Angle(-vTrunkAxisRight, vHorizontalShoulderAbdProjection);
                }
            }

            LeftShoulderHorAbductionAngle = vAngleShoulderHorAbductionNew;
            vCross = Vector3.Cross(-vTrunkAxisRight, vHorizontalShoulderAbdProjection);
            vSign = Mathf.Sign(Vector3.Dot(vTrunkAxisUp, vCross));
            LeftShoulderHorizontalAbductionSignedAngle = vSign * LeftShoulderHorAbductionAngle * GetSign("System.Single LeftShoulderHorAbductionAngle");
             
            //get the cross product in order to determine the sign of the angle

            float vAngleShoulderRotationNew = - UpArTransform.rotation.eulerAngles.x;//  -UpArTransform.rotation.eulerAngles.x -360f);
         
            if (vAngleShoulderRotationNew < -180)
            {
                vAngleShoulderRotationNew =  360f +vAngleShoulderRotationNew ;
            }
            LeftShoulderRotationSignedAngle = vAngleShoulderRotationNew;// * vSign * GetSign("System.Single LeftShoulderRotationAngle");

            //Calculate the velocity and accelerations
            if (DeltaTime != 0)
            {
                VelocityAndAccelerationExtraction(vAngleElbowFlexionNew, vAngleShoulderRotationNew,
                    vAngleShoulderHorAbductionNew,
                    vAngleShoulderVertAbductionNew, vAngleShoulderFlexionNew, vAngleForeArmPronationNew, DeltaTime);
            }

            //notify listeners that analysis on this component has been completed. 
            NotifyAnalysisCompletionListeners();
        }

        //TODO: Review all velocity and acceleration angles
        public void VelocityAndAccelerationExtraction(float vAngleElbowFlexionNew, float vAngleShoulderRotationNew, float vAngleShoulderHorAbductionNew,
            float vAngleShoulderVertAbductionNew, float vAngleShoulderFlexionNew, float vAngleElbowPronationNew, float vDeltaTime)
        {
            float vAngularVelocityElbowFlexionNew = (vAngleElbowFlexionNew - LeftElbowFlexionAngle) / DeltaTime;
            LeftElbowFlexionAngularAcceleration = (vAngularVelocityElbowFlexionNew - LeftElbowFlexionAngularVelocity) / DeltaTime;
            LeftElbowFlexionAngularVelocity = vAngularVelocityElbowFlexionNew;
            float vAngularVelocityShoulderRotationNew = (vAngleShoulderRotationNew - Mathf.Abs(LeftShoulderRotationSignedAngle)) / DeltaTime;
            LeftShoulderRotationAngularAcceleration = (vAngularVelocityShoulderRotationNew - LeftShoulderRotationAngularVelocity) / DeltaTime;
            LeftShoulderRotationAngularVelocity = vAngularVelocityShoulderRotationNew;
            float vAngularVelocityShoulderHorAbductionNew = (vAngleShoulderHorAbductionNew - Mathf.Abs(LeftShoulderHorAbductionAngle)) / DeltaTime;
            LeftShoulderHorAbductionAngularAcceleration = (vAngularVelocityShoulderHorAbductionNew - LeftShoulderHorAbductionAngularVelocity) / DeltaTime;
            LeftShoulderHorAbductionAngularVelocity = vAngularVelocityShoulderHorAbductionNew;
            float vAngularVelocityShoulderVertAbductionNew = (vAngleShoulderVertAbductionNew - Mathf.Abs(LeftShoulderVertAbductionAngle)) / DeltaTime;
            LeftShoulderVertAbductionAngularAcceleration = (vAngularVelocityShoulderVertAbductionNew - LeftShoulderVertAbductionAngularVelocity) / DeltaTime;
            LeftShoulderVertAbductionAngularVelocity = vAngularVelocityShoulderVertAbductionNew;
            float vAngularVelocityShoulderFlexionNew = (vAngleShoulderFlexionNew - Mathf.Abs(LeftShoulderFlexionAngle)) / DeltaTime;
            LeftShoulderFlexionAngularAcceleration = (vAngularVelocityShoulderFlexionNew - LeftShoulderFlexionAngularVelocity) / DeltaTime;
            LeftShoulderFlexionAngularVelocity = vAngularVelocityShoulderFlexionNew;
            float vAngularVelocityElbowPronationNew = (vAngleElbowPronationNew - Mathf.Abs(LeftForeArmPronationSignedAngle)) / DeltaTime;
            LeftElbowPronationAngularAcceleration = (vAngularVelocityElbowPronationNew - LeftForeArmPronationAngularVelocity) / DeltaTime;
            LeftForeArmPronationAngularVelocity = vAngularVelocityElbowPronationNew;
            LeftElbowFlexionPeakAngularVelocity = Mathf.Max(Mathf.Abs(LeftElbowFlexionAngularVelocity), LeftElbowFlexionPeakAngularVelocity);

        }

        Vector3 GetProjection(Vector3 vDirection)
        {
            Vector3 vProjection = Vector3.zero;
            return vProjection;

        }

        Vector3 GetPlaneNormal(Vector3 vDirection, Transform vTransform, Transform vParent)
        {
            Vector3 vProjection = Vector3.zero;



            float vOutputRightPlane = float.MaxValue;
            float vOutputLeftPlane = float.MaxValue;
            float vOutputUpPlane = float.MaxValue;
            float vOutputDownPlane = float.MaxValue;
            float vOutputForwardPlane = float.MaxValue;
            float vOutputBackPlane = float.MaxValue;

            bool vIsIntersectRightPlane = false;
            bool vIsIntersectLeftPlane = false;
            bool vIsIntersectUpPlane = false;
            bool vIsIntersectDownPlane = false;
            bool vIsIntersectForwardPlane = false;
            bool vIsIntersectBackPlane = false;

            //right plane check
            Vector3 vNormalCheck = vParent.right;
            var vPlane = new Plane(vNormalCheck, (vTransform.position + vNormalCheck).normalized * 3f);
            vIsIntersectRightPlane = vPlane.Raycast(new Ray(vTransform.position, vDirection), out vOutputRightPlane);
            //left plane check 
            vNormalCheck = -vParent.right;
            vPlane = new Plane(vNormalCheck, (vTransform.position + vNormalCheck).normalized * 3f);
            vIsIntersectLeftPlane = vPlane.Raycast(new Ray(vTransform.position, vDirection), out vOutputLeftPlane);

            //up plane check 
            vNormalCheck = vParent.up;
            vPlane = new Plane(vNormalCheck, (vTransform.position + vNormalCheck).normalized * 3f);
            vIsIntersectUpPlane = vPlane.Raycast(new Ray(vTransform.position, vDirection), out vOutputUpPlane);
            //down plane check
            vNormalCheck = -vParent.up;
            vPlane = new Plane(vNormalCheck, (vTransform.position + vNormalCheck).normalized * 3f);
            vIsIntersectDownPlane = vPlane.Raycast(new Ray(vTransform.position, vDirection), out vOutputDownPlane);

            //forward plane check 
            vNormalCheck = vParent.forward;
            vPlane = new Plane(vNormalCheck, (vTransform.position + vNormalCheck).normalized * 3f);
            vIsIntersectForwardPlane = vPlane.Raycast(new Ray(vTransform.position, vDirection), out vOutputForwardPlane);
            //down plane check
            vNormalCheck = -vParent.forward;
            vPlane = new Plane(vNormalCheck, (vTransform.position + vNormalCheck).normalized * 3f);
            vIsIntersectBackPlane = vPlane.Raycast(new Ray(vTransform.position, vDirection), out vOutputBackPlane);

            vOutputRightPlane = Mathf.Abs(vOutputRightPlane);
            vOutputLeftPlane = Mathf.Abs(vOutputLeftPlane);
            vOutputUpPlane = Mathf.Abs(vOutputUpPlane);
            vOutputDownPlane = Mathf.Abs(vOutputDownPlane);
            vOutputForwardPlane = Mathf.Abs(vOutputForwardPlane);
            vOutputBackPlane = Mathf.Abs(vOutputBackPlane);
            //check the smallest distance of all intersections 
            if (vOutputRightPlane < vOutputLeftPlane && vOutputRightPlane < vOutputUpPlane
                && vOutputRightPlane < vOutputDownPlane && vOutputRightPlane < vOutputForwardPlane
                && vOutputRightPlane < vOutputBackPlane)
            {
                return vParent.right;
            }
            else if (vOutputLeftPlane < vOutputRightPlane && vOutputLeftPlane < vOutputUpPlane
                && vOutputLeftPlane < vOutputDownPlane && vOutputLeftPlane < vOutputForwardPlane
                && vOutputLeftPlane < vOutputBackPlane)
            {

                return vParent.right;
            }
            else if (vOutputUpPlane < vOutputRightPlane && vOutputUpPlane < vOutputLeftPlane
              && vOutputUpPlane < vOutputDownPlane && vOutputUpPlane < vOutputForwardPlane
              && vOutputUpPlane < vOutputBackPlane)
            {
                return vParent.up;
            }
            else if (vOutputDownPlane < vOutputRightPlane && vOutputDownPlane < vOutputLeftPlane
            && vOutputDownPlane < vOutputUpPlane && vOutputDownPlane < vOutputForwardPlane
            && vOutputDownPlane < vOutputBackPlane)
            {
                return vParent.up;
            }
            else if (vOutputForwardPlane < vOutputRightPlane && vOutputForwardPlane < vOutputLeftPlane
            && vOutputForwardPlane < vOutputUpPlane && vOutputForwardPlane < vOutputDownPlane
            && vOutputForwardPlane < vOutputBackPlane)
            {
                return vParent.forward;
            }
            else if (vOutputBackPlane < vOutputRightPlane && vOutputBackPlane < vOutputLeftPlane
           && vOutputBackPlane < vOutputUpPlane && vOutputBackPlane < vOutputDownPlane
           && vOutputBackPlane < vOutputForwardPlane)
            {

                return vParent.forward;
            }


            return vProjection;
        }


    }
}