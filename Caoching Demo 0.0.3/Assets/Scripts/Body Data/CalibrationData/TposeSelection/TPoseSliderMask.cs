// /**
// * @file TPoseSliderMask.cs
// * @brief Contains the TPoseSliderMask class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date September 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */ 

using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
namespace Assets.Scripts.Body_Data.CalibrationData.TposeSelection
{
     
    public delegate void DeleteRequested(TPoseSliderMask vThis);
    /// <summary>
    /// A width resizable mask that is placed over a slider. 
    /// </summary>
    public class TPoseSliderMask : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler
    {
        public Selectable Selectable;
        public DeleteRequested DeleteRequestedEvent;
        public DraggableButton LeftDragger;
        public DraggableButton RightDragger;
        public Vector2 OffsetMaxRight;
        public Vector2 OffsetMinLeft;
        public Image Background;
        public RectTransform Foreground;
        public Slider Slider;
        public Image PoseMarker;
        private float mPosePos;
        public RectTransform Rectransform;
        private Vector2 mPoseMarkerPosition;
        private bool mIsSelected;
        public Outline Outline;
        public TPoseSelection PoseSelection = new TPoseSelection();
        public Camera RenderingCam;
        void Awake()
        {
            LeftDragger.OnDragMovementAction = MoveLeftEdge;
            RightDragger.OnDragMovementAction = MoveRightEdge;
            LeftDragger.PoseSliderMask = this;
            RightDragger.PoseSliderMask = this;
            Selectable = GetComponent<Selectable>();
            Outline = GetComponent<Outline>();
        }
        /// <summary>
        /// Initializes componenets with a background image that determines the overall size of the mask, a foreground rectransform object which this TPoseSliderMask will be parented under,
        /// and a DeleteRequested hander to handle deletion requests. 
        /// </summary>
        /// <param name="vBackGroundImage"></param>
        /// <param name="vForground"></param>
        /// <param name="vHandler"></param>
        public void Init(Slider vSlider,Image vBackGroundImage, RectTransform vForground, DeleteRequested vHandler,Camera vRenderingCam)
        {
            RenderingCam = vRenderingCam;
            LeftDragger.RenderingCam = RenderingCam;
            RightDragger.RenderingCam = RenderingCam;
            Slider = vSlider;
            Background = vBackGroundImage;
            Foreground = vForground;
            gameObject.SetActive(true);
            DeleteRequestedEvent += vHandler;
            SetTPoseMarker();
        }

        /// <summary>
        /// moves the left edge of the marker
        /// </summary>
        /// <param name="vNewPos"></param>
        /// <param name="vDir"></param>
        public void MoveLeftEdge(float vNewPos, float vDir)
        {
            float vTPosePos = 1f / (Slider.maxValue) * Background.rectTransform.rect.width * (float)mPosePos;
            if (FloatComparer.AreEqual(vDir, 0f, float.Epsilon))
            {
                return;
            }
            float vHalfWidth = Background.rectTransform.rect.width / 2f;
            OffsetMinLeft = Rectransform.offsetMin;

            //going right direction

            var vLeft = Rectransform.offsetMin;
            vLeft.x = vHalfWidth + vNewPos;
            Rectransform.offsetMin = vLeft;
          
            if (Rectransform.offsetMin.x <= 0)
            {
                var vNewMin = Rectransform.offsetMin;
                vNewMin.x = 0;
                Rectransform.offsetMin = vNewMin;
            }
            if (Rectransform.offsetMin.x >= vTPosePos)
            {
                vLeft = Rectransform.offsetMin;
                vLeft.x = vTPosePos;
                Rectransform.offsetMin = vLeft;
            }
            float vPossiblePos = Rectransform.offsetMin.x * (Slider.maxValue) / Background.rectTransform.rect.width;
            PoseSelection.PoseIndexLeft = (int)vPossiblePos;
        }

