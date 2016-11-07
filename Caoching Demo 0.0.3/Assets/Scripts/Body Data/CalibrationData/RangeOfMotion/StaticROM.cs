using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion
{
    public class StaticROM
    {
        public Body Reference = null;

        public Dictionary<BodyStructureMap.SubSegmentTypes, SimpleROM> squeletteRom = new Dictionary<BodyStructureMap.SubSegmentTypes, SimpleROM>();
        public BodyFlags mFlags;
        Vector3 localForward;
        Vector3 localRight;
        Vector3 localUp;

        private Camera viewerCam = null;

        private void TempCameraSettings()
        {
            Camera[] alls = Camera.allCameras;
            for (int i = 0; i < alls.Length; ++i)
            {
                if (alls[i].name == "PanelCamera")
                {
                    Rect t_Bounds = alls[i].rect;
                    t_Bounds.width = 1;
                    alls[i].rect = t_Bounds;
                    i = alls.Length;
                }
            }
        }

        public StaticROM()
        {
            SimpleROM t_LowerSpine = new SimpleROM();
            t_LowerSpine.SetXMinMax(-40, 40);
            t_LowerSpine.SetYMinMax(-40, 40);
            t_LowerSpine.SetZMinMax(-40, 40);
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine, t_LowerSpine);
            SimpleROM t_UpperSpine = new SimpleROM();            
            t_UpperSpine.SetXMinMax(-40,40);            
            t_UpperSpine.SetYMinMax(-40,40);            
            t_UpperSpine.SetZMinMax(-40, 40);            
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine, t_UpperSpine) ;


            SimpleROM t_RightUpperArm = new SimpleROM();
            t_RightUpperArm.SetXMinMax(-60, 100); // up/down
            t_RightUpperArm.SetYMinMax(-100, 100);  // front/back  
            t_RightUpperArm.SetZMinMax(-90, 90);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm, t_RightUpperArm);
            SimpleROM t_RightForeArm = new SimpleROM();
            t_RightForeArm.SetYMinMax(-80, 80);  // flex
            t_RightForeArm.SetZMinMax(-20, 150);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm, t_RightForeArm);

            SimpleROM t_LeftUpperArm = new SimpleROM();
            t_LeftUpperArm.SetXMinMax(-60, 100); // up/down
            t_LeftUpperArm.SetYMinMax(-100, 100);  // front/back  
            t_LeftUpperArm.SetZMinMax(-90, 90);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm, t_LeftUpperArm);
            SimpleROM t_LeftForeArm = new SimpleROM();
            t_LeftForeArm.SetYMinMax(-80, 80);  // flex 
            t_LeftForeArm.SetZMinMax(-20, 150);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm, t_LeftForeArm);


            SimpleROM t_RightThigh = new SimpleROM();
            t_RightThigh.SetXMinMax(-90, 50); // up/down
            t_RightThigh.SetYMinMax(-60, 60);  //right/left  
            t_RightThigh.SetZMinMax(-60, 60);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh, t_RightThigh);

            SimpleROM t_RightCalf = new SimpleROM();
            t_RightCalf.SetYMinMax(-80, 80);  // flex 
            t_RightCalf.SetZMinMax(-90, 90);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf, t_RightCalf);

            SimpleROM t_LeftThigh = new SimpleROM();
            t_LeftThigh.SetXMinMax(-90, 50); // up/down
            t_LeftThigh.SetYMinMax(-60, 60);  //right/left  
            t_LeftThigh.SetZMinMax(-60, 60);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh, t_LeftThigh);
            SimpleROM t_LeftCalf = new SimpleROM();
            t_LeftCalf.SetYMinMax(-80, 80);  // flex 
            t_LeftCalf.SetZMinMax(-90, 90);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf, t_LeftCalf);
        }

        private void ClampAngles(ref Vector3 tAngles, AngleConstraint tpitch, AngleConstraint tYaw, AngleConstraint tRoll)
        {
            if (tpitch != null)
            {
                if (tAngles.x < tpitch.minAngle)
                    tAngles.x = tpitch.minAngle - tAngles.x;
                else if (tAngles.x > tpitch.maxAngle)
                    tAngles.x = tpitch.maxAngle - tAngles.x;
                else
                    tAngles.x = 0;
            }
            else
                tAngles.x = 0;

            if (tYaw != null)
            {
                if (tAngles.y < tYaw.minAngle)
                    tAngles.y = tYaw.minAngle - tAngles.y;
                else if (tAngles.y > tYaw.maxAngle)
                    tAngles.y = tYaw.maxAngle - tAngles.y;
                else
                    tAngles.y = 0;
            }
            else
                tAngles.y = 0;


            if (tRoll != null)
            {
                if (tAngles.z < tRoll.minAngle)
                    tAngles.z = tRoll.minAngle - tAngles.z;
                else if (tAngles.z > tRoll.maxAngle)
                    tAngles.z = tRoll.maxAngle - tAngles.z;
                else
                    tAngles.z = 0;
            }
            else
                tAngles.z = 0;
        }

        private void ExtractAngles(Quaternion tQuatLocal, ref Vector3 tAngles)
        {
//             localForward = tQuatLocal * Vector3.forward;
//             localRight   = tQuatLocal * Vector3.right;
//             localUp      = tQuatLocal * Vector3.up;
// 
//             Vector3 Pitchcomp = Vector3.ProjectOnPlane(localUp, Vector3.right);   // pitch angle
//             Vector3 Yawcomp = Vector3.ProjectOnPlane(localForward, Vector3.up);        //yaw angle 
//             Vector3 Rollcomp = Vector3.ProjectOnPlane(localRight, Vector3.forward);  // roll angle
// 
//             tAngles.x = Vector3.Angle(Vector3.up, Pitchcomp);
//             tAngles.y = Vector3.Angle(Vector3.forward, Yawcomp);
//             tAngles.z = Vector3.Angle(Vector3.right, Rollcomp);

            tAngles = (tQuatLocal.eulerAngles);

            if (tAngles.x > 180)
                tAngles.x -= 360;
            if (tAngles.y > 180)
                tAngles.y -= 360;
            if (tAngles.z > 180)
                tAngles.z -= 360;
        }

        public void capRotation(BodyStructureMap.SegmentTypes aSegType, BodyStructureMap.SubSegmentTypes aSubType, BodySubSegment aSubSeg,ref Quaternion aQuat, bool local = false)
        {
            if (viewerCam == null)
            {
                TempCameraSettings();
            }

            Quaternion t_currentLocal = aSubSeg.GetSubSegmentTransform().localRotation;
            Quaternion t_currentWorld = aSubSeg.GetSubSegmentTransform().rotation;
            Quaternion t_LocalToWorld = t_currentWorld * Quaternion.Inverse(t_currentLocal);
            Quaternion t_WorldToLocal = Quaternion.Inverse(t_LocalToWorld);
            Quaternion tQuat;
            if (!local)
                tQuat = t_WorldToLocal * aQuat;
            else
                tQuat = aQuat;

            AngleConstraint Xconstraint = squeletteRom[aSubType].XMinMax;
            AngleConstraint Yconstraint = squeletteRom[aSubType].YMinMax;
            AngleConstraint Zconstraint = squeletteRom[aSubType].ZMinMax;
            Vector3 tAngles = Vector3.zero;
            Vector3 tEuler = tQuat.eulerAngles;

            ExtractAngles(tQuat, ref tAngles);
            

            ClampAngles(ref tAngles, Xconstraint, Yconstraint, Zconstraint);

            Quaternion pitch = Quaternion.identity;// Quaternion.AngleAxis(tAngles.x, localUp);
            Quaternion yaw00  = Quaternion.identity;// Quaternion.AngleAxis(tAngles.y, localForward);
            Quaternion roll0  = Quaternion.identity;// Quaternion.AngleAxis(tAngles.z, localRight);
            Quaternion newLocal = Quaternion.identity;

            if (mFlags.IsAdjustingSegmentAxis)
            {
                if (mFlags.IsProjectingXZ) pitch = Quaternion.Euler(tAngles.x, 0, 0);
                if (mFlags.IsProjectingXY) yaw00 = Quaternion.Euler(0, tAngles.y, 0);
                if (mFlags.IsProjectingYZ) roll0 = Quaternion.Euler(0, 0, tAngles.z);

                newLocal = yaw00 * pitch * roll0;
            }
            else
            {
                newLocal = Quaternion.Euler(tAngles.x, tAngles.y, tAngles.z) ;
                //                 if (mFlags.IsCalibrating)
                //                 {
                //                     if (mFlags.IsProjectingXZ) pitch = Quaternion.AngleAxis(tAngles.x, localRight);
                //                     if (mFlags.IsProjectingXY) yaw00 = Quaternion.AngleAxis(tAngles.y, localUp);
                //                     if (mFlags.IsProjectingYZ) roll0 = Quaternion.AngleAxis(tAngles.z, localForward);
                //                 }
                //                 else
                //                 {
                //                     if (mFlags.IsProjectingXZ) pitch = Quaternion.AngleAxis(tAngles.x, Vector3.right);1
                //                     if (mFlags.IsProjectingXY) yaw00 = Quaternion.AngleAxis(tAngles.y, Vector3.up);
                //                     if (mFlags.IsProjectingYZ) roll0 = Quaternion.AngleAxis(tAngles.z, Vector3.forward);
                //                 }
                //newLocal = yaw00 * pitch * roll0;

            }


            //if(local)
                tQuat = newLocal;
            //else
            //    tQuat = t_LocalToWorld * newLocal;


            //aQuat.Set(tQuatLocal.x, tQuatLocal.y, tQuatLocal.z, tQuatLocal.w);
            //aQuat = Quaternion.Euler(tAngles);

            if (Reference != null)
            {
                BodySubSegment refsubSeg = Reference.BodySegments.Find(x => x.SegmentType == aSegType).BodySubSegmentsDictionary[(int)aSubType];
                refsubSeg.UpdateSubsegmentOrientation(tQuat, 0/*local ? 0 : 3*/, true);
            }


        }
    }
}
