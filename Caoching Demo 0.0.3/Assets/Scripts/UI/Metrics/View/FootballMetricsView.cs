
/** 
* @file FootballMetricsView.cs
* @brief Contains the FootballMetricsView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.NFLDemo;
using Assets.Scripts.Utils.Animations;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.UI.Metrics.View
{
    public class FootballMetricsView : NonSquatWrapperView, IResettableMetricView
    { 

        public Image ElbowFlexionFill;
        public PlayerStreamManager PlayerStreamManager;

        [SerializeField]
        private ArmMetricsView mArmMetrics;
        public FillAnimationComponent FillAnimation;
        public int NumberOfFramesToCount = 10;
        private int mFrameCount;
        private float mRightElbowVelSum ;
        [SerializeField] private float mPeakAngularVelocity;
        public AnalysisContentPanel AnalysisContentPanel;
 
        internal ArmMetricsView ArmMetricsView
        {
            get
            {
                if (mArmMetrics == null)
                {
                    mArmMetrics = FindObjectOfType<ArmMetricsView>();
                }
                return mArmMetrics;
            }
        }
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
                if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightArm))
                {
                    RightArmAnalysis vRightArmAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightArm] as
                                RightArmAnalysis;
                    if (vRightArmAnalysis != null)
                    {
                        float vElbowFlexionVel = vRightArmAnalysis.RightElbowFlexionAngularVelocity;
                        mRightElbowVelSum += Mathf.Abs(vElbowFlexionVel);
                        if (mPeakAngularVelocity < vRightArmAnalysis.RightElbowFlexionPeakAngularVelocity)
                        {
                            mPeakAngularVelocity = vRightArmAnalysis.RightElbowFlexionPeakAngularVelocity;
                            AnalysisContentPanel.UpdatePeakValueText(mPeakAngularVelocity);
                        }

                    }
                }
            }

 
            if (mFrameCount == NumberOfFramesToCount)
            {
                mFrameCount = 0;
                float vElbowFlexAvg = mRightElbowVelSum / NumberOfFramesToCount;
                FillAnimation.StartAnimation(vElbowFlexAvg);
                mRightElbowVelSum = 0;
            }

            mFrameCount++;
        }
        /// <summary>
        /// Showsthe FootballMetricsView in scene
        /// </summary>
        public  override  void Show()
        {
            gameObject.SetActive(true);
            ArmMetricsView.gameObject.SetActive(true); 
        }

        /// <summary>
        /// Hides the FootballMetricsView in scene
        /// </summary>
        public override void Hide()
        {
            gameObject.SetActive(false);
            ArmMetricsView.gameObject.SetActive(false);
        }

        /// <summary>
        /// Resets values to zero
        /// </summary>
        public void ResetValues()
        {
            mFrameCount = 0;
            mRightElbowVelSum = 0f;
            mPeakAngularVelocity = 0;
        }

    }
}
