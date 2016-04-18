
/** 
* @file AbstractControlPanelBuilder.cs
* @brief  Builds abstract control panels
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews
{
    /// <summary>
    /// Builds abstract control panels
    /// </summary>
    public class AbstractControlPanelBuilder
    {

        public AbstractControlPanel BuildPanel(ControlPanelType vType)
        {  
            string vPath = ControlPanelRegistry.RequestControlPanelPath(vType);
            GameObject vInstance = GameObject.Instantiate(Resources.Load(vPath)) as GameObject;
            //TODO: create a component that checks if resources are available and displays it to the user in case resources arent
            return  vInstance.GetComponent<AbstractControlPanel>();
           
        }
    }
}
