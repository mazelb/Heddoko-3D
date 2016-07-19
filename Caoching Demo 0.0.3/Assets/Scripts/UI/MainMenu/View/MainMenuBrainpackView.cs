/** 
* @file MainMenuBrainpackView.cs
* @brief Contains the MainMenuBrainpackView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Interfaces;
using Assets.Scripts.UI.Loading;
using Assets.Scripts.Utils.UnityUtilities;
using HeddokoLib.networking;
using HeddokoLib.utils;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu.View
{
    /// <summary>
    /// Represents the brainpack connection view in the main menu
    /// </summary> 
    public class MainMenuBrainpackView : MonoBehaviour, IBrainpackConnectionView
    {
        public Sprite HalomanConnected;
        public Sprite HalomanConnecting;
        public Sprite HalomanFailure;
        public Image Haloman;
        public Image HaloForHaloman;
        public Button PairButton;
        public Button UnpairButton;
        public Button BackButton;
        public SlideBlock SlideBlock;
        public FadeInFadeOutEffect FadeInFadeOutEffect;
        public GameObject BrainpackComPortInput;
        public Dropdown DropDownList;
        private Dictionary<string, string> mDropdownItems;
        public SerialPortConnectionController ConnectionController;
        //   public ScrollablePanel ScrollablePanel;
        /// <summary>
        /// RectTransform associated with this view
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                return GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// On Start, hook listener's into Controller events
        /// </summary> 
        void Start()
        {
            //BrainpackConnectionController.Instance.BpListUpdateHandle += UpdateDropDown;
            //BrainpackConnectionController.Instance.ConnectingStateEvent += OnConnection;
            //BrainpackConnectionController.Instance.ConnectedStateEvent += OnConnect;
            //BrainpackConnectionController.Instance.DisconnectedStateEvent += OnDisconnect;
            //BrainpackConnectionController.Instance.FailedToConnectStateEvent += FailedConnection;
            ConnectionController.BpListUpdateHandle += UpdateDropDown;
            ConnectionController.ConnectingStateEvent += OnConnection;
            ConnectionController.ConnectedStateEvent += OnConnect;
            ConnectionController.DisconnectedStateEvent += OnDisconnect;
            ConnectionController.FailedToConnectStateEvent += FailedConnection;
            PairButton.onClick.AddListener(PairButtonEngaged);
            UnpairButton.onClick.AddListener(UnpairButtonEngaged);
            ScreenResolutionManager.Instance.NewResolutionSetEvent += SlideBlock.ResetPosition;
            DropDownList.onValueChanged.AddListener(ChangeBrainpackComport);
        }

        /// <summary>
        /// Change the brainpack comport
        /// </summary>
        /// <param name="vArg0"></param>
        private void ChangeBrainpackComport(int vArg0)
        {
            List<string> vKeys = mDropdownItems.Keys.ToList();
            if (vKeys.Count > 0)
            {
                var vKey = vKeys[vArg0];
                ConnectionController.BrainpackComPort = mDropdownItems[vKey];
            }

        }

        /// <summary>
        /// Update the dropdown menu
        /// </summary>
        /// <param name="vObj"></param>
        private void UpdateDropDown(Dictionary<string, string> vObj)
        {
            //Stop the loading progress
            DisablingProgressBar.Instance.StopAnimation();
            //Clear the list
            DropDownList.ClearOptions();
            //Verify the number of found items
            if (vObj.Count > 0)
            {
                List<Dropdown.OptionData> vList = new List<Dropdown.OptionData>();
                mDropdownItems = vObj;
                foreach (var vKvPair in vObj)
                {
                    vList.Add(new Dropdown.OptionData(vKvPair.Key));
                }
                DropDownList.options = vList;
                //Set the first item as the default
                var vKey = DropDownList.options[0].text;
                ConnectionController.BrainpackComPort = mDropdownItems[vKey];

                var vMsg =
                    "Found " + vObj.Count + " Battery pack";
                //plural
                vMsg += vObj.Count > 1 ? "s" : string.Empty;
                Notify.Template("fade").Show(
               vMsg,
              customHideDelay: 3f,
              hideAnimation: Notify.AnimationCollapse, clearSequence: true
          );
            }
            else
            {
                var message =
                    "Could not locate any Battery packs. Please try again.";
                Notify.Template("fade").Show(
                 message,
                customHideDelay: 4f,
                hideAnimation: Notify.AnimationCollapse
            );
            }

        }


        public void SearchBrainpacks()
        {
            DisablingProgressBar.Instance.StartProgressBar("SEARCHING FOR BATTERY PACKS");
            BrainpackConnectionController.Instance.DisconnectBrainpack();
            PacketCommandRouter.Instance.Process(this, new HeddokoPacket(HeddokoCommands.GetBrainpackResults, ""));
        }
        /// <summary>
        /// Display the connecting views
        /// </summary> 
        public void OnConnection()
        {
            Debug.Log("OnConnection");
            PairButton.gameObject.SetActive(false);
            UnpairButton.gameObject.SetActive(true);
            UnpairButton.interactable = false;
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true);
            HaloForHaloman.sprite = HalomanConnecting;
            FadeInFadeOutEffect.FadeEffectTime = 2.5f;
        }

        /// <summary>
        /// on connect view
        /// </summary> 
        public void OnConnect()
        {
            UnpairButton.interactable = true;
            UnpairButton.gameObject.SetActive(true);
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true);
            HaloForHaloman.sprite = HalomanConnected;
            FadeInFadeOutEffect.FadeEffectTime = 1.5f;

        }

        /// <summary>
        ///  Display the   Disconnected views
        /// </summary> 
        public void OnDisconnect()
        {
            UnpairButton.interactable = false;
            UnpairButton.gameObject.SetActive(false);
            PairButton.gameObject.SetActive(true);
            PairButton.interactable = true;
            HaloForHaloman.gameObject.SetActive(false);
        }

        /// <summary>
        ///  Display the failed connection views
        /// </summary> 
        public void FailedConnection()
        {
            UnpairButton.interactable = false;
            UnpairButton.gameObject.SetActive(false);
            PairButton.gameObject.SetActive(true);
            PairButton.interactable = true;
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true);
            HaloForHaloman.sprite = HalomanFailure;
            FadeInFadeOutEffect.FadeEffectTime = 0.5f;
            ConnectionController.ResetTries();
            PairButton.gameObject.SetActive(true);
        }

        /// <summary>
        ///  The pairing button has been engaged
        /// </summary>
        public void PairButtonEngaged()
        {

            PairButton.gameObject.SetActive(false);
            UnpairButton.gameObject.SetActive(true);
            UnpairButton.interactable = false;
            ConnectionController.ConnectToBrainpack();
        }

        public void UnpairButtonEngaged()
        {
            ConnectionController.DisconnectBrainpack();
            PairButton.gameObject.SetActive(true);
        }

        public void SetWarningBoxMessage(string vMsg)
        {
            //does nothing
        }

        /// <summary>
        /// Enables and shows the brainpack connection view
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            //   ScrollablePanel.Show();

        }

        /// <summary>
        ///  Disables and hides the brainpack connection view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) ||
                (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.C)))
            {
                BrainpackComPortInput.SetActive(!BrainpackComPortInput.activeInHierarchy);
            }
        }
    }
}
