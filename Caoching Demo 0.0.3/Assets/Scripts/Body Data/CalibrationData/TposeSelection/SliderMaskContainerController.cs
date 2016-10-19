// /**
// * @file SliderMaskContainerController.cs
// * @brief Contains the SliderMaskContainerController class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date September 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
 using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.CalibrationData.TposeSelection
{
    /// <summary>
    /// A container,that adds or removes multiple TPoseSliderMasks
    /// </summary>
    public class SliderMaskContainerController : MonoBehaviour
    {
        public Slider Slider;
        public RectTransform Foreground;
        public Image Background;
        public TPoseSliderMask PoseSliderMaskPrefab;
        private List<TPoseSliderMask> mLiveList = new List<TPoseSliderMask>();
         public RecordingPlayerView PlayerView;
        public Camera RenderingCam;

        void Awake()
        {
            if (PlayerView != null)

            {
                PlayerView.RecordingPlayerViewLayoutCreatedEvent += Initialize;
            }

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                CreateSliderMask();
            }
        }
        /// <summary>
        /// Initialize controller when the recording player view is created.
        /// </summary>
        /// <param name="vView"></param>
        private void Initialize(RecordingPlayerView vView)
        {
            Slider = vView.PbControlPanel.PlaybackProgressSubControl.PlaySlider;
            //find the child by name "Background"
            var vBackgroundImageGo =
                vView.PbControlPanel.PlaybackProgressSubControl.PlaySlider.transform.FindChild("Background");
            var vBackgroundImage = vBackgroundImageGo.GetComponent<Image>();
            Background = vBackgroundImage;
            //find the child by name "Fill Area"
            var vForegroundGo = vView.PbControlPanel.PlaybackProgressSubControl.PlaySlider.transform.FindChild("Fill Area");
            Foreground = vForegroundGo.GetComponent<RectTransform>();
        }

        public List<TPoseSelection> PoseSelectionList
        {
            get
            {
                List<TPoseSelection> vTPoseSelectionList = new List<TPoseSelection>();

                for (int vI = 0; vI < mLiveList.Count; vI++)
                {
                    vTPoseSelectionList.Add(mLiveList[vI].PoseSelection);
                }
                return vTPoseSelectionList;
            }
        }

        public void CreateSliderMask()
        {
            var vObj = Instantiate(PoseSliderMaskPrefab);
            vObj.Init(Slider,Background, Foreground, DeleteSliderMask,RenderingCam);
            mLiveList.Add(vObj);
        }

        void DeleteSliderMask(TPoseSliderMask vPoseSliderMask)
        {
            mLiveList.Remove(vPoseSliderMask);
            vPoseSliderMask.DeleteRequestedEvent -= DeleteSliderMask;
            Destroy(vPoseSliderMask.gameObject);
        }

        /// <summary>
        /// A Tpose requested handler
        /// </summary>
        /// <param name="vBodyFrame"></param>
        public void TPoseRequestedHandler(BodyFrame vBodyFrame)
        {
            CreateSliderMask();
        }

        
        public void Clear()
        {
            while (mLiveList.Count > 0)
            {
                var vGo = mLiveList[0];
                mLiveList.RemoveAt(0);
                vGo.DeleteRequestedEvent -= DeleteSliderMask;
                Destroy(vGo.gameObject);
            }
        }

    }
}