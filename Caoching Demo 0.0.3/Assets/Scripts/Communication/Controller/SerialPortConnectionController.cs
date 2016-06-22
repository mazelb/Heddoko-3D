// /**
// * @file SerialPortConnectionController.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 06 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using System.IO.Ports;
using Assets.Scripts.Frames_Pipeline.BodyFrameConversion;
using HeddokoLib.heddokoProtobuff.Decoder;
using UnityEngine;

namespace Assets.Scripts.Communication.Controller
{
    public abstract class SerialPortConnectionController:AbstractSuitConnection
    {
        public System.Action<Dictionary<string, string>> BpListUpdateHandle;
        public string BrainpackComPort { get; set; }
        [SerializeField]
        internal int vTotalTries;

        internal SerialPort mSerialPort;
         public SerialPort SerialPort
        {
            get { return mSerialPort; }
        }

        /// <summary>
        /// returns the current state of the controller
        /// </summary>
        public abstract BrainpackConnectionState ConnectionState { get; }

        public void Connect(string vComport)
        {
            mSerialPort = new SerialPort(vComport); 
            mSerialPort.Open();
        }

        /// <summary>
        /// reset the number of reconnect tries
        /// </summary>
        public void ResetTries()
        {
            vTotalTries = 0;
        }
    }
}