/** 
* @file ScrollablePanel.cs
* @brief Contains the ScrollablePanel class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections.Generic;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.UnityUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.View
{
    public class ScrollablePanel : MonoBehaviour
    {
        public Button UpArrow;
        public Button DownArrow;
        public List<ScrollableContent> Contents = new List<ScrollableContent>();
        public ScrollableContent CurrentlySelectedContent { get; set; }
        public GameObject AvailableRecordingButtonPrefab;
        // private bool mImportCompleted;

        private bool mContentLoaded;
        public Scrollbar Scrollbar;

        private Transform mContentPanel;

        private Transform ContentPanel
        {
            get
            {
                if (mContentPanel == null)
                {
                    mContentPanel = SceneObjectFinder<Transform>.FindObjectByName(transform, name + "Group") as Transform;

                }
                return mContentPanel;
            }
        }

        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            UpArrow.onClick.AddListener(UpButtonEngaged);
            DownArrow.onClick.AddListener(DownButtonEngaged);
        }

        public void Show()
        {
            if (!mContentLoaded)
            {
                mContentLoaded = true;
                LoadContent();
            }
            gameObject.SetActive(true);
        }

        /// <summary>
        /// reloads content from the list of content
        /// </summary>
        public void ReloadContent()
        {
            foreach (var vButton in Contents)
            {
                Destroy(vButton.ContentButton.gameObject);
            }
            mContentLoaded = false;
            LoadContent();
        }

        /// <summary>
        /// loads the content from the list of scrollable content
        /// </summary>
        public void LoadContent()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                GameObject vNewAvRecButton = Instantiate(AvailableRecordingButtonPrefab);
                Button vAvRecButton = vNewAvRecButton.GetComponentInChildren<Button>();
                Contents[i].ContentButton = vAvRecButton;
                if (CurrentlySelectedContent.Equals(Contents[i]))
                {
                    if (CurrentlySelectedContent.ContentButton == null)
                    {
                        CurrentlySelectedContent.ContentButton = vAvRecButton;
                    }
                }

                vNewAvRecButton.GetComponentInChildren<Text>().text = Contents[i].Key;

                //copy the variable i and pass it into the listener
                int vTemp = i;
                vAvRecButton.onClick.AddListener(() => SwitchToContent(vTemp));
                vNewAvRecButton.transform.SetParent(ContentPanel, false);
            }
            if (CurrentlySelectedContent != null && CurrentlySelectedContent.ContentButton != null)
            {
                CurrentlySelectedContent.ContentButton.transform.SetAsFirstSibling();


                FadeInFadeOutEffect vFadinOut =
                    CurrentlySelectedContent.ContentButton.GetComponent<FadeInFadeOutEffect>();
                if (vFadinOut != null)
                {
                    vFadinOut.enabled = true;
                }
            }
        }



        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Chooses and plays the recording
        /// </summary>
        /// <param name="vIndex"></param>
        private void SwitchToContent(int vIndex)
        {
            FadeInFadeOutEffect vFadinOut =
                      CurrentlySelectedContent.ContentButton.GetComponent<FadeInFadeOutEffect>();
            if (vFadinOut != null)
            {
                vFadinOut.enabled = false;
            }
            Contents[vIndex].ContentButton.transform.SetAsFirstSibling();
            Contents[vIndex].CallbackAction.Invoke();
            CurrentlySelectedContent = Contents[vIndex];
            CurrentlySelectedContent.ContentButton.transform.SetAsFirstSibling();
            vFadinOut = CurrentlySelectedContent.ContentButton.GetComponent<FadeInFadeOutEffect>();
            if (vFadinOut != null)
            {
                vFadinOut.enabled = true;
            }
        }




        /// <summary>
        /// the up arrow button has been engaged
        /// </summary>
        private void UpButtonEngaged()
        {
            float vNewVal = Scrollbar.value + 0.12f;
            Scrollbar.value = vNewVal;
        }

        /// <summary>
        /// the down arrow button has been engaged
        /// </summary>
        private void DownButtonEngaged()
        {
            float vNewVal = Scrollbar.value - 0.12f;
            Scrollbar.value = vNewVal;
        }
    }
}
