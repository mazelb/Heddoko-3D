/** 
* @file RulaVisualAngleAnalysis .cs
* @brief Contains the RulaVisualAngleAnalysis  and RulaPostureAngles classes
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using System.Collections.Generic; 
using Assets.Scripts.UI.ArcAngle;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.View.Anaylsis
{
    /// <summary>
    /// provides analysis for joint angles congruent with RULA point system
    /// </summary>
    public class RulaVisualAngleAnalysis : MonoBehaviour
    {

        public WMG_Pie_Graph PieGraph;
        private RulaPostureAngles mPostureAngles;

        private int mPointCount;
        private int mPoint;
        private AnaylsisFeedBackContainer.PosturePosition mCurrentPosturePos;
        public bool MaskOnlyRange = true;
        public float SignedAngle;
        public float MaskExtendAnimateDur = 0.75f;
        public float MaskExplodeAnimateDur = 2f;
        public bool GraphicsEnabled = false;
        public Vector3DAxis VectorAxisGo;
        /// <summary>
        /// This image will mask the graph, such that only a portion of the pie chart will be visible
        /// </summary>
        public Image MaskingImage;

        public bool IsCountingPoints;
        /// <summary>
        /// The second transform to compare to
        /// </summary>
        [SerializeField]
        private Transform mTransformComparison;
        [SerializeField]
        private Transform mCenterObject;
        public bool IsAnimating { get; set; }

        public string Info { get; set; }
        /// <summary>
        /// The object to have this pie chart center around
        /// </summary>
        public Transform CenteredObject
        {
            get { return mCenterObject; }
            set
            {
                mCenterObject = value;
                PieGraph.transform.position = mCenterObject.position;
            }
        }



        public float AveragePointCount
        {
            get
            {
                if (mPointCount == 0)
                {
                    return 0;
                }
                return (float)Point / mPointCount;
            }
        }

        public int Point
        {
            get { return mPoint; }
            set
            {
                mPoint = value;
            }
        }

        //start animation process
        public void Animate()
        {
            gameObject.SetActive(true);
            StopCoroutine(StartAnim());
            if (RulaSettings.GetActionMap(mCurrentPosturePos) != null && MaskOnlyRange)
            {
                IsAnimating = true;
                RulaSettings.GetActionMap(mCurrentPosturePos).Invoke(this);
                StartCoroutine(StartAnim());
            }
        }

        private IEnumerator StartAnim()
        {
            Quaternion vPieGraphRot = PieGraph.transform.rotation;
            Quaternion vEndRotation = MaskingImage.transform.rotation;
            MaskingImage.transform.rotation = vPieGraphRot;
            Quaternion vStartQuaterion = MaskingImage.transform.rotation;
            MaskingImage.fillAmount = 0f;
            VectorAxisGo.transform.localScale = Vector3.zero;
            Vector3 vEndVectorSize = Vector3.one*0.5f + new Vector3(0.25f, 0, 0.25f); 
            //  MaskingImage.transform.localScale = Vector3.zero;
            float vStartTime = Time.time;
            float vEndFill = RulaSettings.GetRangeMapValue(CurrentPosturePos) / 360f;
            while (true)
            {
                float vPercentage = (Time.time - vStartTime) / MaskExplodeAnimateDur;
                vPercentage = vPercentage * vPercentage * vPercentage * (vPercentage * (6f * vPercentage - 15f) + 10f);
                float vNewFill = Mathf.Lerp(0, vEndFill, vPercentage);
                Quaternion vNewRotation = Quaternion.Lerp(vStartQuaterion, vEndRotation, vPercentage);

                MaskingImage.fillAmount = vNewFill;
                MaskingImage.transform.rotation = vNewRotation;
                PieGraph.transform.rotation = vPieGraphRot;
                if (vPercentage > 1f)
                {
                    MaskingImage.fillAmount = vEndFill;
                    MaskingImage.transform.rotation = vEndRotation;
                    PieGraph.transform.rotation = vPieGraphRot;
                    break;
                }
                yield return null;
            }
            //animate the mask grow first

            vStartTime = Time.time;
            while (true)
            {
                float vPercentage = (Time.time - vStartTime) / MaskExtendAnimateDur;

                vPercentage = 1f - Mathf.Cos(vPercentage * Mathf.PI * 0.5f);
                Vector3 vNewSize = Vector3.Lerp(Vector3.zero, vEndVectorSize, vPercentage);
                VectorAxisGo.transform.localScale = vNewSize;

                if (vPercentage >= 1f)
                {
                    VectorAxisGo.transform.localScale = vEndVectorSize;
                    break;
                }
                yield return null;
            }

            IsAnimating = false;
        }
        /// <summary>
        /// The second transform to compare to
        /// </summary>
        public Transform TransformComparison
        {
            get { return mTransformComparison; }
            set { mTransformComparison = value; }
        }



        public AnaylsisFeedBackContainer.PosturePosition CurrentPosturePos
        {
            get { return mCurrentPosturePos; }
            set { mCurrentPosturePos = value; }
        }


        /// <summary>
        /// Initialize component with the posture position, the object in which to center the transform on, the transform from which
        ///  the arc's values will be derived from, and an offset to reposition the pie chart
        /// </summary>
        /// <param name="vPosturePosition"> the posture position</param>
        /// <param name="vCenteredObject">the object in which to center the transform on</param>
        /// <param name="vTransformComparison">the transform from which the arc's values will be derived from</param>
        /// <param name="vShow">Show after initialization. Default to false</param>
        public void Init(AnaylsisFeedBackContainer.PosturePosition vPosturePosition, Transform vCenteredObject, Transform vTransformComparison, bool vShow = false)
        {
            mCurrentPosturePos = vPosturePosition;
            CenteredObject = vCenteredObject;
            TransformComparison = vTransformComparison;
            mPostureAngles = new RulaPostureAngles { PosturePosition = vPosturePosition };
            WMG_List<float> vValues = new WMG_List<float>();
            // WMG_List<string> vLabels = new WMG_List<string>();
            WMG_List<Color> vSliceColors = new WMG_List<Color>();
            WMG_List<string> vSliceLabels = new WMG_List<string>();

            foreach (var vRulePostureAngleRange in mPostureAngles.RulaAngleRanges)
            {
                vValues.Add(vRulePostureAngleRange.Range / 360f);
                vSliceColors.Add(vRulePostureAngleRange.Color);
                vSliceLabels.Add(vRulePostureAngleRange.PointName);
            }
            PieGraph.sliceValues = vValues;
            PieGraph.sliceValuesChanged(false, true, false, 0);
            PieGraph.sliceColors = vSliceColors;
            PieGraph.sliceColorsChanged(false, true, false, 0);
            PieGraph.sliceLabels = vSliceLabels;
            PieGraph.sliceLabelsChanged(false, true, false, 0);
            gameObject.SetActive(vShow);
            if (vShow)
            {
                Show();
            }
        }

        /// <summary>
        /// Update the layer mask to match the passed in parameter
        /// </summary>
        /// <param name="vLayer"></param>
        public void UpdateMask(LayerMask vLayer)
        {
            gameObject.layer = vLayer;
            PieGraph.gameObject.layer = vLayer;
            VectorAxisGo.SetLayer(vLayer);
            foreach (var vChildTrans in VectorAxisGo.transform.GetComponentsInChildren<Transform>())
            {
                vChildTrans.gameObject.layer = vLayer;
            }

            foreach (var vGo in PieGraph.getSlices())
            {
                vGo.layer = vLayer;
            }
        }

        public void EnableCalculation()
        {
            gameObject.SetActive(true);
        }

        public void DisableGraphics()
        {
            PieGraph.gameObject.SetActive(false);
            VectorAxisGo.gameObject.SetActive(false);
        }
        public void Show(bool vEnableGraphics = false)
        {
            gameObject.SetActive(true);
            PieGraph.gameObject.SetActive(vEnableGraphics);
            VectorAxisGo.gameObject.SetActive(vEnableGraphics);
        
        }

        public void StartAnimationProcess()
        {
            PieGraph.gameObject.SetActive(true);
            VectorAxisGo.gameObject.SetActive(true);
            Animate();

        }
        /// <summary>
        /// Hide the gameobject
        /// </summary>
        public void Hide()
        {
            StopCoroutine(StartAnim());
            DisableGraphics();
            gameObject.SetActive(false);
        }

        public void ResetPoints()
        {

            Point = 0;
            mPointCount = 0;
        }
        /// <summary>
        /// Update angles after they have been processed in Body.Analysis
        /// </summary>
        void LateUpdate()
        {
            if (!IsAnimating)
            {
                if (RulaSettings.GetActionMap(mCurrentPosturePos) != null)
                {
                    RulaSettings.GetActionMap(mCurrentPosturePos).Invoke(this);
                }
            }

        }

        /// <summary>
        /// Flip fill modes
        /// </summary>
        public void FlipModes()
        {
            MaskOnlyRange = !MaskOnlyRange;
        }
    }
    
    public static class RulaPoint
    {

        public static string ZeroPoint = "ZeroPoint";
        public static string OnePoint = "OnePoint";
        public static string TwoPoint = "TwoPoint";

        public static string ThreePoint = "ThreePoint";
        public static string FourPoint = "FourPoint";
        public static string Null = "Null";
        public static string OnePointTrExtBuffer = "OnePointTrExtBuffer";
        public static string ZeroPointTrunkTwistBuff = "ZeroPointTrunkTwistBuff";
        private static Dictionary<string, int> sGPointMapping; 
        public static int GetPoint(string vVal)
        {
            if (sGPointMapping == null)
            {
                sGPointMapping = new Dictionary<string, int>();
                sGPointMapping.Add(ZeroPoint,0);
                sGPointMapping.Add(OnePoint,1);
                sGPointMapping.Add(TwoPoint,2);
                sGPointMapping.Add(ThreePoint,3);
                sGPointMapping.Add(FourPoint,4);
                sGPointMapping.Add(Null,0);
                sGPointMapping.Add(OnePointTrExtBuffer,1);
                sGPointMapping.Add(ZeroPointTrunkTwistBuff,0);
            }
            if (sGPointMapping.ContainsKey(vVal))
            {
                return sGPointMapping[vVal];
            }
            return 0;
        }
      
    }


}