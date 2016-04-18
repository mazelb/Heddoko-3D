using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour 
{
	public Camera nodCamera = null;
	public Camera stretchCamera = null;

	// Use this for initialization
	void Start () 
	{
		if(nodCamera != null && stretchCamera != null)
		{
			nodCamera.enabled = false;
			stretchCamera.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.C) && nodCamera != null && stretchCamera != null) 
		{
			if(nodCamera.enabled)
			{
				nodCamera.enabled = false;
				stretchCamera.enabled = true;
			}
			else
			{
				nodCamera.enabled = true;
				stretchCamera.enabled = false;
			}
		}
	}
}
