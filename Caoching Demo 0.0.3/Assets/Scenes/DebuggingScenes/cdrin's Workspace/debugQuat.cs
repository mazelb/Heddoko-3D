using UnityEngine;
using System.Collections;

public class debugQuat : MonoBehaviour
{
    public Transform reference;
    public float angle;
    public Vector3 axe;

	// Use this for initialization
	void Start ()
    {
        reference.position = transform.position + Vector3.one;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.localRotation.ToAngleAxis(out angle, out axe);
	}
}
