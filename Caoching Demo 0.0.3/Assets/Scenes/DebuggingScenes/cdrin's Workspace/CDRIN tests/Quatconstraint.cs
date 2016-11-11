using UnityEngine;
using System.Collections;

public class Quatconstraint : MonoBehaviour
{
    ProjectedAngles m_ProjectedAngles;
    public Vector2 XminMax = new Vector2(-30, 30);
    public Vector2 YminMax = new Vector2(-30, 30);
    public Vector2 ZminMax = new Vector2(-30, 30);
    public bool animatePingPong = false;

    public Vector3 eulerBegin = new Vector3(0, 0, 150);
    public Vector3 eulerEnd = new Vector3(0, 0, 150);
    private Quaternion EndQuat;
    private Quaternion beginQuat;


    // Use this for initialization
    void Start()
    {
        beginQuat = transform.localRotation;
        eulerBegin = beginQuat.eulerAngles;

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

        transform.localRotation = Constraint(tQuat.eulerAngles);
    }

    Quaternion Constraint(Vector3 euler)
	{
		euler.x = euler.x % 360;
		euler.y = euler.y % 360;
		euler.z = euler.z % 360;

		if (euler.x > +180) euler.x = -360+ euler.x;
		if (euler.x < -180) euler.x = 360 + euler.x;
		if (euler.y > +180) euler.y = -360+ euler.y;
		if (euler.y < -180) euler.y = 360 + euler.y;
		if (euler.z > +180) euler.z = -360+ euler.z;
		if (euler.z < -180) euler.z = 360 + euler.z;

		euler = ClampAngle(euler);
        Quaternion tRet = ComputeQuaternion(euler);

        return tRet;
    }

    private Vector3 ClampAngle(Vector3 a_ProjAngles)
    {
        Vector3 offset = a_ProjAngles;
             if (a_ProjAngles.x > XminMax.y)
                       offset.x = XminMax.y;
        else if (a_ProjAngles.x < XminMax.x)
                       offset.x = XminMax.x;

             if (a_ProjAngles.y > YminMax.y)
                       offset.y = YminMax.y ;
        else if (a_ProjAngles.y < YminMax.x)
                       offset.y = YminMax.x ;

             if (a_ProjAngles.z > ZminMax.y)
                       offset.z = ZminMax.y ;
        else if (a_ProjAngles.z < ZminMax.x)
                       offset.z = ZminMax.x ;

        return offset;
    }
	
    private Vector3 computeOffset(Vector3 a_ProjAngles)
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
		return Quaternion.Euler(a_WantedAngles);
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
    }

}


