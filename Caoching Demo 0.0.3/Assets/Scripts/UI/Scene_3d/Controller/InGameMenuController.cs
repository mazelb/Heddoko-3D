/** 
* @file InGameMenuController.cs
* @brief Contains the InGameMenuController class and the GameMenuState enum
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.Scene_3d.View;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.Controller
{
    /// <summary>
    /// The controller for the in game menu view
    /// </summary> 

    /**
    * InGameMenuController class
    * @brief The controller for the in game menu view 
    */

    public  class InGameMenuController : MonoBehaviour
    {
        private GameMenuState mCurrentState = GameMenuState.MenuOff;
        public GameMenuView MainMenu;
        public GameObject LoadedRecordingsPanel;
        public ConnectToBrainpackView ConnectToBrainPackPanel;
        public ImportRecordingView ImportRecordingView;
        public Button PlayRecordingButton;
        public Button ResetInitFrameButton;
        public Button ShowMenuButton;
        public Button ExitMenuButton;

        /// <summary>
        /// On start, update the buttons listeners to appropriate function in InGameMenuController
        /// </summary> 
        /**
        * Start()
        * @brief On start, update the buttons listeners to appropriate function in InGameMenuController 
        */
        public void Start()
        {
            ConnectToBrainPackPanel.CurrentButton.onClick.AddListener(() => ChangeState((int)GameMenuState.ConnectToBrainPack));
            ImportRecordingView.CurrentButton.onClick.AddListener(() => ChangeState((int)GameMenuState.ImportRecordings));
            ShowMenuButton.onClick.AddListener(()=> ChangeState((int)GameMenuState.Default));
            ExitMenuButton.onClick.AddListener(() => ChangeState((int)GameMenuState.MenuOff));
        }

        /// <summary>
        /// Changes the current state of the menu
        /// </summary>
        /// <param name="vNewState">The state to set the InGameMenuController</param>
        /**
        * ChangeState(int vNewState)
        * @brief Changes the current state of the menu
        * @param  int vNewState: The state to set the InGameMenuController
        */
        public void ChangeState(int vNewState)
        {
            switch (mCurrentState)
            {
                case GameMenuState.MenuOff:
                    if (vNewState == (int)GameMenuState.Default)
                    {
                        mCurrentState = GameMenuState.Default;
                        ShowMenuButton.gameObject.SetActive(false);
                        ExitMenuButton.gameObject.SetActive(true);
                        MainMenu.Show();
                        ImportRecordingView.Hide();
                        ConnectToBrainPackPanel.Hide();
                    }

                    PlayRecordingButton.interactable = false; 
                    ResetInitFrameButton.interactable = false;
                    break;
                case GameMenuState.ImportRecordings:
                    if (vNewState == (int)GameMenuState.ImportRecordings)
                    {
                        break;
                    }
                    if (vNewState == (int)GameMenuState.ConnectToBrainPack)
                    {
                        ImportRecordingView.Hide();
                        ConnectToBrainPackPanel.Show();
                        mCurrentState = GameMenuState.ConnectToBrainPack;
                        break;
                    }
                    if (vNewState == (int)GameMenuState.MenuOff)
                    {
                        ImportRecordingView.Hide();
                        MainMenu.Hide();
                        mCurrentState = GameMenuState.MenuOff;
                        PlayRecordingButton.interactable = true;
                        ResetInitFrameButton.interactable = true;
                        ExitMenuButton.gameObject.SetActive(false);
                        ShowMenuButton.gameObject.SetActive(true);
                        break;
                    }
                    break;
                case GameMenuState.ConnectToBrainPack:
                    if (vNewState == (int)GameMenuState.ConnectToBrainPack)
                    {
                        break;
                    }
                    if (vNewState == (int)GameMenuState.ImportRecordings)
                    {
                        ConnectToBrainPackPanel.Hide();
                        ImportRecordingView.Show();
                        mCurrentState = GameMenuState.ImportRecordings;
                        break;
                    }
                    if (vNewState == (int)GameMenuState.MenuOff)
                    {
                        ConnectToBrainPackPanel.Hide();
                        MainMenu.Hide();
                        mCurrentState = GameMenuState.MenuOff;
                        PlayRecordingButton.interactable = true;
                        ResetInitFrameButton.interactable = true;
                        ExitMenuButton.gameObject.SetActive(false); 
                        ShowMenuButton.gameObject.SetActive(true);
                        break;
                    }
                    break;
                case GameMenuState.Default:
                    if (vNewState == (int)GameMenuState.Default)
                    {
                        break;
                    }
                    if (vNewState == (int)GameMenuState.ConnectToBrainPack)
                    {
                        ConnectToBrainPackPanel.Show();
                        mCurrentState = GameMenuState.ConnectToBrainPack;
                        break;
                    }
                    if (vNewState == (int)GameMenuState.ImportRecordings)
                    {
                        ImportRecordingView.Show();
                        mCurrentState = GameMenuState.ImportRecordings;
                        break;
                    }
                    if (vNewState == (int)GameMenuState.MenuOff)
                    {
                        MainMenu.Hide();
                        mCurrentState = GameMenuState.MenuOff;
                        PlayRecordingButton.interactable = true;
                        ResetInitFrameButton.interactable = true;
                        ExitMenuButton.gameObject.SetActive(false); 
                        ShowMenuButton.gameObject.SetActive(true);
                        break;
                    }
                    break;
            }
        } 
    }
    /// <summary>
    /// The state of the Menu
    /// </summary>
    public enum GameMenuState
    {
        MenuOff = 0,
        ImportRecordings = 1,
        ConnectToBrainPack = 2,
        GameMenuStateCount = 3,
        Default = 4
    }
}
