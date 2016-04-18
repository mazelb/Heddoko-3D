/** 
* @file BrainpackConnectionView.cs
* @brief Contains the BrainpackConnectionView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Interfaces;
using Assets.Scripts.UI; 
using Assets.Scripts.Utils.UnityUtilities;
using Assets.Scripts.Utils.UnityUtilities.Repos;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Communication.View
{
    /// <summary>
    /// Represents the brainpack connection view in scene
    /// </summary> 
    public class BrainpackConnectionView: MonoBehaviour, IBrainpackConnectionView
    { 
        public Sprite HalomanConnected;
        public Sprite HalomanConnecting;
        public Sprite HalomanFailure;
        public Image Haloman;
        public Image HaloForHaloman;
        public Button PairButton;
        public Button UnpairButton;
        private WarningBoxView mWarningBoxView;
        public FadeInFadeOutEffect FadeInFadeOutEffect;

        /// <summary>
        /// Returns the WarningBoxView of this current view
        /// </summary> 
        private WarningBoxView WarningBox
        {
            get
            {
                if (mWarningBoxView == null)
                {
                    //find it the warning box in the scene
                    mWarningBoxView = FindObjectOfType<WarningBoxView>();
                    if (mWarningBoxView == null)
                    {
                        GameObject vWarningBoxPanel = Instantiate(PrefabRepo.WarningIconPanelPrefab);
                        vWarningBoxPanel.SetActive(true);
                        //get the current transforms child index
                        int vChildIndex = transform.GetSiblingIndex();
                        //get the parent transform 
                        Transform vParent = transform.parent;
                        vWarningBoxPanel.transform.SetParent(vParent, false);
                        vWarningBoxPanel.transform.SetSiblingIndex(vChildIndex + 1);
                        mWarningBoxView = vWarningBoxPanel.GetComponent<WarningBoxView>();
                        vWarningBoxPanel.SetActive(false);
                    }
                }
                return mWarningBoxView;
            }
        }

        /// <summary>
        /// Returns the RectTransform associated with this view
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
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            BrainpackConnectionController.Instance.ConnectingStateEvent += OnConnection;
            BrainpackConnectionController.Instance.ConnectedStateEvent += OnConnect;
            BrainpackConnectionController.Instance.DisconnectedStateEvent += OnDisconnect;
            BrainpackConnectionController.Instance.FailedToConnectStateEvent += FailedConnection;
            PairButton.onClick.AddListener(PairButtonEngaged);
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        ///  Display the failed connection views
        /// </summary> 
        public void FailedConnection()
        {
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true);
            HaloForHaloman.sprite = HalomanFailure;
            FadeInFadeOutEffect.FadeEffectTime = 0.5f;
            BrainpackConnectionController.Instance.ResetTries();
            PairButton.gameObject.SetActive(true);
            WarningBox.Show(); 
        }

        /// <summary>
        ///  Display the   Disconnected views
        /// </summary> 
        public void OnDisconnect()
        {
            HaloForHaloman.gameObject.SetActive(false);
            WarningBox.Show();
        }

        /// <summary>
        /// Display the connecting views
        /// </summary> 
        public void OnConnection()
        {
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
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true);
            HaloForHaloman.sprite = HalomanConnected;
            FadeInFadeOutEffect.FadeEffectTime = 1.5f; 
        }

        /// <summary>
        /// Enables and shows the brainpack connection view
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        ///  Disables and hides the brainpack connection view
        /// </summary>
        public void Hide()
        {
            FadeInFadeOutEffect.enabled = false;
            HaloForHaloman.enabled = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        ///  The pairing button has been engaged
        /// </summary>
        public void PairButtonEngaged()
        {
            BrainpackConnectionController.Instance.ConnectToBrainpack();
            PairButton.gameObject.SetActive(false);
            WarningBox.Hide();
        }

        /// <summary>
        /// Sets the warning box text to the passed in param
        /// </summary>
        /// <param name="vMsg">The message to change to </param>
        public void SetWarningBoxMessage(string vMsg)
        {
            WarningBox.WarningText.text= vMsg;
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            WarningBox.WarningText.text.Replace("\\n", "\n");
        }


 
    }
}
