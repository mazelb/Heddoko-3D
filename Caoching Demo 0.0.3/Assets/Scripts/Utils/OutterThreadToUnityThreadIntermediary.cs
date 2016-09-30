
/** 
* @file OutterThreadToUnityThreadIntermediary.cs
* @brief Contains the OutterThreadToUnityThreadIntermediary class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// This helper class allows for non unity-based threads to send messages to the main unity thread by enqueing an action
    /// </summary>
    public class OutterThreadToUnityThreadIntermediary : MonoBehaviour
    {
        private static OutterThreadToUnityThreadIntermediary sInstance;
        private Queue<Action> mQueue = new Queue<Action>();
        private Thread mUnityThread;

        /// <summary>
        /// this is used when a thread needs to communicate a message to the unity thread but 
        /// it can be overwritten by subsequent message types
        /// </summary>
        private Dictionary<string, Action> mOverWrittableActionQueue = new Dictionary<string, Action>();
        public static float FrameDeltaTime { get; private set; }

        public static OutterThreadToUnityThreadIntermediary Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = FindObjectOfType<OutterThreadToUnityThreadIntermediary>();
                    if (sInstance == null)
                    {
                        GameObject vGo = new GameObject();
                        vGo.name = "Non unity thread to unity thread helper";
                        sInstance = vGo.AddComponent<OutterThreadToUnityThreadIntermediary>();
                    }
                    sInstance.Init();
                }
                return sInstance;
            }
        }
        /// <summary>
        /// Unity time
        /// </summary>
        public static float UnityTime { get; private set; }

        public string ApplicationPath { get; set; }

        void Awake()
        {
            Init();
        }

        /// <summary>
        /// Initialize parameters
        /// </summary>
        public void Init()
        {
            mUnityThread = Thread.CurrentThread;
            ApplicationPath = Application.persistentDataPath;
        }



        /// <summary>
        /// checks if the current passed in thread is a unity thread
        /// </summary>
        /// <param name="vThread"></param>
        /// <returns></returns>
        public static bool InUnityThread()
        {
            return Instance.mUnityThread.Equals(Thread.CurrentThread);
        }

        public static Coroutine HelpStartCoroutine(IEnumerator vCoroutine)
        {
            return Instance.StartCoroutine(vCoroutine);
        }

        /// <summary>
        /// Queues up an action in unity
        /// </summary>
        /// <param name="vAction"></param>
        public static void QueueActionInUnity(Action vAction)
        {
            try
            {
                Instance.mQueue.Enqueue(vAction);
            }
            catch (Exception vException)
            {
                Delegate[] vActions = vAction.GetInvocationList();
                foreach (Delegate vDel in vActions)
                {
                    Debug.Log(vDel.Method.ReflectedType.FullName + "." + vDel.Method.Name);
                }
            }
        }

        public static void EnqueueOverwrittableActionInUnity(string vKey, Action vAction)
        {
            try
            {
                if (Instance.mOverWrittableActionQueue.ContainsKey(vKey))
                {
                    Instance.mOverWrittableActionQueue[vKey] = vAction;
                }
                else
                {
                    Instance.mOverWrittableActionQueue.Add(vKey, vAction);
                }
            }
            catch (Exception vException)
            {
                Debug.Log(vException.StackTrace);
            }
           
        }

        void Update()
        {
            if (mOverWrittableActionQueue.Count > 0)
            {
                List<string> vKeys = new List<string>(mOverWrittableActionQueue.Keys);
                for (int vI = 0; vI < vKeys.Count; vI++)
                {
                    lock (mOverWrittableActionQueue)
                    {
                        mOverWrittableActionQueue[vKeys[vI]].Invoke();
                        mOverWrittableActionQueue.Remove(vKeys[vI]);
                    }
                }
            }
            if (mQueue.Count > 0)
            {
                Action vAction = mQueue.Dequeue();
                if (vAction != null)
                {
                    vAction.Invoke();
                }
                else
                {
                    print("found null action!");
                }
            }

            FrameDeltaTime = Time.deltaTime;
            UnityTime = Time.time;
        }


    }
}
