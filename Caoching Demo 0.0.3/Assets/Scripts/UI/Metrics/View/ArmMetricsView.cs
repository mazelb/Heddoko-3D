
/** 
* @file ArmMetricsView.cs
* @brief Contains the ArmMetricsView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Pipeline.Analysis.Arms; 
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics.View
{
    public class ArmMetricsView : MonoBehaviour, IResettableMetricView
    {
        [SerializeField]
        private float mMaxElbowFlexion = 175f;
       

        public Image ElbowVelocityFill; 
        public Image ElbowFlexionFill;
        public Text ElbowFlexionText;
        public bool DisplayRightArmAnalysis;
        public PlayerStreamManager PlayerStreamManager;

        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            PlayerStreamManager.ResettableViews.Add(this);
        }

        // ReSharper disable once UnusedMember.Local
        void Update()
        {
            Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
            if (vCurrentBody != null)
            {
                if (DisplayRightArmAnalysis)
                {
                    if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightArm))
                    {
                        RightArmAnalysis vRightArmAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightArm] as
                                    RightArmAnalysis;
                        if (vRightArmAnalysis != null)
                        {
                            ElbowFlexionFill.fillAmount = Mathf.Abs(vRightArmAnalysis.RightElbowFlexionAngle) / mMaxElbowFlexion;
                            ElbowFlexionText.text = (int)Mathf.Abs(vRightArmAnalysis.RightElbowFlexionAngle) + "°"; 
                        }
                    }
                }
                else
                {
                    if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftArm))
                    {
                        LeftArmAnalysis vLeftArmAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftArm] as
                                    LeftArmAnalysis;
                        if (vLeftArmAnalysis != null)
                        {
                            ElbowFlexionFill.fillAmount = Mathf.Abs(vLeftArmAnalysis.LeftElbowFlexionAngle) / mMaxElbowFlexion;
                            ElbowFlexionText.text = (int)Mathf.Abs(vLeftArmAnalysis.LeftElbowFlexionAngle) + "°";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reset the text and fill amounts in scene
        /// </summary>
        public void ResetValues()
        {
            ElbowVelocityFill.fillAmount = 0;
            ElbowFlexionFill.fillAmount = 0;
            ElbowFlexionText.text = 0 + "°";
        }
    }
}
