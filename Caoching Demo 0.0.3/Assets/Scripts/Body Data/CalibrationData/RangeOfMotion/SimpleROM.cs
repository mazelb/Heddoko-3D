using UnityEngine;
using System.Collections;
using MathNet.Numerics;

namespace Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion
{
    //enum ROM_LOCAL_AXE { X = 1, Y = 2, Z = 4, LOCK = 0 };
    enum ROM_LOCAL_AXE { PITCH = 1, YAW = 2, ROLL = 4, LOCK = 0 };

    [System.Serializable]
    public class AngleConstraint
    {
        public AngleConstraint() { }
        public AngleConstraint(Vector3 aAxe, float aMin, float aMax) { axe = aAxe; minAngle = aMin; maxAngle = aMax; }

        public Vector3 axe;
        public float minAngle;
        public float maxAngle;
        public float lastCompute;

        public AngleConstraint NegateForIMU(Vector3 a_NewAxe)
        {
            return new AngleConstraint(a_NewAxe, -minAngle, -maxAngle);
        }
    }

    [System.Serializable]
    public class SimpleROM
    {
        //enum axe {RIGHT, UP, FWD };

        [HideInInspector]
        public string Name; // only here to have a nice name in inspector while displaying the array

        public void SetPitchMinMax(float min, float max, Vector3 axe)
        {
            if (PitchMinMax == null)
            {
                //PitchMinMax = new AngleConstraint(Vector3.right, min, max);
                PitchMinMax = new AngleConstraint(axe, min, max);
            }
            else
            {
                PitchMinMax.minAngle = min;
                PitchMinMax.maxAngle = max;
            }
        }
        public void SetYawMinMax(float min, float max, Vector3 axe)
        {
            if (YawMinMax == null)
            {
                //YawMinMax = new AngleConstraint(Vector3.up, min, max);
                YawMinMax = new AngleConstraint(axe, min, max);
            }
            else
            {
                YawMinMax.minAngle = min;
                YawMinMax.maxAngle = max;
            }
        }
        public void SetRollMinMax(float min, float max, Vector3 axe)
        {
            if (RollMinMax == null)
            {
                //RollMinMax = new AngleConstraint(Vector3.forward, min, max);
                RollMinMax = new AngleConstraint(axe, min, max);
            }
            else
            {
                RollMinMax.minAngle = min;
                RollMinMax.maxAngle = max;
            }
        }

        public AngleConstraint PitchMinMax = null;
        public AngleConstraint YawMinMax   = null;
        public AngleConstraint RollMinMax  = null;
    }
}
