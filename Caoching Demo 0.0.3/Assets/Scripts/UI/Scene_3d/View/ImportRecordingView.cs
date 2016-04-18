/**  
* @file ImportRecordingView.cs 
* @brief Contains the ImportRecordingView class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/ 

using Assets.Scripts.UI.MainScene.Model;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.View
{
    /// <summary>
    /// The Import Recording view , displays the numbers of recordings available
    /// </summary>
    public class ImportRecordingView : MonoBehaviour, IInGameMenuItem
    {
        public GameObject AvailableRecordingPanel;
        public GameObject AvailableRecordingButtonPrefab; 
        private bool mImportCompleted { get; set; }
        /// <summary>
        /// hides the current view from the scene(the button remains active)
        /// </summary>
        public void Hide()
        {
            AvailableRecordingPanel.SetActive(false); 
        }
        public Button CurrentButton
        {
            get
            {
                return gameObject.GetComponent<Button>();
            }
        }
        /// <summary>
        /// Shows the view in the scene 
        /// </summary>
        public void Show()
        {
            if (!mImportCompleted)
            {
                Transform vContentPanel = SceneObjectFinder<Transform>.FindObjectByName(AvailableRecordingPanel.transform, "ScrollView") as Transform;
                string[] vRecordingsFiles = BodyRecordingsMgr.Instance.FilePaths;
                if (vRecordingsFiles == null)
                {
                    BodyRecordingsMgr.Instance.ScanRecordings(Application.dataPath+"/Resources/Recordings");
                    vRecordingsFiles = BodyRecordingsMgr.Instance.FilePaths;
                }
                if (vContentPanel != null)
                {
                    vContentPanel = vContentPanel.GetChild(0);
                    for (int i = 0; i < vRecordingsFiles.Length; i++)
                    {

                        GameObject vNewAvRecButton = Instantiate(AvailableRecordingButtonPrefab);
                        Button vAvRecButton = vNewAvRecButton.GetComponent<Button>();
                        string vCleanedName = vRecordingsFiles[i].Replace(Application.dataPath + "/Resources/Recordings" + "\\", null);
                        vAvRecButton.GetComponentInChildren<Text>().text = vCleanedName;
                        //copy the variable i and pass it into the listener
                        int vTemp = i; 
                        vAvRecButton.onClick.AddListener(() => ChooseRecording(vTemp));
                        vNewAvRecButton.transform.SetParent(vContentPanel, false);
                    }
                }
                mImportCompleted = true;
            }
            gameObject.SetActive(true);
            AvailableRecordingPanel.SetActive(true);
        }
        /// <summary>
        /// Select the recording
        /// </summary>
        /// <param name="vIndex"></param>
        public void ChooseRecording(int vIndex)
        { 
           BodySelectedInfo.Instance.UpdateSelectedRecording(vIndex);
        }
    }
}
