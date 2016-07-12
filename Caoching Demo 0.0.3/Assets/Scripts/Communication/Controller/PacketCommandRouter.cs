
/** 
* @file SocketCommandRouter.cs
* @brief Contains the SocketCommandRouter class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text; 
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.DebugContext.logging;
using HeddokoLib.networking;
using HeddokoLib.utils;

namespace Assets.Scripts.Communication.Controller
{
    /**
    * Body class 
    * @brief SocketCommandRouter class. This classes is responsible to reroute incoming and outgoing command from and to the socket client. 
    * @note Do not process a command within a registered command. This will cause a cause a stackoverflow exception. 
    * Reroute commands to other functions and not to itself. 
    */
    public class PacketCommandRouter
    {
        private static PacketCommandRouter sInstance;
        private SynchronousClient mSocketClient;
        private Command mCommand = new Command();
        private object mFrameTheadAccessLock = new object();

        //On brainpack data retreival, send the data to the bodyframethread
        private BodyFrameThread mBodyFrameThread;
        public SynchronousClient ClientSocket
        {
            get
            {
                if (mSocketClient == null)
                {
                    mSocketClient = new SynchronousClient();
                }
                return mSocketClient;
            }
        }

        public static PacketCommandRouter Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new PacketCommandRouter();
                    sInstance.Initialize();
                }
                return sInstance;
            }
        }
      

        internal BodyFrameThread FrameThread
        {
            get
            {
                lock (mFrameTheadAccessLock)
                {
                    return mBodyFrameThread;
                }
            }
            set
            {
                BodyFrameThread vTemp = value;
                lock (mFrameTheadAccessLock)
                {
                    mBodyFrameThread = vTemp;
                }
            }
        }

        /**
        * Initialize 
        * @brief Begins command delegate registration. Please see documentation for HeddokoCommand for further information on what each numerical value 
        * represents  
        * @note The document can be found here, you need to have a valid Heddoko sharepoint account in order to access the link. 
        https://heddoko.sharepoint.com/_layouts/OneNote.aspx?id=%2FSiteAssets%2FTeam%20Site%20Notebook&wd=target%28Data%20structures.one%7CCB02DD1E-F126-43C5-A4F2-10252682A3FA%2F%29onenote:https://heddoko.sharepoint.com/SiteAssets/Team%20Site%20Notebook/Data%20structures.one#section-id={CB02DD1E-F126-43C5-A4F2-10252682A3FA}&end
        */
        public void Initialize()
        {
            mCommand.Register(HeddokoCommands.BPConnectionSucess, SuitConnectionSuccess);
            mCommand.Register(HeddokoCommands.RequestToConnectToBP, SendHighPriorityMessage);
            mCommand.Register(HeddokoCommands.SendBPData, ReRouteRawFrameData);
             mCommand.Register(HeddokoCommands.RegisterListener, SendHighPriorityMessage);
            mCommand.Register(HeddokoCommands.RegisterListenerAck, RegisterAck);
            mCommand.Register(HeddokoCommands.ConnectionAck, ConnectionAcknowledged);
            mCommand.Register(HeddokoCommands.StopHeddokoUnityClient, Stop);
            mCommand.Register(HeddokoCommands.StopRecordingReq, SendHighPriorityMessage);
            mCommand.Register(HeddokoCommands.ClientError, SocketClientError);
            mCommand.Register(HeddokoCommands.DisconnectBrainpack, SendHighPriorityMessage);
            mCommand.Register(HeddokoCommands.DiscoAcknowledged, DisconnectAcknowledged);
            mCommand.Register(HeddokoCommands.SetRecordingPrefixReq, SendMediumPriorityMessage);
            mCommand.Register(HeddokoCommands.ShutdownBrainpackReq, SendUrgentPriorityMessage);
            mCommand.Register(HeddokoCommands.ShutdownBrainpackResp, ShutdownBrainpackResp);
            mCommand.Register(HeddokoCommands.ResetBrainpackReq, SendHighPriorityMessage);
            mCommand.Register(HeddokoCommands.ResetBrainpackResp, ResetBrainpackResp);
            mCommand.Register(HeddokoCommands.GetBrainpackStateReq, SendMediumPriorityMessage);
            mCommand.Register(HeddokoCommands.GetBrainpackStateResp, GetBrainpackStateResp);
            mCommand.Register(HeddokoCommands.SetBrainpackTimeReq, SendMediumPriorityMessage);
            mCommand.Register(HeddokoCommands.SetBrainpackTimeResp, SetBrainpackTimeResp);
            mCommand.Register(HeddokoCommands.GetResponseMessageReq, SendMediumPriorityMessage);
            mCommand.Register(HeddokoCommands.GetResponseMessageResp, RerouteResponseMessage);
            mCommand.Register(HeddokoCommands.GetBrainpackVersionReq, SendMediumPriorityMessage);
            mCommand.Register(HeddokoCommands.StartRecordingReq, SendHighPriorityMessage);
            mCommand.Register(HeddokoCommands.ClearBuffer, SendMediumPriorityMessage);
            mCommand.Register(HeddokoCommands.EnableSleepTimerReq, SendHighPriorityMessage);
            mCommand.Register(HeddokoCommands.DisableSleepTimerReq, SendHighPriorityMessage);
            mCommand.Register(HeddokoCommands.EnableSleepTimerResp, SleepTimerEnabled);
            mCommand.Register(HeddokoCommands.DisableSleepTimerResp, SleepTimerDisabled);
            mCommand.Register("TimeoutException", TimeoutExcCallback);
            mCommand.Register(HeddokoCommands.ReturnBrainpackResults, BrainpackResultsReturned);
            mCommand.Register(HeddokoCommands.GetBrainpackResults, SendHighPriorityMessage);

        }

      

        /// <summary>
        /// Brainpack results returned from service
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void BrainpackResultsReturned(object vSender, object vArgs)
        {
            Dictionary<string, string> vReturnedResults = new Dictionary<string, string>();
            HeddokoPacket vHeddokoPacket = (HeddokoPacket) vArgs;
            string vResult = HeddokoPacket.Unwrap(vHeddokoPacket.Payload);
            var vExplodedResult = vResult.Split(new[] {"<BPLine>"},StringSplitOptions.RemoveEmptyEntries);
            foreach (var vExploded in vExplodedResult)
            {
                var vSubStrings = vExploded.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                vReturnedResults.Add(vSubStrings[0], vSubStrings[1]);
            }

            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                BrainpackConnectionController.Instance.OnBrainpackListUpdate(vReturnedResults) ;

            });
        }

        /// <summary>
        /// Listener registration acknowledged
        /// </summary>
        /// <param name="vsender"></param>
        /// <param name="vargs"></param>
        private void RegisterAck(object vsender, object vargs)
        {
          
        }

        /// <summary>
        /// Callback on timeout exception, notifies the app of the state of the socket
        /// </summary>
        /// <param name="vsender"></param>
        /// <param name="vargs"></param>
        private void TimeoutExcCallback(object vsender, object vargs)
        { 
            //clear out the buffer
            if (FrameThread != null && FrameThread.InboundSuitBuffer != null)
            {
                FrameThread.InboundSuitBuffer.Clear();
            }
            OutterThreadToUnityThreadIntermediary.EnqueueOverwrittableActionInUnity("TimoutException", () =>
            {
                BrainpackConnectionController.Instance.TimeoutHandler();
            });
        }

        private void SleepTimerDisabled(object vsender, object vargs)
        {
        
        }

        private void SleepTimerEnabled(object vsender, object vargs)
        {
       
        }


        /// <summary>
        /// Reroute status message responses from the brainpack
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void RerouteResponseMessage(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Unwrap(vHeddokoPacket.Payload);
            BrainpackConnectionController.Instance.Output = vPayload;
            if (!string.IsNullOrEmpty(vPayload))
            {
                Action vAction = () =>
                {
                    if (BrainpackConnectionController.Instance.BrainpackStatusResponse != null)
                    {
                        BrainpackConnectionController.Instance.BrainpackStatusResponse.Invoke(vPayload);
                    }
                };
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);
            }
        }
        /**
        * Process(object vSender, HeddokoPacket vPacket)
        * @brief Attempt to prcess the packet. Wrapper function for the Command.Process command
        * @param  object vSender: the sender. object vArgs: the requested list of devices
        * @note will not invoke a CommandDelegate if the packet contains an invalid command
        * @return bool that indicates that the command delegate was succesfully invoked
        */
        public bool Process(object vSender, HeddokoPacket vPacket)
        {
            bool vIsProcessed = mCommand.Process(vSender, vPacket);
            return vIsProcessed;
        }
        /// <summary>
        /// Sets the bodyframe thread to the passed in parameter
        /// </summary>
        /// <param name="vThread"></param>
        public void SetBodyFrameThread(BodyFrameThread vThread)
        {
            FrameThread = vThread;
        }


        /// <summary>
        /// Disconnect the current body frame thread 
        /// </summary>
        /// <param name="vThread"></param>
        public void DisconnectFrameThread()
        {
            FrameThread = null;
        }

        /// <summary>
        /// Connection has been acknowledged by the brainpack server
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void ConnectionAcknowledged(object vSender, object vArgs)
        {
            //todo: send message to interested parties that need to know if the brainpack connection is finished
        }
        /**
       * BrainpackListRxRequest(object vSender, object vArgs) 
       * @brief Server request responded with a list of currently available brainpack device
       * @param  object vSender: the sender. object vArgs: the requested list of devices
       * @ note: please note if no devices are found, then a string of length zero is sent
       * Also note that this is a todo in case we ever need to monitor multiple suits in any given one instance
       */
        private void BrainpackListRequestReceived(object vSender, object vArgs)
        {
            StringBuilder vSb = new StringBuilder();
            HeddokoPacket vPacket = (HeddokoPacket)vArgs;
            byte[] vDataBytes = vPacket.Payload;
            vSb.Append(Encoding.UTF8.GetString(vDataBytes, 0, vDataBytes.Length));
            string vPreExploded = vSb.ToString();
            string[] aList = vPreExploded.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> vMacAddresses = new List<string>();
            if (aList.Length != 0)
            {
                for (int i = 0; i < aList.Length; i++)
                {
                    vMacAddresses.Add(aList[i]);
                }
            }
        }



        private void DisconnectAcknowledged(object vSender, object vArg)
        {

            //todo:
        }

        /// <summary>
        /// A error produced by the socket client and informs the BrainpackConnectionController
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SocketClientError(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Unwrap(vHeddokoPacket.Payload);
            //tell the brainpackconnection controller of the error
            BrainpackConnectionController.Instance.FailedToConnectListener(vPayload);
        }
        /**
     * ReRouteRawFrameData(object vSender, object vArgs) 
     * @brief Routes the incoming raw frame data to the BodyFrameThread. 
     * @param vSender: the sender, vArgs: the heddokopack containing the brainpack data. 
     */

        private void ReRouteRawFrameData(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Unwrap(vHeddokoPacket.Payload);
            if (FrameThread != null && FrameThread.InboundSuitBuffer != null)
            {
                FrameThread.InboundSuitBuffer.Enqueue(vHeddokoPacket);
            }
            DebugLogger.Instance.LogMessage(LogType.BrainpackFrame, "Brainpack frame rx:"+ vPayload);
           
        }


        /// <summary>
        /// Server's response with the status of the connection to the server
        /// </summary>
        /// <param name="vSender">ignored parameter</param>
        /// <param name="vArgs">The HeddokoPacket that was sent. It's payload contains its status</param>
        /**
        * SuitConnectionSuccess(object vSender, object vArgs)
        * @brief Server's response with the status of the connection to the server
        * @param object args: the parameters necessary for this
        * function to perform
        * @note Please not that this will throw an exception if
        * y requirements are not met with the given parameter
        * @return returns an arbitrary value
        */
        private void SuitConnectionSuccess(object vSender, object vArgs)
        {
            //BPConnectionSucess 
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            byte[] vPayload = vHeddokoPacket.Payload;
            string vUnwrappedPayload = HeddokoPacket.Unwrap(vPayload);
            bool vConnectionSuccessfull = (vUnwrappedPayload == "true");

            BrainpackConnectionController.Instance.BrainpackConnectedTrigger.Trigger(vConnectionSuccessfull);
            //BrainpackConnectionResult(vConnectionSuccessfull); //send a message to the BrainpackConnectionController with the result of the connection

        }

        /// <summary>
        /// Send a raw frame to the vSender
        /// </summary>
        /// <param name="vSender">the sender</param>
        /// <param name="vArgs">the raw frame data</param>
        /**
       * SendRawFrameData(object vSender, object vArgs) 
       * @brief Send a raw frame to the vSender
       * @param vSender: the sender, vArgs: the raw frame data
       */
        private void SendRawFrameData(object vSender, object vArgs)
        {
            Socket vSocket = (Socket)vSender;
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            byte[] vPacketBody = vHeddokoPacket.Payload;
            //  AsynchronousSocketListener.Send(vSocket, vPacketBody); 
        }


        private void Stop(object vSender, object vArgs)
        {
            ClientSocket.Stop();

        }

        /// <summary>
        /// response received after a state request
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vArgs"></param>
        private void GetBrainpackStateResp(object vVsender, object vArgs)
        {
            //unwrap the message

            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Unwrap(vHeddokoPacket.Payload);
            Action vAction = () =>
            {
                if (BrainpackConnectionController.Instance.BrainpackStatusResponse != null)
                {
                    BrainpackConnectionController.Instance.BrainpackStatusResponse.Invoke(vPayload);
                }
            };
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);
            /*
                        Action vAction = () =>
                        {
                            if (BrainpackConnectionController.BrainpackStatusResponse != null)
                            {
                                BrainpackConnectionController.BrainpackStatusResponse.Invoke(vPayload);
                            }
                        };
                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);*/
        }

        /// <summary>
        /// response from the brainpack
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vArgs"></param>
        private void SetBrainpackTimeResp(object vVsender, object vArgs)
        {
            /*  Action vAction = () =>
              {
                  if (BrainpackConnectionController.BrainpackTimeSetResp != null)
                  {
                      BrainpackConnectionController.BrainpackTimeSetResp.Invoke();
                  }
              };
              OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);*/
        }

        /// <summary>
        /// Response after the brainpack has been reset
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vArgs"></param>
        private void ResetBrainpackResp(object vVsender, object vArgs)
        {
            /*  Action vAction = () =>
              {
                  if (BrainpackConnectionController.ResetBrainpackResp != null)
                  {
                      BrainpackConnectionController.ResetBrainpackResp.Invoke();
                  }
              };
              OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);*/
        }

        /// <summary>
        /// Response to when the brainpack has been shut down
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vVargs"></param>
        private void ShutdownBrainpackResp(object vVsender, object vVargs)
        {
            /* Action vAction = () =>
             {
                 BrainpackConnectionController.BrainpackShutdown.Invoke();
             };
             OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);*/
        }


        /// <summary>
        /// Sends a high priority message
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SendHighPriorityMessage(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            DebugLogger.Instance.LogMessage(LogType.SocketClientSend, "from application: " + vHeddokoPacket.Command);
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            PriorityMessage vMessage = new PriorityMessage() { Priority = Priority.High, MessagePayload = vPayload };
            ClientSocket.AddMessage(vMessage);
        }
        /// <summary>
        /// Sends a Urgent priority message
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SendUrgentPriorityMessage(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            DebugLogger.Instance.LogMessage(LogType.SocketClientSend, "from application: " + vHeddokoPacket.Command);
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            PriorityMessage vMessage = new PriorityMessage() { Priority = Priority.Urgent, MessagePayload = vPayload };
            ClientSocket.AddMessage(vMessage);
        }
        /// <summary>
        /// Sends a Low priority message
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SendLowPriorityMessage(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            DebugLogger.Instance.LogMessage(LogType.SocketClientSend, "from application: " + vHeddokoPacket.Command);
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            PriorityMessage vMessage = new PriorityMessage() { Priority = Priority.Low, MessagePayload = vPayload };
            ClientSocket.AddMessage(vMessage);
        }
        /// <summary>
        /// Sends a Medium priority message
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SendMediumPriorityMessage(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            DebugLogger.Instance.LogMessage(LogType.SocketClientSend, "from application: " + vHeddokoPacket.Command);
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            PriorityMessage vMessage = new PriorityMessage() { Priority = Priority.Medium, MessagePayload = vPayload };
            ClientSocket.AddMessage(vMessage);

        }



    }
}
