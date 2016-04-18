/*@file PriorityQueueTest.cs
* @brief Contains the PriorityQueueTest class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System; 
using Assets.Scripts.Communication.Communicators;
using Assets.Scripts.Utils.DebugContext.logging;
using HeddokoLib.adt;
using UnityEngine;
using LogType = Assets.Scripts.Utils.DebugContext.logging.LogType;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// priority queue unit testing
    /// </summary>
    public class PriorityQueueTest : MonoBehaviour
    {
        public PriorityQueue<PriorityMessage> PriorityMessages = new PriorityQueue<PriorityMessage>();
        public Priority NextPriority;
        public int MinimumStartMessages = 10;
        public int MaxStartMsgs = 1000;
        public int ChosenNumberOfMessages = 0;

        public void Start()
        {
            int vTotalRemoval = 0;
            try
            {
                Debug.Log("<color=green> Starting unit test: insertion</color> ");
                 
                ChosenNumberOfMessages = Random.Range(MinimumStartMessages, MaxStartMsgs);
                int vTemp = ChosenNumberOfMessages;
                DebugLogger.Instance.LogMessage(LogType.UnitTest, "total number of messages to be added:  " + vTemp);
                DebugLogger.Instance.LogMessage(LogType.UnitTest, "++++++++++++++ STARTING INSERTION +++++++++++++++" );

                while (vTemp > 0)
                {
                    int vNextP = Random.Range(0, 4);
                    NextPriority = (Priority)vNextP;
                    PriorityMessage vP = new PriorityMessage()
                    {
                        MessagePayload = (NextPriority.GetName()),
                        Priority = NextPriority
                    };
                    PriorityMessages.Add(vP);
                    DebugLogger.Instance.LogMessage(LogType.UnitTest, "Adding "+vP.ToString());
                    vTemp--;
                }
                PriorityMessage lP = new PriorityMessage()
                {
                    MessagePayload = (NextPriority.GetName()),
                    Priority = Priority.Urgent
                };
                PriorityMessages.Add(lP);
                Debug.Log("<color=green> Starting unit test: removal</color> ");
                DebugLogger.Instance.LogMessage(LogType.UnitTest, "++++++++++++++ STARTING REMOVAL +++++++++++++++");
              
                while (PriorityMessages.Count > 0)
                {
                    if (PriorityMessages.Count == 1)
                    {
                        string s = "fdadf";
                    }
                    PriorityMessage vP = PriorityMessages.RemoveFirstItem();
                    DebugLogger.Instance.LogMessage(LogType.UnitTest, "removing "+ vP.ToString() + " count is "+PriorityMessages.Count );
                    vTotalRemoval++;
                }
                Debug.Log("<color=green> Completed adding and removal</color> ");

            }
            catch (Exception vE)
            {
                Debug.Log("<color=red> Priority queue error :</color> " + vE);
                Debug.Log("<color=red> total removed so far :</color> " + vTotalRemoval);
            }
          


        }
    }

    public enum PriorityQueueTestType
    {
        BasicInsertionAndRemoval,

    }
}
