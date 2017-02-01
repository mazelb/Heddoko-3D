        using UnityEngine;
using System.Collections;

public class cccccccc : MonoBehaviour
{
    public Transform Copy;
    public Vector3 CurrentRot;
    public Vector3 Euler;
	// Use this for initialization
	void Start () {
	
	}

    private float lastYValue;
	// Update is called once per frame
	void Update ()
	{
        if(Copy!=null)
        {
             
            CurrentRot = Copy.transform.rotation.eulerAngles;
            Euler = Copy.transform.rotation.eulerAngles;
            var vClamped = ClampAngle(CurrentRot.y + 180, -180, 180) ;
            var vCurr = ClampAngle(CurrentRot.y,-180,180);
            var vClosest = ClosestAngle(lastYValue, vCurr, vClamped);
            CurrentRot.x  = ClampAngle(CurrentRot.x + 180, 0, 360);
            CurrentRot.z  = ClampAngle(CurrentRot.z + 180, 0, 360);
            CurrentRot.y = vClosest;
            transform.rotation = Quaternion.Euler(CurrentRot);
            Debug.DrawLine(transform.position, (transform.position + transform.forward).normalized * 3f, Color.blue, 0.1f);
        }
    
	}

    void LateUpdate()
    {
        lastYValue = Euler.y;
    }
    private static float ClampAngle(float target, float minValue, float maxValue)
    {
        float clampedValue;

        target = Mathf.Repeat(target, 360);
        if (maxValue >= minValue)
        {
            clampedValue = Clamp(target, minValue, maxValue);
        }
        else
        {
            minValue -= 360;
            target -= 360;
            if (maxValue > 180)
            {
                maxValue -= 360;
                clampedValue = Clamp(target, maxValue, minValue);
            }
            else
            {
                clampedValue = Clamp(target, minValue, maxValue);
            }

            clampedValue = Mathf.Repeat(clampedValue, 360);
        }
        return clampedValue;
    }
    private static float Clamp(float target, float min, float max)
    {
        if (target >= min && target <= max)
            return target;

        return ClosestAngle(target, min, max);


    }

    private static float ClosestAngle(float target, float min, float max)
    {
        var minMin = float.MaxValue;
        var minMax = float.MaxValue;
        for (var i = -1; i <= 1; ++i)
        {
            var tar = (target + (360 * i));
            var disMin = Mathf.Abs(min - tar);
            if (minMin > disMin)
            {
                minMin = disMin;
            }
            var disMax = Mathf.Abs(max - tar);
            if (minMax > disMax)
            {
                minMax = disMax;
            }
        }
        return minMax > minMin ? min : max;
    }
}
