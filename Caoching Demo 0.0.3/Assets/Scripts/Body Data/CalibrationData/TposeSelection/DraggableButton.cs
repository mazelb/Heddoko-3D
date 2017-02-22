// /**
// * @file DraggableButton.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 09 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.CalibrationData.TposeSelection
{
    public class DraggableButton : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public TPoseSliderMask PoseSliderMask;
        public Button Button;
        private Vector2 mPreviousPos;
        private Vector2 mCurrPos;
        public float Direction;
        public float Width;  
        public Action<float, float> OnDragMovementAction;
        public Camera RenderingCam;

        public void OnDrag(PointerEventData vEventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(PoseSliderMask.Background.rectTransform, vEventData.position, RenderingCam, out mCurrPos);
            Width = PoseSliderMask.Rectransform.rect.width;
            Direction = mCurrPos.x - mPreviousPos.x;
            mPreviousPos = mCurrPos;
            if (OnDragMovementAction != null)
            {
                OnDragMovementAction(mCurrPos.x, Direction);
            }
        }

        public void OnPointerDown(PointerEventData vEventData)
        { 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(PoseSliderMask.Rectransform, vEventData.position, RenderingCam, out mPreviousPos);
        }


    }
}