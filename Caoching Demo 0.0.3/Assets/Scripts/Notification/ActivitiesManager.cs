// /**
// * @file ActivitiesManager.cs
// * @brief Contains the ActivitiesManager
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using Assets.Scripts.Licensing.Model;
using HeddokoSDK.Models.Activity;
using HeddokoSDK.Models.Enum;

namespace Assets.Scripts.Notification
{
    public class ActivitiesManager
    {
        private UserProfileModel mModel;
        private string mClientToken;
        private Dictionary<UserEventType, List<Action<NotificationMessage>>> mEventTypeHandlers = new Dictionary<UserEventType, List<Action<NotificationMessage>>>();

        /// <summary>
        /// Create an instance of an Activities manager
        /// </summary>
        /// <param name="vModel"></param>
        /// <param name="vClientToken"></param>
        public ActivitiesManager(UserProfileModel vModel, string vClientToken)
        {
            mModel = vModel;
            mClientToken = vClientToken;
            mModel.Client.OpenConnection();
            mModel.Client.SubscribeOnGettingNotification(mClientToken, OnNotificationReceived);
        }

        public void Dispose()
        {
            if (mModel != null && mModel.Client != null)
            {
                mModel.Client.CloseConnection();
            }
        }

        /// <summary>
        /// A handler for on notification received
        /// </summary>
        /// <param name="vObj"></param>
        private void OnNotificationReceived(NotificationMessage vObj)
        {
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
            mModel.Client.OpenStreamConnection();
            mModel.Client.SubscribeOnGettingNotification(mClientToken, OnNotificationReceived);
        }
    }
}