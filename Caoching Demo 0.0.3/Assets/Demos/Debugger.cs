using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Body_Data.view;


public class Debugger : MonoBehaviour {

    public BodyFrame CurrentBodyFrame;
    public BodyFrame InitialBodyFrame;
    public Text Updater;
    public BodyView View;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(View!= null)
        {
            //string debugLog = "Current Frame Data" + View.AssociatedBody.CurrentBodyFrame.FrameData[BodyStructureMap.SensorPositions.SP_UpperSpine];
          //  debugLog += "\n Initial Frame data " + View.AssociatedBody.InitialBodyFrame.FrameData[BodyStructureMap.SensorPositions.SP_UpperSpine];
            //Updater.text = debugLog;
        }
        
    }

 
}
    