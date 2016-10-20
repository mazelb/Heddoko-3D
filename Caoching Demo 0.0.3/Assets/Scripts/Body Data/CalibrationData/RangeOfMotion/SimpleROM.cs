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

        public void SetPitchMinMax(float min, float max)
        {
            if (PitchMinMax == null)
            {
                PitchMinMax = new AngleConstraint(Vector3.right, min, max);
            }
            else
            {
                PitchMinMax.minAngle = min;
                PitchMinMax.maxAngle = max;

            }
        }
        public void SetYawMinMax(float min, float max)
        {
            if (PitchMinMax == null)
            {
                PitchMinMax = new AngleConstraint(Vector3.up, min, max);
            }
            else
            {
                PitchMinMax.minAngle = min;
                PitchMinMax.maxAngle = max;
            }
        }
        public void SetRollMinMax(float min, float max)
        {
            if (PitchMinMax == null)
            {
                PitchMinMax = new AngleConstraint(Vector3.forward, min, max);
            }
            else
            {
                PitchMinMax.minAngle = min;
                PitchMinMax.maxAngle = max;

            }
        }

        public AngleConstraint PitchMinMax = null;
        public AngleConstraint YawMinMax   = null;
        public AngleConstraint RollMinMax  = null;
    }
}
