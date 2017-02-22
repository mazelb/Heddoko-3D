// /**
// * @file LocalizationBinderLoader.cs
// * @brief Contains the LocalizationBinderLoader class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
 
using Assets.Scripts.UI.DemoKit;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.Localization
{
    /// <summary>
    /// Loads a language asset on startup into the LocalizationBinderContainer singleton
    /// </summary>
    public class LocalizationBinderLoader : MonoBehaviour
    {
        public LanguageBinderAsset LanguageBinderAsset;

        void Start()
        {
            if (LanguageBinderAsset != null)
            {
             var vBinder =  Utils.JsonUtilities.StringToObj<LocalizationBinderContainer>(LanguageBinderAsset.ParsedContent);
                LocalizationBinderContainer.SetInstance(vBinder);
            }
            else
            {
                string vMsg = "NULL LANG BINDING";
                Notify.Template("fade")
              .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true); 
            }
        }
    }
}