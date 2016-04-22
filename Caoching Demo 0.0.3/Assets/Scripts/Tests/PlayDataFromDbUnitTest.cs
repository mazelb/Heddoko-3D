
/** 
* @file PlaybackControl.cs
* @brief Contains the PlaybackControl  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Unit test to play data from the database. 
    /// </summary>
    public class PlayDataFromDbUnitTest : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
     //   public Button BackButton;
        private Body mRootNode;
        private PlaybackControlPanel mControlPanel;
        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            TestCreateLayout();

            mControlPanel = (PlaybackControlPanel)mPanelNodes[0].PanelSettings.GetPanelOfType(ControlPanelType.RecordingPlaybackControlPanel);
            mControlPanel.SingleRecordingLoadSubControl.enabled = false;
            
            
        }

        void TestCreateLayout()
        {
            BodiesManager.Instance.CreateNewBody("Root");
            mRootNode = BodiesManager.Instance.GetBodyFromUUID("Root");
            CurrentLayout = new Layout(LayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Main";
            mPanelNodes[0].PanelSettings.Init(ControlPanelTypeList[0], true, mRootNode);
        }

        public override void CreateDefaultLayout()
        {
          


        }



        /// <summary>
        /// releases current resources
        /// </summary>
        public override void Hide()
        {
            foreach (var vPanelNodes in mPanelNodes)
            {
                vPanelNodes.PanelSettings.ReleaseResources();
            }
            gameObject.SetActive(false);
            PreviousView.Show();


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


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                BodyFramesRecording vRecording =
                    BodyRecordingsMgr.Instance.GetRecordingByUuid("2e6a7559-d623-43b2-8336-9b0de00d4d96");
                mControlPanel.NewRecordingSelected(vRecording);
            }    
        }
    }
}