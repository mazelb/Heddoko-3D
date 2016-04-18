/** 
* @file SettingsView.cs
* @brief Contains the SettingsView abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews;
using Kender.uGUI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{
    /// <summary>
    /// View for a settings panel in scene
    /// </summary>
    public class SettingsView : AbstractView
    {
        public Button SettingsButton;
        public RectTransform Panel;
        public ComboBox BrainpackComboBox;
        public SetRecordingPrefix SetRecordingPrefix;
        public Text StatusTextBox;

        public Button ClearTextButton;
        public Button SetBrainpackTimeButton;
        public Button GetBrainpackVersionButton;
        public Button ResetBrainpackButton;
        public Button PowerOffButton;
        public Button QuitApplicationButton;
        public Button CloseButton;
       // public Button ClearStatusText;
        private float mSetTimer = 10f;
        private float mTimer;
        /// <summary>
        /// Items that are depended on the state of the connection to the brainpack
        /// </summary>
        public Selectable[] BrainpackDependentItems;

        private void Awake()
        {
            mTimer = mSetTimer;
            BrainpackConnectionController.Instance.ConnectedStateEvent += () => AllowSelectableItemInteraction(true);
            BrainpackConnectionController.Instance.DisconnectedStateEvent += () => AllowSelectableItemInteraction(false);
            
            SetBrainpackTimeButton.onClick.AddListener(SetTimeCommand);
            GetBrainpackVersionButton.onClick.AddListener(GetBrainpackVersionCommand);
            ResetBrainpackButton.onClick.AddListener(ResetBrainpackCommand);
            PowerOffButton.onClick.AddListener(PowerOffBrainpackCommand);
            ClearTextButton.onClick.AddListener(() =>
            {
                StatusTextBox.text = "";
            });
            QuitApplicationButton.onClick.AddListener(Application.Quit);
            AllowSelectableItemInteraction(false);
            StatusTextBox.text = "";
        }

        public void UpdateStatusText(string vMsg)
        {
            StatusTextBox.text += vMsg + "\n";
        }
        /// <summary>
        /// Sends the command to power off the brainpack
        /// </summary>
        private void PowerOffBrainpackCommand()
        {
            BrainpackConnectionController.Instance.PowerOffBrainpackCmd();
        }

        /// <summary>
        /// Send a command to reset the brainpack
        /// </summary>
        private void ResetBrainpackCommand()
        {
            BrainpackConnectionController.Instance.InitiateSuitReset();
        }

        /// <summary>
        /// sends a command to get the brainpack version
        /// </summary>
        private void GetBrainpackVersionCommand()
        {
            BrainpackConnectionController.Instance.GetBrainpackVersCmd();
            ResetRequestResponseTimer();
        }

        /// <summary>
        /// Sends the command to set the time
        /// </summary>
        private void SetTimeCommand()
        {
            BrainpackConnectionController.Instance.SetBrainpackTimeCmd();
        }

        /// <summary>
        /// sends the command to get the current brainpack state
        /// </summary>
        private void GetStateCommand()
        {
            BrainpackConnectionController.Instance.GetBrainpackStateCmd();
            ResetRequestResponseTimer();
        }

        public override void Hide()
        {
            SettingsButton.interactable = true;
            Panel.gameObject.SetActive(false);
        }


        public override void Show()
        {
            SettingsButton.interactable = false;
            Panel.gameObject.SetActive(true);
            bool vBPConnected = BrainpackConnectionController.Instance.ConnectionState ==
                                BrainpackConnectionState.Connected;
            AllowSelectableItemInteraction(vBPConnected);
        }

        public override void CreateDefaultLayout()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Allows the interaction of the BrainpackDependentItems 
        /// </summary>
        /// <param name="vFlag"> </param>
        private void AllowSelectableItemInteraction(bool vFlag)
        {
            foreach (Selectable vBrainpackDependentItem in BrainpackDependentItems)
            {
                vBrainpackDependentItem.interactable = vFlag;
            }
            SetRecordingPrefix.SetInteraction(vFlag);
        }
        /// <summary>
        /// Resets the increment request timer
        /// </summary>
        private void ResetRequestResponseTimer()
        {
            mTimer = mSetTimer;
        }
 

       
    }
}
