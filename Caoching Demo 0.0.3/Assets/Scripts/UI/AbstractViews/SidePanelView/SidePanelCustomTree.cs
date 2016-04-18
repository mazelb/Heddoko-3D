
using System.Collections.Generic;
using UIWidgets;

namespace Assets.Scripts.UI.AbstractViews.SidePanelView
{
    public class SidePanelCustomTree : TreeViewCustom<SideViewPanelComponent, ISideViewPanelItem>
    {
        protected override void SetData(SideViewPanelComponent vComponent, ListNode<ISideViewPanelItem> vItem)
        {
            vComponent.SetData(vItem.Node, vItem.Depth); 
        }


       
        /// <summary>
        /// Apply highlight coloring on hightlight event
        /// </summary>
        /// <param name="vComponent"></param>
        protected override void HighlightColoring(SideViewPanelComponent vComponent)
        {
            base.HighlightColoring(vComponent);
            vComponent.Text.color = HighlightedColor;
        }

        protected override void SelectColoring(SideViewPanelComponent vComponent)
        {
            base.SelectColoring(vComponent);
            vComponent.Text.color = SelectedColor;
        }

        protected override void DefaultColoring(SideViewPanelComponent vComponent)
        {
            base.DefaultColoring(vComponent);
            vComponent.Text.color = DefaultColor;
        }



    }
}
