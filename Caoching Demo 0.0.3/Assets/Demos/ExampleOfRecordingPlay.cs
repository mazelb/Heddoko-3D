/** 
* @file ExampleOfRecordingPlay.cs
* @brief Contains the ExampleOfRecordingPlay class and functionalities
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
*/

using System.Collections;
using Assets.Demos;
using Assets.Scripts.Body_Data;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;

/**
* BodyView class 
* @brief Contains an example on how to play a Body's recording
*/
public class ExampleOfRecordingPlay : MonoBehaviour
{

    private Body mBody;

    public Button PlayButton;
    private bool mPlayButtonPushed;
    public Button ResetButton;
    public string mBodyRecordingUUID = "FBBE8B37-151D-4D8A-8138-CC2BAAFA5BF7";
    public float PauseThreadTimer = 1f;
    private float mInternalTimer = 1f;
    private bool mResetRoutStarted = false; // pause thread routine started
    public DisplayAngleExtractions DisplayAngleExtractions;
    /**
    * Start()
    * @brief Automatically called by unity on start. This start function will prep the body to be able to play a recording 
    */
    private void Start()
    {
        //scan recordings in directory
        BodyRecordingsMgr.Instance.ScanRecordings(FilePathReferences.RecordingsDirectory);

        //read the recordings
        BodyRecordingsMgr.Instance.ReadAllRecordings();

        //set the body
        mBody = BodiesManager.Instance.GetBodyFromRecordingUUID(mBodyRecordingUUID);
    }

    /**
    * Play 
    * @brief Will play the recording for the prepped body
    */
    public void Play()
    {
        if(!mPlayButtonPushed)
        {
            if(mBody != null)
            {
                mPlayButtonPushed = true;
                PlayButton.gameObject.SetActive(false);
                mBody.PlayRecording(mBodyRecordingUUID);
                DisplayAngleExtractions.CurrentBody = mBody;
            }
        }        
    }

    /**
    * ResetOrientations 
    * @brief will set the Initial Frame
    */
    public void ResetInitialFrame()
    {
        mBody.View.ResetInitialFrame();
        //StartCoroutine(StartCountdown());
    }

    /**
    * Pause 
    * @brief Pauses the recording's play back
    */
    public void ChangePauseState()
    {
        mBody.View.PauseFrame();
    }

    private IEnumerator StartCountdown()
    {
        if (mResetRoutStarted)
        {
            mInternalTimer += PauseThreadTimer; //if this has already started, just add to the timer and then exit
            yield  break;
        }
        mInternalTimer = PauseThreadTimer;
        mResetRoutStarted = true;
        ChangePauseState();
        while (true)
        {
            mInternalTimer -= Time.deltaTime;
            if (mInternalTimer < 0)
            {
                break;
            }
            yield return null;
        }
        ChangePauseState();
        mResetRoutStarted = false;
    }

}
