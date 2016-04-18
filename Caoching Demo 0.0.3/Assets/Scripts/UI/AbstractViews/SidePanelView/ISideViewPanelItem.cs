
/** 
* @file ISideViewPanelItem.cs
* @brief Contains the ISideViewPanelItem interface
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UIWidgets;

namespace Assets.Scripts.UI.AbstractViews.SidePanelView
{
    public delegate void OnToggleUpdate(AbstractView vView, TreeNodeToggle vToggle);
    public interface ISideViewPanelItem: IObservable
    {
        void Display(SideViewPanelComponent vItem);
        AbstractView AssociatedView { get; set; }
        void SetToggle(TreeNodeToggle vToggle);

          event OnToggleUpdate ToggleUpdate;
    }
}