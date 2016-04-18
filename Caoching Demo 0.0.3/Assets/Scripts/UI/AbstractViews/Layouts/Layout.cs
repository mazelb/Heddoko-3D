/** 
* @file Layout.cs
* @brief Contains the Layout class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.Templates;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.Layouts
{
    /// <summary>
    /// Acts as an identifier for a LayoutContainer
    /// </summary>
    public class Layout
    {
        private Guid mId = new Guid();
        private LayoutContainerStruct mLayoutContainerStructure;
        private AbstractView mParentViewLayout;
        private List<AbstractControlPanel> mControlPanels; 
        /// <summary>
        /// Constructor that automatically creates a layout
        /// </summary>
        /// <param name="vLayoutType"></param>
        /// <param name="vParentViewLayout"></param>
        public Layout(LayoutType vLayoutType, AbstractView vParentViewLayout)
        { 
            mParentViewLayout = vParentViewLayout;
           mLayoutContainerStructure = LayoutCreationManager.Instance.CreateLayoutContainer(mParentViewLayout,  vLayoutType); 
        }

        /// <summary>
        /// Constructor takes in the parent view layout
        /// </summary>
        /// <param name="vParentViewLayout"></param>
        public Layout(AbstractView vParentViewLayout)
        {
            mParentViewLayout = vParentViewLayout;
        }

        public LayoutContainerStruct ContainerStructure
        {
            get { return mLayoutContainerStructure; }
        }
    }


    public enum LayoutType
    {
        OneLeftByTwoRight,
        OneRightByTwoLeft,
        HalfHalfVertical,
        HalfHalfHorizontal,
        Single,
        VerticalTSplitBot,
        VerticalTSplitTop
    }
}
