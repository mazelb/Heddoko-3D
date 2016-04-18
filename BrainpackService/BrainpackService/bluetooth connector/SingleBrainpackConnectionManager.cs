/** 
* @file SingleBrainpackConnectionManager.cs
* @brief Contains the SingleBrainpackConnectionManager class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
 
using System.Collections.Generic; 
using System.Net.Sockets; 
using HeddokoLib.adt; 
using InTheHand.Net; 

namespace BrainpackService.bluetooth_connector
{
    /**
    * SingleBrainpackConnectionManager class 
    * @brief SingleBrainpackConnectionManager class, responsible for maintaining connections with a single and streaming 
    * data to interested listeners 
    */
    public class SingleBrainpackConnectionManager
    {
        // private BluetoothClient mCurrentBrainpack; //The current brainpack in use
        private static SingleBrainpackConnectionManager sGInstance;
        private Brainpack mCurrentBrainpack;
        public List<Socket> Listeners = new List<Socket>(1);
        private object mListLock = new object();
        private object mWorkingFlagLock = new object();
        private const int MaxBufferSize = 1024;
        public CircularQueue<byte[]> Buffer { get; set; }=  new CircularQueue<byte[]>(MaxBufferSize, true); 
        private bool mIsWorking;


        public static SingleBrainpackConnectionManager Instance
        {
            get
            {
                if (sGInstance == null)
                {
                    sGInstance = new SingleBrainpackConnectionManager();
                }
                return sGInstance;
            }
        }
        /**
        * RegisterListenerToBrainpack (Socket vListener, BluetoothAddress vAddress)
        * @brief Will register the listener with the current streaming brain pack
        * @param  vCaller: the interested listener vAddress: the interested bluetooth address 
        * @note: will check if the vListener is already registered. If not, then 
        */
        public void RegisterListenerToBrainpack(Socket vListener, BluetoothAddress vAddress)
        {
            /*lock (mListLock)
            {
                //check if the listener is already in the list
                if (!mListeners.Contains(vListener))
                {
                    mListeners.Add(vListener);
                } //unlock
            }*/

            if (!mIsWorking)
            {
                StartFunction(vAddress);
            }
        }
        /**
        * RemoveListener (BrainpackListener vListener)
        * @brief Will remove the current listener
        * @param  vCaller: the interested listener  
        */
        public bool RemoveListener(Socket vListener)
        {
            bool vWasRemoved = false;
            lock (mListLock)
            {
                //check if the listener is already in the list
                if (Listeners.Contains(vListener))
                {
                    Listeners.Remove(vListener);
                    vWasRemoved = true;
                } //unlock 
            }
            return vWasRemoved;
        }

        /**
        * StartFunction(BluetoothAddress vAddress)
        * @brief Will start the current working thread 
        * @param vAddress: the addres of the brainpack
        */

        private void StartFunction(BluetoothAddress vAddress)
        {
            //restart the function
            Stop();
            lock (mWorkingFlagLock)
            {
                mIsWorking = true;
            }
            mCurrentBrainpack = new Brainpack();
            mCurrentBrainpack.SetNewDevice(vAddress);
            mCurrentBrainpack.OutboundBuffer = Buffer; 
        }

        public void SetBrainpackConnection(BluetoothAddress vAddress)
        {
            
        }
        /**
        * Stop()
        * @brief Stops the current working thread 
        */
        public void Stop()
        {
            lock (mWorkingFlagLock)
            {
                mIsWorking = false;
            }
            mCurrentBrainpack?.Stop();
        }
        /**
        * PushDataFromBrainPack(()
        * @brief Pushes data that was received from the brain pack and sends an ansync command
        */
        private void PushDataFromBrainPack()
        {
            while (true)
            {
                if (Buffer.Count == 0)
                {
                    continue;
                }
                Buffer.Dequeue();
                //send a command in the following list for data
                lock (mListLock)//lock the list from being accessed
                { 
                }//end lock

                if (!mIsWorking)
                {
                    break;
                }
            }
        }

        public byte[] GetData()
        {

            return Buffer.Dequeue();
        }


 
 

    }


}
