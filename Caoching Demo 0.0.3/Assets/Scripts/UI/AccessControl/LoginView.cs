
/** 
* @file LoginView.cs
* @brief Contains the LoginView class, implementing AbstractControlPanel  abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using Assets.Scripts.UI.AbstractViews.AbstractPanels;
 
namespace Assets.Scripts.UI.AccessControl
{
    /// <summary>
    /// provides a log in view
    /// </summary>
    public class LoginView : AbstractControlPanel
    {
        private LoginControl mLoginControl;

        public override void ReleaseResources()
        {
           
        }
    }
}