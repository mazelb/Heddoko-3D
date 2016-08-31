
/** 
* @file SquatMetricsView.cs
* @brief Contains the SquatMetricsView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.MainMenu; 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Shows the metrics for Squats
    /// </summary>
    public class SquatMetricsView : MonoBehaviour
    {

        private PlayerStreamManager mPlayerStreamManager;
        public float UpperRangeOfSquatMotion = 110f;
        public float LowerRangeOfSquatMotion = 90f;
        public float LargestAngleSum = 170f;

        [SerializeField]
        private float mMaxKneeFlexion = 150f;

        private int mPreviousSquatValue = 0;
        public Text NumberOfSquats;
        public Image VisualSquatFeedback;

        [SerializeField]
        private LegMetricsView mLegMetricsView;
        public LegMetricsView LegMetricsView
        {
            get
            {
                if (mLegMetricsView == null)
                {
                    mLegMetricsView = FindObjectOfType<LegMetricsView>();
                }
                return mLegMetricsView;
            }
        }

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

        void Update()
        {
            Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
            if (vCurrentBody != null)
            {
                RightLegAnalysis vRightLegAnalysis =
                    vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                        RightLegAnalysis;
                vRightLegAnalysis.StartCountingSquats(true);
                //NumberSquatsOfText.text = "Total number = " + vRightLegAnalysis.NumberofRightSquats;
                float vAngleKneeflex = Mathf.Abs(vRightLegAnalysis.RightKneeFlexion);


                float vPositionOfSquat = vAngleKneeflex / mMaxKneeFlexion;
                if (vPositionOfSquat > 1)
                {
                    VisualSquatFeedback.fillAmount = 1;
                }
                else
                {
                    VisualSquatFeedback.fillAmount = vPositionOfSquat;
                }
                int vCurrentSquats = Mathf.FloorToInt(vRightLegAnalysis.NumberofSquats);
                UpdateSquatsCount(vCurrentSquats);
            }
        }

        /// <summary>
        /// Brings the view into screen
        /// </summary>
        public void Show()
        {
            LegMetricsView.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the view out of screen 
        /// </summary>
        public void Hide()
        {
            LegMetricsView.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the number of Squats
        /// </summary>
        /// <param name="vNewVal"></param>
        private void UpdateSquatsCount(int vNewVal)
        {
            NumberOfSquats.text = "" + vNewVal;
        }

        /// <summary>
        /// Rests the values
        /// </summary>
        public void ResetValues()
        {
            NumberOfSquats.text = 0 + "";
            VisualSquatFeedback.fillAmount = 0;
        }
    }
}
