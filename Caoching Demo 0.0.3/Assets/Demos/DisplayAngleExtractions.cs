
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
        private string mVText = "";
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
            mVText = "";
            if (CurrentBody != null)
            {
                if (vButtonPressed)  
                {
                    InfoPanel.SetActive(true);
                    mVText = "Angle Extractions" +"\n";
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
            DisplayText.text = mVText;
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
                mVText += "Right Hip Flexion / Extension: " + vRightLegAnalysis.AngleHipFlexion + "\n";
                mVText += "Right Hip Abduction/Adduction: " + vRightLegAnalysis.AngleHipAbduction+ "\n";
                mVText += "Right Hip Internal/External Rotation: " + vRightLegAnalysis.AngleHipRotation+ "\n";
                mVText += "Right Knee Flexion/Extension: " + vRightLegAnalysis.AngleKneeFlexion + "\n";
                mVText += "Right Tibial Internal/External Rotation: " + vRightLegAnalysis.AngleKneeRotation + "\n";

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
                mVText += "Left Hip Flexion / Extension: " + vLeftLegAnalysis.AngleHipFlexion + "\n";
                mVText += "Left Hip Abduction/Adduction: " + vLeftLegAnalysis.AngleHipAbduction + "\n";
                mVText += "Left Hip Internal/External Rotation: " + vLeftLegAnalysis.AngleHipRotation + "\n";
                mVText += "Left Knee Flexion/Extension: " + vLeftLegAnalysis.AngleKneeFlexion + "\n";
                mVText += "Left Tibial Internal/External Rotation: " + vLeftLegAnalysis.AngleKneeRotation + "\n";
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

                mVText += "Right Shoulder Flexion/Extension: " + vRightArmAnalysis.AngleShoulderFlexion + "\n";
                mVText += "Right Shoulder Abduction/Adduction Vertical: " + vRightArmAnalysis.AngleShoulderVertAbduction + "\n";
                mVText += "Right Shoulder Abduction/Adduction Horizontal: " + vRightArmAnalysis.AngleShoulderHorAbduction + "\n";
                mVText += "Right Shoulder Internal/External Rotation: " + vRightArmAnalysis.AngleShoulderRotation + "\n";
                mVText += "Right Elbow Flexion/Extension: " + vRightArmAnalysis.AngleElbowFlexion + "\n";
                mVText += "Right Forearm Supination/Pronation: " + vRightArmAnalysis.AngleElbowPronation + "\n";

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

                mVText += "Left Shoulder Flexion/Extension: " + vLeftArmAnalysis.AngleShoulderFlexion + "\n";
                mVText += "Left Shoulder Abduction/Adduction Vertical: " + vLeftArmAnalysis.AngleShoulderVertAbduction + "\n";
                mVText += "Left Shoulder Abduction/Adduction Horizontal: " + vLeftArmAnalysis.AngleShoulderHorAbduction + "\n";
                mVText += "Left Shoulder Internal/External Rotation: " + vLeftArmAnalysis.AngleShoulderRotation + "\n";
                mVText += "Left Elbow Flexion/Extension: " + vLeftArmAnalysis.AngleElbowFlexion + "\n";
                mVText += "Left Forearm Supination/Pronation: " + vLeftArmAnalysis.AngleElbowPronation + "\n";
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
                mVText += "   Number of Turns :  " + vTorsoAnalysis.NumberOfTurns;
                mVText += "   Turn Magnitude  :  " + vTorsoAnalysis.AngleIntegrationTurns;
                mVText += "   Number of Turns :  " + vTorsoAnalysis.NumberOfTurns;
                mVText += "   Number of Flips :  " + vTorsoAnalysis.NumberOfFlips;
                mVText += "   Flip Magnitude :  " + vTorsoAnalysis.AngleIntegrationFlips;
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
