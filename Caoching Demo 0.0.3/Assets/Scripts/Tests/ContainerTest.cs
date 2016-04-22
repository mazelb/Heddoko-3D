 
/** 
* @file ContainerTest.cs
* @brief Contains the ContainerTest class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
 
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts; 
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Layout, and layoutcontainer testing
    /// </summary>
    public class ContainerTest : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        public Button BackButton;
        private Body mRootNode;
         void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            TestCreateLayout();
            BackButton.onClick.AddListener(Hide);
            
        }

        void TestCreateLayout()
        {
            
        }

        public override void CreateDefaultLayout()
        {
            BodiesManager.Instance.CreateNewBody("Root");
            mRootNode = BodiesManager.Instance.GetBodyFromUUID("Root");
            CurrentLayout = new Layout(LayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Main";
            mPanelNodes[0].PanelSettings.Init(ControlPanelTypeList[0], true, mRootNode);
 
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
    }

}
 