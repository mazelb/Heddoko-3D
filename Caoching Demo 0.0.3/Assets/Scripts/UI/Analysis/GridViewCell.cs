/** 
* @file GridViewCell.cs
* @brief Contains the GridViewCell abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Cameras;
using Assets.Scripts.UI.AbstractViews;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Analysis
{
    /// <summary>
    /// A sub view component used by GridViewLayoutManager
    /// </summary>
    public class GridViewCell : AbstractView, IPointerEnterHandler, IPointerExitHandler
    {
        public Canvas ControlCanvas;
        private Camera mSubViewCamera;
        public AnalysisSubViewCamControl CameraControl; 
        public float[] Margin = { 0, 0, 0, 0 };
        public GridViewLayoutManager Parent;
        private Vector2 mAnchorMin = Vector2.zero;
        private Vector2 mAnchorMax = Vector2.zero;
        private RectTransform mRectTransform;

        /// <summary>
        /// Sets the anchors of the associated Rect transform
        /// </summary>
        /// <param name="vMin"></param>
        /// <param name="vMax"></param>
        public void SetAnchors(Vector2 vMin, Vector2 vMax)
        {
            CheckCamera();
            mAnchorMin  = vMin;
            mAnchorMax = vMax;
            mAnchorMin.x += Margin[0];
            mAnchorMin.y += Margin[1];
            mAnchorMax.x -= Margin[2];
            mAnchorMax.y -= Margin[3];
            mRectTransform = ControlCanvas.GetComponent<RectTransform>();
            mRectTransform.anchorMax = vMax;
            mRectTransform.anchorMin = vMin;
            mRectTransform.pivot = new Vector2(0.5f,0.5f);
            mRectTransform.offsetMin = mRectTransform.offsetMax = new Vector2(0, 0);

            mRectTransform.rect.max.Set(0,0);

            if (mSubViewCamera != null)
            {
                //get the ratio of this current canvas relative to the parent
                Vector2 vCamMin = mAnchorMin;
                vCamMin.x *= Parent.ContainerRatio.x;
                vCamMin.y *= Parent.ContainerRatio.y ;
                

                Vector2 vCamMax = mAnchorMax;
                vCamMax.x *= Parent.ContainerRatio.x;
                vCamMax.y *= Parent.ContainerRatio.y;

                float vWidth = mRectTransform.rect.width / Screen.width ;
                float vHeight = mRectTransform.rect.height/ Screen.height;

                mSubViewCamera.rect = new Rect(vCamMin.x+Parent.Container.anchorMin.x,vCamMin.y + Parent.Container.anchorMin.y, vWidth, vHeight);
            }
 
        }

        /// <summary>
        /// Check if the camera has been set 
        /// </summary>
        private void CheckCamera()
        {
            if (mSubViewCamera == null)
            {
                mSubViewCamera = CameraControl.AssociatedCamera;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        { 
            CameraControl.EnableZoom();
        }

        public void OnPointerExit(PointerEventData eventData)
        { 
            CameraControl.DisableZoom();
        }

        public override void CreateDefaultLayout()
        {
            throw new System.NotImplementedException();
        }
    }


}
