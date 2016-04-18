
/** 
* @file MoveAlongBezierCurve.cs
* @brief Contains the MoveAlongBezierCurve class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Script that allows an object to move along a  passed in Bezier Curve
/// </summary>
public class MoveAlongBezierCurve : MonoBehaviour
{
    [SerializeField]
    private BezierCurve Curve;

    public float timeSpeed = 1;
    public float minimumDistance = 0.01f;
    private int mIndex;
    void Start()
    {
      //  StartCoroutine(FollowPath());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(MoveNext());
        }
    }

    IEnumerator MoveNext()
    {
        float vStartTime = 0;
        int vIndexLength = Curve.pointCount;
        
        while (true)
        {
            try
            {
                if (mIndex >= Curve.pointCount)
                {
                    mIndex = 0;
                }
                int nextIndex = mIndex + 1;
                if (mIndex == Curve.pointCount - 1)
                {
                    nextIndex = 0;
                }
                vStartTime += Time.deltaTime;
                float vPercentage = vStartTime / timeSpeed;
                Vector3 vNewPosition = BezierCurve.GetPoint(Curve[mIndex], Curve[nextIndex], vPercentage);
                if (vPercentage >= 1)
                {
                    vStartTime = 0;

                    mIndex++;
                    break;

                }
                transform.position = vNewPosition;
            }
            catch (IndexOutOfRangeException)
            {
                Debug.Log("here");
            }

            yield return null;
        }
    }
    IEnumerator FollowPath()
    {
        float vStartTime = 0;
        int vIndexLength = Curve.pointCount;
        int vIndex = 0;
        while (true)
        {
            try
            {
                if (vIndex >= Curve.pointCount)
                {
                    vIndex = 0;
                }
                int nextIndex = vIndex + 1;
                if (vIndex == Curve.pointCount - 1)
                {
                    nextIndex = 0;
                }
                vStartTime += Time.deltaTime;
                float vPercentage = vStartTime / timeSpeed;
                Vector3 vNewPosition = BezierCurve.GetPoint(Curve[vIndex], Curve[nextIndex], vPercentage);
                if (vPercentage >= 1)
                {
                    vStartTime = 0;

                    vIndex++;


                }
                transform.position = vNewPosition;
            }
            catch (IndexOutOfRangeException)
            {
                Debug.Log("here");
            }

            yield return null;
        }

    }
}
