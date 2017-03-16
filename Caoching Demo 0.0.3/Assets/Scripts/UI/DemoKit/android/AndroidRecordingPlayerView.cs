/**  
* @file AndroidRecordingPlayerView.cs 
* @brief Contains the AndroidRecordingPlayerView class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/

using System;
using System.Collections;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using Assets.Scripts.UI.DemoKit.android;
using System.Linq;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.UI.AbstractViews.camera;
using UnityEngine;

namespace Assets.Scripts.UI.DemoKit
{
    /// <summary>
    /// A view to allow playback controls on android
    /// </summary>
    public class AndroidRecordingPlayerView : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        private PlaybackControlPanel mPlaybackControlPanel;
        internal Body RootBody;
        internal PanelNode RootNode;
        public PlaybackRecordingsButton[] Buttons;
        public GameObject AnimatedObjectToEnable;
        public PanelCamera PanelCam;

        void Start()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            CreateDefaultLayout();
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].InitializeActions(InitBodyPlayback);
            }
            StartCoroutine(AnimateAfterOneSec());
        }

        public override void CreateDefaultLayout()
        {
            BodiesManager.Instance.CreateNewBody("Root");
            RootBody = BodiesManager.Instance.GetBodyFromUUID("Root");
            CurrentLayout = new Layout(LayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            RootNode = mPanelNodes[0];
            RootNode.name = "Main";
            RootNode.PanelSettings.Init(ControlPanelTypeList[0], true, RootBody);
            RootNode.PanelCamUpdated += SetPanelCamSize;
            //get loading button component and disable it
            mPlaybackControlPanel = (PlaybackControlPanel)RootNode.PanelSettings.GetPanelOfType(ControlPanelType.RecordingPlaybackControlPanel);
             StartCoroutine(SetClickListener());
        }

        IEnumerator SetClickListener()
        {
            yield return new WaitForEndOfFrame();
            PanelCam = RootNode.PanelSettings.CameraToBodyPair.PanelCamera;
            var vList =RootBody.RenderedBody.TransformMapping.Values.ToList();
            for (int i = 0; i < vList.Count; i++)
            {
                int vTemp = i;
                vList[vTemp].Selectable.SegmentPressedEvent += (PanelCam.CameraZoom.SetTarget); 
                vList[vTemp].Selectable.SegmentPressedEvent += (PanelCam.Orbitter.SetTarget); 
            }

        }


        /// <summary>
        /// releases current resources
        /// </summary>
        public override void Hide()
        {
            if (mPanelNodes != null)
            {
                foreach (var vPanelNodes in mPanelNodes)
                {
                    vPanelNodes.PanelSettings.ReleaseResources();
                }
            }
            gameObject.SetActive(false);
            if (PreviousView != null)
            {
                PreviousView.Show();
            }


        }

        IEnumerator AnimateAfterOneSec()
        {
            yield return new WaitForSeconds(1f);
            AnimatedObjectToEnable.SetActive(true);
        }
        public override void Show()
        {
            gameObject.SetActive(true);
            if (CurrentLayout == null)
            {
                CreateDefaultLayout();
            }
            else
            {
                mPanelNodes[0].PanelSettings.RequestResources();
            }
        }

        private void SetPanelCamSize(PanelNode vNode)
        {
            vNode.PanelSettings.CameraToBodyPair.PanelCamera.PanelRenderingCamera.orthographicSize = 1.12f;
        }

        public void InitBodyPlayback(string[] vLines)
        {
            throw new NotImplementedException("Changed body frames recording to be a master class, have to change the behaviour of the android playback mechanics");
            //CsvBodyFramesRecording vTempRecording = new CsvBodyFramesRecording();
            //vTempRecording.ExtractRecordingUUIDs(vLines);
            ////verify the recording doesn't exist in the rec manager
            //List<BodyFramesRecordingBase> vRecording = BodyRecordingsMgr.Instance.Recordings;
            //bool vContain = vRecording.Any(x => x.BodyRecordingGuid == vTempRecording.BodyRecordingGuid);
            //if (!vContain)
            //{
            //    vTempRecording.ExtractRawFramesData(vLines);
            //    BodyRecordingsMgr.Instance.AddNewRecording(vTempRecording);
            //}
            //BodyFramesRecordingBase vRecorder =  BodyRecordingsMgr.Instance.GetRecordingByUuid(vTempRecording.BodyRecordingGuid);

            //mPlaybackControlPanel.NewRecordingSelected(vRecorder); 
        }
    }
}