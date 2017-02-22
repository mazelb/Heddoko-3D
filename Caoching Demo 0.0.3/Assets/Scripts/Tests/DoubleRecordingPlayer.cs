
/** 
* @file ContainerTest.cs
* @brief Contains the ContainerTest class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Body_Data;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Layout, and layoutcontainer testing
    /// </summary>
    public class DoubleRecordingPlayer : AbstractView
    {
        private LayoutType LayoutType = LayoutType.HalfHalfHorizontal;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);

        private Body mRootNode;
        private Body mRightNodeBody;
        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            TestCreateLayout();


        }

        void TestCreateLayout()
        {

        }

        public override void CreateDefaultLayout()
        {
            BodiesManager.Instance.CreateNewBody("SingleRec_LeftPanelBody");
            BodiesManager.Instance.CreateNewBody("SingleRec_RightPanelBody");
            mRootNode = BodiesManager.Instance.GetBodyFromUUID("SingleRec_LeftPanelBody");
            mRightNodeBody = BodiesManager.Instance.GetBodyFromUUID("SingleRec_RightPanelBody");
            CurrentLayout = new Layout(LayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Left";
            mPanelNodes[1].name = "Right";
            mPanelNodes[0].PanelSettings.Init(ControlPanelTypeList[0], true, mRootNode);
            mPanelNodes[1].PanelSettings.Init(ControlPanelTypeList[1], true, mRightNodeBody);
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
                mPanelNodes[1].PanelSettings.RequestResources();
            }
        }
    }

}
