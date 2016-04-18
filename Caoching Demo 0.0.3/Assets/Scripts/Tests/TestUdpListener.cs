
/** 
* @file TestUdpListener.cs
* @brief Contains the TestUdpListener class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using Assets.Scripts.Communication.Communicators;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Testing the udp listener
    /// </summary>
  public   class TestUdpListener : MonoBehaviour
  {
      private UdpListener mListener;

      private void Awake()
      {
          mListener = new UdpListener();
          mListener.Start();
      }

      void OnApplicationQuit()
      {
            mListener.Stop();

      }
  }
}
