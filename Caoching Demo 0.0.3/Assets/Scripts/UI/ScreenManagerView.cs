/**
* @file ScreenManagerView.cs
* @brief Contains the 
* @author Mohammed Haider( 
* @date 05 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections; 
using UIWidgets;
using UnityEngine; 
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ScreenManagerView : MonoBehaviour
    { 
        public GraphicsQualityManager GraphicsQualityManager;
        public Dropdown ResolutionDropDown;
        public Dropdown GraphicsDropDown;
        public SlideBlock CurrSlideBlock;
        public Toggle VSyncToggle;
 
        void Awake()
        {
            CurrSlideBlock = GetComponent<SlideBlock>();
            ScreenResolutionManager.Instance.AllResolutionsScannedEvent += UpdateResolutionDropdown;
            ScreenResolutionManager.Instance.NewResolutionSetEvent += RedrawSlideBlock;
            UpdateResolutionDropdown(ScreenResolutionManager.GetAllSupportedResolution());
            //set the resolution in the dropdown
            ResolutionDropDown.value = ScreenResolutionManager.GetCurrentResolutionIndex(); 
            ResolutionDropDown.onValueChanged.AddListener(ScreenResolutionManager.SelectResolutionId);
            GraphicsQualityManager.AvailableGraphicsQualityScannedEvent += SetGraphicsDropdownValues;
            VSyncToggle.isOn = GraphicsQualityManager.VsyncOn;
            VSyncToggle.onValueChanged.AddListener((x) =>
            {
                int vSyncCount = x ? 1 : 0;
                GraphicsQualityManager.SetVsync(vSyncCount);
            });
             
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vWidth"></param>
        /// <param name="vHeight"></param>
        /// <param name="vIsFullscreen"></param>
        /// <param name="vRefreshRate"></param>
        private void RedrawSlideBlock()
        {
            StartCoroutine(ResetResSlideBlock());
            StartCoroutine(ResetResSlideBlock());

        }

        public IEnumerator ResetResSlideBlock()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            CurrSlideBlock.ResetPosition();
        }

        void SetGraphicsDropdownValues()
        {
            GraphicsDropDown.options.Clear();
            string[] vNames = QualitySettings.names;
            for (int vI = 0; vI < vNames.Length; vI++)
            {
                string vGraphicsData = vNames[vI];
                Dropdown.OptionData vOptionData = new Dropdown.OptionData(vGraphicsData);
                GraphicsDropDown.options.Add(vOptionData);
            }
            GraphicsDropDown.onValueChanged.RemoveAllListeners();
            GraphicsDropDown.onValueChanged.AddListener(x => GraphicsQualityManager.SetNewQualitySetting(x));

        }

       
        /// <summary>
        /// Updates the resolution drop down view
        /// </summary>
        /// <param name="vResolutions"></param>
        private void UpdateResolutionDropdown(Resolution[] vResolutions)
        {
            //Clear all options
            ResolutionDropDown.options.Clear();
            for (int vI = 0; vI < vResolutions.Length; vI++)
            {
                string vResolutionData = vResolutions[vI].width + " X " + vResolutions[vI].height;
                Dropdown.OptionData vOptionData = new Dropdown.OptionData(vResolutionData);
                ResolutionDropDown.options.Add(vOptionData);
            }
        }

       
        
    }
}