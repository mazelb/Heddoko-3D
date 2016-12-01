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

        public void Init()
        {
            rom.SetXMinMax(-60, 100); // up/down
            rom.SetYMinMax(-100, 100);  // front/back
            rom.SetZMinMax(-90, 90);   // twist
        }

        public void Awake() { Init(); }
        public void Start() { Init(); }
        public void Update() {  }
    }
}
