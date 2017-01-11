using UnityEngine;
using System.Collections;
using MathNet.Numerics;

namespace Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion
{
    [ExecuteInEditMode]
    public class SimpleROMMB : MonoBehaviour
    {
        public bool ReferenceIsHoriz;
        public bool InvertReference;
        public SimpleROM rom = new SimpleROM();

        public bool ToggleXConstraint;
        public bool ToggleYConstraint;
        public bool ToggleZConstraint;

        public Vector3 axePitch = Vector3.right;
        public Vector3 axeYaw   = Vector3.up;
        public Vector3 axeRoll  = Vector3.forward;

        public void Init()
        {
            rom.SetPitchMinMax(-60, 100, axePitch); // up/down
            rom.SetYawMinMax(-50, 50 , axeYaw   );  // front/back
            rom.SetRollMinMax(-30, 30  , axeRoll  );   // twist
        }

        public void Awake() { Init(); }
        public void Start() { Init(); }
        public void Update() {  }
    }
}
