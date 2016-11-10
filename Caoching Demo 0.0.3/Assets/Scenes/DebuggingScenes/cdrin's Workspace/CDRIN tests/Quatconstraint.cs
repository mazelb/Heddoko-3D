using UnityEngine;
using System.Collections;

public class Quatconstraint : MonoBehaviour
{
    ProjectedAngles m_ProjectedAngles;
    public Vector3 m_AnglesAnalysis;
    public Vector2 XminMax = new Vector2(-30, 30);
    public Vector2 YminMax = new Vector2(-30, 30);
    public Vector2 ZminMax = new Vector2(-30, 30);
    public bool animatePingPong = false;

    ///*public*/ Vector3 axis;
    ///*public*/ float angle;

    //Quaternion Xmin, Xmax;
    //Quaternion Ymin, Ymax;
    //Quaternion Zmin, Zmax;

    public Vector3 eulerBegin = new Vector3(0, 0, 150);
    public Vector3 eulerEnd = new Vector3(0, 0, 150);
    private Quaternion EndQuat;
    private Quaternion beginQuat;
    public Quaternion CurrentQuat;


    // Use this for initialization
    void Start()
    {
        beginQuat = transform.localRotation;
        eulerBegin = beginQuat.eulerAngles;

        //Xmin = Quaternion.AngleAxis(XminMax.x, transform.right);
        //Xmax = Quaternion.AngleAxis(XminMax.y, transform.right);
        //Ymin = Quaternion.AngleAxis(YminMax.x, transform.up);
        //Ymax = Quaternion.AngleAxis(YminMax.y, transform.up);
        //Zmin = Quaternion.AngleAxis(ZminMax.x, transform.forward);
        //Zmax = Quaternion.AngleAxis(ZminMax.y, transform.forward);

        if (m_ProjectedAngles == null)
            m_ProjectedAngles = gameObject.GetComponent<ProjectedAngles>();
    }

    static int i = 0;
    bool pos = true;

    private Quaternion Animate()
    {
        beginQuat = Quaternion.Euler(eulerBegin);
        EndQuat = Quaternion.Euler(eulerEnd);
        if(animatePingPong)
        {

            if (pos)
            {
                ++i;
                if (i == 100)
                    pos = false;
            }
            else
            {
                --i;
                if (i == 0)
                    pos = true;
            }
            //EndQuat = Quaternion.Euler(eulerEnd);
            Quaternion tQuat = Quaternion.Slerp(beginQuat, EndQuat, i * 0.01f);

            //if (Quaternion.Angle(tQuat, EndQuat) < 1) pos = false;
            //else if (Quaternion.Angle(tQuat, beginQuat) < 1) pos = true;
            return tQuat;
        }
        else
        {
            return EndQuat;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Quaternion tQuat = Animate();
        //CurrentQuat = tQuat;

//         float angle = ExtractAngles(CurrentQuat, Vector3.up, Vector3.right);
//         tQuat = Quaternion.AngleAxis(ClampAngleX(angle), Vector3.right);
//         CurrentQuat = CurrentQuat * tQuat;
// 
//         angle = ExtractAngles(CurrentQuat, Vector3.forward, Vector3.up);
//         tQuat = Quaternion.AngleAxis(ClampAngleY(angle), Vector3.up);
//         CurrentQuat = CurrentQuat * tQuat;
// 
//         angle = ExtractAngles(CurrentQuat, Vector3.right, Vector3.forward);
//         tQuat = Quaternion.AngleAxis(ClampAngleZ(angle), Vector3.forward);
//         CurrentQuat = CurrentQuat * tQuat;
        ExtractAngles(tQuat, ref m_AnglesAnalysis);
        CurrentQuat = tQuat * Constraint(m_AnglesAnalysis);

        transform.localRotation = CurrentQuat; // Constraint(m_ProjectedAngles.m_ReferenceProjectedAngles);
    }

    Quaternion Constraint(Vector3 a_ProjAngles)
    {

        ////currentEuler = aQuat.eulerAngles;
        //ExtractAngles(aQuat, ref ProjectedAngles);
        a_ProjAngles = ClampAngle(a_ProjAngles);
        Quaternion tRet = ComputeQuaternion(a_ProjAngles);

        return tRet;
    }


    private Vector3 ClampAngle(Vector3 a_ProjAngles)
    {
        Vector3 offset = Vector3.zero;
             if (a_ProjAngles.x > XminMax.y)
                       offset.x = XminMax.y - a_ProjAngles.x;
        else if (a_ProjAngles.x < XminMax.x)
                       offset.x = XminMax.x - a_ProjAngles.x;

             if (a_ProjAngles.y > YminMax.y)
                       offset.y = YminMax.y - a_ProjAngles.y;
        else if (a_ProjAngles.y < YminMax.x)
                       offset.y = YminMax.x - a_ProjAngles.y;

             if (a_ProjAngles.z > ZminMax.y)
                       offset.z = ZminMax.y - a_ProjAngles.z;
        else if (a_ProjAngles.z < ZminMax.x)
                       offset.z = ZminMax.x - a_ProjAngles.z;

        return offset;
    }

    private float ClampAngleX(float a_ProjAngles)
    {
        float offset = 0;
        if (a_ProjAngles > XminMax.y)
            offset = XminMax.y - a_ProjAngles;
        else if (a_ProjAngles < XminMax.x)
            offset = XminMax.x - a_ProjAngles;
        
        return offset;
    }

    private float ClampAngleY(float a_ProjAngles)
    {
        float offset = 0;
        if (a_ProjAngles > YminMax.y)
            offset = YminMax.y - a_ProjAngles;
        else if (a_ProjAngles < YminMax.x)
            offset = YminMax.x - a_ProjAngles;

        return offset;
    }

    private float ClampAngleZ(float a_ProjAngles)
    {
        float offset = 0;
        if (a_ProjAngles > ZminMax.y)
            offset = ZminMax.y - a_ProjAngles;
        else if (a_ProjAngles < ZminMax.x)
            offset = ZminMax.x - a_ProjAngles;

        return offset;
    }


    public static Quaternion ComputeQuaternion(Vector3 a_WantedAngles)
    {
        
    }


    public float ExtractAngles(Quaternion a_QuatLocal, Vector3 a_Globalaxis, Vector3 a_normal)
    {
        Vector3 localAxis = a_QuatLocal * a_Globalaxis;
        Vector3 upProj = Vector3.ProjectOnPlane(localAxis, a_normal);   // pitch angle
        upProj.Normalize();
        Mathf.Atan2(upProj.y, upProj.x);
        return Vector3.Angle(a_Globalaxis, upProj);
    }
    public static void ExtractAngles(Quaternion a_QuatLocal, ref Vector3 a_Angles)
    {
        Vector3 localForward = a_QuatLocal * Vector3.forward;
        Vector3 localRight = a_QuatLocal * Vector3.right;
        Vector3 localUp = a_QuatLocal * Vector3.up;

        Vector3 upProj = Vector3.ProjectOnPlane(localUp, Vector3.right);            // pitch angle
        Vector3 forwardProj = Vector3.ProjectOnPlane(localForward, Vector3.up);     //yaw angle 
        Vector3 rightProj = Vector3.ProjectOnPlane(localRight, Vector3.forward);    // roll angle

        upProj.Normalize();
        forwardProj.Normalize();
        rightProj.Normalize();

        a_Angles.x = Vector3.Angle(Vector3.up, upProj);
        a_Angles.y = Vector3.Angle(Vector3.forward, forwardProj);
        a_Angles.z = Vector3.Angle(Vector3.right, rightProj);
    }

}


