/** 
* @file MainMenuController.cs
* @brief Contains the MainMenuController class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using Assets.Scripts.UI.MainMenu.View;
using UnityEngine;

namespace Assets.Scripts.UI.MainMenu.Controller
{
    /// <summary>
    /// Represents the controller for the main menu. This class allows for the changing of context views
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {

        //The main menu view
        public MainMenuView MainMenuView;
        public SplashScreen SplashScreen;
 
        [SerializeField]
        private MainMenuState mCurrentState = MainMenuState.SplashScreen;


        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            bool vSplashScreenEnabled = SplashScreen != null && SplashScreen.gameObject.activeSelf;
            //if conditions fail, load directly into mainmenu view
            if (!vSplashScreenEnabled)
            {
                ChangeState(MainMenuState.MainMenu);
            }
            MainMenuView.BrainpackButton.onClick.AddListener(() => ChangeState(MainMenuState.BrainpackView));
            MainMenuView.BrainpackConnectionView.BackButton.onClick.AddListener(() => ChangeState(MainMenuState.MainMenu));
            MainMenuView.ExitButton.onClick.AddListener(QuitApplication);
            MainMenuView.ActivitiesButton.onClick.AddListener(() => ChangeState(MainMenuState.ActivityContext));

            if (MainMenuView.RecordingSelectionView != null)
            {
                // MainMenuView.RecordingSelectionView.pre.onClick.AddListener(() => ChangeState(MainMenuState.MainMenu));
                MainMenuView.RecordingSelectionView.PreviousView = MainMenuView;
                MainMenuView.RecordingsSelectionButton.onClick.AddListener(() => ChangeState(MainMenuState.RecordingsSelection));
            }

        }

        /// <summary>
        /// changes the state of the main menu
        /// </summary>
        /// <param name="vNewState"></param>
        public void ChangeState(MainMenuState vNewState)
        {
            switch (mCurrentState)
            {
                case (MainMenuState.SplashScreen):
                    {
                        if (vNewState == MainMenuState.MainMenu)
                        {
                            MainMenuView.ShowMainMenuContextView();
                            mCurrentState = MainMenuState.MainMenu;
                            // ReSharper disable once RedundantJumpStatement
                            break;
                        }
                        break;
                    }

                case (MainMenuState.ActivityContext):
                    {
                        if (vNewState == MainMenuState.MainMenu)
                        {
                            MainMenuView.HideActivityContextView();
                            MainMenuView.ShowMainMenuContextView();
                            mCurrentState = MainMenuState.MainMenu;
                            // ReSharper disable once RedundantJumpStatement
                            break;
                        }
                        break;
                    }

                case (MainMenuState.BrainpackView):
                    {

                        if (vNewState == MainMenuState.MainMenu)
                        {
                            MainMenuView.HideBrainpackContextView();
                            MainMenuView.ShowMainMenuContextView();
                            mCurrentState = MainMenuState.MainMenu;
                            // ReSharper disable once RedundantJumpStatement
                            break;
                        }
                        break;
                    }
                case (MainMenuState.RecordingsSelection):
                    {

                        if (vNewState == MainMenuState.MainMenu)
                        {
                            MainMenuView.HideRecordingsSelection();
                            MainMenuView.ShowMainMenuContextView();
                            mCurrentState = MainMenuState.MainMenu;
                            // ReSharper disable once RedundantJumpStatement
                            break;
                        }
                        break;
                    }

                case (MainMenuState.MainMenu):
                    {
                        if (vNewState == MainMenuState.ActivityContext)
                        {
                            MainMenuView.HideMainMenuContextView();
                            MainMenuView.ShowActivitiesContextView();
                            mCurrentState = MainMenuState.ActivityContext;
                            break;
                        }

                        if (vNewState == MainMenuState.BrainpackView)
                        {
                            MainMenuView.HideMainMenuContextView();
                            MainMenuView.ShowBrainpackContextView();
                            mCurrentState = MainMenuState.BrainpackView;
                            break;
                        }

                        if (vNewState == MainMenuState.RecordingsSelection)
                        {
                            MainMenuView.HideMainMenuContextView();
                            MainMenuView.ShowRecordingsSelection();
                            mCurrentState = MainMenuState.RecordingsSelection;
                            // ReSharper disable once RedundantJumpStatement
                            break;
                        }

                        break;
                    }
            }
        }
        private void QuitApplication()
        {
            //Todo: add a modal window asking if EU is sure that they want to quit
            Application.Quit();
        }
        /// <summary>
        /// Switch to the main menu view
        /// </summary>
        public void SwitchToMainMenu()
        {
            ChangeState(MainMenuState.MainMenu);
        }
        /// <summary>
        /// The Splash screen has finished loading. 
        /// </summary>
        public void SplashScreenTransitionFinished()
        {
            ChangeState(MainMenuState.MainMenu);
        }

    }
 
    /// <summary>
    /// Determine which state the main menu is in
    /// </summary>
    public enum MainMenuState
    {
        SplashScreen,
        MainMenu,
        BrainpackView,
        ActivityContext,
        RecordingsSelection
    }
}
