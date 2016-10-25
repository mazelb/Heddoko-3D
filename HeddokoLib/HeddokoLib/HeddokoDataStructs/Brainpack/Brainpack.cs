// /**
// * @file Brainpack.cs
// * @brief Contains the brainpack model
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Net;

namespace HeddokoLib.HeddokoDataStructs.Brainpack
{

    /// <summary>
    /// An abstract model of a brainpack.
    /// </summary>
    public class Brainpack
    {
        private string mId = "";
        private string mLocation;
        private string mVersion;
        private string mStatus;
        private string mQAStatus;
        private string mPowerBoardId;
        private string mDataBoardId;
        private string mKitId;
        private EndPoint mEndPoint;

        public string KitId
        {
            get { return mKitId; }
            set { mKitId = value; }
        }

        public string DataBoardId
        {
            get { return mDataBoardId; }
            set { mDataBoardId = value; }
        }

        public string QaStatus
        {
            get { return mQAStatus; }
            set { mQAStatus = value; }
        }

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

        public string Id
        {
            get { return mId; }
            set { mId = value; }
        }


        public EndPoint Point
        {
            get { return mEndPoint; }
            set { mEndPoint = value; }
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
    }
}