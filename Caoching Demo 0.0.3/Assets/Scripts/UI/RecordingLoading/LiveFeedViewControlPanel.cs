/** 
* @file LiveFeedViewControlPanel.cs
* @brief contains the LiveFeedViewControlPanel class and its required components 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.AbstractSuitSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.MainMenu.View;
using Assets.Scripts.Utils.DebugContext;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// A live feed view 
    /// </summary>
    public class LiveFeedViewControlPanel: AbstractControlPanel
    {
        public SuitStateChangerControl SuitChanger;
        private Body mBody;
        private AbstractSuitConnection mSuitConnection;
        public Button TPoseButton;
        public MainMenuBrainpackView BpConnectionView;
        
         
        private bool mIsInitialized = false;

        public Body Body
        {
            get { return mBody; }
            set
            {
                mBody = value;
                TPoseButton.onClick.RemoveAllListeners();
               TPoseButton.onClick.AddListener(() =>
               {
                   if (Body.InitialBodyFrame != null)
                   {
                       Body.View.ResetInitialFrame();
                   }
               });
            }
        }
        public void Show()
        {
            gameObject.SetActive(true);
            if (!mIsInitialized)
            { 
                SuitChanger.gameObject.SetActive(true);
                mIsInitialized = true;
            }
        }

        void Awake()
        {
        }
        void OnEnable()
        {
            InputHandler.RegisterKeyboardAction(KeyCode.Home, ()=>Body.View.ResetInitialFrame());
   
        }
        void OnDisable()
        {
            InputHandler.RemoveKeybinding(KeyCode.Home, () => Body.View.ResetInitialFrame());
        }
        public AbstractSuitConnection SuitConnection
        {
            get { return mSuitConnection; }
            set
            {
                mSuitConnection = value;
                SuitChanger.SuitConnection = value;
            }
        }

        public override ControlPanelType PanelType
        {
            get { return ControlPanelType.LiveBPFeedView; }
        }

        public override void ReleaseResources()
        {
            
        }
    }
}