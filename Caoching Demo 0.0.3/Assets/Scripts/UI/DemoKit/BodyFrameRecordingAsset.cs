
/* @file BodyFrameRecordingAsset.cs
* @brief Contains the BodyFrameRecordingAsset class
* @author Mohammed Haider(mohammed @heddoko.com)
* @date  April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
 
using UnityEngine;

namespace Assets.Scripts.UI.DemoKit
{ 
    public class BodyFrameRecordingAsset : ScriptableObject
    {
        public string Path; 
        [SerializeField]
        private string[] mLines;

        public string[] Lines
        {
            get { return mLines; }
            set { mLines = value; }
        }

        public void Init(string vPath)
        {
            Path = vPath;
            //BodyRecordingsMgr.Instance.ReadRecordingFile(Path, RecordingAddCallback);

            BodyRecordingReader vTempReader = new BodyRecordingReader(vPath);
            vTempReader.IsFromDatFile = false;
           Debug.Log(vTempReader.ReadFile(vPath));
            Lines = vTempReader.GetRecordingLines(); 
         
        }
 


    }
}