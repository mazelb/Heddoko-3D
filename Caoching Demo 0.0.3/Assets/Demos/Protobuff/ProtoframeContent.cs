// /**
// * @file ProtoframeContent.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Demos.Protobuff
{
    /// <summary>
    /// Use this with a PniSensorDataCollector to add a subframe of data 
    /// </summary>
    public class ProtoframeContent
    {
        private Dictionary<int, string> mFrameContents = new Dictionary<int, string>();
        public float TimeStamp = -1;


        public void AddContent(int vIndex, string vItem)
        {

            mFrameContents.Add(vIndex, vItem);
        }


        public bool HasIndex(int vIndex)
        {
            return mFrameContents.ContainsKey(vIndex);
        }

        /// <summary>
        /// Adds padding for indices that do not have any data
        /// </summary>
        public void AddPadding()
        {
            for (int i = 0; i < 9; i++)
            {
                if (!mFrameContents.ContainsKey(i))
                {
                    //mFrameContents
                    // mFrameContent.AddContent(vIndex, "," + vRawQuaternion + "," + vRawEuler + "," + vMappedQuat + "," + vMappedEuler + ","+ ","+vMagnometer + ","+ vAccelData +"," + vInMagTransience + "," + vIsCalibrated);

                    mFrameContents.Add(i,", " + "0;0;0;0" + ", " + "0;0;0" + ", " + "0;0;0;0" + ", " + "0;0;0" + ", "+ ", " + "0;0;0" + ", "+ "0;0;0" + ", " + "NO VAL" + ", " + "NO VAL" + ",");
                }
            }
        }

        public override string ToString()

        {
            AddPadding();
            StringBuilder vReturn = new StringBuilder();
            vReturn.Append(TimeStamp + ",");
            foreach (var vFrameContent in mFrameContents)
            {
                vReturn.Append(vFrameContent.Key);
                vReturn.Append(vFrameContent.Value);
            }
            return vReturn.ToString();

        }
    }
}