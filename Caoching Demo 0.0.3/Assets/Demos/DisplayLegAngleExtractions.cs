
/** 
* @file DisplayLegAngleExtractions.cs
* @brief Contains the DisplayLegAngleExtractions class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Legs; 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    /// <summary>
/// Displays the leg angle extractions
/// </summary>
  public  class DisplayLegAngleExtractions : MonoBehaviour
    {
        public Text DisplayLegAngleText;
        public Body CurrentBody { get; set; }
        void Update()
        {
            if (CurrentBody != null)
            {
                string vText = "Angle Extractions";
                LeftLegAnalysis vLeftLegAnalysis;
                RightLegAnalysis vRightLegAnalysis;
                if (CurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg))
                {
                    vLeftLegAnalysis =
                        CurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                            LeftLegAnalysis;
                    vText += "Left Hip Flexion / Extension: " + vLeftLegAnalysis.AngleHipFlexion + "\n";
                    vText += "Left Hip Abduction/Adduction: " + vLeftLegAnalysis.AngleHipAbduction + "\n";
                    vText += "Left Hip Internal/External Rotation: " + vLeftLegAnalysis.AngleHipRotation + "\n";
                    vText += "Knee Flexion/Extension: " + vLeftLegAnalysis.AngleKneeFlexion + "\n";
                    vText += "Tibial Internal/External Rotation: " + vLeftLegAnalysis.AngleKneeRotation + "\n";
                   
                }
                if (CurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightLeg))
                {
                    vRightLegAnalysis =
                        CurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                            RightLegAnalysis;


                    vText += "Right Hip Flexion/Extension: " + vRightLegAnalysis.AngleHipFlexion + "\n";
                    vText += "Right Hip Abduction/Adduction: " + vRightLegAnalysis.AngleHipAbduction + "\n";
                    vText += "Right Hip Internal/External Rotation: " + vRightLegAnalysis.AngleHipRotation + "\n";
                    vText += "Knee Flexion/Extension: " + vRightLegAnalysis.AngleKneeFlexion + "\n";
                    vText += "Tibial Internal/External Rotation: " + vRightLegAnalysis.AngleKneeRotation + "\n";
                        
                }
                DisplayLegAngleText.text = vText;
            }
        }

    }
}
