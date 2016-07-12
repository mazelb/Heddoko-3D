/** 
* @file BodyRecordingsMgr.cs
* @brief Contains the BodyRecordingsMgr class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.Frames_Recorder.FramesReader;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.Utils;


/**
* RecordingsManager class 
* @brief manager class for recordings (interface later)
*/
// ReSharper disable once CheckNamespace
public class BodyRecordingsMgr : IDatabaseConsumer
{
    /// <summary>
    /// Delegate definition of a callback function
    /// </summary>
    /// <param name="vRecording"></param>
    public delegate void BodyFramesRecordingFoundDel(BodyFramesRecordingBase vRecording);
    #region Singleton definition
    // ReSharper disable once InconsistentNaming
    private static readonly BodyRecordingsMgr instance = new BodyRecordingsMgr();
    public delegate void StopActionDelegate();
    public event StopActionDelegate StopActionEvent;

    public Database Database { get; set; }
    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static BodyRecordingsMgr()
    {
    }

    private BodyRecordingsMgr()
    {
    }

    public static BodyRecordingsMgr Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion


    //Scanned files 
    private string[] mFilePaths;

    //Main recordings directory path 
    private string mDirectoryPath;

    //Recordings available
    public List<BodyFramesRecordingBase> Recordings = new List<BodyFramesRecordingBase>();

    //Map Body UUID to Recording UUID
    Dictionary<string, List<string>> mRecordingsDictionary = new Dictionary<string, List<string>>();

    Dictionary<string, List<string>> RecordingsDictionary
    {
        get { return mRecordingsDictionary; }
    }
    public string[] FilePaths
    {
        get { return mFilePaths; }
    }


    /**
    * ScanRecordings()
    * @param vDirectoryPath: The path in which the function will scan
    * @return int: the number of files found
    * @brief Scans a specific folder for recordings
    */
    public int ScanRecordings(string vDirectoryPath)
    {
        mDirectoryPath = vDirectoryPath;
        string[] vDatFilePaths = Directory.GetFiles(mDirectoryPath, "*.dat");
        string[] vCsvFilePaths = Directory.GetFiles(mDirectoryPath, "*.csv");
        //ignore logindex.dat
        vDatFilePaths = vDatFilePaths.Where(vInvalid => vInvalid.IndexOf("logindex", StringComparison.OrdinalIgnoreCase) < 0).ToArray();
        //combine the two
        mFilePaths = new string[vDatFilePaths.Length + vCsvFilePaths.Length];
        Array.Copy(vDatFilePaths, mFilePaths, vDatFilePaths.Length);
        Array.Copy(vCsvFilePaths, 0, mFilePaths, vDatFilePaths.Length, vCsvFilePaths.Length);


        return mFilePaths.Length;
    }

    /**
    * ReadAllRecordings()
    * @brief reads all recodings in the mDirectoryPath
    */
    public void ReadAllRecordings()
    {
        //For each recording found
        for (int i = 0; i < mFilePaths.Length; i++)
        {
            ReadRecordingFile(mFilePaths[i]);
        }
    }

    /**
    * ReadRecordingFile()
    * @param vFilePath: The recording file path, no threads are used
    * @brief Reads a specific recording file
    */
    public void ReadRecordingFile(string vFilePath)
    {
        //Read recording file
        //ignore meta files
        if (vFilePath.EndsWith("meta"))
        {
            return;
        }
        //Create a Recording reader based on the file extension
        BodyRecordingReaderBase vRecordingBase = null;

        if (vFilePath.EndsWith("csv") || vFilePath.EndsWith("dat"))
        {
            vRecordingBase = new CsvBodyRecordingReader(vFilePath);


        }
        else if (vFilePath.EndsWith("hsm"))
        {
            vRecordingBase = new ProtoBodyRecordingReader(vFilePath);
        }

        if (vRecordingBase != null)
        {
            if (vRecordingBase.ReadFile(vFilePath) > 0)
            {
                AddNewRecording(vRecordingBase);
            }
        }
        //if (vTempReader.ReadFile(vTempReader.FilePath) > 0)
        //{

        //}

    }

    /// <summary>
    /// Reads a specific recording file with a callback action on completion of the file read
    /// </summary>
    /// <param name="vFilePath">the path of the file</param>
    /// <param name="vCallbackAction">the callback action that accepts a BodyFrameRecording</param>
    public void ReadRecordingFile(string vFilePath, Action<BodyFramesRecordingBase> vCallbackAction)
    {
        //Read recording file
        //ignore meta files
        if (vFilePath.EndsWith("meta"))
        {
            return;
        }
        FilePathReqCallback vCallbackstruct = new FilePathReqCallback(vFilePath, vCallbackAction);
        ThreadPool.QueueUserWorkItem(ReaderWorker, vCallbackstruct); 
    }


