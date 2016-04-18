/** 
* @file PanelNodeTemplate.cs
* @brief contains a template for panel nodes to use
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.Templates
{
    /// <summary>
    /// A template for panel nodes to use
    /// </summary>
   public  class PanelNodeTemplate
    {
        public Dictionary<ControlPanelType,SubControlType> Mapping = new Dictionary<ControlPanelType, SubControlType>();

        public RectOffset HorizontalOrVerticalPadding;
        public float PrefferedWidthOrHeight;
        public float Spacing;
        public HorizontalOrVerticalLayoutGroupType HorizontalOrVerticalLayoutType;

        /// <summary>
        /// Create a new Template for a panel node with well defined parameters
        /// </summary>
        /// <param name="vPadding">the amount of padding of the resulting template</param>
        /// <param name="vPrefHeightOrWidthMod">the preffered height or width modifier </param>
        /// <param name="vSpacing">spacing between inner objects(if applicable)</param>
        /// <param name="vType"></param>
        public PanelNodeTemplate(RectOffset vPadding, float vPrefHeightOrWidthMod, float vSpacing,
            HorizontalOrVerticalLayoutGroupType vType)
        {
            HorizontalOrVerticalPadding = vPadding;
            PrefferedWidthOrHeight = vPrefHeightOrWidthMod;
            Spacing = vSpacing;
            HorizontalOrVerticalLayoutType = vType;
        }

        /// <summary>
        /// Because monobehaviours do not like being called by the new keyword, this is a workaround method that attaches a 
        /// new HorizontalOrVerticalLayoutGroup to the calling PanelNode.
        /// </summary>
        /// <param name="vPanelNode"></param>
        /// <param name="vTemplate"></param>
        /// <returns></returns>
        public static HorizontalOrVerticalLayoutGroup AttachHorizontalOrVerticalLayoutGroup(PanelNode vPanelNode, PanelNodeTemplate vTemplate)
        {
            if (vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>() != null)
            {
                // ReSharper disable once AccessToStaticMemberViaDerivedType
                GameObject.Destroy(vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>()); 
            }
            if (vTemplate.HorizontalOrVerticalLayoutType == HorizontalOrVerticalLayoutGroupType.Horizontal)
            {
                vPanelNode.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            else if((vTemplate.HorizontalOrVerticalLayoutType == HorizontalOrVerticalLayoutGroupType.Vertical))
            {
                vPanelNode.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            else if (vTemplate.HorizontalOrVerticalLayoutType == HorizontalOrVerticalLayoutGroupType.Null)
            {
                return null;
            }
            return vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>();
        }

        /// <summary>
        /// Because monobehaviours do not like being called by the new keyword, this is a workaround method that attaches a 
        /// new HorizontalOrVerticalLayoutGroup to the calling PanelNode.
        /// </summary>
        /// <param name="vPanelNode"></param>
        /// <returns></returns>
        public static HorizontalOrVerticalLayoutGroup AttachHorizontalOrVerticalLayoutGroup(PanelNode vPanelNode, HorizontalOrVerticalLayoutGroupType vType)
        {
            if (vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>() != null)
            {
                // ReSharper disable once AccessToStaticMemberViaDerivedType
                GameObject.Destroy(vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>());
            }
            if ( vType == HorizontalOrVerticalLayoutGroupType.Horizontal)
            {
                vPanelNode.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            else if ((vType == HorizontalOrVerticalLayoutGroupType.Vertical))
            {
                vPanelNode.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            else if (vType == HorizontalOrVerticalLayoutGroupType.Null)
            {
                return null;
            }
            return vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>();
        }
    }
    public enum HorizontalOrVerticalLayoutGroupType
    {
        Null,
        Horizontal,
        Vertical
    }
}

 
