
/** 
* @file RecordingPlayerView.cs
* @brief Contains the RecordingPlayerView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Single Player view
    /// </summary>
    public class RecordingPlayerView : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        private bool mIsInitialized = false;
        private Body mRootNode;
        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            CreateDefaultLayout();

        }



        public override void CreateDefaultLayout()
        {
            BodiesManager.Instance.CreateNewBody("Root");
            mRootNode = BodiesManager.Instance.GetBodyFromUUID("Root");
            CurrentLayout = new Layout(LayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Main";
            mPanelNodes[0].PanelSettings.Init(ControlPanelTypeList[0], false, mRootNode);
            mIsInitialized = true;
        }



        /// <summary>
        /// releases current resources
        /// </summary>
        public override void Hide()
        {
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