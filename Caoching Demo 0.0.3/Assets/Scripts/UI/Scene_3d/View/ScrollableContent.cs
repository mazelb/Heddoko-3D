/** 
* @file ScrollablePanel.cs
* @brief Contains the ScrollableContent class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.View
{
    /// <summary>
    /// The content of a scrollable panel. Contains the name of the content, 
    /// a callback action
    /// </summary>
   public class ScrollableContent
    {
        public string Key { get; set; }
        public Action CallbackAction { get; set; }
        public Button ContentButton { get; set; }
        public string ContentValue { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ScrollableContent)
            {
                string vKey = ((ScrollableContent) obj).Key;
                return (vKey.Equals(this.Key));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
