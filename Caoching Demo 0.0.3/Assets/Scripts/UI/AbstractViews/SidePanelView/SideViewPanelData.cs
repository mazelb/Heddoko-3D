
/** 
* @file SideViewPanelData.cs
* @brief Contains the SideViewPanelData class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SidePanelView
{
    /// <summary>
    /// Data component of a side view panel
    /// </summary>
    [SerializeField]
    public class SideViewPanelData
    {
        [SerializeField]
        public Sprite Icon;

        [SerializeField]
        public string Name;
    }
}