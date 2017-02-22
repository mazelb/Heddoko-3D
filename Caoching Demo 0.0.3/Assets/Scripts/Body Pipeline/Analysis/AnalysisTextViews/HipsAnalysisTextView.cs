using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisTextViews
{
    public class HipsAnalysisTextView : AnaylsisTextView
    {
        private string mLabelName = "Hips Analysis";
        public Text RightHipFlexionText;
        public Text LeftHipFlexionText;
        public Text RightHipAbductionText;
        public Text LeftHipAbductionText;
        public Text RightHipRotation;
        public Text LeftHipRotationText;


        public override string LabelName
        {
            get
            {
                return mLabelName;
            }
        }

        protected override void BodyUpdated()
        {
            //todo: display a panel that the values for the current analysis frame have not been computed. 
            //this is in the controller.   
        }


        private void UpdateLeftHipTextView(float vLeftHipFlexionSignedAngle, float vLeftHipAbductionSignedAngle, float vLeftHipRotationSignedAngle)
        {

            LeftHipFlexionText.text = FeedbackAngleToString(vLeftHipFlexionSignedAngle);
            LeftHipAbductionText.text = FeedbackAngleToString(vLeftHipAbductionSignedAngle);
            LeftHipRotationText.text = FeedbackAngleToString(vLeftHipRotationSignedAngle);

        }

        private void UpdateRightHipTextView(float vRightHipFlexionSignedAngle, float vRightHipAbductionSignedAngle, float vRightHipRotationSignedAngle)
        {

            RightHipFlexionText.text = FeedbackAngleToString(vRightHipFlexionSignedAngle);
            RightHipAbductionText.text = FeedbackAngleToString(vRightHipAbductionSignedAngle);
            RightHipRotation.text = FeedbackAngleToString(vRightHipRotationSignedAngle);
        }


        public override void ClearText()
        {
            RightHipFlexionText.text = "";
            LeftHipFlexionText.text = "";
            RightHipAbductionText.text = "";
            LeftHipAbductionText.text = "";
            RightHipRotation.text = "";
            LeftHipRotationText.text = "";
        }


        public void UpdateView(TPosedAnalysisFrame vFrame)
        {
            UpdateLeftHipTextView(vFrame.LeftHipFlexionSignedAngle, vFrame.LeftHipAbductionSignedAngle,
                vFrame.LeftHipRotationSignedAngle);
            UpdateRightHipTextView(vFrame.RightHipFlexionSignedAngle, vFrame.RightHipAbductionSignedAngle,
                vFrame.RightHipRotationSignedAngle);
        }
    }
}