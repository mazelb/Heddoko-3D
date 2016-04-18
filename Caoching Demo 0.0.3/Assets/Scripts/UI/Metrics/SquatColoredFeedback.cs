/** 
* @file SquatColoredFeedback.cs
* @brief Contains the SquatColoredFeedback class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Returns feedback in a colored form
    /// </summary>
    public class SquatColoredFeedback : MonoBehaviour
    {
        [SerializeField]
        private Gradient mGradient;

        public RectTransform WhiteColor;
        public RectTransform WhiteToRedSubGradient;
        public RectTransform RedToOrangeSubGradient;
        public RectTransform OrangeToYellowSubGradient;
        public RectTransform YellowToGreenSubGradient;
        public RectTransform GreenSubGradient;
        public RectTransform GreenToYellowSubGradient;
        public RectTransform YellowToOrangeSubGradient;
        public RectTransform OrangeToRedSubGradient;
        [SerializeField ] private float mGreenPosOffsetPercentage;
        private Scrollbar mScrollbar;

        void Awake()
        {
            mScrollbar = GetComponent<Scrollbar>();
           
        }

        public float offsetTest;
        [Range(0.01f, 1f)] public float PerfectSquatResize = 1;


        public float WRRatio = 0.06f;
        public float RORatio = 0.08f;
        public float OYRatio = 0.2f;
        public float YGRatio = 0.5f;

        /// <summary>
        /// sets the gradient of the image according to the perfect squat area
        /// </summary>
        /// <param name="vStandingPrc"></param>
        /// <param name="vSquatAreaStartPrc"></param>
        /// <param name="vSquatAreaEndPrc"></param>
        public void SetGradient(float vStandingPrc, float vSquatAreaStartPrc, float vSquatAreaEndPrc)//,float vBottomGradPrc )
        {
            return;
            //Disable UIGradients
            // WhiteToRedSubGradient.GetComponent<UIGradient>().enabled = false;
/*
            RedToOrangeSubGradient.GetComponent<UIGradient>().enabled = false;
            OrangeToYellowSubGradient.GetComponent<UIGradient>().enabled = false;
            YellowToGreenSubGradient.GetComponent<UIGradient>().enabled = false;
            GreenSubGradient.GetComponent<UIGradient>().enabled = false;
            GreenToYellowSubGradient.GetComponent<UIGradient>().enabled = false;
            YellowToOrangeSubGradient.GetComponent<UIGradient>().enabled = false;
            OrangeToRedSubGradient.GetComponent<UIGradient>().enabled = false;
*/

     

            //----------------------Set positions --------------------------//


            //------(Start) Set the Red to green images positions ---------//
            float vSetTopPos = 1 - vStandingPrc;
            Vector2 vTopAnchorMin = WhiteToRedSubGradient.anchorMin;
            vTopAnchorMin.y = vSetTopPos;
            WhiteToRedSubGradient.anchorMin = vTopAnchorMin;

            //The Distance from white red to yellow green
            float vDistWRYG = vTopAnchorMin.y-(1-vSquatAreaStartPrc);
            float vHalfDistanceWRYG = vDistWRYG/2f;
          //  float vRedToGreenRange =   vSetTopPos- (1 - vSquatAreaStartPrc);
           // float vRedToGreenRangeSubsection = vRedToGreenRange / 2f;
            /*float[] vRedToGreenPositions = new float[2];

            //set the first element
            vRedToGreenPositions[0] = vSetTopPos - vRedToGreenRangeSubsection;


            for (int i = 1; i < vRedToGreenPositions.Length; i++)
            {
                vRedToGreenPositions[i] = vRedToGreenPositions[i - 1] - vRedToGreenRangeSubsection;
            }*/
            Vector2 vRedToOrangeMin = RedToOrangeSubGradient.anchorMin;
            Vector2 vRedToOrangeMax = RedToOrangeSubGradient.anchorMax;
            vRedToOrangeMin.y = vTopAnchorMin.y - vHalfDistanceWRYG;//vRedToGreenPositions[0];
            vRedToOrangeMax.y = vTopAnchorMin.y;
            RedToOrangeSubGradient.anchorMin = vRedToOrangeMin;
            RedToOrangeSubGradient.anchorMax = vRedToOrangeMax;

            Vector2 vOrangeToYellowMin = OrangeToYellowSubGradient.anchorMin;
            Vector2 vOrangeToYellowMax = OrangeToYellowSubGradient.anchorMax;

        
            vOrangeToYellowMin.y = vRedToOrangeMin.y - vHalfDistanceWRYG + (vHalfDistanceWRYG * mGreenPosOffsetPercentage);  
            vOrangeToYellowMax.y = vRedToOrangeMin.y;
            OrangeToYellowSubGradient.anchorMin = vOrangeToYellowMin;
            OrangeToYellowSubGradient.anchorMax = vOrangeToYellowMax;

            //get the Distance between vOrangeToYellowMin and the total Distance between squat start and squat end
            float vDistOToStartEnd = vOrangeToYellowMin.y - (vSquatAreaEndPrc - vSquatAreaStartPrc);
            vDistOToStartEnd *= PerfectSquatResize;
            Vector2 vYellowToGreenMin = YellowToGreenSubGradient.anchorMin;
            Vector2 vYellowToGreenMax = YellowToGreenSubGradient.anchorMax;

            //make this image occupy 90%
            vYellowToGreenMin.y = vOrangeToYellowMin.y - vDistOToStartEnd*offsetTest; //((vSquatAreaEndPrc- vSquatAreaStartPrc)/2f);//1 - (vSquatAreaEndPrc + vSquatAreaStartPrc)/2f;//vRedToGreenPositions[2];
            vYellowToGreenMax.y = vOrangeToYellowMin.y;
            YellowToGreenSubGradient.anchorMin = vYellowToGreenMin;
            YellowToGreenSubGradient.anchorMax = vYellowToGreenMax;
            //------(End) Set the Red to green images positions ---------//


            //------(Start) Set the green image position ----------------//
