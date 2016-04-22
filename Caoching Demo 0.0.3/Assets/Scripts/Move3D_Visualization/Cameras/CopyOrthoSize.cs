using UnityEngine;
using System.Collections;

public class CopyOrthoSize : MonoBehaviour
{

    private Camera mThisCamera;
    public Camera ToCopy;
	// Use this for initialization
	void Start ()
	{
	    mThisCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    mThisCamera.orthographicSize = ToCopy.orthographicSize;
	}
}
