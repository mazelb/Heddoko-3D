// /**
// * @file PniSensorDataCollector.cs
// * @brief Contains the PniSensorDataCollector
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Demos.Protobuff
{
    public class PniSensorDataCollector
    {
        private FileStream mFileStream;
        private string mPath;
        private List<ProtoframeContent> mContents = new List<ProtoframeContent>();
        private ProtoframeContent mFrameContent = null;

        /// <summary>
        /// update the collection
        /// </summary>
        /// <param name="vIndex"></param>
        /// <param name="vTimestamp"></param>
        /// <param name="vRawQuaternion"></param>
        /// <param name="vRawEuler"></param>
        /// <param name="vMappedQuat"></param>
        /// <param name="vMappedEuler"></param>
        /// <param name="vMagnometer"></param>
        /// <param name="vAccelData"></param>
        /// <param name="vInMagTransience"></param>
        /// <param name="vIsCalibrated"></param>
        public void UpdateCollection(int vIndex, float vTimestamp, string vRawQuaternion, string vRawEuler, string vMappedQuat,
            string vMappedEuler, string vMagnometer, string vAccelData, bool vInMagTransience, bool vIsCalibrated)
        {
            if (mFrameContent == null)
            {
                mFrameContent = new ProtoframeContent();
                mContents.Add(mFrameContent);
            }
            if (mFrameContent.TimeStamp < 0)
            {
                mFrameContent.TimeStamp = vTimestamp;
            }
            if (!mFrameContent.HasIndex(vIndex))
            {
                mFrameContent.AddContent(vIndex, "," + vRawQuaternion + "," + vRawEuler + "," + vMappedQuat + "," + vMappedEuler + ","  + vMagnometer + "," + vAccelData + "," + vInMagTransience + "," + vIsCalibrated + ",");
            }
            else
            {
                mFrameContent = new ProtoframeContent();
                mFrameContent.AddContent(vIndex, "," + vRawQuaternion + "," + vRawEuler + "," + vMappedQuat + "," + vMappedEuler + "," + vMagnometer + "," + vAccelData + "," + vInMagTransience + "," + vIsCalibrated + ",");
                mContents.Add(mFrameContent);
            }
        }
        /// <summary>
        /// Write contents to file
        /// </summary>
        /// <param name="vPath"></param>
        public void Write()
        {
            var vDir = Application.persistentDataPath + Path.DirectorySeparatorChar + "PniSensorDataCollection";
            if (!Directory.Exists(vDir))
            {
                Directory.CreateDirectory(vDir);
            }
            mPath = vDir + Path.DirectorySeparatorChar + DateTime.Now.ToString("ddd d MMM") + "0" + ".csv";
            if (File.Exists(mPath))
            {
                int vAppendedIdx = 1;
                while (true)
                {
                    mPath = vDir + Path.DirectorySeparatorChar + DateTime.Now.ToString("ddd d MMM") + vAppendedIdx + ".csv";
                    if (!File.Exists(mPath))
                    {
                        break;
                    }
                    vAppendedIdx++;
                }
            }
            FileStream vStream;
            using (vStream = new FileStream(mPath, FileMode.Create))
            {
                using (StreamWriter vWriter = new StreamWriter(vStream))
                {
                    WriteHeading(vWriter);
                    for (int vI = 0; vI < mContents.Count; vI++)
                    {
                        vWriter.WriteLine(mContents[vI].ToString());
                    }
                }
            }
        }

        private void WriteHeading(StreamWriter vWriter)
        {
           string vTimeStamp=   "TimeStamp,";
            string vHeading =  "Sensor Index,";
            vHeading += "Raw Quat,";
            vHeading += "Raw Euler(YPR),";
            vHeading += "Mapped Quat,";
            vHeading += "Mapped Euler,";
            vHeading += "Magnometer data,";
            vHeading += "Accel Data,";
            vHeading += "In Mag transience? ,";
            vHeading += "Is Calibrated?,";
            vWriter.Write(vTimeStamp);
            for (int i = 0; i < 9; i ++)
            {
                vWriter.Write(vHeading);
            }
            vWriter.WriteLine();
        }

        public void Clear()
        {
            mContents.Clear();
        }
    }

}