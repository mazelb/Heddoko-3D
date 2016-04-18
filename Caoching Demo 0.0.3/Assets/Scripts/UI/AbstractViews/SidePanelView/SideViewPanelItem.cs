
/** 
* @file SideViewPanelItem.cs
* @brief Contains the SideViewPanelItem class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SidePanelView
{
    /// <summary>
    /// A component used by the main side view panel
    /// </summary>
    [SerializeField]
    public class SideViewPanelItem : ISideViewPanelItem
    {
        public event OnChange OnChange;
        public event OnToggleUpdate ToggleUpdate;

        [SerializeField]
        private string mName;
        private TreeNodeToggle mToggle;
        [SerializeField]
        private Sprite mIcon;

        public SideViewPanelItem(string vItemName)
        {
            mName = vItemName;

        }

        public SideViewPanelItem(string vItemName, Sprite vIcon, AbstractView vAssociatedView)
        {
            Name = vItemName;
            Icon = vIcon;
            AssociatedView = vAssociatedView;
        }
        void Changed()
        {
            if (OnChange != null)
            {
                OnChange();
            }
        }
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        public Sprite Icon
        {
            get { return mIcon; }
            set { mIcon = value; }
        }


        public void Display(SideViewPanelComponent vComponent)
        {
            vComponent.Icon.sprite = Icon;
            vComponent.Text.text = Name;
            vComponent.name = Name;
        }

        public AbstractView AssociatedView { get; set; }
        public void SetToggle(TreeNodeToggle vToggle)
        {
            mToggle = vToggle;
            if (ToggleUpdate != null)
            {
                ToggleUpdate.Invoke(this.AssociatedView, this.mToggle);
            }
        }


    }
}