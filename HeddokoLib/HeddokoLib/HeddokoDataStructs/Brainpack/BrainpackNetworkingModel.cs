// /**
// * @file Brainpack.cs
// * @brief Contains the brainpack model
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using System.Net;

namespace HeddokoLib.HeddokoDataStructs.Brainpack
{

    /// <summary>
    /// An abstract model of a brainpack.
    /// </summary>
    public class BrainpackNetworkingModel:  IEqualityComparer
    {
        private string mId = "";
        private string mLocation;
        private string mVersion;
        private string mStatus;
        private string mQaStatus;
        private string mPowerBoardId;
        private string mDataBoardId;
        private string mKitId;
        private EndPoint mEndAdvertisingEndPoint;
        public int TcpControlPort;
        public string TcpIpEndPoint;

        /// <summary>
        /// The kit Id associated with this brainpacks
        /// </summary>
        public string KitId
        {
            get { return mKitId; }
            set { mKitId = value; }
        }

        /// <summary>
        /// The databoard ID associated with this brainpack
        /// </summary>
        public string DataBoardId
        {
            get { return mDataBoardId; }
            set { mDataBoardId = value; }
        }

        public string QaStatus
        {
            get { return mQaStatus; }
            set { mQaStatus = value; }
        }

        /// <summary>
        /// The powerboard id associated with this brainpack
        /// </summary>
        public string PowerBoardId
        {
            get { return mPowerBoardId; }
            set { mPowerBoardId = value; }
        }

        public string Status
        {
            get { return mStatus; }
            set { mStatus = value; }
        }

        /// <summary>
        /// The version of the firmware
        /// </summary>
        public string Version
        {
            get { return mVersion; }
            set { mVersion = value; }
        }

        public string Location
        {
            get { return mLocation; }
            set { mLocation = value; }
        }

        /// <summary>
        /// The brainpacks unique identifier found at advertising time.
        /// </summary>
        public string Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// The remote end point of the brainpack found at advertising time
        /// </summary>
        public EndPoint Point
        {
            get { return mEndAdvertisingEndPoint; }
            set { mEndAdvertisingEndPoint = value; }
        }

        /// <summary>
        /// returns 1 if version 1 is greater than version 2, 0 if equal, -1 if less. 
        /// </summary>
        /// <param name="vVersion1"></param>
        /// <param name="vVersion2"></param>
        /// <returns></returns>
        public static int CompareBrainpackVersion(string vVersion1, string vVersion2)
        {
            Version vVersionFirst = new Version(vVersion1);
            Version vVersionSecond = new Version(vVersion2);

            int vComparison = vVersionFirst.CompareTo(vVersionSecond);
            return vComparison;
        }

        public bool Equals(object vX, object vY)
        {
            if (vX == null || vY == null)
            {
                return false;
            }
            if (vX.GetType() != typeof (BrainpackNetworkingModel) || vX.GetType() != typeof (BrainpackNetworkingModel))
            {
                return false;
            }
            BrainpackNetworkingModel vModelX = (BrainpackNetworkingModel) vX;
            BrainpackNetworkingModel vModelY = (BrainpackNetworkingModel) vY;
            return vModelX.Id.Equals(vModelY.Id);
        }

        public int GetHashCode(object vObj)
        {
            BrainpackNetworkingModel vModel = (BrainpackNetworkingModel)vObj;
            return vModel.Id.GetHashCode();
        }
    }
}