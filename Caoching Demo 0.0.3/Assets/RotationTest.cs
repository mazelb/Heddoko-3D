using UnityEngine;
using System.Collections;

public class RotationTest : MonoBehaviour
{

    public Vector4 Negator = Vector4.one;
    [Range(0,6)]
    public int caseSwitch = 0;
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.A))
	    {
	        Reset();
	    }
	    if (Input.GetKeyDown(KeyCode.S))
	    {
	        Set();
	    }
        
	}

    void Reset()
    {
        transform.rotation = Quaternion.identity;
    }

    void Set()
    {
        Quaternion vRot = transform.rotation;
        switch (caseSwitch)
        {
            case 0:
                Debug.Log("xzyw");
                SetRot(vRot.x,vRot.z, vRot.y,vRot.w);
                break;
            case 1:
                Debug.Log("zxyw");
                SetRot(vRot.z, vRot.x, vRot.y, vRot.w);
                break;
            case 2:
                Debug.Log("yxzw");
                SetRot(vRot.y, vRot.x, vRot.z, vRot.w);
                break;
            case 3:
                Debug.Log("yzxw");
                SetRot(vRot.y, vRot.z, vRot.x, vRot.w);
                break;
            case 4:
                Debug.Log("zyxw");
                SetRot(vRot.z, vRot.y, vRot.x, vRot.w);
                break;
            case 5:
                Debug.Log("x-zyw");
                SetRot(vRot.x,-vRot.z, vRot.y, vRot.w);
                break;
            case 6:
                Debug.Log("xz-yw");
                SetRot(vRot.x, -vRot.y, vRot.z, vRot.w);
                break;
        } 
    }

    void SetRot(float x, float y, float z, float w)
    {
        Quaternion vRot = transform.rotation;
        vRot.x =x;
        vRot.y =y;
        vRot.z=z;
        vRot.w =w;
        transform.rotation = vRot;
    }

}
