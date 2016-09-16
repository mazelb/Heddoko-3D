// /**
// * @file LocalRecordingView.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    public delegate void RecordingFileSelectedDelegate(string vRecordingFilePath);
    public class LocalRecordingView : MonoBehaviour
    {
        public event RecordingFileSelectedDelegate RecFileSelectedEvent;
        public RectTransform Bounds;
        public Canvas RenderingCanvas;
        public Camera RenderingCam;
        private Rect mRectBoundaries;

        /// <summary>
        /// Disables exploration view
        /// </summary>  
        public void Hide()
        {
            StopCoroutine(ShowAfterThreeFrames());
            UniFileBrowser.use.enabled = false;
            UniFileBrowser.use.CloseFileWindow();
        }

        /// <summary>
        /// on enable, enable the explorer panel
        /// </summary>
        private void OnEnable()
        {
            if (RenderingCam.isActiveAndEnabled)
            {
                Show();
            }
        }

        /// <summary>
        /// on disable, disable the explorer panel
        /// </summary>
        private void OnDisable()
        {
            UniFileBrowser.use.enabled = false;
            UniFileBrowser.use.CloseFileWindow();
        }

        /// <summary>
        /// Enables exploration view
        /// </summary>
        public void Show()
        {
            StartCoroutine(ShowAfterThreeFrames());
        }

        IEnumerator ShowAfterThreeFrames()
        { 
            yield return new WaitForEndOfFrame();
            UniFileBrowser.use.enabled = true;
            SetTransform();
        }

        private void SetTransform()
        {
            var vPaths = new[] { "dat", "hsm" };
            //initialize the browser settings
#if DEBUG  
            vPaths = new[] { "csv", "dat", "hsm" };
#endif
            UniFileBrowser.use.SetFileExtensions(vPaths);
            mRectBoundaries = RectTransformExtension.GetScreenRect(Bounds, RenderingCanvas);
            UniFileBrowser.use.showVolumes = true;
            UniFileBrowser.use.SetFileWindowSize(mRectBoundaries.size);
            UniFileBrowser.use.SetFileWindowPosition(mRectBoundaries.position);
            UniFileBrowser.use.allowWindowDrag = false;
            UniFileBrowser.use.allowWindowResize = false;
            UniFileBrowser.use.allowMultiSelect = false;
            UniFileBrowser.use.OpenFileWindow(SelectRecordingFile);
            UniFileBrowser.use.ErrorHandler -= ErrorHandler;
            UniFileBrowser.use.ErrorHandler += ErrorHandler;
            UniFileBrowser.use.allowWindowClose = false;
            UniFileBrowser.use.ShowCancelButton = false;
        }

        private void ErrorHandler(string vError)
        {
            UniFileBrowser.use.enabled = false;
            ModalWindow.ModalPanel.SingleChoice("ERROR", vError, () => { UniFileBrowser.use.enabled = true; });
        }
        /// <summary>
        /// callback on a recording file that was selected
        /// </summary>
        /// <param name="vRecordingSelected"></param>
        private void SelectRecordingFile(string vRecordingSelected)
        {
            if (RecFileSelectedEvent != null)
            {
                RecFileSelectedEvent(vRecordingSelected);
            }
            Hide();
        }
    }


    public static class RectTransformExtension
    {
        /// <summary>
        /// get a rect transform's rect in screen space
        /// </summary>
        /// <param name="vRectTransform"></param>
        /// <param name="vCanvas"></param>
        /// <returns></returns>
        public static Rect GetScreenRect(RectTransform vRectTransform, Canvas vCanvas)
        {

            Vector3[] vCorners = new Vector3[4];
            Vector3[] vScreenCorners = new Vector3[2];

            vRectTransform.GetWorldCorners(vCorners);

            if (vCanvas.renderMode == RenderMode.ScreenSpaceCamera || vCanvas.renderMode == RenderMode.WorldSpace)
            {
                vScreenCorners[0] = RectTransformUtility.WorldToScreenPoint(vCanvas.worldCamera, vCorners[1]);
                vScreenCorners[1] = RectTransformUtility.WorldToScreenPoint(vCanvas.worldCamera, vCorners[3]);
            }
            else
            {
                vScreenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, vCorners[1]);
                vScreenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, vCorners[3]);
            }

            vScreenCorners[0].y = Screen.height - vScreenCorners[0].y;
            vScreenCorners[1].y = Screen.height - vScreenCorners[1].y;

            return new Rect(vScreenCorners[0], vScreenCorners[1] - vScreenCorners[0]);
        }
    }

}