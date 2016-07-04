using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.Tests
{
  public  class NotificationTest : MonoBehaviour
    {
      void Awake()
      {

        }

      void Update()
      {
          if (Input.GetKeyDown(KeyCode.I))
          {

                var message = string.Format("{0} movements have been exported ", 10);
                Notify.Template("FadingFadoutNotifyTemplate").Show(message, 4.5f, hideAnimation: Notify.FadeOutAnimation, showAnimation: Notify.FadeInAnimation, sequenceType: NotifySequence.First);
            }
      }
    }
}
