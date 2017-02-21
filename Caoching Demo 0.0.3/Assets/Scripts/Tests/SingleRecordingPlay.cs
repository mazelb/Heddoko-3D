using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;

using System.Collections.Generic;
using Assets.Scripts.Body_Data;
using UnityEngine.UI;
namespace Assets.Scripts.Tests
{
    public class SingleRecordingPlay : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
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
            }
        }
    }
}