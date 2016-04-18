
/** 
* @file SuitSocketClientSettings.cs
* @brief Contains the SuitSocketClientSettings class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Communication
{
    /**
    * Body class 
    * @brief SuitSocketClientSettings class, settings that will allow a socket connection to connect to the 
    * windows brainpack service 
    * Todo: set the port settings in player prefs-> player prefs talk directly with the windows registry. More can be found here
    http://docs.unity3d.com/ScriptReference/PlayerPrefs.html
    */
    [Serializable]
    public class SuitSocketClientSettings : ISocketClientSetting
    {
        [SerializeField]
        private string mConnectionName = "localhost";
        [SerializeField]
        private string mServerIp = "127.0.0.1";
        //local host and  server ip should not be changed and discarded. Since, this class is implementing
        //ISocketClientSetting, it needs to use a setter that will store its value in to ignoredValue, leaving 
        //mConnectionName and mServerIP intact. Port can be changed.
        private string mIgnoredValue;
        [SerializeField]
        private int mPort = 11000; 

        public string ConnectionName
        {
            get
            {
              return mConnectionName;  
            }
            set
            {
                mIgnoredValue = value;
            }
        }

        public string ServerIp {
            get
            {
                return mServerIp;
            }
            set
            {
                mIgnoredValue = value;
            }
        }
        public int Port {
            get { return mPort; }
            set { mPort = value; }
        }
    }
}