/*
            Vector2 vGreenAncMin = GreenSubGradient.anchorMin;
            Vector2 vGreenAncMax = GreenSubGradient.anchorMax;
            vGreenAncMax.y = vYellowToGreenMin.y;
            vGreenAncMin.y = 1 - vSquatAreaEndPrc;
            GreenSubGradient.anchorMin = vGreenAncMin;
            GreenSubGradient.anchorMax = vGreenAncMax;
           */

            //------(End) Set the green image position ----------------//

            //------(Start) Set the green to red images positions ---------//

            //calculate the subsections between green to red
       //    float vGreenToRedSubRange = 1 - vSquatAreaEndPrc;
        //    vGreenToRedSubRange /= 2f;

          /*  float[] vGreenToRedPosition = new float[2];
            //set the first vGreenToRedPosition
            vGreenToRedPosition[0] =  vSquatAreaEndPrc- vGreenToRedSubange;

            for (int i = 1; i < vGreenToRedPosition.Length; i++)
            {
                vGreenToRedPosition[i] = vGreenToRedPosition[i - 1] - vGreenToRedSubange;
            }
*/

            Vector2 vGreenToYellowMin = GreenToYellowSubGradient.anchorMin;
            Vector2 vGreenToYellowMax = GreenToYellowSubGradient.anchorMax;
            vGreenToYellowMin.y = vYellowToGreenMin.y- vDistOToStartEnd * (1- offsetTest);//vGreenToRedPosition[0];
            vGreenToYellowMax.y = vYellowToGreenMin.y;// vGreenAncMin.y;
            GreenToYellowSubGradient.anchorMin = vGreenToYellowMin;
            GreenToYellowSubGradient.anchorMax = vGreenToYellowMax;
 
             
            Vector2 vYellowToOrangeMin = YellowToOrangeSubGradient.anchorMin;
            Vector2 vYellowToOrangeMax = YellowToOrangeSubGradient.anchorMax;
            vYellowToOrangeMin.y = vGreenToYellowMin.y - vGreenToYellowMin.y/2f; //vGreenToRedPosition[1];
            vYellowToOrangeMax.y = vGreenToYellowMin.y;
            YellowToOrangeSubGradient.anchorMin = vYellowToOrangeMin;
            YellowToOrangeSubGradient.anchorMax = vYellowToOrangeMax;

            Vector2 vOrangeToRedMin = OrangeToRedSubGradient.anchorMin;
            Vector2 vOrangeToRedMax = OrangeToRedSubGradient.anchorMax;
            vOrangeToRedMax.y = vYellowToOrangeMin.y;
            vOrangeToRedMin.y = 0;
            OrangeToRedSubGradient.anchorMin = vOrangeToRedMin;
            OrangeToRedSubGradient.anchorMax = vOrangeToRedMax;
            //------(End) Set the green to red images positions ---------//


            //------------------End set positions -----------------------------//
            //enable UIGradients
         //   WhiteToRedSubGradient.GetComponent<UIGradient>().enabled = true;
           /* RedToOrangeSubGradient.GetComponent<UIGradient>().enabled = true;
            OrangeToYellowSubGradient.GetComponent<UIGradient>().enabled = true;
            YellowToGreenSubGradient.GetComponent<UIGradient>().enabled = true;
            GreenSubGradient.GetComponent<UIGradient>().enabled = true;
            GreenToYellowSubGradient.GetComponent<UIGradient>().enabled = true;
            YellowToOrangeSubGradient.GetComponent<UIGradient>().enabled = true;
            OrangeToRedSubGradient.GetComponent<UIGradient>().enabled = true;
*/
        }
        public void SetGradientVersion2(float vStandingPrc, float vSquatAreaStartPrc, float vSquatAreaEndPrc)//,float vBottomGradPrc )
        {
            return;
      /*      //Disable UIGradients
            WhiteToRedSubGradient.GetComponent<UIGradient>().enabled = false;
            RedToOrangeSubGradient.GetComponent<UIGradient>().enabled = false;
            OrangeToYellowSubGradient.GetComponent<UIGradient>().enabled = false;
            YellowToGreenSubGradient.GetComponent<UIGradient>().enabled = false;
            GreenSubGradient.GetComponent<UIGradient>().enabled = false;
            GreenToYellowSubGradient.GetComponent<UIGradient>().enabled = false;
            YellowToOrangeSubGradient.GetComponent<UIGradient>().enabled = false;
            OrangeToRedSubGradient.GetComponent<UIGradient>().enabled = false;*/

            //----------------------Set positions --------------------------//


            //------(Start) Set the Red to green images positions ---------//
            float vSetTopPos = 1 - vStandingPrc;

            Vector2 vTopAnchorMin = WhiteColor.anchorMin;//WhiteToRedSubGradient.anchorMin;
            vTopAnchorMin.y = vSetTopPos;
            WhiteColor.anchorMin = vTopAnchorMin;

            WhiteToRedSubGradient.anchorMax = vTopAnchorMin;
            //Distance between standing position and start position
            float vStandStartDist = Mathf.Abs(vSquatAreaStartPrc - vStandingPrc);


            WhiteToRedSubGradient.anchorMin = vTopAnchorMin;

            //The Distance from white red to yellow green
            float vDistWRYG = vTopAnchorMin.y - (1 - vSquatAreaStartPrc);
            float vHalfDistanceWRYG = vDistWRYG / 2f;
            //  float vRedToGreenRange =   vSetTopPos- (1 - vSquatAreaStartPrc);
            // float vRedToGreenRangeSubsection = vRedToGreenRange / 2f;
            /*float[] vRedToGreenPositions = new float[2];

            //set the first element
            vRedToGreenPositions[0] = vSetTopPos - vRedToGreenRangeSubsection;


            for (int i = 1; i < vRedToGreenPositions.Length; i++)
            {
                vRedToGreenPositions[i] = vRedToGreenPositions[i - 1] - vRedToGreenRangeSubsection;
            }*/
            Vector2 vRedToOrangeMin = RedToOrangeSubGradient.anchorMin;
            Vector2 vRedToOrangeMax = RedToOrangeSubGradient.anchorMax;
            vRedToOrangeMin.y = vTopAnchorMin.y - vHalfDistanceWRYG;//vRedToGreenPositions[0];
            vRedToOrangeMax.y = vTopAnchorMin.y;
            RedToOrangeSubGradient.anchorMin = vRedToOrangeMin;
            RedToOrangeSubGradient.anchorMax = vRedToOrangeMax;

            Vector2 vOrangeToYellowMin = OrangeToYellowSubGradient.anchorMin;
            Vector2 vOrangeToYellowMax = OrangeToYellowSubGradient.anchorMax;


            vOrangeToYellowMin.y = vRedToOrangeMin.y - vHalfDistanceWRYG + (vHalfDistanceWRYG * mGreenPosOffsetPercentage);
            vOrangeToYellowMax.y = vRedToOrangeMin.y;
            OrangeToYellowSubGradient.anchorMin = vOrangeToYellowMin;
            OrangeToYellowSubGradient.anchorMax = vOrangeToYellowMax;

            //get the Distance between vOrangeToYellowMin and the total Distance between squat start and squat end
            float vDistOToStartEnd = vOrangeToYellowMin.y - (vSquatAreaEndPrc - vSquatAreaStartPrc);
            vDistOToStartEnd *= PerfectSquatResize;
            Vector2 vYellowToGreenMin = YellowToGreenSubGradient.anchorMin;
            Vector2 vYellowToGreenMax = YellowToGreenSubGradient.anchorMax;

            //make this image occupy 90%
            vYellowToGreenMin.y = vOrangeToYellowMin.y - vDistOToStartEnd * offsetTest; //((vSquatAreaEndPrc- vSquatAreaStartPrc)/2f);//1 - (vSquatAreaEndPrc + vSquatAreaStartPrc)/2f;//vRedToGreenPositions[2];
            vYellowToGreenMax.y = vOrangeToYellowMin.y;
            YellowToGreenSubGradient.anchorMin = vYellowToGreenMin;
            YellowToGreenSubGradient.anchorMax = vYellowToGreenMax;
            //------(End) Set the Red to green images positions ---------//


            //------(Start) Set the green image position ----------------//
            /*
                        Vector2 vGreenAncMin = GreenSubGradient.anchorMin;
                        Vector2 vGreenAncMax = GreenSubGradient.anchorMax;
                        vGreenAncMax.y = vYellowToGreenMin.y;
                        vGreenAncMin.y = 1 - vSquatAreaEndPrc;
                        GreenSubGradient.anchorMin = vGreenAncMin;
                        GreenSubGradient.anchorMax = vGreenAncMax;
                       */

            //------(End) Set the green image position ----------------//

            //------(Start) Set the green to red images positions ---------//

            //calculate the subsections between green to red
            //    float vGreenToRedSubRange = 1 - vSquatAreaEndPrc;
            //    vGreenToRedSubRange /= 2f;

            /*  float[] vGreenToRedPosition = new float[2];
              //set the first vGreenToRedPosition
              vGreenToRedPosition[0] =  vSquatAreaEndPrc- vGreenToRedSubange;

              for (int i = 1; i < vGreenToRedPosition.Length; i++)
              {
                  vGreenToRedPosition[i] = vGreenToRedPosition[i - 1] - vGreenToRedSubange;
              }
  */

            Vector2 vGreenToYellowMin = GreenToYellowSubGradient.anchorMin;
            Vector2 vGreenToYellowMax = GreenToYellowSubGradient.anchorMax;
            vGreenToYellowMin.y = vYellowToGreenMin.y - vDistOToStartEnd * (1 - offsetTest);//vGreenToRedPosition[0];
            vGreenToYellowMax.y = vYellowToGreenMin.y;// vGreenAncMin.y;
            GreenToYellowSubGradient.anchorMin = vGreenToYellowMin;
            GreenToYellowSubGradient.anchorMax = vGreenToYellowMax;


            Vector2 vYellowToOrangeMin = YellowToOrangeSubGradient.anchorMin;
            Vector2 vYellowToOrangeMax = YellowToOrangeSubGradient.anchorMax;
            vYellowToOrangeMin.y = vGreenToYellowMin.y - vGreenToYellowMin.y / 2f; //vGreenToRedPosition[1];
            vYellowToOrangeMax.y = vGreenToYellowMin.y;
            YellowToOrangeSubGradient.anchorMin = vYellowToOrangeMin;
            YellowToOrangeSubGradient.anchorMax = vYellowToOrangeMax;

            Vector2 vOrangeToRedMin = OrangeToRedSubGradient.anchorMin;
            Vector2 vOrangeToRedMax = OrangeToRedSubGradient.anchorMax;
            vOrangeToRedMax.y = vYellowToOrangeMin.y;
            vOrangeToRedMin.y = 0;
            OrangeToRedSubGradient.anchorMin = vOrangeToRedMin;
            OrangeToRedSubGradient.anchorMax = vOrangeToRedMax;
            //------(End) Set the green to red images positions ---------//


            //------------------End set positions -----------------------------//
            //enable UIGradients
            //   WhiteToRedSubGradient.GetComponent<UIGradient>().enabled = true;
/*
            RedToOrangeSubGradient.GetComponent<UIGradient>().enabled = true;
            OrangeToYellowSubGradient.GetComponent<UIGradient>().enabled = true;
            YellowToGreenSubGradient.GetComponent<UIGradient>().enabled = true;
            GreenSubGradient.GetComponent<UIGradient>().enabled = true;
            GreenToYellowSubGradient.GetComponent<UIGradient>().enabled = true;
            YellowToOrangeSubGradient.GetComponent<UIGradient>().enabled = true;
            OrangeToRedSubGradient.GetComponent<UIGradient>().enabled = true;
*/

        }
        /// <summary>
        /// sets the scroll value according to the passed in param
        /// </summary>
        /// <param name="vVal">new scroll value</param>
        public void SetScrollValue(float vVal)
        {
            mScrollbar.value = vVal;
        }


        void Update()
        {
           // SetGradient(25f / 170f, 90f / 170f, 110f / 170f);
        }
    }
}
