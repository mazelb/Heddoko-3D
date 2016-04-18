/** 
* @file AnalysisContentPanel.cs
* @brief Contains the AnalysisContentPanel class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Cameras;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// A content panel that displays content regarding analysis
    /// </summary>
public class AnalysisContentPanel: MonoBehaviour
    {

        public Text PeakValueText;
        public Text FeedbackText;
        public Image FeedbackImage;
        public ArcAngleFill ArcAngleFill;
        public float Threshold = 5f;
        public Animator Animator;


        void Awake()
        {
            Animator = GetComponent<Animator>();
        }
        /// <summary>
        /// updates the peak value from the given float. vNewVal is rounded to an int 
        /// </summary>
        /// <param name="vNewVal">new peak value</param>
        public void UpdatePeakValueText(float vNewVal)
        {
            PeakValueText.text = "" + Mathf.RoundToInt(vNewVal);
        }

        /// <summary>
        /// Display the content panel in scene
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            Animator.SetBool("Show",true);
        }

        public void UpdateFeedbackText(CameraMovementPointSetting vPointParameters)
        {
            float vAngle =  ArcAngleFill.Angle ;
            CameraMovementPointSetting.Feedback vFeedback;
            if (vAngle > Threshold)
            {
                vFeedback =  vPointParameters.GetFeedBackTextAtCurrentPoint(CameraMovementPointSetting.FeedbackTextCategory.GreaterThan);
            }
            else if (vAngle < Threshold *-1f)
            {
                vFeedback = vPointParameters.GetFeedBackTextAtCurrentPoint(CameraMovementPointSetting.FeedbackTextCategory.LessThan);
            }
            else
            {
               vFeedback = vPointParameters.GetFeedBackTextAtCurrentPoint(CameraMovementPointSetting.FeedbackTextCategory.Good);
            }
            FeedbackText.text = vFeedback.FeedbackMSG;
            FeedbackImage.sprite = vFeedback.FeedbackImg;

        }
        /// <summary>
        /// Hides the content panel in scene
        /// </summary>
        public void Hide()
        {
            // gameObject.SetActive(false);
            Animator.SetBool("Show", false);
        }
    }
}
