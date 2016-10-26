/** 
* @file BodiesManager.cs
* @brief Contains the BodiesManager class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine; 
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.Utils;

/**
* BodiesManager class 
* @brief manager class for Bodies (interface later)
*/
public sealed class BodiesManager 
{
    #region Singleton definition
    private static readonly BodiesManager instance = new BodiesManager();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static BodiesManager()
    {
    }

    private BodiesManager()
    {
    }

    public static BodiesManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    //Bodies available
    public List<Body> Bodies = new List<Body>();

    /**
    * BodyExist()
    * @param vBodyUUID: The body UUID
    * @return bool: True if the body exists
    * @brief searches if the body exists in the manager
    */
    public bool BodyExist(string vBodyUUID)
    {
        return Bodies.Exists(x => x.BodyGuid == vBodyUUID);
    }

    /**
    * CreateNewBody()
    * @brief Creates a new body and adds it to the list
    */
    public Body CreateNewBody(string vBodyUUID = "")
    {
        Body vBody = new Body();
        if (!string.IsNullOrEmpty(vBodyUUID))
        {
            vBody.BodyGuid = vBodyUUID;
        }
        bool vIsUnityThead = OutterThreadToUnityThreadIntermediary.InUnityThread();
        vBody.InitBody(vBodyUUID, vIsUnityThead);
        Bodies.Add(vBody);
        return vBody;
    }

    /**
    * AddNewBody()
    * @param vBodyUUID: The body UUID
    * @brief Adds a body to the list
    */
    public void AddNewBody(string vBodyUUID)
    {
        //If the body doesn't exist create a new one otherwise do nothing 
        if (!BodyExist(vBodyUUID))
        {
            CreateNewBody(vBodyUUID);
        }
    }

    /**
    * GetBodyFromUUID()
    * @param vBodyUUID: The Body UUID
    * @return Body: The body found
    * @brief looks for a body by its UUID
    */
    public Body GetBodyFromUUID(string vBodyUUID)
    {
        if (BodyExist(vBodyUUID))
        {
            return Bodies.Find(x => x.BodyGuid == vBodyUUID);
        }

        return null;
    }

    /**
    * GetBodyFromRecordingUUID()
    * @param vRecUUID: The body recording UUID
    * @return Body: The body found
    * @brief looks for a body based on its recording UUID
    */
    public Body GetBodyFromRecordingUUID(string vRecUUID)
    {
        //Find the recording first
        BodyFramesRecordingBase vTempRecording = BodyRecordingsMgr.Instance.GetRecordingByUuid(vRecUUID) ;
        if(vTempRecording != null)
        {
            //Find the related body
            return GetBodyFromUUID(vTempRecording.BodyGuid);
        }
          
        return null;
    }

    

}
