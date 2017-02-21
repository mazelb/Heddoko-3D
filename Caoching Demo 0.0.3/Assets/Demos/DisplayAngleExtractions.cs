
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Trunk;
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
                mVText += "Right Hip Flexion / Extension: " + vRightLegAnalysis.RightHipFlexionAngle + "\n";
                mVText += "Right Hip Abduction/Adduction: " + vRightLegAnalysis.RightHipAbductionAngle+ "\n";
                mVText += "Right Hip Internal/External Rotation: " + vRightLegAnalysis.RightHipRotationSignedAngle+ "\n";
                mVText += "Right Knee Flexion/Extension: " + vRightLegAnalysis.RightKneeFlexionSignedAngle + "\n";
                mVText += "Right Tibial Internal/External Rotation: " + vRightLegAnalysis.RightKneeRotationSignedAngle + "\n";

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
                mVText += "Left Hip Flexion / Extension: " + vLeftLegAnalysis.LeftHipFlexionAngle + "\n";
                mVText += "Left Hip Abduction/Adduction: " + vLeftLegAnalysis.LeftHipAbductionAngle + "\n";
                mVText += "Left Hip Internal/External Rotation: " + vLeftLegAnalysis.LeftHipRotationSignedAngle + "\n";
                mVText += "Left Knee Flexion/Extension: " + vLeftLegAnalysis.LeftKneeFlexionSignedAngle + "\n";
                mVText += "Left Tibial Internal/External Rotation: " + vLeftLegAnalysis.LeftKneeRotationSignedAngle + "\n";
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

                mVText += "Right Shoulder Flexion/Extension: " + vRightArmAnalysis.RightShoulderFlexionAngle + "\n";
                mVText += "Right Shoulder Abduction/Adduction Vertical: " + vRightArmAnalysis.RightShoulderVertAbductionAngle + "\n";
                mVText += "Right Shoulder Abduction/Adduction Horizontal: " + vRightArmAnalysis.RightShoulderHorAbductionAngle + "\n";
                mVText += "Right Shoulder Internal/External Rotation: " + vRightArmAnalysis.RightShoulderRotationSignedAngle + "\n";
                mVText += "Right Elbow Flexion/Extension: " + vRightArmAnalysis.RightElbowFlexionAngle + "\n";
                mVText += "Right Forearm Supination/Pronation: " + vRightArmAnalysis.RightForeArmPronationSignedAngle + "\n";

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

                mVText += "Left Shoulder Flexion/Extension: " + vLeftArmAnalysis.LeftShoulderFlexionAngle + "\n";
                mVText += "Left Shoulder Abduction/Adduction Vertical: " + vLeftArmAnalysis.LeftShoulderVertAbductionAngle + "\n";
                mVText += "Left Shoulder Abduction/Adduction Horizontal: " + vLeftArmAnalysis.LeftShoulderHorAbductionAngle + "\n";
                mVText += "Left Shoulder Internal/External Rotation: " + vLeftArmAnalysis.LeftShoulderRotationSignedAngle + "\n";
                mVText += "Left Elbow Flexion/Extension: " + vLeftArmAnalysis.LeftElbowFlexionAngle + "\n";
                mVText += "Left Forearm Supination/Pronation: " + vLeftArmAnalysis.LeftForeArmPronationSignedAngle + "\n";
            }
        }

        private void ShowTorsoInfo()
        {
            
            TrunkAnalysis vTorsoAnalysis;
            if (CurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_Trunk))
            {
                vTorsoAnalysis =
                    CurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_Trunk] as
                        TrunkAnalysis;
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
