
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.UI.ModalWindow
{

    //  This script will be updated in Part 2 of this 2 part series.
    public class ModalPanel : MonoBehaviour
    {

        public Text Question;
        public Image IconImage;
        public Button YesButton;
        public Button NoButton;
        public Button CancelButton;
        public GameObject ModalPanelObject;

        private static ModalPanel mModalPanel;

        public static ModalPanel Instance()
        {
            if (!mModalPanel)
            {
                mModalPanel = FindObjectOfType<ModalPanel>();
                DontDestroyOnLoad(mModalPanel.gameObject);
            }

            return mModalPanel;
        }

        /// <summary>
        /// Modal Window with three options: yes, no  and cancel
        /// </summary>
        /// <param name="vQuestion">The Modal question</param> 
        /// <param name="vYesEvent">The event on yes</param>
        /// <param name="vNoEvent">The event on No</param>
        /// <param name="vCancelEvent">The event on cancel</param>
        public void Choice(string vQuestion, UnityAction vYesEvent, UnityAction vNoEvent, UnityAction vCancelEvent)
        {
            ModalPanelObject.SetActive(true);

            YesButton.onClick.RemoveAllListeners();
            YesButton.onClick.AddListener(vYesEvent);
            YesButton.onClick.AddListener(ClosePanel);

            NoButton.onClick.RemoveAllListeners();
            NoButton.onClick.AddListener(vNoEvent);
            NoButton.onClick.AddListener(ClosePanel);

            CancelButton.onClick.RemoveAllListeners();
            CancelButton.onClick.AddListener(vCancelEvent);
            CancelButton.onClick.AddListener(ClosePanel);

            this.Question.text = vQuestion;

            this.IconImage.gameObject.SetActive(false);
            YesButton.gameObject.SetActive(true);
            NoButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(true);
        }
        /// <summary>
        /// Modal Window with three options: yes, no and cancel and with an image to display
        /// </summary>
        /// <param name="vQuestion">The Modal question</param>
        /// <param name="vIconImage">The image to display</param>
        /// <param name="vYesEvent">The event on yes</param>
        /// <param name="vNoEvent">The event on No</param>
        /// <param name="vCancelEvent">The event on cancel</param>
        public void Choice(string vQuestion, Sprite vIconImage, UnityAction vYesEvent, UnityAction vNoEvent, UnityAction vCancelEvent)
        {
            ModalPanelObject.SetActive(true);

            YesButton.onClick.RemoveAllListeners();
            YesButton.onClick.AddListener(vYesEvent);
            YesButton.onClick.AddListener(ClosePanel);

            NoButton.onClick.RemoveAllListeners();
            NoButton.onClick.AddListener(vNoEvent);
            NoButton.onClick.AddListener(ClosePanel);

            CancelButton.onClick.RemoveAllListeners();
            CancelButton.onClick.AddListener(vCancelEvent);
            CancelButton.onClick.AddListener(ClosePanel);

            Question.text = vQuestion;
            IconImage.sprite = vIconImage;

            this.IconImage.gameObject.SetActive(true);
            YesButton.gameObject.SetActive(true);
            NoButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(true);
        }
        /// <summary>
        /// A modal window with 2 options, yes and no(closes the window)
        /// </summary>
        /// <param name="vQuestion">The Modal question</param>
        /// <param name="vIconImage">The image to display</param>
        /// <param name="vYesEvent">The event on yes</param>
        /// <param name="vNoEvent">The event on No</param>
        public void Choice(string vQuestion, Sprite vIconImage, UnityAction vYesEvent, UnityAction vNoEvent)
        {
            ModalPanelObject.SetActive(true);

            YesButton.onClick.RemoveAllListeners();
            YesButton.onClick.AddListener(vYesEvent);
            YesButton.onClick.AddListener(ClosePanel);

            NoButton.onClick.RemoveAllListeners();
            NoButton.onClick.AddListener(vNoEvent);
            NoButton.onClick.AddListener(ClosePanel);

            Question.text = vQuestion;
            IconImage.sprite = vIconImage;

            IconImage.gameObject.SetActive(true);
            YesButton.gameObject.SetActive(true);
            NoButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(false);
        }

        public static void SingleChoice(string vQuestion, UnityAction vOkEvent)
        {
            Instance().ModalPanelObject.SetActive(true);

            Instance().CancelButton.onClick.RemoveAllListeners();
            Instance().CancelButton.onClick.AddListener(Instance().ClosePanel);
            Instance().CancelButton.onClick.AddListener(vOkEvent);

            Instance().Question.text = vQuestion;

            Instance().YesButton.gameObject.SetActive(false);
            Instance().NoButton.gameObject.SetActive(false);
            Instance().CancelButton.gameObject.SetActive(true);
        }
        void ClosePanel()
        {
            ModalPanelObject.SetActive(false);
        }
    }
}
