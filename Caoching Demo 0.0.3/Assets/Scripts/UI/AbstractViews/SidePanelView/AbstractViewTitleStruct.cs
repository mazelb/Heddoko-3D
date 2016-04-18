
/** 
* @file AbstractViewTitleStruct.cs
* @brief Contains the AbstractViewTitleStruct class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SidePanelView
{
    /// <summary>
    /// an abstract view with its title
    /// </summary>
    [Serializable]
   public class AbstractViewTitleStruct
    {
        [SerializeField] public AbstractView View;
        [SerializeField] public string Title;
        [SerializeField] public Sprite Icon;
    }
}
