using UnityEngine;
using System.Collections;

public class RotateSerialViewer : MonoBehaviour
{

    public Transform SegmentToView;
    public static Vector3 GXRawVector;
    public static Vector3 GYRawVector;
    public static Vector3 GZRawVector;

    public Vector3 XRawVector;
    public Vector3 YRawVector;
    public Vector3 ZRawVector;

    public static Vector3 GXFirstProccessedVector;
    public static Vector3 GYFirstProccessedVector;
    public static Vector3 GZFirstProccessedVector;
    public static Vector3 GPniX { get; set; }
    public static Vector3 GPniY { get; set; }
    public static Vector3 GPniZ { get; set; }
    
     public static Vector3 GUnityOrthoNormalPni { get; set; }

    public Vector3 XFirstProccessedVector;
    public Vector3 Pnix;
    public Vector3 PniY;
    public Vector3 PniZ;
    public Vector3 YFirstProccessedVector;
    public Vector3 ZFirstProccessedVector;

    public static Vector3 GCrossedZX;
    public Vector3 CrosseedZX; 
 
    // Update is called once per frame
    void Update()
    {
        XRawVector = GXRawVector;
        YRawVector = GYRawVector;
        ZRawVector = GZRawVector;

        XFirstProccessedVector = GXFirstProccessedVector;
        YFirstProccessedVector = GYFirstProccessedVector;
        ZFirstProccessedVector = GZFirstProccessedVector;

        CrosseedZX = GCrossedZX;
        Pnix = GPniX;
        PniY = GPniY;
        PniZ = GPniZ;
     }

    void OnDrawGizmos()
    {
        if (SegmentToView != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(SegmentToView.transform.position, GUnityOrthoNormalPni * 10f);
        }
         
         
    }
}
