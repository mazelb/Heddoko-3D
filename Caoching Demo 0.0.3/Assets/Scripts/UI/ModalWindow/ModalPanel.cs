
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace Assets.Scripts.UI.ModalWindow
{

    /// <summary>
    /// a simple modal panel
    /// </summary>
    public class ModalPanel : MonoBehaviour
    {

        public Text Question;
        
        public Image IconImage;
        public Button YesButton;
        public Button NoButton;
        public Button CancelButton;
        public GameObject ModalPanelObject;
        public Text TopLabel;
        public Image TopLabelImage;
        public Image PanelImage;

        private static ModalPanel sModalPanel;

        public static ModalPanel Instance()
        {
            if (!sModalPanel)
            {
                sModalPanel = FindObjectOfType<ModalPanel>();
                DontDestroyOnLoad(sModalPanel.gameObject);
            }

            return sModalPanel;
        }

     internal void Awake()
        {
            Instance().ClosePanel(); 
        }
        /// <summary>
        /// Modal Window with three options: yes, no  and cancel
        /// </summary>
        /// <param name="vLabel"></param>
        /// <param name="vQuestion">The Modal question</param> 
        /// <param name="vYesEvent">The event on yes</param>
        /// <param name="vNoEvent">The event on No</param>
        /// <param name="vCancelEvent">The event on cancel</param>
        /// <param name="vTopLabelColor"></param>
        /// <param name="vLabelColor"></param>
        public void Choice(string vLabel,string vQuestion, UnityAction vYesEvent, UnityAction vNoEvent, UnityAction vCancelEvent, Color vTopLabelColor = default(Color), Color vLabelColor = default(Color))
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

            IconImage.gameObject.SetActive(false);
            YesButton.gameObject.SetActive(true);
            NoButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(true);
            Instance().SetColors(vTopLabelColor, vLabelColor);
            Instance().SetLabelText(vLabel);
        }

        /// <summary>
        /// Modal Window with three options: yes, no and cancel and with an image to display
        /// </summary>
        /// <param name="vLabel">The label of the modal panel</param>
        /// <param name="vQuestion">The Modal question</param>
        /// <param name="vIconImage">The image to display</param>
        /// <param name="vYesEvent">The event on yes</param>
        /// <param name="vNoEvent">The event on No</param>
        /// <param name="vCancelEvent">The event on cancel</param>
        /// <param name="vTopLabelColor"></param>
        /// <param name="vLabelColor"></param>
        public void Choice(string vLabel, string vQuestion, Sprite vIconImage, UnityAction vYesEvent, UnityAction vNoEvent, UnityAction vCancelEvent, Color vTopLabelColor = default(Color), Color vLabelColor = default(Color))
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

            IconImage.gameObject.SetActive(true);
            YesButton.gameObject.SetActive(true);
            NoButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(true);
            Instance().SetColors(vTopLabelColor, vLabelColor);
            Instance().SetLabelText(vLabel);
        }

        /// <summary>
        /// A modal window with 2 options, yes and no(closes the window)
        /// </summary>
        /// <param name="vLabel"></param>
        /// <param name="vQuestion">The Modal question</param>
        /// <param name="vYesEvent">The event on yes</param>
        /// <param name="vNoEvent">The event on No</param>
        /// <param name="vTopLabelColor"></param>
        /// <param name="vLabelColor"></param>
        public void Choice(string vLabel, string vQuestion,   UnityAction vYesEvent, UnityAction vNoEvent, Color vTopLabelColor = default(Color), Color vLabelColor = default(Color))
        {
            ModalPanelObject.SetActive(true);

            YesButton.onClick.RemoveAllListeners();
            YesButton.onClick.AddListener(vYesEvent);
            YesButton.onClick.AddListener(ClosePanel);

            NoButton.onClick.RemoveAllListeners();
            NoButton.onClick.AddListener(vNoEvent);
            NoButton.onClick.AddListener(ClosePanel);

            Question.text = vQuestion;
           // IconImage.sprite = vIconImage;

            IconImage.gameObject.SetActive(true);
            YesButton.gameObject.SetActive(true);
            NoButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(false);
            Instance().SetColors(vTopLabelColor, vLabelColor);
            Instance().SetLabelText(vLabel);
        }

        public static void SingleChoice(string vLabel, string vQuestion, UnityAction vOkEvent, Color vTopLabelColor = default(Color) , Color vLabelColor = default(Color))
        {
            
            Instance().ModalPanelObject.SetActive(true);

            Instance().CancelButton.onClick.RemoveAllListeners();
            Instance().CancelButton.onClick.AddListener(Instance().ClosePanel);
            Instance().CancelButton.onClick.AddListener(vOkEvent);

            Instance().Question.text = vQuestion;

            Instance().YesButton.gameObject.SetActive(false);
            Instance().NoButton.gameObject.SetActive(false);
            Instance().CancelButton.gameObject.SetActive(true);
            Instance().SetColors(vTopLabelColor, vLabelColor);
            Instance().SetLabelText(vLabel);
        }

        /// <summary>
        /// Sets the colors of the top label and the content label
        /// </summary>
        /// <param name="vToplabelColor">The color of the top label. Default value is set, will not change colors</param>
        /// <param name="vLabelColor">The color of the main label. Default value is set, will not change colors</param>
        private void SetColors(Color vToplabelColor = default(Color), Color vLabelColor  = default(Color))
        {
            if (vToplabelColor != default(Color))
            {
                TopLabel.color = vToplabelColor;
            }
            if (vLabelColor != default(Color))
            {
                PanelImage.color = vLabelColor;
            }
            
        }

        /// <summary>
        /// Sets the label 
        /// </summary>
        /// <param name="vLabel">the message to set</param>
        private void SetLabelText(string vLabel)
        {
            if (!string.IsNullOrEmpty(vLabel))
            {
                TopLabel.text = vLabel;
            }
        }
        void ClosePanel()
        {
            ModalPanelObject.SetActive(false);
        }
    }
}
