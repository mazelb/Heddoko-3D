﻿/** 
* @file CsvBodyFramesRecording.cs
* @brief Contains the CsvBodyFramesRecording class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Frames_Pipeline; 
using UnityEngine;

namespace Assets.Scripts.Frames_Recorder.FramesRecording
{ /**
* CsvBodyFramesRecording class 
* @brief Class containing raw frames from Recorded CSV file
* CSV line structure (single frame):
* RECORDING GUID
* CONTAINING BODY GUID
* SUIT GUID
* BOIMECH_sensorID_1, Yaw;Pitch;Roll, ... BOIMECH_sensorID_9, Yaw;Pitch;Roll, FLEXCORE_sensorID_1, SensorValue, ... ,FLEXCORE_sensorID_4, SensorValue
*/
    [Serializable]
    public class CsvBodyFramesRecording : BodyFramesRecordingBase
    {
        //Number of UUIDs (lines) before the beginning of the frames recorded in CSV
        [SerializeField]
        //Recording content
        public List<BodyRawFrame> RecordingRawFrames = new List<BodyRawFrame>();
        [SerializeField]
        //current raw frame index
        private int currentRawFrameIndex;
        // statistics of a recording
        // public RecordingStats Statistics = new RecordingStats();

        public bool FromDatFile { get; set; }

     

        /// <summary>
        /// Extracts raw frame data found in a csv recording reader.
        /// </summary>
        /// <param name="vRecordingReader"></param>
        internal void ExtractRawFramesData(CsvBodyRecordingReader vRecordingReader)
        {
            ExtractRawFramesData(vRecordingReader.GetRecordingLines());
        }

        [SerializeField]
        public override string FormatRevision { get; set; }
        /// <summary>
        /// The title of the recording
        /// </summary>
        [SerializeField]
        public override string Title { get;   set; }

        /**
    * CreateNewRecordingUUID()
    * @brief Creates a new recording UUID
    */

        public void CreateNewRecordingUUID()
        {
            BodyRecordingGuid = Guid.NewGuid().ToString();
        }

        /**
    * AddRecordingRawFrame()
    * @param vFrame: The frame to add to the recording
    * @brief Adds a frame to the recording
    */

        public void AddRecordingRawFrame(BodyRawFrame vFrame)
        {
            RecordingRawFrames.Add(vFrame);
        }

        public void SaveRecording()
        {
            //TODO:
        }

        /**
    * GetNextFrame()
    * @param 
    * @brief Adds a frame to the recording
    * @return Returns the next frame from the Raw Frame Recordings
    */

        /**
     * PopulateRecordingUUIDs()
     * @param vRecordingLines: The recording content
     * @brief Gets the UUID of the recording and Body from the content
     */

        public void ExtractRecordingUUIDs(string[] vRecordingLines)
        {
            //The minimum amount of lines in the recording
            if (vRecordingLines.Length > sNumberOfUUIDs)
            {
                ExtractRecordingUUIDsHelper(vRecordingLines[0], ref BodyRecordingGuid);
                ExtractRecordingUUIDsHelper(vRecordingLines[1], ref BodyGuid);
                ExtractRecordingUUIDsHelper(vRecordingLines[2], ref SuitGuid);
            }
        }
        
        /// <summary>
        /// Helper to extract recording UUID, that validates a  recording line, checking if the line is 
        /// in the UUID format
        /// </summary>
        /// <param name="vRecordingLine">The recording line to validate</param>
        /// <param name="vUUID">The uuid that will store the new uuid extracted from vRecordingline.</param>
        private void ExtractRecordingUUIDsHelper(string vRecordingLine, ref string vUUID)
        {
            if (vRecordingLine.Split('-').Length - 1 != 4)
            {
                vUUID = Guid.NewGuid().ToString();
            }
            else
            {
                vUUID = vRecordingLine;
            }
        }

        /**
    * PopulateRawFramesData()
    * @param vRecordingLines: The recording full content
    * @brief Creates raw frames from CSV data
    */

        public void ExtractRawFramesData(string[] vRecordingLines)
        {
            //The minimum amount of lines in the recording
            if (vRecordingLines.Length > sNumberOfUUIDs)
            {
                //Get the data line by line and add them as frames 
                for (uint i = (sNumberOfUUIDs); i < vRecordingLines.Length; i++)
                {
                    BodyRawFrame vTempRaw = new BodyRawFrame();
                    // vTempRaw.IsDecoded = !FromDatFile;
                    vTempRaw.BodyRecordingGuid = BodyRecordingGuid;
                    vTempRaw.BodyGuid = BodyGuid;
                    vTempRaw.SuitGuid = SuitGuid;
                    vTempRaw.IsDecoded = !FromDatFile;
                    vTempRaw.RawFrameData = vRecordingLines[i].Split(",".ToCharArray(),
                        StringSplitOptions.RemoveEmptyEntries);
                    RecordingRawFrames.Add(vTempRaw);

                }
            }
            //analyze statistics of a current recording
  
        }

    
        //    //Send each line to a BodyFrameStream
        //    //Each line start by sending Frame start "S"
        //    //Send the frame data 
        //    //Send Frame End: "E"

        //private void TransmitAllFrames()
        //{
        //    foreach (string vFileLine in mFileLines)
        //    {
        //        //string[] vFrameData = vFileLine.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //    }

        //    //Send each line to a BodyFrameStream
        //    //Each line start by sending Frame start "S"
        //    //Send the frame data 
        //    //Send Frame End: "E"
        //}

   

        public override int RecordingRawFramesCount
        {
            get
            {
                int vCount = 0;
                if (RecordingRawFrames != null)
                {
                    vCount = RecordingRawFrames.Count;
                }
                return vCount;
            } 
        }


        public override BodyRawFrameBase GetBodyRawFrameAt(int vIndex)
        {
            return RecordingRawFrames[vIndex];
        }

        /// <summary>
        /// Extracts raw data from a recording reader base. 
        /// </summary>
        /// <param name="vRecordingReaderbase"></param>
        public override void ExtractRawFramesData(BodyRecordingReaderBase vRecordingReaderbase)
        {
            FromDatFile = vRecordingReaderbase.FilePath.EndsWith("dat");
            ExtractRawFramesData((CsvBodyRecordingReader) vRecordingReaderbase);
        }
    }

    /// <summary>
    /// The recording frames type
    /// </summary>
    public enum BodyFramesRecordingType
    {
        Proto,
        Csv
    }
}