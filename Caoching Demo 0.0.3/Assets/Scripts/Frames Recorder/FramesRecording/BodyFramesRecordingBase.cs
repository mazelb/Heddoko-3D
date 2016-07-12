using System;
using Assets.Scripts.Frames_Recorder.FramesReader;
using Assets.Scripts.UI.Tagging;
using System.Collections.Generic;
using Assets.Scripts.Frames_Pipeline;
using UnityEngine;

namespace Assets.Scripts.Frames_Recorder.FramesRecording
{
    public abstract  class BodyFramesRecordingBase
    {
        public static uint sNumberOfUUIDs = 3;

        [SerializeField]
        //Recording Unique GUID for ease of cloud access
        public String BodyRecordingGuid;

        [SerializeField]
        //Unique GUID of the Body it belongs to
        public string BodyGuid;

        public DateTime CreationTime = DateTime.Now;
        public string BrainpackFirmwareVersion;
        [SerializeField]
        //Unique GUID of the suit it belongs to
        public string SuitGuid;

        public abstract int RecordingRawFramesCount { get;  }
        public abstract string FormatRevision { get; set; }
        /// <summary>
        /// tags attached to this recording
        /// </summary>
        [SerializeField]
        public List<Tag> Tags = new List<Tag>();
        public abstract string Title { get; set; }
        public abstract BodyRawFrameBase GetBodyRawFrameAt(int vIndex); 
        /// <summary>
        /// ExtractRawFramesData from a BodyRecordingReader
        /// </summary>
        /// <param name="vRecording"></param>
        /// <param name="vReaderBase">the recording reader to extract raw frames data from</param>
        public static BodyFramesRecordingBase ExtractRawFramesDataFromRecordingBase(ref BodyFramesRecordingBase vRecording, BodyRecordingReaderBase vReaderBase)
        {
            //get the type of recording reader
            Type vType = vReaderBase.GetType();
            if (vType == typeof(CsvBodyRecordingReader))
            { 
                vRecording.ExtractRawFramesData((CsvBodyRecordingReader)vReaderBase);
                return vRecording;
            }
            else if (vType == typeof(ProtoBodyRecordingReader))
            { 
                vRecording.ExtractRawFramesData((ProtoBodyRecordingReader)vReaderBase);
                return vRecording;
            }

            return null;
        }

        public abstract void ExtractRawFramesData(BodyRecordingReaderBase vRecordingReaderbase);

        /// <summary>
        /// A factory of BodyFramesRecordings. Creates a BodyFramesRecording based on the type of reader passed in. 
        /// Will not extract recordings. Will set the Recording's uuids
        /// </summary>
        /// <param name="vReaderBase">the reader to extract data from</param>
        /// <returns></returns>
        public static BodyFramesRecordingBase RecordingFactory(BodyRecordingReaderBase vReaderBase)
        {
            Type vType = vReaderBase.GetType();
            if (vType == typeof(CsvBodyRecordingReader))
            {
                var vRecordingReader = vReaderBase as CsvBodyRecordingReader;
                CsvBodyFramesRecording vRecording = new CsvBodyFramesRecording();
                vRecording.FromDatFile = false;
                vRecording.ExtractRecordingUuiDs(vRecordingReader.GetRecordingLines());
                return vRecording;
            }
            else if (vType == typeof(ProtoBodyRecordingReader))
            {
                ProtoBodyFramesRecording vRecording = new ProtoBodyFramesRecording();
                vRecording.SetUids(vReaderBase.FilePath);
                return vRecording;
            }

            return null;
        }

        /// <summary>
        /// Adds a tag to the recording
        /// </summary>
        /// <param name="vTag"></param>
        public void AddTag(Tag vTag)
        {
            if (!Tags.Contains(vTag))
            {
                Tags.Add(vTag);
            }
        }
    }
}