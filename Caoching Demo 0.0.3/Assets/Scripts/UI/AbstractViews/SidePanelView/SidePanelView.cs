
/** 
* @file SidePanelView.cs
* @brief Contains the SidePanelView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.Utils.Containers;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SidePanelView
{
    /// <summary>
    /// The side panel in the app main screen
    /// </summary>
    public class SidePanelView : AbstractView
    {
        public SidePanelCustomTree Tree;
        [SerializeField]
        List<AbstractViewTitleStruct> mSubMovementList = new List<AbstractViewTitleStruct>();

        [SerializeField]
        private FontAwesomeSpriteContainer mSpriteContainer;

        private List<AbstractView> mAbstractViews = new List<AbstractView>();


        void Start()
        {
            CreateDefaultLayout();
        }



        private ObservableList<TreeNode<TreeViewItem>> mNodes;
        public override void CreateDefaultLayout()
        {

            Tree.Nodes = CreateData();
            Tree.Start();
        }

        ObservableList<TreeNode<ISideViewPanelItem>> CreateData()
        {
            var vNodes = new ObservableList<TreeNode<ISideViewPanelItem>>();
            vNodes.Add(Node(new SideViewPanelItem("DASHBOARD", mSpriteContainer[11], null)));

            var vSubMovementNodes = CreateSubMovementList();
            vNodes.Add(Node(new SideViewPanelItem("MOVEMENTS", mSpriteContainer[69], null), vSubMovementNodes));

            vNodes.Add(Node(new SideViewPanelItem("MY ACCOUNT", mSpriteContainer[96], null)));
            vNodes.Add(Node(new SideViewPanelItem("SETTINGS", mSpriteContainer[60], null)));
            vNodes.Add(Node(new SideViewPanelItem("HELP", mSpriteContainer[27], null)));



            return vNodes;
        }

        /// <summary>
        /// Hides the other panels from view
        /// </summary>  
        private void HideOtherPanels()
        {

        }



        /// <summary>
        /// Construct a set of nodes that are children of the " MOVEMENTS" node
        /// </summary>
        /// <returns></returns>
        ObservableList<TreeNode<ISideViewPanelItem>> CreateSubMovementList()
        {
            var vSubMovementNodeList = new ObservableList<TreeNode<ISideViewPanelItem>>();
            foreach (var vSubmoveStruct in mSubMovementList)
            {
                var vSidePanelItem = new SideViewPanelItem(vSubmoveStruct.Title, vSubmoveStruct.Icon, vSubmoveStruct.View);
                vSidePanelItem.ToggleUpdate += ShowView;
                var vNode = Node(vSidePanelItem);

                vSubMovementNodeList.Add(vNode);
            }

            return vSubMovementNodeList;
        }

        ObservableList<TreeNode<ISideViewPanelItem>> CreateSubMovementLis1t()
        {
            var vSubMovementNodeList = new ObservableList<TreeNode<ISideViewPanelItem>>();
            foreach (var vSubmoveStruct in mSubMovementList)
            {
                var vSidePanelItem = new SideViewPanelItem(vSubmoveStruct.Title, vSubmoveStruct.Icon, vSubmoveStruct.View);
                var vNode = Node(vSidePanelItem);
                vSubMovementNodeList.Add(vNode);
            }

            return vSubMovementNodeList;
        }
        TreeNode<ISideViewPanelItem> Node(ISideViewPanelItem item, ObservableList<TreeNode<ISideViewPanelItem>> nodes = null)
        {
            return new TreeNode<ISideViewPanelItem>(item, nodes, false, true);
        }
   
        /// <summary>
        /// Shows the view associate with the toggle
        /// </summary>
        /// <param name="vView"></param>
        /// <param name="vToggleComponent"></param>
        public void ShowView(AbstractView vView,TreeNodeToggle vToggleComponent)
        {
            if (vView == null)
            {
                return;
            }
            if (!mAbstractViews.Contains(vView))
            {
                mAbstractViews.Add(vView);
            }
            Debug.Log("NOT NULL");
            vToggleComponent.OnClick.RemoveAllListeners();
            vToggleComponent.OnClick.AddListener(() =>
            {
                UnityEngine.Debug.Log("clicked ");
                HideAllOtherViews(vView);
                if (!vView.gameObject.activeInHierarchy)
                {
                    vView.Show();
                }
            });
        }
 
        /// <summary>
        /// Hides all other views besides the one passed in
        /// </summary>
        /// <param name="vView"></param>
        private void HideAllOtherViews(AbstractView vView)
        {
            foreach (var vAbstracView in mAbstractViews)
            {
                if (vAbstracView == vView)
                {
                    continue;
                }
                if (vAbstracView.gameObject.activeInHierarchy)
                {
                    vAbstracView.Hide();
                }
            }
        }
    }
}
