// /**
// * @file MakeLanguageAsset.cs
// * @brief Contains the MakeLanguageAsset class
// * @author Mohammed Haider(mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.IO;
using Assets.Scripts.UI.DemoKit;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class MakeLanguageAsset : EditorWindow
    {
        [MenuItem("Window/Asset Creation/Create Json Language Asset")]
        static void Init()
        {
            Startnt();
        }
        public static void Startnt()
        {
            LanguageBinderAsset vLanguageAsset = ScriptableObject.CreateInstance<LanguageBinderAsset>();

            string vPath = LaunchWindow();
            if (vPath.Length > 0)
            {

                vLanguageAsset.Init(vPath);
                EditorUtility.SetDirty(vLanguageAsset);
                //Get the file name
                FileInfo vFileInfo = new FileInfo(vPath);

                AssetDatabase.CreateAsset(vLanguageAsset, "Assets/Resources/ScriptableAssets/" + vFileInfo.Name + ".asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                Selection.activeObject = vLanguageAsset;
                EditorUtility.SetDirty(vLanguageAsset);
            }



        }

        private static string LaunchWindow()
        {
            return EditorUtility.OpenFilePanel("Select language binder file", "", "binder");


        }
    }
}