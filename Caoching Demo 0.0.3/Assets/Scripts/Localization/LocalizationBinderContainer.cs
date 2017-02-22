// /**
// * @file LocalizationBinderContainer.cs
// * @brief Contains the LocalizationBinderContainer class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date September 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;

namespace Assets.Scripts.Localization
{
    /// <summary>
    /// A container for different localization binders
    /// </summary>
    public class LocalizationBinderContainer
    {

        public Dictionary<string, LocalizationBinder> mContainer = new Dictionary<string, LocalizationBinder>();
        private string mCurrentKey = "En-Us";
        private static LocalizationBinderContainer sInstance;

        private static LocalizationBinderContainer Instance
        {
            get
            {
                if (sInstance == null)
                { 
                    sInstance = new LocalizationBinderContainer();

                }
                return sInstance;
            }
             set { sInstance = value; }
        }

        public static void SetInstance(LocalizationBinderContainer vContainer)
        {
            Instance = vContainer;
        }

        /// <summary>
        /// Set language
        /// </summary>
        /// <param name="vKey"></param>
        public static void SetLanguage(string vKey)
        {
            Instance.mCurrentKey = vKey;
        }
        
        /// <summary>
        /// returns a localized string based on the current language set.
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vIsPlural"></param>
        /// <returns></returns>
        public static string GetString(KeyMessage vKey, bool vIsPlural=false)
        {
            if (string.IsNullOrEmpty(Instance.mCurrentKey))
            {
                return "NULL";
            }

            return Instance.mContainer[Instance.mCurrentKey].GetString(vKey, vIsPlural); 
        }
    }
 
    
}