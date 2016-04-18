/** 
* @file MainMenuBrainpackView.cs
* @brief Contains the MainMenuBrainpackView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Interfaces;
using Assets.Scripts.UI.Scene_3d.View;
using Assets.Scripts.Utils.UnityUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu.View
{
    /// <summary>
    /// Represents the brainpack connection view in the main menu
    /// </summary> 
    public class MainMenuBrainpackView: MonoBehaviour, IBrainpackConnectionView
    {
        public Sprite HalomanConnected;
        public Sprite HalomanConnecting;
        public Sprite HalomanFailure;
        public Image Haloman;
        public Image HaloForHaloman;
        public Button PairButton;
        public Button UnpairButton;
        public Button BackButton;
        
        public FadeInFadeOutEffect FadeInFadeOutEffect;
        public GameObject BrainpackComPortInput;
        public ScrollablePanel ScrollablePanel;
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
            BrainpackConnectionController.Instance.ConnectingStateEvent += OnConnection;
            BrainpackConnectionController.Instance.ConnectedStateEvent += OnConnect;
            BrainpackConnectionController.Instance.DisconnectedStateEvent += OnDisconnect;
            BrainpackConnectionController.Instance.FailedToConnectStateEvent += FailedConnection;
            PairButton.onClick.AddListener(PairButtonEngaged);
            UnpairButton.onClick.AddListener(UnpairButtonEngaged);
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

            Debug.Log("OnConnect");
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
            Debug.Log("FailedConnection");
            UnpairButton.interactable = false;
            UnpairButton.gameObject.SetActive(false);
            PairButton.gameObject.SetActive(true);
            PairButton.interactable = true;
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true);
            HaloForHaloman.sprite = HalomanFailure;
            FadeInFadeOutEffect.FadeEffectTime = 0.5f;
            BrainpackConnectionController.Instance.ResetTries();
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
            Debug.Log("Connect to brainpack");
            BrainpackConnectionController.Instance.ConnectToBrainpack();
        }

        public void UnpairButtonEngaged()
        {
            BrainpackConnectionController.Instance.DisconnectBrainpack();
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
            ScrollablePanel.Show();
            
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
