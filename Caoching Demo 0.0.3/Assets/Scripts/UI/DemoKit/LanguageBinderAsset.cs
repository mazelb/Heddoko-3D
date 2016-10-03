  /**
  * @file LanguageBinderAsset.cs
  * @brief Contains the LanguageBinderAsset
  * @author Mohammed Haider( mohammed@heddoko.com)
  * @date October 2016
  * Copyright Heddoko(TM) 2016,  all rights reserved
  */

using System.IO;
using Assets.Scripts.Localization;
using UnityEngine;

namespace Assets.Scripts.UI.DemoKit
{
    public class LanguageBinderAsset : ScriptableObject
    {
        public string Path;
        public string ParsedContent;
        
        public void Init(string vPath)
        {
            Path = vPath;
            var vContents = File.ReadAllText(vPath); 
            ParsedContent = vContents;
        }
        
    }
}