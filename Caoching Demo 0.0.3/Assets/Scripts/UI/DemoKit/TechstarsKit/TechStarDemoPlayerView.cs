/**
* @file TechStarDemoPlayerView.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.Body_Data;
using Assets.Scripts.UI.AbstractViews; 
using Assets.Scripts.UI.AbstractViews.camera;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    public delegate void UpdatedView(TechStarDemoPlayerView vView);
    /// <summary>
    /// To be used in the techstar scene. Loads an avatar that waits for a response to start playing
    /// </summary>
    public class TechStarDemoPlayerView : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public event UpdatedView ViewUpdatedEvent;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        private PanelNode mRootNode;
        private List<ControlPanelType> mMainControlPanel = new List<ControlPanelType>();
        public  DemoPlayer Player;
        public Body DefaultBody;
        public PanelCamera PanelCam { get; set; }
        
        public void Init()
        {
            CreateDefaultLayout();
        }

        public override void CreateDefaultLayout()
        { 
            BodiesManager.Instance.CreateNewBody("DefaultBody");
            DefaultBody = BodiesManager.Instance.GetBodyFromUUID("DefaultBody");
            mMainControlPanel.Add(ControlPanelType.DemoKit);

            CurrentLayout = new Layout(LayoutType.Single, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Main";
            mRootNode = mPanelNodes[0];
            
            //one panel only to have a playback
            //the other to just render data but, set the arc's layer to be something completely different from another
            BodySegment.IsTrackingHeight = false;
            mRootNode.PanelSettings.Init(mMainControlPanel, true, DefaultBody);
       
            mRootNode.PanelCamUpdated += CameraRenderedPairCreation;
        }

        /// <summary>
        /// Once the PanelNode has 
        /// </summary>
        /// <param name="vNode"></param>
        void CameraRenderedPairCreation(PanelNode vNode)
        {
            Player = mRootNode.PanelSettings.GetPanelOfType(ControlPanelType.DebugRecordingPanel) as DemoPlayer;
            PanelCam = mRootNode.PanelSettings.CameraToBodyPair.PanelCamera;
            if (ViewUpdatedEvent != null)
            {

                ViewUpdatedEvent( this);
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

        public override void Show()
        {
            gameObject.SetActive(true);
            if (CurrentLayout == null)
            {
                CreateDefaultLayout();
            }
            else
            {
                mRootNode.PanelSettings.RequestResources();
            }
        }
    }
}