        /// <summary>
        /// Move the right edge of the mask
        /// </summary>
        /// <param name="vNewPos"></param>
        /// <param name="vDir"></param>
        public void MoveRightEdge(float vNewPos, float vDir)
        {
            float vTPosePos = 1f / (Slider.maxValue) * Background.rectTransform.rect.width * (float)(Slider.maxValue - mPosePos);
            if (FloatComparer.AreEqual(vDir, 0f, float.Epsilon))
            { 
                return;
            }
            float vHalfWidth = Background.rectTransform.rect.width / 2f;
            OffsetMaxRight = Rectransform.offsetMax;
            var vRight = Rectransform.offsetMax;
            vRight.x = vNewPos - vHalfWidth;
            Rectransform.offsetMax = vRight;
            if (Rectransform.offsetMax.x <= -vTPosePos)
            { 
                vRight = Rectransform.offsetMax;
                vRight.x = -vTPosePos;
                Rectransform.offsetMax = vRight;
                PoseSelection.PoseIndexRight = (int)Slider.value;
            }
            if (Rectransform.offsetMax.x >= 0)
            {
                vRight = Rectransform.offsetMax;
                vRight.x = 0;
                Rectransform.offsetMax = vRight;
            }
            float vPossiblePos = Rectransform.offsetMax.x * (Slider.maxValue) / Background.rectTransform.rect.width;
            PoseSelection.PoseIndexRight = (int)( vPossiblePos + Slider.maxValue);
 
        }

        /// <summary>
        /// sets the transform of the object to the samesize as the background image, and sets its parent to the Foreground.
        /// </summary>
        void SetTransform()
        {
            if (Rectransform == null)
            {
                Rectransform = GetComponent<RectTransform>();
            }
            Rectransform.SetParent(Background.transform, false);
            Rectransform.anchorMax = Vector3.one;
            Rectransform.anchorMin = Vector3.zero;
            Rectransform.sizeDelta = Vector2.zero;
            Rectransform.anchoredPosition = Vector2.zero;
            Rectransform.SetParent(Foreground.transform, false);
            Rectransform.transform.SetAsLastSibling();
        }

        void Update()
        {
            if (mIsSelected)
            {
                if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
                {
                    if (DeleteRequestedEvent != null)
                    {
                        DeleteRequestedEvent(this);
                    }
                }
            }

            PoseMarker.transform.position = mPoseMarkerPosition;
        }

        /// <summary>
        /// Sets the tpose marker position
        /// </summary>
        /// <param name="vNewPos"></param>
        public void SetTPoseMarkerPosition(Vector3 vNewPos)
        {
            mPoseMarkerPosition = vNewPos;
        }

        /// <summary>
        /// Sets and calculates the tpose marker position. updates the PoseSelection model.
        /// </summary>
        void SetTPoseMarker()
        {
            mPosePos = Slider.value;
            SetTransform();
            ReduceSize();
            SetTPoseMarkerPosition(Slider.handleRect.transform.position);
            float vRightSideTpose = 1f / (Slider.maxValue) * Background.rectTransform.rect.width * (float)(Slider.maxValue - Slider.value);
            float vLeftSideTPose = 1f / (Slider.maxValue) * Background.rectTransform.rect.width * (float)Slider.value;
            var vRight = Rectransform.offsetMax;
            vRight.x = -vRightSideTpose;
            Rectransform.offsetMax = vRight;
            var vLeft = Rectransform.offsetMin;
            vLeft.x = vLeftSideTPose;
            Rectransform.offsetMin = vLeft;
            PoseSelection.PoseIndex = (int)Slider.value;
            PoseSelection.PoseIndexLeft = (int)Slider.value;
            PoseSelection.PoseIndexRight = (int)Slider.value;
        }

        /// <summary>
        /// Reduce the size of the object
        /// </summary>
        void ReduceSize()
        {
            var vExistingDelta = Background.rectTransform.sizeDelta;
            var vHalfWidth = Background.rectTransform.rect.width;
            vExistingDelta.x = -vHalfWidth;
            Rectransform.sizeDelta = vExistingDelta;
        }

        /// <summary>
        /// on selection object handler
        /// </summary>
        /// <param name="vEventData"></param>
        public void OnSelect(BaseEventData vEventData)
        {
           // when the object is selected, set IsSelected to true.
            mIsSelected = true;
            Outline.enabled = true;
        }

        /// <summary>
        ///  OnPointerDown object handler
        /// </summary>
        /// <param name="vEventData"></param>
        public void OnPointerDown(PointerEventData vEventData)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject, vEventData); 

        }

        /// <summary>
        ///  OnPointerDown object handler
        /// </summary>
        /// <param name="vEventData"></param>
        public void OnDeselect(BaseEventData vEventData)
        { 
            Outline.enabled = false;
            mIsSelected = false;
        }


    }
}