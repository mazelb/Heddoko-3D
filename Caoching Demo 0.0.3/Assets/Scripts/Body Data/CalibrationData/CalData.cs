using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Calibration1.CalibrationTransformation;

namespace Assets.Scripts.Body_Data.CalibrationData
{
    public class CalData
    {
        public Dictionary<string, List<Quaternion>> SensTPoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> SensSPoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> SensZPoseQuat = new Dictionary<string, List<Quaternion>>();

        public Dictionary<string, List<Quaternion>> transTtoZPoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> transZtoSPoseQuat = new Dictionary<string, List<Quaternion>>();

        public Dictionary<string, List<Quaternion>> transTtoZIdealPoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> transZtoSIdealPoseQuat = new Dictionary<string, List<Quaternion>>();

        public Dictionary<string, List<float>> TimePose = new Dictionary<string, List<float>>();
        public Dictionary<string, List<Quaternion>> Q   = new Dictionary<string, List<Quaternion>>();
        public List<float> TimeQ = new List<float>();
        public Dictionary<string, List<float>> sTtoZ = new Dictionary<string, List<float>>();
        public Dictionary<string, List<float>> sZtoS = new Dictionary<string, List<float>>();       
        public bool CalCompleted = false;

        public bool Tp = false;
        public bool Sp = false;
        public bool Zp = false;
        public float K = 2.0F * Mathf.PI / 360;

        public CalData()
        {
            List<Quaternion> Quat = new List<Quaternion>();
            List<float> t = new List<float>();

            SensTPoseQuat.Add("Up", Quat);
            SensTPoseQuat.Add("Lo", Quat);
            TimePose.Add("T", t);

            SensSPoseQuat.Add("Up", Quat);
            SensSPoseQuat.Add("Lo", Quat);
            TimePose.Add("S", t);

            SensZPoseQuat.Add("Up", Quat);
            SensZPoseQuat.Add("Lo", Quat); 
            TimePose.Add("Z", t);

            transTtoZPoseQuat.Add("Up", Quat);
            transTtoZPoseQuat.Add("Lo", Quat);
            transZtoSPoseQuat.Add("Up", Quat);
            transZtoSPoseQuat.Add("Lo", Quat);
        }
        public void Association()
        {
            float startT = TimePose["T"][0];
            float endT   = TimePose["T"][TimePose["T"].Count - 1];
            float tThalf = (endT - startT) / 2.0F;

            float startZ = TimePose["Z"][0];
            float endZ   = TimePose["Z"][TimePose["Z"].Count - 1];
            float tZhalf = (endZ - startZ) / 2.0F;

            float startS = TimePose["S"][0];
            float endS   = TimePose["S"][TimePose["S"].Count - 1];
            float tShalf = (endS - startS) / 2.0F; 
            for (int i = 0; i < Q.Count; i++)
            {
                if( tThalf <  TimeQ[i] && TimeQ[i] < tZhalf)
                {
                    transTtoZPoseQuat["Up"][i] = Q["Up"][i];
                    transTtoZPoseQuat["Lo"][i] = Q["Lo"][i];
                }
                if (tZhalf <  TimeQ[i] && TimeQ[i] < tShalf)
                {
                    transZtoSPoseQuat["Up"][i] = Q["Up"][i];
                    transZtoSPoseQuat["Lo"][i] = Q["Lo"][i];
                }
            }            
            int nq = transTtoZPoseQuat["Up"].Count;
            for (int i = 0; i < nq; i++)
            {
                sTtoZ["Up"].Add(Parametrisation(transTtoZPoseQuat["Up"][i], transTtoZPoseQuat["Up"][0], transTtoZPoseQuat["Up"][nq - 1]));
                sTtoZ["Lo"].Add(Parametrisation(transTtoZPoseQuat["Lo"][i], transTtoZPoseQuat["Lo"][0], transTtoZPoseQuat["Lo"][nq - 1]));
                Vector3 vTtoZ = new Vector3(0.0F, -90.0F * K * sTtoZ["Up"][i], 0.0F);
                transTtoZIdealPoseQuat["Up"].Add(Quaternion.Euler(vTtoZ));
                vTtoZ = new Vector3(0.0F,-90.0F * K * sTtoZ["Lo"][i],0.0F);
                transTtoZIdealPoseQuat["Lo"].Add(Quaternion.Euler(vTtoZ));
            }
            nq = transZtoSPoseQuat["Up"].Count;
            for (int i = 0; i < nq; i++)
            {
                sZtoS["Up"].Add(Parametrisation(transZtoSPoseQuat["Up"][i], transZtoSPoseQuat["Up"][0], transZtoSPoseQuat["Up"][nq - 1]));
                sZtoS["Lo"].Add(Parametrisation(transZtoSPoseQuat["Lo"][i], transZtoSPoseQuat["Lo"][0], transZtoSPoseQuat["Lo"][nq - 1]));
                Vector3 vZtoS = new Vector3(0.0F, 0.0F, -90.0F * K * sZtoS["Up"][i]);
                transZtoSIdealPoseQuat["Up"].Add(Quaternion.Euler(vZtoS));
                vZtoS = new Vector3(0.0F, 0.0F,-90.0F * K * sTtoZ["Lo"][i]);
                transZtoSIdealPoseQuat["Lo"].Add(Quaternion.Euler(vZtoS));
            }
        }
        public float Parametrisation(Quaternion q, Quaternion qi, Quaternion qf)
        {
            float s = (float) ( Math.Sqrt((q.w  - qi.w) * (q.w  - qi.w) + (q.x  - qi.x) * (q.x  - qi.x) + (q.y  - qi.y) * (q.y  - qi.y) + (q.z  - qi.z) * (q.z  - qi.z)) /
                                Math.Sqrt((qf.w - qi.w) * (qf.w - qi.w) + (qf.x - qi.x) * (qf.x - qi.x) + (qf.y - qi.y) * (qf.y - qi.y) + (qf.z - qi.z) * (qf.z - qi.z)));
            return s;
        }
        public void UpdateCalibrationStatus(string s)
        {
            if(s == "Tpose")
            {
                Tp = true;
            }
            else if (s == "Spose")
            {
                Sp = true;
            }
            else if (s == "Zpose")
            {
                Zp = true;
            }
            else if (Tp == true && Sp == true && Zp == true)
            {
                CalCompleted = true;
            }
        }
        public void Calibration()
        {
            Association();
            ShiuTransform s = new ShiuTransform();
            ParkTransform p = new ParkTransform();
        }
    }
}





