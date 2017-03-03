/** 
* @file LegSpriteMover.cs
* @brief Contains the LegSpriteMover class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.Metrics;
using UnityEngine;

namespace Assets.Scripts.UI._2DSkeleton
{
    /// <summary>
    /// 
    /// </summary>
    public class LegSpriteMover : MonoBehaviour, ISpriteMover
    {
        public float LeftLegHipMulti = 1;
        public float LeftLegKneeMultiplier = 1;
        public float RightKneeMultiplier = 1;
        public float RightHipMulti;


        public float KneeAngleShowOffset;
        public float HipsAngleShowOffset;

        public bool IsRightLeg = true;
        public Transform HipMotor;
        public Transform KneeJoint;
        private PlayerStreamManager mPlayerStreamManager;
        public ShadedAngleArea ShadedAngleAreaHip;
        public ShadedAngleArea ShadedAngleAreaKnee;
        public bool ShowShadedAngles = true;
        public bool ShowKneeAngles = true;
        public bool ShowHipAngles = true;
        public ActivitiesContextController ActivityContextController;
   //     public AngleInfoMetrics AngleInfoMetrics;
        public Renderer[] Renderers;

        public PlayerStreamManager PlayerStreamManager
        {
            get
            {
                if (mPlayerStreamManager == null)
                {
                    mPlayerStreamManager = FindObjectOfType<PlayerStreamManager>();
                }
                return mPlayerStreamManager;
            }
        }
        public void ResetOrientations()
        {
            HipMotor.rotation = Quaternion.identity;
            KneeJoint.rotation = Quaternion.identity;
        }

        /// <summary>
        /// apply transformations from their respective flexions
        /// </summary>
        public void ApplyTransformations()
        {
            Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
            Vector3 vHipMotorRot = HipMotor.rotation.eulerAngles;
            Vector3 vKneeJointRot = KneeJoint.rotation.eulerAngles;
            float vHipAngle = 0;
            float vKneeAngle = 0;
            if (IsRightLeg)
            {
                RightLegAnalysis vRightLegAnalysis =
                      vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                          RightLegAnalysis;
                vHipAngle = RightHipMulti * vRightLegAnalysis.RightHipFlexionAngle;
                vKneeAngle = RightKneeMultiplier * vRightLegAnalysis.RightKneeFlexionSignedAngle;
                vKneeAngle = Mathf.Abs(vKneeAngle);
                vHipAngle = Mathf.Abs(vHipAngle);
                vHipMotorRot.z = vHipAngle;
                vKneeJointRot.z = vKneeAngle;

            }
            else
            {
                LeftLegAnalysis vLeftLegAnalysis =
                     vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                         LeftLegAnalysis;
                vHipAngle = LeftLegHipMulti * vLeftLegAnalysis.LeftHipFlexionAngle;
                vHipAngle = Mathf.Abs(vHipAngle);
                vKneeAngle = LeftLegKneeMultiplier * vLeftLegAnalysis.LeftKneeFlexionSignedAngle;
                vKneeAngle = Mathf.Abs(vKneeAngle);
                vHipMotorRot.z = vHipAngle;
                vKneeJointRot.z = vKneeAngle;
            }
            //  Quaternion vHipRot = Quaternion.Euler(vHipMotorRot)* Quaternion.AngleAxis(180f, Vector3.right*vLeftLegHipMulti) * Quaternion.AngleAxis(180f, Vector3.up *y);
            //   Quaternion vKneeRot = Quaternion.Euler(vKneeJointRot)* Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(180f, Vector3.up) ;
            Quaternion vHipRot = Quaternion.Euler(vHipMotorRot);
            Quaternion vKneeRot = Quaternion.Euler(vKneeJointRot);
            // HipMotor.rotation = Quaternion.Euler(vHipMotorRot);
            // KneeJoint.rotation = Quaternion.Euler(vKneeJointRot);
            HipMotor.rotation = vHipRot;
            KneeJoint.localRotation = vKneeRot;
            if (ShowShadedAngles)
            {
                if (ShadedAngleAreaHip != null)
                {
                    ShadedAngleAreaHip.ShowAngle = ShowHipAngles;
                    ShadedAngleAreaHip.SetFill(HipsAngleShowOffset - vHipAngle);
                }
                if (ShadedAngleAreaKnee != null)
                {
                    ShadedAngleAreaKnee.ShowAngle = ShowKneeAngles;
                    ShadedAngleAreaKnee.SetFill(KneeAngleShowOffset - vKneeAngle);

                }
            }
            else
            {
                ShadedAngleAreaHip.ShowAngle = false;
                ShadedAngleAreaHip.ShowAngle = false;
            }

     /*       if (AngleInfoMetrics)
            {
                if (Renderers[0].enabled)
                {
                    AngleInfoMetrics.UpdateInfoPanel(IsRightLeg);
                }
            }*/
        }

        /// <summary>
        /// Applies translations
        /// </summary>
        /// <param name="vNewDisplacement"></param>
        public void ApplyTranslations(float vNewDisplacement)
        {
            Vector3 v3 = transform.localPosition;
            v3.y = vNewDisplacement;
            transform.localPosition = v3;
        }

        /// <summary>
        /// activates or deactives the gameobject
        /// </summary>
        /// <param name="vFlag"></param>
        public void SetActive(bool vFlag)
        {
            foreach (var vRenderer in Renderers)
            {
                vRenderer.enabled = vFlag;
            }
            ShadedAngleAreaKnee.gameObject.SetActive(vFlag);
            ShadedAngleAreaHip.gameObject.SetActive(vFlag);
        }

       /* void Update()
        {
            if (Debug.isDebugBuild && IsRightLeg)
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    AngleInfoMetrics.gameObject.SetActive(!AngleInfoMetrics.gameObject.activeInHierarchy);
                }
            }

        }*/

    }
}
