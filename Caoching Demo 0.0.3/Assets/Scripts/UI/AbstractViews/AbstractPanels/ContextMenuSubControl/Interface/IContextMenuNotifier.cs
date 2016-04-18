/** 
* @file IContextMenuNotifier.cs
* @brief Contains the IContextMenuNotifier class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.ContextMenuSubControl
{
    /// <summary>
    /// An interface for context menu notifiers
    /// </summary>
    public interface IContextMenuNotifier
    {
        void RegisterObserver(IContextMenuSubControlObserver vObserver);
        void RemoveObserver(IContextMenuSubControlObserver vObserver);
        void NotifyObservers();
    }
}