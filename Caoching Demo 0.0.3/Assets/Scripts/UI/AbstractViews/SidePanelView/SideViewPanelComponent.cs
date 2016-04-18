
/** 
* @file SideViewPanelComponent.cs
* @brief Contains the SideViewPanelComponent class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UIWidgets;
using UnityEngine;
namespace Assets.Scripts.UI.AbstractViews.SidePanelView
{
    /// <summary>
    /// A component required by the side view
    /// </summary>
    public class SideViewPanelComponent : TreeViewComponentBase<ISideViewPanelItem>
    {
        ISideViewPanelItem mItem;

        public ISideViewPanelItem Item
        {
            get
            {
                 
                return mItem;
            }
            set
            {
                if (mItem != null)
                {
                    mItem.OnChange -= UpdateView;
                }
                mItem = value;
                if (mItem != null)
                {
                    mItem.OnChange += UpdateView;

                    mItem.SetToggle(this.Toggle);
                }
                UpdateView();
                


            }
        }

        public override void SetData(TreeNode<ISideViewPanelItem> vNode, int vDepth)
        {
            if (vNode != null)
            {
                Node = vNode;
                SetToggleRotation(Node.IsExpanded);
            }
            if (Filler != null)
            {
                Filler.preferredWidth = vDepth * PaddingPerLevel;
            }

           /* if ((Toggle != null) && (Toggle.gameObject != null))
            {
                var toggle_active = (vNode.Nodes != null) && (vNode.Nodes.Count > 0);
                if (Toggle.gameObject.activeSelf != toggle_active)
                {
                    Toggle.gameObject.SetActive(toggle_active);
                }
            }*/

            Item = (vNode == null) ? null : vNode.Item;
        }

        protected virtual void UpdateView()
        { 
            if (Item == null)
            {
                Icon.sprite = null;
                Text.text = string.Empty;
            }
            else
            {
                Item.Display(this);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy(); 
            if (mItem != null)
            {
                mItem.OnChange -= UpdateView;
            }
        }

 


        protected override void SetToggleRotation(bool isExpanded)
        {
            //Do nothing.  
        }
    }
}
