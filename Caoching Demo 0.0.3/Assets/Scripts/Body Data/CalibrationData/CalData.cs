using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Body_Data.CalibrationData
{
    public class CalData
    {          
        public Dictionary<string, List<Quaternion>> ZPoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> SPoseQuat = new Dictionary<string, List<Quaternion>>();
        public CalData()
        {
            List<Quaternion> Quat = new List<Quaternion>();
            ZPoseQuat.Add("UA", Quat);
            ZPoseQuat.Add("LA", Quat);
            SPoseQuat.Add("UA", Quat);
            SPoseQuat.Add("LA", Quat); 
        }
    }
}
