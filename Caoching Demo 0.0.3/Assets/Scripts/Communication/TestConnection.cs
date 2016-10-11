// /**
// * @file TestConnection.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Communication.Communicators;
using UnityEngine;

namespace Assets.Scripts.Communication
{
    public class TestConnection : MonoBehaviour
    {
        public SuitConnectionManager SuitConnectionManager;

        void Start()
        {
            SuitConnectionManager = new SuitConnectionManager();
            SuitConnectionManager.ConnectToSuit("127.0.0.1",8844);
            SuitConnectionManager.UpdateFirmware("C:/firmware.bin");
        }
    }
}