/** 
* @file AngleInfoMetrics.cs
* @brief Contains the AngleInfoMetrics class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.MainMenu; 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Shows analytical information about leg knee flexion and veloctiy inside a textbox
    /// </summary>
    public class AngleInfoMetrics : MonoBehaviour
    {
        public Text AngleInfoDisplayText;
        public Text Header;
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        private PlayerStreamManager mPlayerStreamManager;

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

        public ActivitiesContextController ActivitiesContextController;
        void Update()
        {
            
           
         /*   if (vCurrentBody != null)
            {
                string vText = "Angle Extractions + \n";
                LeftLegAnalysis vLeftLegAnalysis;
                RightLegAnalysis vRightLegAnalysis;
                if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg))
                {
                    vLeftLegAnalysis =
                        vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                            LeftLegAnalysis;
                    double vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleLeftHipFlexion * 100) / 100;
                    vText = "Left Hip Flexion / Extension: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                    //vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleLeftHipAbduction * 100) / 100;
               
                  //  vText += "Left Hip Abduction/Adduction: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                   // vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleLeftHipRotation * 100) / 100;
                   // vText += "Left Hip Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleKneeFlexion * 100) / 100;
                    vText += "Left Knee Flexion/Extension: " + string.Format("{0:N2}", vTruncatedVal*-1.0) + "\n" + "\n";

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleKneeRotation * 100) / 100;
                    vText += "Left Tibial Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.mAngularVelocityKneeFlexion * 100) / 100;
                    vText += "Left knee angular velocity: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                }
                
                AngleInfoDisplayText.text = vText;
            }*/
        }

        /// <summary>
        /// Update information panel according to whether the right leg or left leg is displayed.
        /// </summary>
        /// <param name="vRightLegInfo"></param>
       public void UpdateInfoPanel(bool vRightLegInfo)
        {
            if (vRightLegInfo)
            {
                UpdateRightLegInfo();
            }
            else
            {
                UpdateLeftLegInfo();
            }
        }

        /// <summary>
        ///  Updates the Panel with information on the right leg
        /// </summary>
        private void UpdateRightLegInfo()
        {
            Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
            Header.text = "Right Leg Angle Extractions";
            string vText = "";
            if (vCurrentBody != null)
            {
                RightLegAnalysis vRightLegAnalysis;
              
               
                if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightLeg))
                {
                    vRightLegAnalysis =
                        vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                            RightLegAnalysis;

                    double vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleHipFlexion * 100) / 100;
                    vText += "Right Hip Flexion/Extension: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                  /*  vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleRightHipAbduction * 100) / 100;
                    vText += "Right Hip Abduction/Adduction: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";
*/
                /*    vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleRightHipRotation * 100) / 100;
                    vText += "Right Hip Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";*/

                    vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleKneeFlexion * 100) / 100;
                    vText += "Right Knee Flexion/Extension: " + string.Format("{0:N2}", vTruncatedVal * -1.0) + "\n" + "\n";

                   /* vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleKneeFlexion * 100) / 100;
                    vText += "Right Tibial Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";*/

                    vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngularVelocityKneeFlexion * 100) / 100;
                    vText += "Right Knee flexion Velocity: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";
                }
                
            }
            AngleInfoDisplayText.text = vText;
        }

        private void UpdateLeftLegInfo()
        {
            Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
            Header.text = "Left Leg Angle Extractions";
            string vText = "";
            if (vCurrentBody != null)
            {
                LeftLegAnalysis vLeftLegAnalysis;


                if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg))
                {
                    vLeftLegAnalysis =
                        vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                            LeftLegAnalysis;

                    vLeftLegAnalysis =
                        vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                            LeftLegAnalysis;
                    double vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleHipFlexion * 100) / 100;
                    vText = "Left Hip Flexion / Extension: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                    //vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleLeftHipAbduction * 100) / 100;

                    //  vText += "Left Hip Abduction/Adduction: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                    // vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleLeftHipRotation * 100) / 100;
                    // vText += "Left Hip Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleKneeFlexion * 100) / 100;
                    vText += "Left Knee Flexion/Extension: " + string.Format("{0:N2}", vTruncatedVal * -1.0) + "\n" + "\n";

                 /*   vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleKneeRotation * 100) / 100;
                    vText += "Left Tibial Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";*/

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngularVelocityKneeFlexion * 100) / 100;
                    vText += "Left knee angular velocity: " + string.Format("{0:N2}", vTruncatedVal) + "\n" + "\n";
                }

            }
            AngleInfoDisplayText.text = vText;
        }
    }
}
