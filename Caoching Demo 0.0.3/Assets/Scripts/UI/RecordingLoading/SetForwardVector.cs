using UnityEngine;
using System.Collections;
using Assets.Scripts.UI;

public class SetForwardVector : MonoBehaviour
{
    public Transform Target;
 
    
	// Use this for initialization
 
	
	// Update is called once per frame
	void Update ()
	{
	    transform.forward = Target.forward; 
	}
}
