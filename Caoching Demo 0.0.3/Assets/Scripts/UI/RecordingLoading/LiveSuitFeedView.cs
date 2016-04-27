/* @file LiveSuitFeedView.cs
* @brief Contains the LiveSuitFeedView class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Layouts;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    ///   A live feed view
    /// </summary>
    public class LiveSuitFeedView : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        private PanelNode mRootNode;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        public Body BrainpackBody;
        public BrainpackConnectionController BpController;
        private LiveFeedViewControlPanel mLiveFeedViewControlPanel;
        private bool mIsInitialized = false;


        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.LiveBPFeedView);
            ControlPanelTypeList.Add(vLeftSide);
            CreateDefaultLayout();
        }


        /// <summary>
        /// Create a default layout for the live feed view
        /// </summary>
        public override void CreateDefaultLayout()
        {
            BodiesManager.Instance.CreateNewBody("BrainpackPlaceholderBody");
            BrainpackBody = BodiesManager.Instance.GetBodyFromUUID("BrainpackPlaceholderBody");
            CurrentLayout = new Layout(LayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mRootNode = mPanelNodes[0];
            mRootNode.name = "Main";
           
            mRootNode.PanelSettings.Init(ControlPanelTypeList[0], false, BrainpackBody);
            mLiveFeedViewControlPanel = (LiveFeedViewControlPanel)mRootNode.PanelSettings.GetPanelOfType(ControlPanelType.LiveBPFeedView);
            mLiveFeedViewControlPanel.SuitConnection = BpController;
            mLiveFeedViewControlPanel.Body = BrainpackBody;
            mLiveFeedViewControlPanel.gameObject.SetActive(true);
            mLiveFeedViewControlPanel.Show();
            mIsInitialized = true;

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
                Debug.Log(name+" requesting resources");
               mPanelNodes[0].PanelSettings.RequestResources();
            }
            BrainpackBody.StreamFromBrainpack();
        }

        public override void Hide()
        {
            //it is entirely possible that the view hasn't been initialized. verify this before proceeding
            if (mIsInitialized)
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
    }
}