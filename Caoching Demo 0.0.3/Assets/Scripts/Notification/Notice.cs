// /**
// * @file Notice.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using UIWidgets;

namespace Assets.Scripts.Notification
{
    public class Notice : IEqualityComparer
    {
        public NotificationManager.NotificationUrgency Urgency;
        public string Message;
        public Notify NotifyViewObject;

        private string mNoticeGuid = Guid.NewGuid().ToString();

        /// <summary>
        /// If two notices are equal
        /// </summary>
        /// <param name="vX"></param>
        /// <param name="vY"></param>
        /// <returns></returns>
        public bool Equals(object vX, object vY)
        {
            if (vX == null || vY == null)
            {
                return false;
            }
            if (vX.GetType() != typeof(Notice) || vY.GetType() != typeof(Notice))
            {
                return false;
            }
            Notice vXObject = (Notice)vX;
            Notice vYObject = (Notice)vY;
            return vXObject.mNoticeGuid.Equals(vYObject.mNoticeGuid);
        }

        /// <summary>
        /// Return the HashCode of the notice
        /// </summary>
        /// <param name="vObj"></param>
        /// <returns></returns>
        public int GetHashCode(object vObj)
        {
            Notice vObject = (Notice)vObj;
            return vObject.mNoticeGuid.GetHashCode();
        }
    }
}