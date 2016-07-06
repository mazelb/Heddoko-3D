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
    public delegate void BodyFramesRecordingFoundDel(BodyFramesRecording vRecording);
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
    public List<BodyFramesRecording> Recordings = new List<BodyFramesRecording>();

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
        vDatFilePaths = vDatFilePaths.Where(vInvalid => vInvalid.IndexOf("logindex",StringComparison.OrdinalIgnoreCase) <0).ToArray();
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

        BodyRecordingReader vTempReader = new BodyRecordingReader(vFilePath);

        if (vTempReader.ReadFile(vTempReader.FilePath) > 0)
        {
            AddNewRecording(vTempReader.GetRecordingLines()  );
        }

    }

 
    private void ReadCallback( object threadContext)
    {
        if (threadContext is Array)
        {
            object[] vObjectArray = (object[]) threadContext;
          
            BodyRecordingReader vTempReader = vObjectArray[0] as BodyRecordingReader;
            if (vTempReader != null)
            {
                StopActionEvent += vTempReader.Stop;
                Action<BodyFramesRecording> vCallbackAction = (Action<BodyFramesRecording>) vObjectArray[1];

                if (vTempReader.ReadFile(vTempReader.FilePath) > 0)
                {
                    string vVersion = vTempReader.CrytoManager.GetCrytpoVersion;
                    AddNewRecording(vTempReader.GetRecordingLines(), vTempReader.FilePath,vTempReader.IsFromDatFile,vVersion, vCallbackAction);
                }
                StopActionEvent -= vTempReader.Stop;
            }
        }
        else
        {
            //todo: add listener here
            BodyRecordingReader vTempReader = (BodyRecordingReader)threadContext;
            StopActionEvent += vTempReader.Stop;
            if (vTempReader.ReadFile(vTempReader.FilePath) > 0)
            {
                try
                {
                    AddNewRecording(vTempReader.GetRecordingLines());
                }
                catch
                {
                    // ignored
                }
            }
            StopActionEvent -= vTempReader.Stop;
        }


    }

    /// <summary>
    /// Reads a specific recording file with a callback action on completion of the file read
    /// </summary>
    /// <param name="vFilePath">the path of the file</param>
    /// <param name="vCallbackAction">the callback action that accepts a BodyFrameRecording</param>
    public void ReadRecordingFile(string vFilePath, Action<BodyFramesRecording> vCallbackAction)
    {
        //Read recording file
        //ignore meta files
        if (vFilePath.EndsWith("meta"))
        {
            return;
        }

        BodyRecordingReader vTempReader = new BodyRecordingReader(vFilePath);
        Thread vThread = new Thread(() =>
        {
            ReadCallback(new object[] {vTempReader, vCallbackAction});
        });
        vThread.Start(); 
    }
 
 
    /**
    * AddNewRecording()
    * @param vRecordingLines: The recording file content in lines
    * @brief Adds a recording to the list
    */
    public void AddNewRecording(string[] vRecordingLines)
    {
        BodyFramesRecording vTempRecording = new BodyFramesRecording();

        vTempRecording.ExtractRecordingUUIDs(vRecordingLines);

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
    }

    /// <summary>
    /// Adds a recording to the list, with a callback performed on completion
    /// </summary>
    /// <param name="vRecordingLines">the lines of recordings</param>
    /// <param name="vrxFromDatFile">was the source received from a dat file?</param>
    /// <param name="vCallbackAction">the callback action with a BodyFramesRecording parameter</param>
    public void AddNewRecording(string[] vRecordingLines,string vTitle, bool vrxFromDatFile,string vVersion, Action<BodyFramesRecording> vCallbackAction)
    {
        BodyFramesRecording vTempRecording = new BodyFramesRecording {FromDatFile = vrxFromDatFile};
        vTempRecording.Title = vTitle;
        vTempRecording.ExtractRecordingUUIDs(vRecordingLines);
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
    * @return List<BodyFramesRecording>: the list of recordings assigned to the body
    * @brief returns all the recordings assigned to a body if they exist
    */
    public List<BodyFramesRecording> GetRecordingsForBody(string vBodyUuid)
    {
        //look for the recording only if the body exists
        if (BodiesManager.Instance.BodyExist(vBodyUuid))
        {
            //get the recordings from the list of recording IDs assigned to that body
            List<string> vListOfRecordingIds;
            List<BodyFramesRecording> vListOfRecordings = new List<BodyFramesRecording>();
            if (RecordingsDictionary.TryGetValue(vBodyUuid, out vListOfRecordingIds))
            {
                for (int vIndex = 0; vIndex < vListOfRecordingIds.Count; vIndex++)
                {
                    BodyFramesRecording vRecording = GetRecordingByUuid(vListOfRecordingIds[vIndex]);
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
    * @return BodyFramesRecording: The recording
    * @brief looks for a recording by its UUID
    */
    public BodyFramesRecording GetRecordingByUuid(string vRecUuid )
    {
        if (RecordingExist(vRecUuid))
        {
            return Recordings.Find(vX => vX.BodyRecordingGuid == vRecUuid);
        }
        
            //Get the recording from the database
            BodyFramesRecording vNewRecording = Database.Connection.GetRawRecording(vRecUuid);
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
        BodyFramesRecording vRecording = GetRecordingByUuid(vRecUuid);
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
        public Action<BodyFramesRecording> CallbackAction;

        public FilePathReqCallback(string vFilePath, Action<BodyFramesRecording> vCallbackAction)
        {
            FilePath = vFilePath;
            CallbackAction = vCallbackAction;
        }
    }

  
}
