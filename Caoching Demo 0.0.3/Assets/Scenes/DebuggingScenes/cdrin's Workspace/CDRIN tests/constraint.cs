using UnityEngine;
using System.Collections;

public class constraint : MonoBehaviour
{
	//public Vector3 axis = Vector3.forward;
	//public Vector2 minMax = new Vector2(0, 90);
	public Vector3 MaxEulerConsttraint = new Vector3(0,0,90); 

	public Vector3 eulerEnd = new Vector3(0,0,150);
	public Quaternion EndQuat;
	private Quaternion beginQuat;

	public Vector3 currentEuler;
	public Vector3 ProjectedAngles;

	// Use this for initialization
	void Start ()
	{
		beginQuat = transform.localRotation;
	}
	
	static int i = 0;
	bool pos = true;
	// Update is called once per frame
	void Update ()
	{
		EndQuat = Quaternion.Euler(eulerEnd);

		if (pos) ++i;
		else --i;
		//EndQuat = Quaternion.Euler(eulerEnd);
		Quaternion tQuat = Quaternion.Slerp(beginQuat, EndQuat, i*0.01f);

		if (Quaternion.Angle(tQuat, EndQuat) < 3)
		{
			pos = false;
		}
		else if (Quaternion.Angle(tQuat, beginQuat) < 3)
		{
			pos = true;
		}

		transform.localRotation = Constraint(tQuat);
	}

	Quaternion Constraint(Quaternion aQuat)
	{
		currentEuler = aQuat.eulerAngles;
		ExtractAngles(aQuat, ref ProjectedAngles);
		ClampAngle();
		aQuat = Quaternion.Euler(currentEuler);

		return aQuat;
	}



	private void ExtractAngles(Quaternion tQuatLocal, ref Vector3 tAngles)
	{
		Vector3 localForward = tQuatLocal * Vector3.forward;
		Vector3 localRight = tQuatLocal * Vector3.right;
		Vector3 localUp = tQuatLocal * Vector3.up;

		Vector3 Pitchcomp = Vector3.ProjectOnPlane(localUp, Vector3.right);   // pitch angle
		Vector3 Yawcomp = Vector3.ProjectOnPlane(localForward, Vector3.up);        //yaw angle 
		Vector3 Rollcomp = Vector3.ProjectOnPlane(localRight, Vector3.forward);  // roll angle

		tAngles.x = Vector3.Angle(Vector3.up, Pitchcomp);
		tAngles.y = Vector3.Angle(Vector3.forward, Yawcomp);
		tAngles.z = Vector3.Angle(Vector3.right, Rollcomp);
	}

	private void ClampAngle()
	{
		if (currentEuler.x > MaxEulerConsttraint.x) {currentEuler.x = MaxEulerConsttraint.x;       pos = !pos; }
		if (currentEuler.y > MaxEulerConsttraint.y) {currentEuler.y = MaxEulerConsttraint.y;       pos = !pos; }
		if (currentEuler.z > MaxEulerConsttraint.z) { currentEuler.z = MaxEulerConsttraint.z;      pos = !pos; }


	}
}