    private void ReaderWorker(object vCallbackStructure)
    {
        FilePathReqCallback vStructure = (FilePathReqCallback)vCallbackStructure;
        var vFilePath = vStructure.FilePath;
        BodyRecordingReaderBase vRecordingBase = null;

        if (vFilePath.EndsWith("csv") || vFilePath.EndsWith("dat"))
        {
            vRecordingBase = new CsvBodyRecordingReader(vFilePath);

        }
        else if (vFilePath.EndsWith("hsm"))
        {
            vRecordingBase = new ProtoBodyRecordingReader(vFilePath);
        }

        if (vRecordingBase != null)
        {
            if (vRecordingBase.ReadFile(vFilePath) > 0)
            {
                AddNewRecording(vRecordingBase, vStructure);
            }
        }

    }


    public void AddNewRecording(BodyRecordingReaderBase vReaderBase, FilePathReqCallback vRecordingCallback)
    {
        BodyFramesRecordingBase vTempRecording = BodyFramesRecordingBase.RecordingFactory(vReaderBase);
        if (!RecordingExist(vTempRecording.BodyRecordingGuid))
        {
            vTempRecording.ExtractRawFramesData(vReaderBase);

            //Add body to the body manager
            BodiesManager.Instance.AddNewBody(vTempRecording.BodyGuid);

            //Map Body to Recording for future play
            MapRecordingToBody(vTempRecording.BodyGuid, vTempRecording.BodyRecordingGuid);

            //Add recording to the list 
            Recordings.Add(vTempRecording);
        }
        if (vRecordingCallback.CallbackAction != null)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => vRecordingCallback.CallbackAction(vTempRecording));
        }
    }

    /// <summary>
    /// Adds a new recording from the contents found in a reader
    /// </summary>
    public void AddNewRecording(BodyRecordingReaderBase vReaderBase)
    {

        BodyFramesRecordingBase vTempRecording = BodyFramesRecordingBase.RecordingFactory(vReaderBase);
        if (!RecordingExist(vTempRecording.BodyRecordingGuid))
        {
            vTempRecording.ExtractRawFramesData(vReaderBase);

            //Add body to the body manager
            BodiesManager.Instance.AddNewBody(vTempRecording.BodyGuid);

            //Map Body to Recording for future play
            MapRecordingToBody(vTempRecording.BodyGuid, vTempRecording.BodyRecordingGuid);

            //Add recording to the list 
            Recordings.Add(vTempRecording);
        }
    }
 
 //   /**
 //* AddNewRecording()
 //* @param vRecordingLines: The recording file content in lines
 //* @brief Adds a recording to the list
 //*/
 //   public void AddNewRecording(BodyFramesRecordingBase vRecordingBase)
 //   {

 //       // CsvBodyFramesRecording vTempRecording = new CsvBodyFramesRecording();

 //       vTempRecording.ExtractRecordingUUIDs(vRecordingLines);

 //       //If recording already exists, do nothing
 //       if (!RecordingExist(vTempRecording.BodyRecordingGuid))
 //       {
 //           vTempRecording.ExtractRawFramesData(vRecordingLines);

 //           //Add body to the body manager
 //           BodiesManager.Instance.AddNewBody(vTempRecording.BodyGuid);

 //           //Map Body to Recording for future play
 //           MapRecordingToBody(vTempRecording.BodyGuid, vTempRecording.BodyRecordingGuid);

 //           //Add recording to the list 
 //           Recordings.Add(vTempRecording);
 //       }
 //   }
    /// <summary>
    /// Adds a recording to the list, with a callback performed on completion
    /// </summary>
    /// <param name="vRecordingLines">the lines of recordings</param>
    /// <param name="vrxFromDatFile">was the source received from a dat file?</param>
    /// <param name="vCallbackAction">the callback action with a CsvBodyFramesRecording parameter</param>
    public void AddNewRecording(string[] vRecordingLines, string vTitle, bool vrxFromDatFile, Action<CsvBodyFramesRecording> vCallbackAction, string vVersion ="0")
    {
        CsvBodyFramesRecording vTempRecording = new CsvBodyFramesRecording { FromDatFile = vrxFromDatFile };
        vTempRecording.Title = vTitle;
        vTempRecording.ExtractRecordingUuiDs(vRecordingLines);
        vTempRecording.FormatRevision = vrxFromDatFile ? vVersion : "0";
        //If recording already exists, do nothing
        if (!RecordingExist(vTempRecording.BodyRecordingGuid))
        {
            vTempRecording.ExtractRawFramesData(vRecordingLines);

            //Add body to the body manager
            BodiesManager.Instance.AddNewBody(vTempRecording.BodyGuid);

            //Map Body to Recording for future play
            MapRecordingToBody(vTempRecording.BodyGuid, vTempRecording.BodyRecordingGuid);

            //Add recording to the list 
            Recordings.Add(vTempRecording);
        }
        if (vCallbackAction != null)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => vCallbackAction(vTempRecording));
        }

    }


    /**
    * CreateNewRecording()
    * @brief Creates a new recording and adds it to the list
    */
    public void CreateNewRecording()
    {
        //TODO:
    }

    /**
    * MapRecordingToBody()
    * @param vBodyUUID: The containing Body UUID
    * @param vRecUUID: The recording UUID
    * @brief maps Recording UUID to Body UUID for future searches
    */
    public void MapRecordingToBody(string vBodyUuid, string vRecUuid)
    {
        if (BodiesManager.Instance.BodyExist(vBodyUuid))
        {
            List<string> vListOfRecordings;
            RecordingsDictionary.TryGetValue(vBodyUuid, out vListOfRecordings);

            //if there are no recordings, create a new mapping
            //if recordings are already mapped, add more to it
            if (vListOfRecordings == null)
            {
                vListOfRecordings = new List<string>();
                vListOfRecordings.Add(vRecUuid);
                RecordingsDictionary.Add(vBodyUuid, vListOfRecordings);
            }
            else
            {
                vListOfRecordings.Add(vRecUuid);
            }
        }
    }

    /**
    * RecordingExist()
    * @param vRecUUID: The recording UUID
    * @return bool: True if the recording exists
    * @brief searches if the recording exists in the manager
    */
    public bool RecordingExist(string vRecUuid)
    {
        return Recordings.Exists(vX => vX.BodyRecordingGuid == vRecUuid);
    }

    /**
    * GetRecordingsForBody()
    * @param vBodyUUID: The containing body UUID
    * @return List<CsvBodyFramesRecording>: the list of recordings assigned to the body
    * @brief returns all the recordings assigned to a body if they exist
    */
    public List<BodyFramesRecordingBase> GetRecordingsForBody(string vBodyUuid)
    {
        //look for the recording only if the body exists
        if (BodiesManager.Instance.BodyExist(vBodyUuid))
        {
            //get the recordings from the list of recording IDs assigned to that body
            List<string> vListOfRecordingIds;
            List<BodyFramesRecordingBase> vListOfRecordings = new List<BodyFramesRecordingBase>();
            if (RecordingsDictionary.TryGetValue(vBodyUuid, out vListOfRecordingIds))
            {
                for (int vIndex = 0; vIndex < vListOfRecordingIds.Count; vIndex++)
                {
                    BodyFramesRecordingBase vRecording = GetRecordingByUuid(vListOfRecordingIds[vIndex]);
                    if (vRecording != null)
                    {
                        vListOfRecordings.Add(vRecording);
                    }
                }

                if (vListOfRecordings.Count > 0)
                {
                    return vListOfRecordings;
                }
            }
        }

        return null;
    }

    /**
    * GetRecordingByUUID()
    * @param vRecUUID: The recording UUID
    * @return CsvBodyFramesRecording: The recording
    * @brief looks for a recording by its UUID
    */
    public BodyFramesRecordingBase GetRecordingByUuid(string vRecUuid)
    {
        if (RecordingExist(vRecUuid))
        {
            return Recordings.Find(vX => vX.BodyRecordingGuid == vRecUuid);
        }

        //Get the recording from the database
        BodyFramesRecordingBase vNewRecording = Database.Connection.GetRawRecording(vRecUuid);
        Recordings.Add(vNewRecording);
        return vNewRecording;
    }

    /// <summary>
    /// Attempt to locate a recording by its UUID and pass it off to an interested delegate
    /// </summary>
    /// <param name="vRecUuid">the recording UUID</param>
    /// <param name="vCallbackDel">the callback delegate that accepts a body frame recording</param>
    public void TryGetRecordingByUuid(string vRecUuid, BodyFramesRecordingFoundDel vCallbackDel)
    {
        BodyFramesRecordingBase vRecording = GetRecordingByUuid(vRecUuid);
        if (vCallbackDel != null)
        {
            vCallbackDel(vRecording);
        }

    }

    /// <summary>
    /// returns a list of recordings by their
    /// </summary>
    /// <param name="vFilter"></param>
    public void TryGetRecordingUuids(string vFilter)
    {

    }

    /// <summary>
    /// Sends a stop signal to any registered listeners
    /// </summary>
    public static void Stop()
    {
        if (Instance.StopActionEvent != null)
        {
            Instance.StopActionEvent.Invoke();
        }
    }
    /// <summary>
    /// A structure to hold callback requests with a string representing a file path
    /// </summary>
    public struct FilePathReqCallback
    {
        public string FilePath;
        public Action<BodyFramesRecordingBase> CallbackAction;

        public FilePathReqCallback(string vFilePath, Action<BodyFramesRecordingBase> vCallbackAction)
        {
            FilePath = vFilePath;
            CallbackAction = vCallbackAction;
        }
    }


}
