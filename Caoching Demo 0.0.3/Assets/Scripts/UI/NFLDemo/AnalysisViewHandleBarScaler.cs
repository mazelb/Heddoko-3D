
/** 
* @file AnalysisViewHandleBarScaler.cs
* @brief Contains the AnalysisViewHandleBarScaler class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// The following script sets a Analysis's handles scale according to a target position
    /// </summary>
    public class AnalysisViewHandleBarScaler : MonoBehaviour
    {
        public Vector3 TargetPos;
        public RectTransform VerticalBar;
        public Transform TargetTransform;
        public Camera UiCamera;
        private RectTransform CurrRectTransform;
        public Camera vNonUiCam;

        [SerializeField]
        private float mMagnitude;
        [SerializeField]
        private float mDistanceFromPos;
        [SerializeField]
        private float mScale;



        void Awake()
        {
            CurrRectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
            TargetPos =  TargetTransform.position;
            TargetPos = UiCamera.WorldToScreenPoint(TargetPos); 
            UpdateParameters();
        }
 
        /// <summary>
        /// Updates the private member variable "mScale" with respect to the target transform, current rect's height 
        /// as well as the current transform's position
        /// </summary>
        private void UpdateParameters()
        {
            
            Vector2 viewportPoint2 = vNonUiCam.WorldToViewportPoint(TargetTransform.position);
            CurrRectTransform.anchorMin = new Vector2(viewportPoint2.x, CurrRectTransform.anchorMin.y);
            VerticalBar.anchorMin = new Vector2(viewportPoint2.x, viewportPoint2.y);
        
          
            VerticalBar.anchorMax = new Vector2(viewportPoint2.x+0.002f, CurrRectTransform.anchorMin.y);
    

        }

     
    }
}