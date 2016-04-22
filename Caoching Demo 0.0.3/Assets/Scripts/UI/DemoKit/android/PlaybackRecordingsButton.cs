

/* @file PlaybackRecordingsButton.cs
* @brief Contains the PlaybackRecordingsButton class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.DemoKit.android
{
    /// <summary>
    /// A button that controls playback. ONLY IN DEMO
    /// </summary>
   public class PlaybackRecordingsButton : MonoBehaviour
   {
       public BodyFrameRecordingAsset RecordingAsset;
       public Button Button;
       public Action<string[]> Playaction ;

       public void Awake()
       {
           Button = GetComponent<Button>();
       }
       public void InitializeActions(Action<string[]> vAction)
       {
           Playaction = vAction;
            Button.onClick.AddListener(()=> Playaction.Invoke(RecordingAsset.Lines));
       }
   }
}
