using UnityEngine;
using System.Collections;

public class ProjectedAngles : MonoBehaviour
{
	public Vector3 SetParentEuler;
	public Vector3 SetReferenceEuler;
	public Vector3 m_ReferenceProjectedAngles;
	public Vector3 m_WantedAngles;
	public Vector3 currentEuler;
	//public Vector3 m_CurrentProjectedLocalAngles;

	public Transform other;
	

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.parent.rotation = Quaternion.Euler(SetParentEuler);
		other.rotation = Quaternion.Euler(SetReferenceEuler);
		ExtractAngles(other.localRotation, ref m_ReferenceProjectedAngles);
		//m_WantedAngles = m_ReferenceProjectedAngles;
		//transform.localRotation = ComputeQuaternion(m_WantedAngles);
		//currentEuler = transform.rotation.eulerAngles;
	}

	public static void ExtractAngles(Quaternion a_QuatLocal, ref Vector3 a_Angles)
	{
		Vector3 localForward = a_QuatLocal * Vector3.forward;
		Vector3 localRight = a_QuatLocal * Vector3.right;
		Vector3 localUp = a_QuatLocal * Vector3.up;

		Vector3 upProj = Vector3.ProjectOnPlane(localUp, Vector3.right)     ;   // pitch angle
		Vector3 forwardProj   = Vector3.ProjectOnPlane(localForward, Vector3.up)   ;        //yaw angle 
		Vector3 rightProj  = Vector3.ProjectOnPlane(localRight, Vector3.forward);  // roll angle

		upProj.Normalize();
		forwardProj.Normalize()  ;
		rightProj.Normalize();

		a_Angles.x = Vector3.Angle(Vector3.up, upProj);
		a_Angles.y = Vector3.Angle(Vector3.forward, forwardProj);
		a_Angles.z = Vector3.Angle(Vector3.right, rightProj);
	}

	public static Quaternion ComputeQuaternion(Vector3 a_WantedAngles)
	{
		//Vector3 localForward = Vector3.forward;
		//Vector3 localRight = Vector3.right;
		//Vector3 localUp = Vector3.up;
		//
		//localUp.y = Mathf.Cos(a_WantedAngles.x * Mathf.Deg2Rad);
		//localUp.z = Mathf.Sin(a_WantedAngles.x * Mathf.Deg2Rad);
		//
		//localForward.x = Mathf.Sin(a_WantedAngles.y * Mathf.Deg2Rad);
		//localForward.z = Mathf.Cos(a_WantedAngles.y * Mathf.Deg2Rad);
		//
		//localRight.x = Mathf.Cos(a_WantedAngles.z * Mathf.Deg2Rad);
		//localRight.y = Mathf.Sin(a_WantedAngles.z * Mathf.Deg2Rad);
		//
		//
		//Vector3 tt = Vector3.Cross(localUp, localForward);
		//
		//Quaternion tret = Quaternion.LookRotation(localForward, localUp);
		//
		//Quaternion tret = Quaternion.Euler(0, a_WantedAngles.y, 0) * Quaternion.Euler(a_WantedAngles.x, 0, 0) * Quaternion.Euler(0, 0, a_WantedAngles.z);
		Quaternion tret = Quaternion.Euler(a_WantedAngles);
		return tret;
	}


}
