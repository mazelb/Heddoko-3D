
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Torso;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    public class DisplayAngleExtractions : MonoBehaviour
    {
        public Body CurrentBody { get; set; }
        public Button LeftLegBut;
        public Button RightLegBut;
        public Button TorsoBut;
        public Button LeftArmBut;
        public Button RightArmBut;
        public ButtonClicked ButtonClickedState;
        public Text DisplayText;
        private string vText = "";
        private bool vButtonPressed;
        public GameObject InfoPanel;

        void Awake()
        {
            InfoPanel.SetActive(false);
            LeftLegBut.onClick.AddListener(() => PressButton(ButtonClicked.LeftLeg));
            RightLegBut.onClick.AddListener(() => PressButton(ButtonClicked.RightLeg));
            LeftArmBut.onClick.AddListener(() => PressButton(ButtonClicked.LeftArm));
            RightArmBut.onClick.AddListener(() => PressButton(ButtonClicked.RightArm));
            TorsoBut.onClick.AddListener(() => PressButton(ButtonClicked.Torso));
        }

        void Update()
        {
            vText = "";
            if (CurrentBody != null)
            {
                if (vButtonPressed)  
                {
                    InfoPanel.SetActive(true);
                    vText = "Angle Extractions" +"\n";
                }
                switch (ButtonClickedState)
                {
                    case ButtonClicked.LeftArm:
                        ShowLeftArmInfo();
                        break;
                    case ButtonClicked.LeftLeg:
                        ShowLeftLegInfo();
                        break;
                    case ButtonClicked.RightArm:
                        ShowRightArmInfo();
                        break;
                    case ButtonClicked.RightLeg:
                        ShowRightLegInfo();
                        break;
                    case ButtonClicked.Torso:
                        ShowTorsoInfo();
                        break;
                }

            }
            DisplayText.text = vText;
        }
        /// <summary>
        /// Show information for the right leg
        /// </summary>
        private void ShowRightLegInfo()
        { 
            RightLegAnalysis vRightLegAnalysis;
            if (CurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightLeg))
            {
                vRightLegAnalysis =
                    CurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                        RightLegAnalysis;
                vText += "Right Hip Flexion / Extension: " + vRightLegAnalysis.AngleHipFlexion + "\n";
                vText += "Right Hip Abduction/Adduction: " + vRightLegAnalysis.AngleHipAbduction+ "\n";
                vText += "Right Hip Internal/External Rotation: " + vRightLegAnalysis.AngleHipRotation+ "\n";
                vText += "Right Knee Flexion/Extension: " + vRightLegAnalysis.AngleKneeFlexion + "\n";
                vText += "Right Tibial Internal/External Rotation: " + vRightLegAnalysis.AngleKneeRotation + "\n";

            }
        }
        /// <summary>
        /// Show information for left leg anaylsis
        /// </summary>
        private void ShowLeftLegInfo()
        {
            
            LeftLegAnalysis vLeftLegAnalysis;
            if (CurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg))
            {
                vLeftLegAnalysis =
                    CurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                        LeftLegAnalysis;
                vText += "Left Hip Flexion / Extension: " + vLeftLegAnalysis.AngleHipFlexion + "\n";
                vText += "Left Hip Abduction/Adduction: " + vLeftLegAnalysis.AngleHipAbduction + "\n";
                vText += "Left Hip Internal/External Rotation: " + vLeftLegAnalysis.AngleHipRotation + "\n";
                vText += "Left Knee Flexion/Extension: " + vLeftLegAnalysis.AngleKneeFlexion + "\n";
                vText += "Left Tibial Internal/External Rotation: " + vLeftLegAnalysis.AngleKneeRotation + "\n";
            }
        }
        private void ShowRightArmInfo()
        {
            RightArmAnalysis vRightArmAnalysis;
            if (CurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightArm))
            {
                vRightArmAnalysis =
                    CurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightArm] as
                        RightArmAnalysis;

                vText += "Right Shoulder Flexion/Extension: " + vRightArmAnalysis.AngleShoulderFlexion + "\n";
                vText += "Right Shoulder Abduction/Adduction Vertical: " + vRightArmAnalysis.AngleShoulderVertAbduction + "\n";
                vText += "Right Shoulder Abduction/Adduction Horizontal: " + vRightArmAnalysis.AngleShoulderHorAbduction + "\n";
                vText += "Right Shoulder Internal/External Rotation: " + vRightArmAnalysis.AngleShoulderRotation + "\n";
                vText += "Right Elbow Flexion/Extension: " + vRightArmAnalysis.AngleElbowFlexion + "\n";
                vText += "Right Forearm Supination/Pronation: " + vRightArmAnalysis.AngleElbowPronation + "\n";

            }
        }

        private void ShowLeftArmInfo()
        {
            LeftArmAnalysis vLeftArmAnalysis;
            if (CurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftArm))
            {
                vLeftArmAnalysis =
                    CurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftArm] as
                        LeftArmAnalysis;

                vText += "Left Shoulder Flexion/Extension: " + vLeftArmAnalysis.AngleShoulderFlexion + "\n";
                vText += "Left Shoulder Abduction/Adduction Vertical: " + vLeftArmAnalysis.AngleShoulderVertAbduction + "\n";
                vText += "Left Shoulder Abduction/Adduction Horizontal: " + vLeftArmAnalysis.AngleShoulderHorAbduction + "\n";
                vText += "Left Shoulder Internal/External Rotation: " + vLeftArmAnalysis.AngleShoulderRotation + "\n";
                vText += "Left Elbow Flexion/Extension: " + vLeftArmAnalysis.AngleElbowFlexion + "\n";
                vText += "Left Forearm Supination/Pronation: " + vLeftArmAnalysis.AngleElbowPronation + "\n";
            }
        }

        private void ShowTorsoInfo()
        {
            
            TorsoAnalysis vTorsoAnalysis;
            if (CurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_Torso))
            {
                vTorsoAnalysis =
                    CurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_Torso] as
                        TorsoAnalysis;
                vText += "   Number of Turns :  " + vTorsoAnalysis.NumberOfTurns;
                vText += "   Turn Magnitude  :  " + vTorsoAnalysis.AngleIntegrationTurns;
                vText += "   Number of Turns :  " + vTorsoAnalysis.NumberOfTurns;
                vText += "   Number of Flips :  " + vTorsoAnalysis.NumberOfFlips;
                vText += "   Flip Magnitude :  " + vTorsoAnalysis.AngleIntegrationFlips;
            }
        }

        public void PressButton(ButtonClicked vClicked)
        {
            vButtonPressed = true;
            ButtonClickedState = vClicked;
        }

        public enum ButtonClicked
        {
            LeftLeg =0,
            RightLeg=1,
            Torso=2,
            LeftArm=3,
            RightArm=4
        }

    }
}
