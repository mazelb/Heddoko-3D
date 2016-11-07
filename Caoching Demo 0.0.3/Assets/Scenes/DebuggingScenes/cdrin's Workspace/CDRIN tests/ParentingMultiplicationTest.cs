using UnityEngine;
using System.Collections;

public class ParentingMultiplicationTest : MonoBehaviour
{
	//public bool local = false;
	//public bool WegalXL = true;
	public Vector3 axis = Vector3.right;
	public float angle = 45;
	public Vector3 Resultat;
	public Transform referenceUnparented;

	// Use this for initialization
	void Start ()
	{
		if (transform.parent == null)
		{
			//Debug.LogWarning("no Start");
			return;
		}
		Debug.Log("Start!");

		if(referenceUnparented == null)
		{
			Debug.LogError("no reference!");
		}

		TestQuaternion();

		
	}


	// Update is called once per frame
	void Update ()
	{
		if (transform.parent == null)
		{
			return;
		}
	}

	void TestQuaternion()
	{

		//Quaternion childglob = transform.rotation;
		//Quaternion childloc = transform.localRotation;

		//Quaternion parentglob = transform.parent.rotation;
		//Quaternion parentloc = transform.parent.localRotation;
		Quaternion tQuat;

		tQuat = TestWegalXL();

		Resultat = tQuat.eulerAngles;

		transform.localRotation = tQuat;

	}

	Quaternion TestWegalXL()
	{

		Quaternion t_currentWorld = transform.rotation;
		Quaternion t_currentLocal = transform.localRotation;

		Quaternion parentglob = transform.parent.rotation;
		Quaternion parentloc = transform.parent.localRotation;

		Quaternion t_LocalToWorld = t_currentWorld * Quaternion.Inverse(t_currentLocal);
		Quaternion t_WorldToLocal = Quaternion.Inverse(t_LocalToWorld);
		Quaternion tQuat = Quaternion.AngleAxis(angle, axis);

		if (referenceUnparented != null)
			referenceUnparented.rotation = tQuat;

		tQuat = t_WorldToLocal * tQuat;

		return tQuat;
	}
	
}
