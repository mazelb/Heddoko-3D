using UnityEngine;
using System.Collections;

public class Quatconstraint : MonoBehaviour
{
    ProjectedAngles m_ProjectedAngles;
    public Vector3 m_AnglesAnalysis;
    public Vector2 XminMax = new Vector2(-30, 30);
    public Vector2 YminMax = new Vector2(-30, 30);
    public Vector2 ZminMax = new Vector2(-30, 30);

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

        if (pos) ++i;
        else --i;
        //EndQuat = Quaternion.Euler(eulerEnd);
        Quaternion tQuat = Quaternion.Slerp(beginQuat, EndQuat, i * 0.01f);

        if (Quaternion.Angle(tQuat, EndQuat) < 3) pos = false;
        else if (Quaternion.Angle(tQuat, beginQuat) < 3) pos = true;

        return tQuat;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion tQuat = Animate();
        ProjectedAngles.ExtractAngles(tQuat,ref m_AnglesAnalysis);
        CurrentQuat = tQuat * Constraint(m_AnglesAnalysis);

        transform.localRotation = CurrentQuat; // Constraint(m_ProjectedAngles.m_ReferenceProjectedAngles);
    }

    Quaternion Constraint(Vector3 a_ProjAngles)
    {

        ////currentEuler = aQuat.eulerAngles;
        //ExtractAngles(aQuat, ref ProjectedAngles);
        a_ProjAngles = ClampAngle(a_ProjAngles);
        Quaternion tRet = Quaternion.Euler(a_ProjAngles);

        return tRet;
    }

    private Vector3 ClampAngle(Vector3 a_ProjAngles)
    {
        Vector3 offset = Vector3.zero;
        if      (a_ProjAngles.x > XminMax.y)
                       offset.x = XminMax.y - a_ProjAngles.x;
        else if (a_ProjAngles.x < XminMax.x)
                       offset.x = XminMax.x - a_ProjAngles.x;
        else           offset.x = 0;

        if      (a_ProjAngles.y > YminMax.y)
                       offset.y = YminMax.y - a_ProjAngles.y;
        else if (a_ProjAngles.y < YminMax.x)
                       offset.y = YminMax.x - a_ProjAngles.y;
        else           offset.y = 0;

        if      (a_ProjAngles.z > ZminMax.y)
                       offset.z = ZminMax.y - a_ProjAngles.z;
        else if (a_ProjAngles.z < ZminMax.x)
                       offset.z = ZminMax.x - a_ProjAngles.z;
        else           offset.z = 0;

        return offset;
//         if (currentEuler.x > MaxEulerConsttraint.x) { currentEuler.x = MaxEulerConsttraint.x; pos = !pos; }
//         if (currentEuler.y > MaxEulerConsttraint.y) { currentEuler.y = MaxEulerConsttraint.y; pos = !pos; }
//         if (currentEuler.z > MaxEulerConsttraint.z) { currentEuler.z = MaxEulerConsttraint.z; pos = !pos; }


    }


}


