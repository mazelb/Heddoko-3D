// /**
// * @file ProtoDemoConnectionController.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 06 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.IO.Ports;
using Assets.Scripts.Communication.Controller;

namespace Assets.Demos
{
    public class ProtoDemoConnectionController : BrainpackConnectionController
    {
        private SerialPort mSerialPort;
        public SerialPort Port
        {
            get { return mSerialPort; }
        }

        void Awake()
        {
            mInstance = this;
        }

        public override void ConnectToBrainpack()
        {
            if (!ProtoDemoController.UseProtoBuff)
            {
                base.ConnectToBrainpack();
            }
            else
            {
                if (Validate(BrainpackComPort))
                {
                    mSerialPort = new SerialPort(BrainpackComPort);
                    try
                    {
                        mSerialPort.Open();
                        mCurrentConnectionState = BrainpackConnectionState.Connected;
                        if (ConnectedStateEvent != null)
                        {
                            ConnectedStateEvent();
                        }
                    }
                    catch (Exception e)
                    {
                        mCurrentConnectionState = BrainpackConnectionState.Disconnected;

                        if (DisconnectedStateEvent != null)
                        {
                            DisconnectedStateEvent();
                        }
                    }


                }


            }
        }


        /// <summary>
        /// Set the Brainpack controller to idle
        /// </summary>
        public new void SetStateToIdle()
        {
            if (!ProtoDemoController.UseProtoBuff)
            {
                base.SetStateToIdle();
            }
            else
            {
                //todo
            }
        }

        public override void DisconnectBrainpack()
        {
            if (!ProtoDemoController.UseProtoBuff)
            {
                base.DisconnectBrainpack();
            }
            else
            { 
                {
                    mCurrentConnectionState = BrainpackConnectionState.Disconnected;
                    mSerialPort.Close();
                    if (DisconnectedStateEvent != null)
                    {
                        DisconnectedStateEvent();
                    }
                }
            }

        }

        public new void BrainpackConnectionResult(bool vConnectionRes)
        {
            if (!ProtoDemoController.UseProtoBuff)
            {
                base.BrainpackConnectionResult(vConnectionRes);
            }
            else
            {
                //todo
            }
        }

        new void Update()
        {
            if (!ProtoDemoController.UseProtoBuff)
            {
                base.Update();
            }
            else
            {

            }
        }
    }
}