using UnityEngine;
using System.Collections;
using MathNet.Numerics;

namespace Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion
{
    //enum ROM_LOCAL_AXE { X = 1, Y = 2, Z = 4, LOCK = 0 };
    enum ROM_LOCAL_AXE { PITCH = 1, YAW = 2, ROLL = 4, LOCK = 0 };

    public class AngleConstraint
    {
        public AngleConstraint() { }
        public AngleConstraint(Vector3 aAxe, float aMin, float aMax) { axe = aAxe; minAngle = aMin; maxAngle = aMax; }

        public Vector3 axe;
        public float minAngle;
        public float maxAngle;
    }

    public class SimpleROM
    {
        //public enum ROM_LOCAL_AXE { X = 1, Y = 2, Z = 4, LOCK = 0 };

        //private ROM_LOCAL_AXE m_axe;
        //public ROM_LOCAL_AXE Axe
        //{
        //    get
        //    { return m_axe; }
        //    set
        //    {
        //        //enforce value to be a bit mask
        //        m_axe = ROM_LOCAL_AXE.LOCK;

        //        if (((value & ROM_LOCAL_AXE.X) == ROM_LOCAL_AXE.X))
        //        {
        //            m_axe = m_axe | ROM_LOCAL_AXE.X;
        //        }
        //        if (((value & ROM_LOCAL_AXE.Y) == ROM_LOCAL_AXE.Y))
        //        {
        //            m_axe = m_axe | ROM_LOCAL_AXE.Y;
        //        }
        //        if (((value & ROM_LOCAL_AXE.Z) == ROM_LOCAL_AXE.Z))
        //        {
        //            m_axe = m_axe | ROM_LOCAL_AXE.Z;
        //        }
        //    }
        //}

        public void SetXMinMax(float min, float max)
        {
            if (XMinMax == null)
            {
                XMinMax = new AngleConstraint(Vector3.right, min, max);
            }
            else
            {
                XMinMax.minAngle = min;
                XMinMax.maxAngle = max;
            }
        }
        public void SetYMinMax(float min, float max)
        {
            if (YMinMax == null)
            {
                YMinMax = new AngleConstraint(Vector3.up, min, max);
            }
            else
            {
                YMinMax.minAngle = min;
                YMinMax.maxAngle = max;
            }
        }
        public void SetZMinMax(float min, float max)
        {
            if (ZMinMax == null)
            {
                ZMinMax = new AngleConstraint(Vector3.forward, min, max);
            }
            else
            {
                ZMinMax.minAngle = min;
                ZMinMax.maxAngle = max;
            }
        }

        public AngleConstraint XMinMax = null;
        public AngleConstraint YMinMax   = null;
        public AngleConstraint ZMinMax  = null;


    }
}
