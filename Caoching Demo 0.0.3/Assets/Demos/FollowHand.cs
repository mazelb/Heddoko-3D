using UnityEngine;
using System.Collections;

public class FollowHand : MonoBehaviour {
    public Transform followTrans;
    public ParticleSystem particles;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = followTrans.position;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "CubeLeft" || col.gameObject.name == "CubeRight")
        { 
            particles.Play();
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name == "CubeLeft" || col.gameObject.name == "CubeRight")
        { 
            particles.Stop();
        }
    }

}
