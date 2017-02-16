// /**
// * @file NotificationManager.cs
// * @brief Contains the NotificationManager.cs class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using Assets.Scripts.UI.AccessControl;
using Assets.Scripts.Utils;
using UIWidgets;

namespace Assets.Scripts.Notification
{
    /// <summary>
    /// Handles notification view
    /// </summary>
    public class NotificationManager
    {
        private readonly List<Notice> NotificationList = new List<Notice>();
        private static readonly NotificationManager gInstance = new NotificationManager();
        private Dictionary<NotificationUrgency, float> NotificationTimeout =
            new Dictionary<NotificationUrgency, float>();

        private NotificationManager()
        {
            NotificationTimeout.Add(NotificationUrgency.High, 10f);
            NotificationTimeout.Add(NotificationUrgency.Low, 10f);
            NotificationTimeout.Add(NotificationUrgency.Medium, 10f);
            NotificationTimeout.Add(NotificationUrgency.Urgent, 10f);
        }
        /// <summary>
        /// Create a new notification
        /// </summary>
        /// <param name="vMsg"></param>
        /// <param name="vUrgency"></param>
        public static void CreateNotification(string vMsg, NotificationUrgency vUrgency)
        {
            var vNotice = new Notice()
            {
                Message = vMsg,
                Urgency = vUrgency
            };
            gInstance.NotificationList.Add(vNotice);
            HandleNotification(vNotice);
        }

        /// <summary>
        /// Handles incoming notification
        /// </summary>
        /// <param name="vNotice"></param>
        private static void HandleNotification(Notice vNotice)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                UIWidgets.Notify vNotifyViewObject = Notify.Template("fade");
                vNotifyViewObject.Show(vNotice.Message, gInstance.NotificationTimeout[vNotice.Urgency],
                    sequenceType: NotifySequence.First, clearSequence: false);
                vNotice.NotifyViewObject = vNotifyViewObject;
            });
        }


        /// <summary>
        /// Removes a notification. IF the notification has an associated view, this is disabled as well. 
        /// </summary>
        /// <param name="vNotice"></param>
          public static void RemoveNotification(Notice vNotice)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                vNotice.NotifyViewObject.Hide();
                gInstance.NotificationList.Remove(vNotice);
            });
     
        }

        public enum NotificationUrgency
        {
            Low,
            Medium,
            High,
            Urgent
        }

        /// <summary>
        /// Stops all current notifications
        /// </summary>
        public static void StopNotifications()
        {
            Notify.NotifyManager.StopAllCoroutines();
            Notify.NotifyManager.Clear();
        }
    }
}