
/**  
* @file DisplayTime.cs 
* @brief Contains the DisplayTime class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/


using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Displays the current time
    /// </summary>
    public class DisplayTime: MonoBehaviour
    {
        public Text TimeText;

        void Awake()
        {
            TimeText = gameObject.GetComponent<Text>();
        }

        void Update()
        {
            TimeText.text = DateTime.Now.ToString("HH:mm:ss.ff tt");
        }
    }
}
