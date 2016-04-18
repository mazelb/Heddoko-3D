using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using UnityEngine;

namespace Assets.Scripts.Tests
{
   public class TemporaryPauserPlayer : MonoBehaviour
   {
       PlaybackControlPanel Panel;

       void Awake()
       {
           Panel = GetComponent<PlaybackControlPanel>();
       }

       void Update()
       {
           if (Input.GetKeyDown(KeyCode.Space))
           {
                Panel.SetPlayState();


           }
       }
   }
}
