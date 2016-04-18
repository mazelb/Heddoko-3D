using UnityEngine;
using System.Collections;

public class TouchMove : MonoBehaviour {
	
	public float vSpeed = 3.0f; 
	public float vRotationSpeed = 1.0f; 
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Detect mouse left clicks
		if (Input.GetMouseButtonDown(0)) {
			// Check if the GameObject is clicked by casting a
			// Ray from the main camera to the touched position.
			Ray ray = Camera.main.ScreenPointToRay 
				(Input.mousePosition);
			
			RaycastHit hit = new RaycastHit();
			
			// Cast a ray of Distance 100, and check if this
			// collider is hit.
			if (GetComponent<Collider>().Raycast (ray, out hit, 100.0f)) {
				// Log a debug message 
				// Move the target forward
				transform.Translate(Vector3.forward * vSpeed);       
				// Rotate the target along the y-axis
				transform.Rotate(Vector3.up * vRotationSpeed);
			} 
		}
	}
}
