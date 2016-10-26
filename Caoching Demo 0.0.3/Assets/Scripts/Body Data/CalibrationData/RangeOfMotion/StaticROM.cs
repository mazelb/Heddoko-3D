using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion
{
    public class StaticROM
    {
        public Body Reference = null;

        public Dictionary<BodyStructureMap.SubSegmentTypes, SimpleROM> squeletteRom = new Dictionary<BodyStructureMap.SubSegmentTypes, SimpleROM>();

        public StaticROM()
        {
            SimpleROM t_LowerSpine = new SimpleROM();
            t_LowerSpine.SetPitchMinMax(-40, 40);
            t_LowerSpine.SetYawMinMax(-40, 40);
            t_LowerSpine.SetRollMinMax(-40, 40);
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine, t_LowerSpine);
            SimpleROM t_UpperSpine = new SimpleROM();            
            t_UpperSpine.SetPitchMinMax(-40,40);            
            t_UpperSpine.SetYawMinMax(-40,40);            
            t_UpperSpine.SetRollMinMax(-40, 40);            
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine, t_UpperSpine) ;


            SimpleROM t_RightUpperArm = new SimpleROM();
            t_RightUpperArm.SetPitchMinMax(-60, 100); // up/down
            t_RightUpperArm.SetYawMinMax(-100, 100);  // front/back  
            t_RightUpperArm.SetRollMinMax(-90, 90);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm, t_RightUpperArm);
            SimpleROM t_RightForeArm = new SimpleROM();
            t_RightForeArm.SetYawMinMax(-80, 80);  // flex
            t_RightForeArm.SetRollMinMax(-20, 150);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm, t_RightForeArm);

            SimpleROM t_LeftUpperArm = new SimpleROM();
            t_LeftUpperArm.SetPitchMinMax(-60, 100); // up/down
            t_LeftUpperArm.SetYawMinMax(-100, 100);  // front/back  
            t_LeftUpperArm.SetRollMinMax(-90, 90);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm, t_LeftUpperArm);
            SimpleROM t_LeftForeArm = new SimpleROM();
            t_LeftForeArm.SetYawMinMax(-80, 80);  // flex 
            t_LeftForeArm.SetRollMinMax(-20, 150);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm, t_LeftForeArm);


            SimpleROM t_RightThigh = new SimpleROM();
            t_RightThigh.SetPitchMinMax(-90, 50); // up/down
            t_RightThigh.SetYawMinMax(-60, 60);  //right/left  
            t_RightThigh.SetRollMinMax(-60, 60);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh, t_RightThigh);

            SimpleROM t_RightCalf = new SimpleROM();
            t_RightCalf.SetYawMinMax(-80, 80);  // flex 
            t_RightCalf.SetRollMinMax(-90, 90);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf, t_RightCalf);

            SimpleROM t_LeftThigh = new SimpleROM();
            t_LeftThigh.SetPitchMinMax(-90, 50); // up/down
            t_LeftThigh.SetYawMinMax(-60, 60);  //right/left  
            t_LeftThigh.SetRollMinMax(-60, 60);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh, t_LeftThigh);
            SimpleROM t_LeftCalf = new SimpleROM();
            t_LeftCalf.SetYawMinMax(-80, 80);  // flex 
            t_LeftCalf.SetRollMinMax(-90, 90);   // twist
            squeletteRom.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf, t_LeftCalf);
        }

        public void capRotation(BodyStructureMap.SegmentTypes aSegType, BodyStructureMap.SubSegmentTypes aSubType, BodySubSegment aSubSeg,ref Quaternion aQuat, bool local = false)
        {
            Quaternion t_currentLocal = aSubSeg.GetSubSegmentTransform().localRotation;
            Quaternion t_currentWorld = aSubSeg.GetSubSegmentTransform().rotation;
            Quaternion t_LocalToWorld = t_currentWorld * Quaternion.Inverse(t_currentLocal);
            Quaternion t_WorldToLocal = Quaternion.Inverse(t_LocalToWorld);
            Quaternion tQuatLocal;
            if (!local)
                tQuatLocal = t_WorldToLocal * aQuat;
            else
                tQuatLocal = aQuat;

            //*
            Quaternion roll0  = Quaternion.AngleAxis(90, Vector3.up);
            Quaternion pitch0 = Quaternion.AngleAxis(0, Vector3.forward); // right vector from forward
            Quaternion yaw0   = Quaternion.AngleAxis(90, Vector3.right); // up vector from forward
             /*/
            Quaternion roll0  = Quaternion.AngleAxis(0, Vector3.forward);
            Quaternion pitch0 = Quaternion.AngleAxis(0, Vector3.right); // right
            Quaternion yaw0   = Quaternion.AngleAxis(0, Vector3.up);
            //*/
            AngleConstraint tpitch = squeletteRom[aSubType].PitchMinMax;
            AngleConstraint tYaw   = squeletteRom[aSubType].YawMinMax;
            AngleConstraint tRoll  = squeletteRom[aSubType].RollMinMax;


            Vector3 tAngles;

            tAngles.x = Quaternion.Angle(pitch0, tQuatLocal); // compute angle on pitch local axis
            tAngles.y = Quaternion.Angle(yaw0, tQuatLocal);   // compute angle on Yaw   local axis
            tAngles.z = Quaternion.Angle(roll0, tQuatLocal);  // compute angle on Roll  local axis
            Vector3 tEuler = tQuatLocal.eulerAngles;

            if (tpitch != null)
            {
                if (tAngles.x < tpitch.minAngle)
                    tAngles.x = tpitch.minAngle - tAngles.x;
                else if (tAngles.x > tpitch.maxAngle)
                    tAngles.x = tpitch.maxAngle - tAngles.x;
                else
                    tAngles.x = 0;
            }

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

            pitch0 = Quaternion.AngleAxis(tAngles.x, Vector3.right);
            yaw0 = Quaternion.AngleAxis(tAngles.y, Vector3.up);
            roll0 = Quaternion.AngleAxis(tAngles.z, Vector3.forward);

            Quaternion offset = yaw0 * pitch0 * roll0;
            if(!local)
                aQuat = t_LocalToWorld * (offset*tQuatLocal);
            else
                aQuat = (offset * tQuatLocal);
                

            //aQuat.Set(tQuatLocal.x, tQuatLocal.y, tQuatLocal.z, tQuatLocal.w);
            //aQuat = Quaternion.Euler(tAngles);
            
            if (Reference != null)
            {
                BodySubSegment refsubSeg = Reference.BodySegments.Find(x => x.SegmentType == aSegType).BodySubSegmentsDictionary[(int)aSubType];
                refsubSeg.UpdateSubsegmentOrientation(aQuat, local? 0:3, true);
            }
        }
    }
}
