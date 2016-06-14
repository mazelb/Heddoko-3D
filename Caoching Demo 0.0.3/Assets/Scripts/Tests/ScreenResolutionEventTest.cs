/**
* @file ScreenResolutionEventTest.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class ScreenResolutionEventTest : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ScreenResolutionManager.Instance.TriggerNewResolutionChangeHandler();
            }
        }
    }
}