
/** 
* @file GlobalCalibrationSettings.cs
* @brief Contains the GlobalCalibrationSettings class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.AbstractSuitSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.UI.Loading;
using HeddokoSDK.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.CalibrationData
{

    public class CalibrationTestCountDown : AbstractSuitsSubControl
    {

        private SubControlType mSubControl = SubControlType.AddCommentSubControl;
        public CircularCountdownTimer LargeTimer;
        public CircularCountdownTimer SmallTimer;
        public Image SmallExpandingOutline;

        private Dictionary<CalibrationType, CalibrationTestDescriptor> mCalibrationImageMap = new Dictionary<CalibrationType, CalibrationTestDescriptor>(3);
        public Sprite TPoseSprite;
        public Sprite ArmsFrontSprite;
        public Sprite ArmsDownSprite;
        public Sprite TposToArmsFrontSprite;
        public Sprite ArmsFrontToDownSprite;
        public Sprite OnCompletionImage;
        public Image CurrentCalibrationImage;

        public AudioSource AudioSource;
        public AudioClip FinishSubCalibrationClip;
        private CalibrationTestDescriptor mOnCompletionDescription;

        public Text Info;
        public Text HoldThisPoseText;


        private void Awake()
        {
            SuitStateChangerControl.StopRecordingEvent += StopCountdownRoutine;
            SuitStateChangerControl.StartRecordingEvent += StartCountDownRoutine;
            CalibrationTestDescriptor vTposeDescritor = new CalibrationTestDescriptor
            {
                CurrentInfo = "T-Pose",
                CurrentSprite = TPoseSprite,
                NextInfo = "Arms Out Front",
                NextSprite = ArmsFrontSprite,
                CompletionTime = GlobalCalibrationSettings.CalibrationTimestamps[CalibrationType.TPoseToArmsForward]
            };

            CalibrationTestDescriptor vArmsFrontDescriptor = new CalibrationTestDescriptor
            {
                CurrentInfo = "Arms Out Front",
                CurrentSprite = ArmsFrontSprite,
                NextInfo = "Arms To The Side",
                NextSprite = ArmsFrontToDownSprite,
                CompletionTime = GlobalCalibrationSettings.CalibrationTimestamps[CalibrationType.ArmsForwardToArmsDown]

            };
            CalibrationTestDescriptor vArmsDownSprite = new CalibrationTestDescriptor
            {
                CurrentInfo = "Arms To The Side",
                CurrentSprite = ArmsFrontToDownSprite,
                NextInfo = "",
                NextSprite = null,
                CompletionTime = GlobalCalibrationSettings.CalibrationTimestamps[CalibrationType.ArmsDown]

            };
            mOnCompletionDescription = new CalibrationTestDescriptor()
            {
                CurrentInfo = "Calibration Complete",
                CurrentSprite = OnCompletionImage
            };
            mCalibrationImageMap.Add(CalibrationType.Tpose, vTposeDescritor);
            mCalibrationImageMap.Add(CalibrationType.ArmsForward, vArmsFrontDescriptor);
            mCalibrationImageMap.Add(CalibrationType.ArmsDown, vArmsDownSprite);
            float vTotalCountdown = mCalibrationImageMap.Count * GlobalCalibrationSettings.CalibrationTimer +
                ((mCalibrationImageMap.Count) * GlobalCalibrationSettings.TransitionTimer);
            float vHoldPoseCount = GlobalCalibrationSettings.CalibrationTimer +
                                   GlobalCalibrationSettings.TransitionTimer;
            LargeTimer.Init(vSeconds: vTotalCountdown);
            SmallTimer.Init(vHoldPoseCount, vNearCompletion: ExpandSmallOutline);
            EnableComponent(false);

        }
        /// <summary>
        /// Starts the countdown routine
        /// </summary>
        private void StartCountDownRoutine()
        {
            EnableComponent(true);
            LargeTimer.StartAnimation();
            StartCoroutine(SmallFillRoutine());
        }

        void EnableComponent(bool vFlag)
        {
            gameObject.SetActive(vFlag);
        }

        private void SetInformation(CalibrationTestDescriptor vDescriptor)
        {
            Info.text = vDescriptor.CurrentInfo;
            CurrentCalibrationImage.sprite = vDescriptor.CurrentSprite;
        }
        private IEnumerator SmallFillRoutine()
        {
            foreach (KeyValuePair<CalibrationType, CalibrationTestDescriptor> vKv in mCalibrationImageMap)
            {
                SetInformation(vKv.Value);
                yield return StartCoroutine(SmallTimer.FillAnimation());
                AudioSource.clip = FinishSubCalibrationClip;
                AudioSource.Play();
            }
            SetInformation(mOnCompletionDescription);
            SmallTimer.InnerFill.fillAmount = 0;
            HoldThisPoseText.text = "";
            StartCoroutine(HideInSeconds(5f));
        }

        private IEnumerator HideInSeconds(float vSeconds)
        {
            yield return new WaitForSeconds(vSeconds);
            StopCountdownRoutine();
        }

        private void StopCountdownRoutine()
        {
            StopAllCoroutines();
            LargeTimer.StopAllCoroutines();
            SmallTimer.StopAllCoroutines();
            EnableComponent(false);
        }

        /// <summary>
        /// Expands a small out line in 3/4 the time of completion of the small count down timer
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExpandSmallOutline()
        {
            float vTimeToFinish = 0.5f;
            Vector2 vStartSizeDelta = SmallTimer.OutlineRectTransform.sizeDelta;
            Vector2 vFinishSizeDelta = vStartSizeDelta + Vector2.one * 75f;
            Color vColor = SmallTimer.Outline.color;
            vColor.a = 0.75f;
            SmallTimer.Outline.color = vColor;
            float vStartAlpha = vColor.a;
            float vStartTime = Time.time;
            while (true)
            {
                float vPercentage = (Time.time - vStartTime) / vTimeToFinish;
                //transform the percentage in a cos curve
                vPercentage = 1f - Mathf.Cos(vPercentage * Mathf.PI * 0.5f);
                float vNewAlpha = Mathf.Lerp(vStartAlpha, 0, vPercentage);
                Vector2 vNewSizeDelta = Vector2.Lerp(vStartSizeDelta, vFinishSizeDelta, vPercentage);
                if (vPercentage >= 1)
                {
                    vColor.a = 0;
                    SmallTimer.Outline.color = vColor;
                    break;
                }
                vColor.a = vNewAlpha;
                SmallTimer.Outline.color = vColor;
                SmallTimer.OutlineRectTransform.sizeDelta = vNewSizeDelta;
                yield return null;
            }
            //reset the start size
            SmallTimer.OutlineRectTransform.sizeDelta = Vector2.zero;
        }

        public override SubControlType SubControlType
        {
            get { return mSubControl; }
        }

        public override void Disable()
        {
            StopCountdownRoutine();
            gameObject.SetActive(false);
        }

      

        void OnApplicationQuit()
        {
            SuitStateChangerControl.StopRecordingEvent -= StopCountdownRoutine;
            SuitStateChangerControl.StartRecordingEvent -= StartCountDownRoutine;
        }
        public override void Enable()
        {
            gameObject.SetActive(true);
        }
 

        public override void OnStatusUpdate(SuitState vSuitState)
        {

        }

        public override void OnConnection()
        {
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.S))
            { StartCountDownRoutine();}
        }


    }

    public class CalibrationTestDescriptor
    {
        public Sprite CurrentSprite;
        public Sprite NextSprite;
        public string CurrentInfo;
        public string NextInfo;
        public float CompletionTime;
    }

}
