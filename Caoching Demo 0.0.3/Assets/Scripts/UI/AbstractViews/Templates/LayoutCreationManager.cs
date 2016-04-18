

/* @file LayoutCreationManager.cs
* @brief Contains the LayoutCreationManager class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;
using Assets.Scripts.UI.AbstractViews.Layouts;


namespace Assets.Scripts.UI.AbstractViews.Templates
{
    public class LayoutCreationManager
    {
        private Dictionary<LayoutType, Func<LayoutContainerStruct>> mLayoutToCreationMapping = new Dictionary<LayoutType, Func<LayoutContainerStruct>>();


        private static LayoutCreationManager sInstance;
        private AbstractView mParentAbstractView;


        public static LayoutCreationManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new LayoutCreationManager();
                    sInstance.Init();
                }
                return sInstance;
            }
        }

        /// <summary>
        /// Set up mappings
        /// </summary>
        private void Init()
        {
            mLayoutToCreationMapping.Add(LayoutType.Single, SingleLayoutCreation);
            mLayoutToCreationMapping.Add(LayoutType.HalfHalfHorizontal, HalfHalfHorizontalLayoutCreation);
            mLayoutToCreationMapping.Add(LayoutType.HalfHalfVertical, HalfHalfVerticalLayoutCreation);
            mLayoutToCreationMapping.Add(LayoutType.OneLeftByTwoRight, OneLeftByTwoRightCreation);
            mLayoutToCreationMapping.Add(LayoutType.OneRightByTwoLeft, OneRightByTwoLeftCreation);
            mLayoutToCreationMapping.Add(LayoutType.VerticalTSplitBot, VertTSplitBotCreation);
            mLayoutToCreationMapping.Add(LayoutType.VerticalTSplitTop, TSplitTopCreation);
        }
        /// <summary>
        /// a layout with  a panel sitting at the foot of two panel nodes  that are 50% vertically split
        /// </summary>
        /// <returns></returns>
        private LayoutContainerStruct VertTSplitBotCreation()
        {
            LayoutContainerStruct vLayoutContainer = SingleLayoutCreatorHelper(mParentAbstractView, HorizontalOrVerticalLayoutGroupType.Vertical);
            PanelNode vRoot = vLayoutContainer.LayoutContainer.GetRoot();
            float vRootHeight = vRoot.PanelSettings.RectTransform.rect.height;
            float vBottomSplit = vRootHeight * (0.25f);
            float vTopSplit = vRootHeight - vBottomSplit;

            RectOffset vFivesTop = new RectOffset(5, 5, 5, 5);
            RectOffset vFivesBot = new RectOffset(5, 5, 5, 5);
            PanelNodeTemplate vBottomSplitTemplate = new PanelNodeTemplate(vFivesTop, vBottomSplit, 5, HorizontalOrVerticalLayoutGroupType.Null);

            PanelNodeTemplate vTopSplitTemplate = new PanelNodeTemplate(vFivesBot, vTopSplit, 5, HorizontalOrVerticalLayoutGroupType.Null);

            PanelNode vTopSplitNode = vLayoutContainer.LayoutContainer.AddPanelNode(vRoot, vRoot.PanelSettings.RectTransform, vTopSplitTemplate);
            PanelNode vBotSplitNode = vLayoutContainer.LayoutContainer.AddPanelNode(vRoot, vRoot.PanelSettings.RectTransform, vBottomSplitTemplate);

            //set the parents
            vTopSplitNode.Parent = vRoot;
            vBotSplitNode.Parent = vRoot;

            RectOffset vFivesBot1 = new RectOffset(5, 5, 5, 5);
            RectOffset vFivesBot2 = new RectOffset(5, 5, 5, 5);
            float vSplitBotHalf = vBotSplitNode.PanelSettings.RectTransform.rect.height / 2f;

            PanelNodeTemplate vBotSplit1 = new PanelNodeTemplate(vFivesBot1, vSplitBotHalf, 5, HorizontalOrVerticalLayoutGroupType.Null);
            PanelNodeTemplate vBotSplit2 = new PanelNodeTemplate(vFivesBot2, vSplitBotHalf, 5, HorizontalOrVerticalLayoutGroupType.Null);
            PanelNode[] vNodes = vLayoutContainer.RenderingPanelNodes = new PanelNode[3];
            foreach (PanelNode vNode in vNodes)
            {
                vNode.Parent = vBotSplitNode;
            }



            List<PanelNode> vNewChildren = vTopSplitNode.SplitPanelInHalf(HorizontalOrVerticalLayoutGroupType.Horizontal, vBotSplit1, vBotSplit2);
            vNodes[0] = vTopSplitNode;
            vNodes[1] = vNewChildren[0];
            vNodes[3] = vNewChildren[1];
            Cleanup();
            return vLayoutContainer;
        }
        private LayoutContainerStruct TSplitTopCreation()
        {
            throw new NotImplementedException();
        }
        private LayoutContainerStruct OneRightByTwoLeftCreation()
        {
            LayoutContainerStruct vLayoutContainer = SingleLayoutCreatorHelper(mParentAbstractView, HorizontalOrVerticalLayoutGroupType.Horizontal);
            PanelNode vRoot = vLayoutContainer.LayoutContainer.GetRoot();
            float vRootWidth = vRoot.PanelSettings.RectTransform.rect.width;
            float vLeftSplit = vRootWidth * 0.25f;
            float vRightSplit = vRootWidth - vLeftSplit;
            RectOffset vFivesOffset = new RectOffset(5, 5, 5, 5);
            PanelNode[] vNodes = new PanelNode[5];
         
            PanelNodeTemplate vLeftSplitTemplate = new PanelNodeTemplate(vFivesOffset, vLeftSplit, 5, HorizontalOrVerticalLayoutGroupType.Null);
            PanelNodeTemplate vRightSplitTemplate = new PanelNodeTemplate(vFivesOffset, vRightSplit, 5, HorizontalOrVerticalLayoutGroupType.Null);

            PanelNode vLeftSpliteNode = vLayoutContainer.LayoutContainer.AddPanelNode(vRoot, vRoot.PanelSettings.RectTransform, vLeftSplitTemplate);
            PanelNode vRightSplitNode = vLayoutContainer.LayoutContainer.AddPanelNode(vRoot, vRoot.PanelSettings.RectTransform, vRightSplitTemplate);
            vRightSplitNode.Parent = vRoot;
            vLeftSpliteNode.Parent = vRoot;

            float vLeftSplitHalf = vLeftSpliteNode.PanelSettings.RectTransform.rect.height / 2f;
            PanelNodeTemplate vLeftSplitTop = new PanelNodeTemplate(vFivesOffset, vLeftSplitHalf, 5, HorizontalOrVerticalLayoutGroupType.Null);
            PanelNodeTemplate vLeftSplitBot = new PanelNodeTemplate(vFivesOffset, vLeftSplitHalf, 5, HorizontalOrVerticalLayoutGroupType.Null);

            //create a set of rendering panel nodes for the layout container. Fill them with its children
            vLayoutContainer.RenderingPanelNodes = vNodes;

            List<PanelNode> vNewChildren = vLeftSpliteNode.SplitPanelInHalf(HorizontalOrVerticalLayoutGroupType.Vertical, vLeftSplitTop, vLeftSplitBot);
            vNodes[0] = vRoot;
            vNodes[1] = vLeftSpliteNode;
            vNodes[2] = vRightSplitNode;
            vNodes[3] = vNewChildren[0];
            vNodes[4] = vNewChildren[1];
            Cleanup();
            return vLayoutContainer;
        }

        private LayoutContainerStruct OneLeftByTwoRightCreation()
        {
            return null;
        }

        private LayoutContainerStruct HalfHalfVerticalLayoutCreation()
        {
            return null;
        }

        /// <summary>
        /// A layout container with a 50/50 horizontal split
        /// </summary>
        /// <returns></returns>
        private LayoutContainerStruct HalfHalfHorizontalLayoutCreation()
        {

            LayoutContainerStruct vLayoutContainer = SingleLayoutCreatorHelper(mParentAbstractView, HorizontalOrVerticalLayoutGroupType.Horizontal);
            PanelNode vRoot = vLayoutContainer.LayoutContainer.GetRoot();
            float vRootWidth = vRoot.PanelSettings.RectTransform.rect.width;
            float vHalfWidth = vRootWidth / 2f;

            RectOffset vFivePadding = new RectOffset(5, 5, 5, 5);
            PanelNodeTemplate vLeftSideNodeTemplate = new PanelNodeTemplate(vFivePadding, vHalfWidth, 5, HorizontalOrVerticalLayoutGroupType.Null);
            PanelNodeTemplate vRightSideNodeTemplate = new PanelNodeTemplate(vFivePadding, vHalfWidth, 5, HorizontalOrVerticalLayoutGroupType.Null);

            PanelNode[] vPanelNodes = vLayoutContainer.RenderingPanelNodes = new PanelNode[2];

            PanelNode vLeftNode = vLayoutContainer.LayoutContainer.AddPanelNode(vRoot, vRoot.PanelSettings.RectTransform, vLeftSideNodeTemplate);
            PanelNode vRightNode = vLayoutContainer.LayoutContainer.AddPanelNode(vRoot, vRoot.PanelSettings.RectTransform, vRightSideNodeTemplate);
            vPanelNodes[0] = vLeftNode;
            vPanelNodes[1] = vRightNode;

            Cleanup();
            return vLayoutContainer;
        }

        /// <summary>
        /// A layout container with a single planel node
        /// </summary>
        /// <returns></returns>
        private LayoutContainerStruct SingleLayoutCreation()
        {
            LayoutContainerStruct vLayoutContainer = SingleLayoutCreatorHelper(mParentAbstractView, HorizontalOrVerticalLayoutGroupType.Null);
            Cleanup();
            return vLayoutContainer;
        }

        /// <summary>
        /// Helper method that creates a layout container with a single panel node
        /// </summary>
        /// <param name="vParentAbstractView">the parent abstract view</param>
        /// <returns>A newly created LayoutContainer</returns>
        public static LayoutContainerStruct SingleLayoutCreatorHelper(AbstractView vParentAbstractView, HorizontalOrVerticalLayoutGroupType vType)
        {


            RectOffset vTens = new RectOffset(10, 10, 10, 10);
            PanelNodeTemplate vTemplate = new PanelNodeTemplate(vTens, 0, 5, vType);
            LayoutContainer vLayoutContainer = new LayoutContainer();
            PanelNode vRoot = vLayoutContainer.AddRoot(vParentAbstractView.RectTransform, vTemplate);
            vRoot.PanelSettings.RectTransform.anchorMin = Vector2.zero;
            vRoot.PanelSettings.RectTransform.anchorMax = Vector2.one;
            vRoot.PanelSettings.RectTransform.offsetMin = vRoot.PanelSettings.RectTransform.offsetMax = new Vector2(0, 0);

            PanelNode[] vPanelNodes = new PanelNode[1];
            vPanelNodes[0] = vRoot;
            LayoutContainerStruct vContainerStruct = new LayoutContainerStruct(vLayoutContainer, vPanelNodes);

            return vContainerStruct;
        }
        /// <summary>
        /// Clean up resources
        /// </summary>
        private void Cleanup()
        {
            mParentAbstractView = null;
            //  mAbstractControlQueue = null;
        }

        /// <summary>
        /// Returns a layout container based on the layout type passed in
        /// </summary>
        /// <param name="vParentView">the parent abstract view</param>
        /// <param name="vLayoutType"></param>
        /// <returns>the newly created layoutcontainer</returns>
        public LayoutContainerStruct CreateLayoutContainer(AbstractView vParentView, LayoutType vLayoutType)
        {
            LayoutContainerStruct vNewContainer = null;
            mParentAbstractView = vParentView;
            if (mLayoutToCreationMapping.ContainsKey(vLayoutType))
            {
                vNewContainer = mLayoutToCreationMapping[vLayoutType].Invoke();
                return vNewContainer;
            }

            return vNewContainer;
        }


    }
    /// <summary>
    /// A structure that holds a layoutcontainer and a list of panel nodes
    /// </summary>
    public class LayoutContainerStruct
    {
        public LayoutContainer LayoutContainer { get; set; }
        public PanelNode[] RenderingPanelNodes { get; set; }

        public LayoutContainerStruct(LayoutContainer vLayoutContainer, PanelNode[] vRenderingPanelNodeList)
        {
            LayoutContainer = vLayoutContainer;
            RenderingPanelNodes = vRenderingPanelNodeList;
        }

    }
}
