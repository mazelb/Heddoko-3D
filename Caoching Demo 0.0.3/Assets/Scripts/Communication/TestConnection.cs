// /**
// * @file TestConnection.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Communication.Communicators;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using UnityEngine;

namespace Assets.Scripts.Communication
{
    public class TestConnection : MonoBehaviour
    {
        public SuitConnectionManager SuitConnectionManager;
        private Brainpack mBrainpack;
        void Start()
        {
            mBrainpack = new Brainpack();
            SuitConnectionManager = new SuitConnectionManager(mBrainpack);
            SuitConnectionManager.ConnectToSuit("127.0.0.1",8844);
            SuitConnectionManager.NetworkSuitConnectionEstablishedEvent += UpdateFirmware;
            SuitConnectionManager.AddBrainpackUpdatedHandler(BrainpackStatusHandle);
        }

        private void BrainpackStatusHandle(Brainpack vVbrainpack)
        {
            Debug.Log(vVbrainpack.Version);
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                SuitConnectionManager.UpdateFirmware("firmware.bin");
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                SuitConnectionManager.RequestSuitStatus();
            }
        }

        void UpdateFirmware()
        {
            
        }
        void OnApplicationQuit()

        {
            SuitConnectionManager.CleanUp();
        }
    }
}