
/** 
* @file RightLegAnalysis.cs
* @brief RightLegAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System; 
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
    /// <summary>
    /// Represents the anaylsis of the right leg segment
    /// </summary>
    [Serializable]
    public class RightLegAnalysis : LegAnalysis
    {
        //Knee Angles
        public float AngleKneeFlexion;
        public float AngleKneeRotation;

        //Hip Angles
        public bool UseGlobalReference = false;
        public float AngleHipFlexion;
        public float AngleHipAbduction;
        public float AngleHipRotation;

        //Accelerations and velocities
        public float AngularVelocityKneeFlexion = 0;
        public float AngularAccelerationKneeFlexion = 0;
        public float AngularVelocityKneeRotation = 0;
        public float AngularAccelerationKneeRotation = 0;
        public float AngularVelocityHipFlexion = 0;
        public float AngularAccelerationHipFlexion = 0;
        public float AngularVelocityHipAbduction = 0;
        public float AngularAccelerationHipAbduction = 0;
        public float AngularVelocityHipRotation = 0;
        public float AngularAccelerationHipRotation = 0;

        //Squats Analytics
        public float NumberofSquats;
        public float AngleSum;
        private bool mStartCountingSquats = true;

        //Detection of vertical Hip position
        public float LegHeight;
        private float mInitThighHeight = 0.475f;
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
            if ( vDeltaTime == 0)
            {
                return;
            }
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
            float vAngularVelocityKneeFlexionNew = Mathf.Abs(vAngleKneeFlexionNew - AngleKneeFlexion) / vDeltaTime;
            AngularAccelerationKneeFlexion = Mathf.Abs(vAngularVelocityKneeFlexionNew - AngularVelocityKneeFlexion) / vDeltaTime;
            AngularVelocityKneeFlexion = vAngularVelocityKneeFlexionNew;
            //Signed angle adduction calculation

         
     

            //Squatts counting
            if ( mStartCountingSquats)
            {
                if (Math.Abs(vAngleKneeFlexionNew) > 15) 
                {
                    AngleSum += Math.Abs(vAngleKneeFlexionNew - AngleKneeFlexion);
                }
                else
                {
                    AngleSum = 0;
                }

                if (Math.Abs(AngleSum) > 140)
                {
                    AngleSum = 0;
                    NumberofSquats++;
                }
            }

            AngleKneeFlexion = vAngleKneeFlexionNew;

            //calculate the Knee Rotation angle (angles between axis projection in XZ plane)
            float vAngleKneeRotationNew = 180 - Mathf.Abs(180 - KneeTransform.rotation.eulerAngles.y);
            float vAngularVelocityKneeRotationNew = Mathf.Abs(vAngleKneeRotationNew - Mathf.Abs(AngleKneeRotation)) / vDeltaTime;
            AngularAccelerationKneeRotation = Mathf.Abs(vAngularVelocityKneeRotationNew - AngularVelocityKneeRotation) / vDeltaTime;
            AngularVelocityKneeRotation = vAngularVelocityKneeRotationNew;
            AngleKneeRotation = vAngleKneeRotationNew;

            //calculate the Hip Flexion angle (angles between axis projection in YZ plane)
            float vAngleHipFlexionNew;

            if (UseGlobalReference)
            {
                vAngleHipFlexionNew = Vector3.Angle(HipGlobalTransform.up, Vector3.ProjectOnPlane(vThighAxisUp, HipGlobalTransform.right));
            }
            else
            {
                vAngleHipFlexionNew = Vector3.Angle(Vector3.ProjectOnPlane(vTorsoAxisUp, vTorsoAxisRight), Vector3.ProjectOnPlane(vThighAxisUp, vTorsoAxisRight));
            }

            float vAngularVelocityHipFlexionNew = Mathf.Abs(vAngleHipFlexionNew - Mathf.Abs(AngleHipFlexion)) / vDeltaTime;
            AngularAccelerationHipFlexion = Mathf.Abs(vAngularVelocityHipFlexionNew - AngularVelocityHipFlexion) / vDeltaTime;
            AngularVelocityHipFlexion = vAngularVelocityHipFlexionNew;
            AngleHipFlexion = vAngleHipFlexionNew;

            //calculate the Hip Abduction angle (angles between axis projection in XY plane)
            float vAngleHipAbductionNew;

            if (UseGlobalReference)
            {
                vAngleHipAbductionNew = Vector3.Angle(HipGlobalTransform.up, Vector3.ProjectOnPlane(vThighAxisUp, HipGlobalTransform.forward));
            }
            else
            {
                vAngleHipAbductionNew = Vector3.Angle(Vector3.ProjectOnPlane(vTorsoAxisUp, vTorsoAxisForward), Vector3.ProjectOnPlane(vThighAxisUp, vTorsoAxisForward));
            }

            float vAngularVelocityHipAbductionNew = Mathf.Abs(vAngleHipAbductionNew - Mathf.Abs(AngleHipAbduction)) / vDeltaTime;
            AngularAccelerationHipAbduction = Mathf.Abs(vAngularVelocityHipAbductionNew - AngularVelocityHipAbduction) / vDeltaTime;
            AngularVelocityHipAbduction = vAngularVelocityHipAbductionNew;
            AngleHipAbduction = vAngleHipAbductionNew;

            //calculate the Hip Rotation angle (angles between axis projection in XZ plane) 
            float vAngleHipRotationNew = 180 - Mathf.Abs(180 - ThighTransform.rotation.eulerAngles.y); 
            float vAngularVelocityRHipRotationNew = Mathf.Abs(vAngleHipRotationNew - Mathf.Abs(AngleHipRotation)) / vDeltaTime;
            AngularAccelerationHipRotation = Mathf.Abs(vAngularVelocityRHipRotationNew - AngularVelocityHipRotation) / vDeltaTime;
            AngularVelocityHipRotation = vAngularVelocityRHipRotationNew;
            AngleHipRotation = vAngleHipRotationNew;
            //*/

            //Calculate Leg height 
            float vThighHeight = mInitThighHeight * Mathf.Abs(Vector3.Dot(vThighAxisUp, Vector3.up));
            float vTibiaHeight = mInitTibiaHeight * Mathf.Abs(Vector3.Dot(vKneeAxisUp, Vector3.up));
            LegHeight = vThighHeight + vTibiaHeight;

            float vThighStride = Mathf.Sqrt((mInitThighHeight * mInitThighHeight) - (vThighHeight * vThighHeight));
            float vTibiaStride = Mathf.Sqrt((mInitTibiaHeight * mInitTibiaHeight) - (vTibiaHeight * vTibiaHeight)); 
            //Debug.Log(vTibiaStride);

            Vector3 vThighDirection = -vThighAxisUp.normalized;
            Vector3 vTibiaDirection = -vKneeAxisUp.normalized;

            RightLegStride = Vector3.ProjectOnPlane((vThighStride * vThighDirection), Vector3.up) + Vector3.ProjectOnPlane((vTibiaStride * vTibiaDirection), Vector3.up);

            //float vDotProd = Vector3.Dot(vThighDirection, vTibiaDirection);

            //if (vDotProd <= 0)
            //{
            //    RightLegStride = Vector3.ProjectOnPlane((vThighStride * vThighDirection), Vector3.up) - Vector3.ProjectOnPlane((vTibiaStride * vTibiaDirection), Vector3.up);
            //}
            //else
            //{
            //    RightLegStride = Vector3.ProjectOnPlane((vThighStride * vThighDirection), Vector3.up) + Vector3.ProjectOnPlane((vTibiaStride * vTibiaDirection), Vector3.up);
            //}
        }
    }
}
