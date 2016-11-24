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
using System.Text;
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
                mFrameContent.AddContent(vIndex, "," + vRawQuaternion + "," + vRawEuler + "," + vMappedQuat + "," + vMappedEuler + "," + vMagnometer + "," + vAccelData + "," + vInMagTransience + "," + vIsCalibrated + ",");
            }
            else
            {
                mFrameContent = new ProtoframeContent();
                mFrameContent.AddContent(vIndex, "," + vRawQuaternion + "," + vRawEuler + "," + vMappedQuat + "," + vMappedEuler + "," + vMagnometer + "," + vAccelData + "," + vInMagTransience + "," + vIsCalibrated + ",");
                mContents.Add(mFrameContent);
            }
        }

        public void UpdateCollection(int vIndex, float vTimestamp, float vQx, float vQy, float vQz, float vQw, float vMx,
            float vMy, float vMz, float vAx, float vAy, float vAz, float vRx, float vRy, float vRz, float vH1, float vP1,
            float vR1, uint vCalStable, uint vMagTransient)
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
                var vString = FormatHelper(vTimestamp, vQx, vQy, vQz, vQw, vMx, vMy, vMz, vAx, vAy, vAz, vRx, vRy, vRz, vH1, vP1, vR1, vCalStable, vMagTransient);
                mFrameContent.AddContent(vIndex, vString);
            }
            else
            {
                mFrameContent = new ProtoframeContent();
                var vString = FormatHelper(vTimestamp, vQx, vQy, vQz, vQw, vMx, vMy, vMz, vAx, vAy, vAz, vRx, vRy, vRz, vH1, vP1, vR1, vCalStable, vMagTransient);
                mFrameContent.AddContent(vIndex, vString);
                mContents.Add(mFrameContent);
            }
        }

        private string FormatHelper(float vTimestamp, float vQx, float vQy, float vQz, float vQw, float vMx,
            float vMy, float vMz, float vAx, float vAy, float vAz, float vRx, float vRy, float vRz, float vH1, float vP1,
            float vR1, uint vCalStable, uint vMagTransient)
        {
            StringBuilder vBuilder = new StringBuilder();
            vBuilder.Append(vTimestamp + ",");
            vBuilder.Append(vQx.ToString("0.0000") + ",");
            vBuilder.Append(vQy.ToString("0.0000") + ",");
            vBuilder.Append(vQz.ToString("0.0000") + ",");
            vBuilder.Append(vQw.ToString("0.0000") + ",");
            vBuilder.Append(vMx.ToString("0.0000") + ",");
            vBuilder.Append(vMy.ToString("0.0000") + ",");
            vBuilder.Append(vMz.ToString("0.0000") + ",");
            vBuilder.Append(vAx.ToString("0.0000") + ",");
            vBuilder.Append(vAy.ToString("0.0000") + ",");
            vBuilder.Append(vAz.ToString("0.0000") + ",");
            vBuilder.Append(vRx.ToString("0.0000") + ",");
            vBuilder.Append(vRy.ToString("0.0000") + ",");
            vBuilder.Append(vRz.ToString("0.0000") + ",");
            vBuilder.Append(vH1.ToString("0.0000") + ",");
            vBuilder.Append(vP1.ToString("0.0000") + ",");
            vBuilder.Append(vR1.ToString("0.0000") + ",");
            vBuilder.Append(vCalStable + ",");
            vBuilder.Append(vMagTransient + ",");

            return vBuilder.ToString();
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
         vDir += Path.DirectorySeparatorChar + DateTime.Now.ToString("ddd d MMM");
            int vAppendedIdx = 0;
            //create a directory for todays date
            while (true)
            {
                mPath  = vDir+ vAppendedIdx;
                if (!Directory.Exists(mPath))
                {
                    break;
                }
                vAppendedIdx++;
            }
            Directory.CreateDirectory(mPath);
            string vFilePath = "";
            for (int i = 0; i < 9; i++)
            {
                FileStream vStream;
                vFilePath = mPath +Path.DirectorySeparatorChar + "Sensor" + i + ".csv";
                using (vStream = new FileStream(vFilePath, FileMode.Create))
                {
                    using (StreamWriter vWriter = new StreamWriter(vStream))
                    {
                        WriteHeading(vWriter);
                        for (int j = 0; j < mContents.Count; j++)
                        {
                            var vLine = mContents[j].ReturnSensorValues(i);
                            if (!string.IsNullOrEmpty(vLine))
                            {
                                vWriter.WriteLine(vLine);
                            }
                        }
                    }
                }
            }



        }

        private void WriteHeading(StreamWriter vWriter)
        {
            string vTimeStamp = "Time(ms),";
            string vHeading = "Qx,";
            vHeading += "Qy,";
            vHeading += " Qz,";
            vHeading += " Qw,";
            vHeading += " Mx,";
            vHeading += " My,";
            vHeading += " Mz,";
            vHeading += " Ax,";
            vHeading += " Ay,";
            vHeading += " Az,";
            vHeading += " Rx,";
            vHeading += " Ry,";
            vHeading += " Rz,";
            vHeading += " H1,";
            vHeading += " P1,";
            vHeading += " R1,";
            vHeading += " CalStable,";
            vHeading += " MagTransient,";
            vWriter.Write(vTimeStamp);
            vWriter.Write(vHeading);
            vWriter.WriteLine();
        }

        public void Clear()
        {
            mContents.Clear();
        }
    }

}