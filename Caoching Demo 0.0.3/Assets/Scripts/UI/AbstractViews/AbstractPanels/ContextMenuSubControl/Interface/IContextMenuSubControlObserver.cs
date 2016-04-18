/** 
* @file IContextMenuSubControlObserver.cs
* @brief Contains the IContextMenuSubControlObserver class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.ContextMenuSubControl
{
    /// <summary>
    /// Provides an interface for any subcontrollers that need to notify of a ContextMenu launch
    /// </summary>
    public interface IContextMenuSubControlObserver
    {

        void Notify();
    }
}