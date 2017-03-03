// /**
// * @file NotificationActivitiesManager.cs
// * @brief Contains the NotificationActivitiesManager
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using HeddokoSDK.Models.Activity;
using HeddokoSDK.Models.Enum;
using Microsoft.AspNet.SignalR.Client;

namespace Assets.Scripts.Notification
{
    /// <summary>
    /// Manages notification activities provided by the back nd
    /// </summary>
    public class NotificationActivitiesManager
    {
        private UserProfileModel mModel;

        private Dictionary<UserEventType, List<Action<NotificationMessage>>> mEventTypeHandlers =
            new Dictionary<UserEventType, List<Action<NotificationMessage>>>();

        /// <summary>
        /// Create an instance of an Activities manager
        /// </summary>
        /// <param name="vModel"></param>
        /// <param name="vClientToken"></param>
        public NotificationActivitiesManager(UserProfileModel vModel)
        {
            mModel = vModel;
            mModel.Client.AddStateChangeListener(Client_HubConnectionStateChanged);
        }

        /// <summary>
        /// Handler for state changes
        /// </summary>
        /// <param name="vObj"></param>
        private void Client_HubConnectionStateChanged(StateChange vObj)
        {
            try
            {
                switch (vObj.NewState)
                {
                    case ConnectionState.Connected:
                        if (vObj.OldState == ConnectionState.Connected)
                        {
                            break;
                        }
                        ThreadPool.QueueUserWorkItem(SetSubScription);
                        break;
                    case ConnectionState.Connecting:

                        break;
                    case ConnectionState.Disconnected:
                        //Stream disconnected

                        break;
                    case ConnectionState.Reconnecting:
                        //todo
                        //Stream reconnecting
                        break;
                }
            }
            catch (Exception vE)
            {

            }

        }

        public void SetSubScription(object vObject)
        {
            try
            {
                mModel.Client.SubscribeOnGettingNotification(mModel.DeviceToken, OnNotificationReceived);
            }
            catch (Exception vE)
            {
                WindowsApplicationManager.WriteToDebugLog("Exception in SetSubScription " + vE.Message);
            }
        }


        public void Dispose()
        {
            if (mModel != null && mModel.Client != null)
            {
                mModel.Client.RemoveStateChangeListener(Client_HubConnectionStateChanged);
                mModel.Client.RemoveDevice(mModel.DeviceToken);
            }
        }

        /// <summary>
        /// A handler for on notification received
        /// </summary>
        /// <param name="vObj"></param>
        private void OnNotificationReceived(NotificationMessage vObj)
        {
            WindowsApplicationManager.WriteToDebugLog("OnNotificationReceived " + vObj.Text);

            if (vObj != null)
            {
                if (mEventTypeHandlers.ContainsKey(vObj.Type))
                {
                    foreach (var vAction in mEventTypeHandlers[vObj.Type])
                    {
                        if (vAction != null)
                        {
                            vAction(vObj);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a notifcation message event handler
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vHandler"></param>
        public void AddNotificationMessageEventHandler(UserEventType vKey, Action<NotificationMessage> vHandler)
        {
            if (!mEventTypeHandlers.ContainsKey(vKey))
            {
                mEventTypeHandlers.Add(vKey, new List<Action<NotificationMessage>>());
            }
            mEventTypeHandlers[vKey].Add(vHandler);
        }

        /// <summary>
        /// removes a notification message event handler
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vHandler"></param>
        public void RemoveNotificationMessageEventHandler(UserEventType vKey, Action<NotificationMessage> vHandler)
        {
            if (mEventTypeHandlers.ContainsKey(vKey))
            {
                mEventTypeHandlers[vKey].Remove(vHandler);
            }
        }

        /// <summary>
        /// Start the instance
        /// </summary>
        public void Start()
        {
            try
            {
                mModel.Client.AddDevice(mModel.DeviceToken);
                mModel.Client.OpenConnection();
            }
            catch (Exception e)
            {
            }

        }



    }
}