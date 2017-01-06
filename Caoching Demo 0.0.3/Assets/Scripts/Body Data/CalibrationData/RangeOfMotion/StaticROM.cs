using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion
{
    public class StaticROM : ScriptableObject
	{
        public Body Reference = null;

        [SerializeField]
        public SimpleROM[] squeletteRom = new SimpleROM[10];
        private Quaternion[] previous = new Quaternion[10];
        //Dictionary<BodyStructureMap.SubSegmentTypes, SimpleROM> squeletteRom = new Dictionary<BodyStructureMap.SubSegmentTypes, SimpleROM>();
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
                    viewerCam = alls[i];
                    Rect t_Bounds = viewerCam.rect;
					t_Bounds.width = 1;
                    viewerCam.rect = t_Bounds;
					i = alls.Length;
				}
			}
		}

		public StaticROM()
		{
            ResetAll();
        }

        private void ClampAngles(ref Vector3 tAngles, AngleConstraint tpitch, AngleConstraint tYaw, AngleConstraint tRoll)
		{
			if (tpitch != null)
			{
				if (tAngles.x < tpitch.minAngle)
					tAngles.x = tpitch.minAngle;
				else if (tAngles.x > tpitch.maxAngle)
					tAngles.x = tpitch.maxAngle ;
			}

			if (tYaw != null)
			{
				if (tAngles.y < tYaw.minAngle)
					tAngles.y = tYaw.minAngle ;
				else if (tAngles.y > tYaw.maxAngle)
					tAngles.y = tYaw.maxAngle ;
			}

			if (tRoll != null)
			{
				if (tAngles.z < tRoll.minAngle)
					tAngles.z = tRoll.minAngle;
				else if (tAngles.z > tRoll.maxAngle)
					tAngles.z = tRoll.maxAngle;
			}
		}

        // return true if in bounds, meaning no constraint applied
        private bool ConstraintYaw(AngleConstraint a_YawConst, ref Quaternion a_quat, BodyStructureMap.SubSegmentTypes a_subType)
        {
            Vector3 t_localOrthoAxe = a_quat * Vector3.forward;
            t_localOrthoAxe.y = 0;
            t_localOrthoAxe.Normalize();
            float t_yawAngle = 0;
            if (t_localOrthoAxe.y < 0.8)
                t_yawAngle = Mathf.Atan2(t_localOrthoAxe.x, t_localOrthoAxe.z) * Mathf.Rad2Deg;
            else
            {
                t_localOrthoAxe = a_quat * Vector3.right;
                t_localOrthoAxe.y = 0;
                t_localOrthoAxe.Normalize();
                t_yawAngle = Mathf.Atan2(t_localOrthoAxe.z, t_localOrthoAxe.x) * Mathf.Rad2Deg;
            }
            float dif = 0;
            if (a_YawConst != null)
            {
                if (t_yawAngle < a_YawConst.minAngle)
                {
                    dif = a_YawConst.minAngle - t_yawAngle;
                }
                else if (t_yawAngle > a_YawConst.maxAngle)
                {
                    dif = a_YawConst.maxAngle - t_yawAngle;
                }
            }
            a_quat = a_quat * Quaternion.Euler(0, dif, 0);
            return Mathf.Abs(dif) < 1;
        }

        // return true if in bounds, meaning no constraint applied
        private bool ConstraintRoll(AngleConstraint a_RollAC, ref Quaternion a_quat, BodyStructureMap.SubSegmentTypes a_subType)
        {
            //Vector3 localOrthoAxe;
            float t_RollAngle;
            Vector3 t_upwards;

            if (a_RollAC.axe == Vector3.up)
                t_upwards = -Vector3.forward;
            else if(a_RollAC.axe == -Vector3.up)
                t_upwards = Vector3.forward;
            else
                t_upwards = Vector3.up;

            Quaternion localWithoutRoll = Quaternion.LookRotation(a_quat * a_RollAC.axe, t_upwards);
            Quaternion localWithRoll = Quaternion.LookRotation(a_quat * a_RollAC.axe, a_quat * t_upwards);

            //compute angle between both quaternion which is our current Roll angle
            t_RollAngle = Quaternion.Angle(localWithoutRoll, localWithRoll);
            // Angle return by unity is always positive, make a quick check to know 
            // if we need to negate angle. (true if rotation is in [180, 360] interval)
            Vector3 eulerAxe = a_quat.eulerAngles;
            eulerAxe.x *= Mathf.Abs(a_RollAC.axe.x);  // nullify angles which are not on the axis currently checked
            eulerAxe.y *= Mathf.Abs(a_RollAC.axe.y);  // nullify angles which are not on the axis currently checked
            eulerAxe.z *= Mathf.Abs(a_RollAC.axe.z);  // nullify angles which are not on the axis currently checked
            // check on whatever axis, angle is, others will be null, thus, we can simply add them each other wether than check one after the other
            if (eulerAxe.x + eulerAxe.y + eulerAxe.z > 180)
                t_RollAngle = -t_RollAngle;

            // if angles are small enough, don't take roll into account ? (not sure why I wrote this :/ )
            if (a_RollAC.axe == Vector3.right || a_RollAC.axe == Vector3.left)
            {
                float other_angle;
                Vector3 random_v;
                localWithoutRoll.ToAngleAxis(out other_angle, out random_v);
                if (t_RollAngle < 0.5f && other_angle < 0.5f)
                    localWithRoll.ToAngleAxis(out t_RollAngle, out random_v); // we don't care about Axis
            }

            a_RollAC.lastCompute = t_RollAngle;
            float dif = 0;
            if (a_RollAC != null)
            {
                if (t_RollAngle < a_RollAC.minAngle)
                {
                    dif = a_RollAC.minAngle - t_RollAngle;
                }
                else if (t_RollAngle > a_RollAC.maxAngle)
                {
                    dif = a_RollAC.maxAngle - t_RollAngle;
                }
            }
            a_quat = a_quat * Quaternion.Euler(a_RollAC.axe * dif);
            return Mathf.Abs(dif) < 0.1f;
        }

        // return true if in bounds, meaning no constraint applied
        private bool ConstraintPitch(AngleConstraint a_PitchAC, ref Quaternion a_quat, BodyStructureMap.SubSegmentTypes a_subType)
        {
            //             //Vector3 localOrthoAxe = a_quat * Vector3.up;
            //             Vector3 localOrthoAxe;// = Vector3.zero;
            //             //special case for arms
            //             if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm ||
            //                 a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm ||
            //                 a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm ||
            //                 a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm)
            //             {
            //                 localOrthoAxe = a_quat * Vector3.right;
            //             }
            //             else
            //             {
            //                 localOrthoAxe = a_quat * Vector3.up;
            //             }
            //             float t_PitchAngle = Mathf.Atan2(localOrthoAxe.z, localOrthoAxe.y) * Mathf.Rad2Deg;
            //             float dif = 0;
            //             if (a_PitchConst != null)
            //             {
            //                 if (t_PitchAngle < a_PitchConst.minAngle)
            //                 {
            //                     dif = a_PitchConst.minAngle - t_PitchAngle;
            //                 }
            //                 else if (t_PitchAngle > a_PitchConst.maxAngle)
            //                 {
            //                     dif = a_PitchConst.maxAngle - t_PitchAngle;
            //                 }
            //             }
            //             a_quat = a_quat * Quaternion.Euler(dif,0 ,0);
            //             return Mathf.Abs(dif) < 1;
            //             
            bool t_inBound = false;

            Vector3 localAxe = a_quat * a_PitchAC.axe;
            localAxe.Normalize();

            Vector3 t_upwards;

            if (a_PitchAC.axe == Vector3.up)
                t_upwards = Vector3.back;
            else if (a_PitchAC.axe == Vector3.down)
                t_upwards = Vector3.forward;
            else
                t_upwards = Vector3.up;

            float t_minAngle = a_PitchAC.minAngle, 
                  t_maxAngle = a_PitchAC.maxAngle;
            float med = t_minAngle + (t_maxAngle - t_minAngle) * 0.5f;

            float t_RadiusConeMax = (t_maxAngle > 0 ? 1 : -1) * Mathf.Sin((90 - t_maxAngle) * Mathf.Deg2Rad);
            float t_RadiusConeMin = (t_minAngle > 0 ? 1 : -1) * Mathf.Sin((90 - t_minAngle) * Mathf.Deg2Rad);

            //get vertical component of current direction
            Vector3 tFlat = Vector3.ProjectOnPlane(localAxe, t_upwards);
            Vector3 tvert = localAxe - tFlat;

            float t_RadiusProjOnConeMax = tvert.magnitude * Mathf.Tan((90 - Mathf.Abs(t_maxAngle)) * Mathf.Deg2Rad);
            float t_RadiusProjOnConeMin = tvert.magnitude * Mathf.Tan((90 - Mathf.Abs(t_minAngle)) * Mathf.Deg2Rad);

            Vector3 tConeCenter = Vector3.zero;
            tConeCenter += tvert;

            Vector3 projMax;
            Vector3 projMin;
            projMax = tConeCenter + (localAxe - tConeCenter).normalized * t_RadiusProjOnConeMax; // max
            projMin = tConeCenter - (localAxe - tConeCenter).normalized * t_RadiusProjOnConeMin; // min
            Vector3 proj = projMax;

            if (t_RadiusProjOnConeMax > t_RadiusConeMax)
            {
                proj = projMax;
                t_inBound = false;
            }
            else if((t_RadiusConeMin < 0 && t_RadiusProjOnConeMin < t_RadiusConeMin) || 
                    (t_RadiusConeMin > 0 && t_RadiusProjOnConeMin < t_RadiusConeMin) )
            {
                proj = projMin;
                t_inBound = false;
            }
            else if (t_RadiusProjOnConeMax > 0)
            {
                proj = projMax;
                t_inBound = true;
            }
            else if (t_RadiusProjOnConeMin < 0)
            {
                proj = projMin;
                t_inBound = true;
            }
            else
            {
                t_inBound = false;
            }

            if (t_inBound) return true; // no constraint to apply, return now
            


            return t_inBound;
        }

        private void ExtractUsingConstraint(ref Quaternion tQuatLocal, AngleConstraint tpitch, AngleConstraint tYaw, AngleConstraint tRoll, BodyStructureMap.SubSegmentTypes a_subType)
        {
            bool roll = ConstraintRoll(tRoll, ref tQuatLocal, a_subType);
            //bool pitch = ConstraintPitch(tpitch, ref tQuatLocal);
            //bool yaw = ConstraintYaw(tYaw, ref tQuatLocal, a_subType);


            //if (!roll)
            //{
            //    yaw = ConstraintYaw(tYaw, ref tQuatLocal);
            //    roll = ConstraintRoll(tYaw, ref tQuatLocal);
            //    if (!yaw || !roll)
            //        Debug.LogError("can't apply roll constraint ?");
            //}
            //
            // 
            //bool pitch = ConstraintPitch(tpitch, ref tQuatLocal);
            //if (!pitch)
            //{
            //    yaw = ConstraintYaw(tYaw, ref tQuatLocal);
            //    roll = ConstraintRoll(tRoll, ref tQuatLocal);
            //    pitch = ConstraintPitch(tpitch, ref tQuatLocal);
            //    if (!yaw || !roll || !pitch)
            //        Debug.LogError("can't apply pitch constraint ?");

            //}



        }

        private void ExtractAngles(Quaternion tQuatLocal, ref Vector3 tAngles)
		{
			tAngles = (tQuatLocal.eulerAngles);

			if (tAngles.x > 180) tAngles.x -= 360;
			if (tAngles.y > 180) tAngles.y -= 360;
			if (tAngles.z > 180) tAngles.z -= 360;
		}


        public void capRotation(BodyStructureMap.SegmentTypes aSegType, BodyStructureMap.SubSegmentTypes aSubType, BodySubSegment aSubSeg, ref Quaternion aQuat, bool local = false, bool a_Update_Ref = true)
        {
            capRotation(aSegType, aSubType, aSubSeg.GetSubSegmentTransform(), ref aQuat, local, a_Update_Ref);
        }
        public Quaternion capRotation(BodyStructureMap.SegmentTypes aSegType, BodyStructureMap.SubSegmentTypes aSubType, Transform tSubSeg, ref Quaternion aQuat, bool local = false, bool a_Update_Ref= true)
		{
			if (viewerCam == null)
			{
				TempCameraSettings();
			}

            if (aSubType != BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm
                 &&
                aSubType != BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm
                 && 
                aSubType != BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm
                 &&
                aSubType != BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm
                 &&
                aSubType != BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf
                 &&
                aSubType != BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf)
                return aQuat; // Quaternion.identity;

            Quaternion t_currentLocal = tSubSeg.localRotation;
			Quaternion t_currentWorld = tSubSeg.rotation;
			Quaternion t_LocalToWorld = t_currentWorld * Quaternion.Inverse(t_currentLocal);
			Quaternion t_WorldToLocal = Quaternion.Inverse(t_LocalToWorld);
			Quaternion tQuat;
			if (!local)
				tQuat = t_WorldToLocal * aQuat;
			else
				tQuat = aQuat;

            AngleConstraint PitchConstraint = null;
            AngleConstraint YawConstraint = null;
            AngleConstraint RollConstraint = null;

            PitchConstraint = squeletteRom[(int)aSubType].PitchMinMax;
            YawConstraint = squeletteRom[(int)aSubType].YawMinMax;
            RollConstraint = squeletteRom[(int)aSubType].RollMinMax;
            Vector3 tAngles = Vector3.zero;
  
            ExtractUsingConstraint(ref tQuat, PitchConstraint, YawConstraint, RollConstraint, aSubType);


            //Quaternion newLocal = Quaternion.identity;
            //if (mFlags.IsAdjustingSegmentAxis)

            //{
            //    Quaternion pitch = Quaternion.identity;// Quaternion.AngleAxis(tAngles.x, localUp);
            //    Quaternion yaw00 = Quaternion.identity;// Quaternion.AngleAxis(tAngles.y, localForward);
            //    Quaternion roll0 = Quaternion.identity;// Quaternion.AngleAxis(tAngles.z, localRight);
            //    if (mFlags.IsProjectingXZ) pitch = Quaternion.Euler(tAngles.x, 0, 0);
            //    if (mFlags.IsProjectingXY) yaw00 = Quaternion.Euler(0, tAngles.y, 0);
            //    if (mFlags.IsProjectingYZ) roll0 = Quaternion.Euler(0, 0, tAngles.z);

            //    newLocal = yaw00 * pitch * roll0;
            //}
            //else
            //{
            //    newLocal = Quaternion.Euler(tAngles.x, tAngles.y, tAngles.z);
            //}


            ////if(local)
            //tQuat = newLocal;
            ////else
            ////    tQuat = t_LocalToWorld * newLocal;


            ////aQuat.Set(tQuatLocal.x, tQuatLocal.y, tQuatLocal.z, tQuatLocal.w);
            ////aQuat = Quaternion.Euler(tAngles);

            if (a_Update_Ref && Reference != null)
            {
                BodySubSegment refsubSeg = Reference.BodySegments.Find(x => x.SegmentType == aSegType).BodySubSegmentsDictionary[(int)aSubType];
                refsubSeg.UpdateSubsegmentOrientation(tQuat, 2/*local ? 0 : 3*/, false);
            }

            return tQuat;
		}

        public void ResetAll()
        {
            //////////////////////////////////////////////////////////////////////////
            //                              body
            //////////////////////////////////////////////////////////////////////////
            ResetLowerSpine();
            ResetUpperSpine();

            //////////////////////////////////////////////////////////////////////////
            //                              arms
            //////////////////////////////////////////////////////////////////////////
            // right
            ResetRightUpperArm();
            ResetRightForeArm();
            // left
            ResetLeftUpperArm();
            ResetLeftForeArm();

            //////////////////////////////////////////////////////////////////////////
            //                              legs
            //////////////////////////////////////////////////////////////////////////
            // right
            ResetRightThigh();
            ResetRightCalf();
            // left
            ResetLeftThigh();
            ResetLeftCalf();
        }
        public void Reset(BodyStructureMap.SubSegmentTypes a_subType)
        {
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine) { ResetLowerSpine(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine) { ResetUpperSpine(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm) { ResetRightUpperArm(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm) { ResetRightForeArm(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm) { ResetLeftUpperArm(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm) { ResetLeftForeArm(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh) { ResetRightThigh(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf) { ResetRightCalf(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh) { ResetLeftThigh(); }
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf) { ResetLeftCalf(); }
        }

        private void ResetLowerSpine()
        {
            SimpleROM t_LowerSpine = new SimpleROM();
            t_LowerSpine.SetPitchMinMax(-40, 40, Vector3.right);
            t_LowerSpine.SetYawMinMax(-40, 40, Vector3.up);
            t_LowerSpine.SetRollMinMax(-40, 40, Vector3.forward);
            t_LowerSpine.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine] = t_LowerSpine;
        }
        private void ResetUpperSpine()
        {
            SimpleROM t_UpperSpine = new SimpleROM();
            t_UpperSpine.SetPitchMinMax(-40, 40, Vector3.right);
            t_UpperSpine.SetYawMinMax(-40, 40, Vector3.up);
            t_UpperSpine.SetRollMinMax(-40, 40, Vector3.forward);
            t_UpperSpine.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine] = t_UpperSpine;
        }
        private void ResetRightUpperArm()
        {
            SimpleROM t_RightUpperArm = new SimpleROM();
            t_RightUpperArm.SetPitchMinMax(-60, 100, Vector3.forward); // up/down
            t_RightUpperArm.SetYawMinMax(-100, 100, Vector3.up);  // front/back  
            t_RightUpperArm.SetRollMinMax(-90, 90, Vector3.right);   // twist
            t_RightUpperArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm] = t_RightUpperArm;
        }
        private void ResetRightForeArm()
        {
            SimpleROM t_RightForeArm = new SimpleROM();
            t_RightForeArm.SetPitchMinMax(0, 0, Vector3.forward);   //
            t_RightForeArm.SetYawMinMax(-150, 5, Vector3.up);  // flex
            t_RightForeArm.SetRollMinMax(-150, 20, Vector3.right);   // 
            t_RightForeArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm] = t_RightForeArm;
        }
        private void ResetLeftUpperArm()
        {

            SimpleROM t_LeftUpperArm = new SimpleROM();
            t_LeftUpperArm.SetPitchMinMax(-60, 100, Vector3.forward); // up/down
            t_LeftUpperArm.SetYawMinMax(-100, 100, Vector3.up);  // front/back  
            t_LeftUpperArm.SetRollMinMax(-90, 90, Vector3.right);   // twist
            t_LeftUpperArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm] = t_LeftUpperArm;
        }
        private void ResetLeftForeArm()
        {
            SimpleROM t_LeftForeArm = new SimpleROM();
            t_LeftForeArm.SetPitchMinMax(0, 0, Vector3.forward);   // 
            t_LeftForeArm.SetYawMinMax(-5, 150, Vector3.up);  // flex 
            t_LeftForeArm.SetRollMinMax(-150, 20, Vector3.right);   // 
            t_LeftForeArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm] = t_LeftForeArm;
        }
        private void ResetRightThigh()
        {
            SimpleROM t_RightThigh = new SimpleROM();
            t_RightThigh.SetPitchMinMax(-90, 50, Vector3.right); // up/down
            t_RightThigh.SetYawMinMax(-60, 60, Vector3.forward);   // twist
            t_RightThigh.SetRollMinMax(-60, 60, Vector3.up);  //right/left  
            t_RightThigh.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh] = t_RightThigh;
        }
        private void ResetRightCalf()
        {
            SimpleROM t_RightCalf = new SimpleROM();
            t_RightCalf.SetPitchMinMax(0, 150, Vector3.right);   // 
            t_RightCalf.SetYawMinMax(0, 0, Vector3.forward);   // twist
            t_RightCalf.SetRollMinMax(-30, 90, Vector3.up);  // flex 
            t_RightCalf.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf] = t_RightCalf;
        }
        private void ResetLeftThigh()
        {
            SimpleROM t_LeftThigh = new SimpleROM();
            t_LeftThigh.SetPitchMinMax(-90, 50, Vector3.right);     // up/down
            t_LeftThigh.SetYawMinMax(-60, 60, Vector3.forward);     //right/left
            t_LeftThigh.SetRollMinMax(-60, 60, Vector3.up);         // twist
            t_LeftThigh.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh] = t_LeftThigh;
        }
        private void ResetLeftCalf()
        {
            SimpleROM t_LeftCalf = new SimpleROM();
            t_LeftCalf.SetPitchMinMax(0, 150, Vector3.right);
            t_LeftCalf.SetYawMinMax(0, 0, Vector3.forward);
            t_LeftCalf.SetRollMinMax(-90, 30, Vector3.up);
            t_LeftCalf.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf] = t_LeftCalf;
        }
    }
}
