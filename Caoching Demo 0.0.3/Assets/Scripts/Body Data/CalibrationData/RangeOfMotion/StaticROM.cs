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
            SimpleROM t_LowerSpine = new SimpleROM();
			t_LowerSpine.SetPitchMinMax(-40, 40, Vector3.right);
			t_LowerSpine.SetYawMinMax(-40, 40, Vector3.up);
			t_LowerSpine.SetRollMinMax(-40, 40, Vector3.forward);
            t_LowerSpine.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine] = t_LowerSpine;
			SimpleROM t_UpperSpine = new SimpleROM();            
			t_UpperSpine.SetPitchMinMax(-40,40, Vector3.right);            
			t_UpperSpine.SetYawMinMax(-40,40, Vector3.up);            
			t_UpperSpine.SetRollMinMax(-40, 40, Vector3.forward);
            t_UpperSpine.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine] = t_UpperSpine;


            SimpleROM t_RightUpperArm = new SimpleROM();
			t_RightUpperArm.SetPitchMinMax(-60, 100, Vector3.forward); // up/down
			t_RightUpperArm.SetYawMinMax(-100, 100, Vector3.up);  // front/back  
			t_RightUpperArm.SetRollMinMax(-90, 90, Vector3.right);   // twist
            t_RightUpperArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm] = t_RightUpperArm;
            SimpleROM t_RightForeArm = new SimpleROM();
			t_RightForeArm.SetPitchMinMax(0, 0, Vector3.forward );   //
			t_RightForeArm.SetYawMinMax(-150, 5, Vector3.up);  // flex
			t_RightForeArm.SetRollMinMax(-150, 20, Vector3.right);   // 
            t_RightForeArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm] = t_RightForeArm;

            SimpleROM t_LeftUpperArm = new SimpleROM();
			t_LeftUpperArm.SetPitchMinMax(-60, 100, Vector3.forward); // up/down
			t_LeftUpperArm.SetYawMinMax(-100, 100, Vector3.up);  // front/back  
			t_LeftUpperArm.SetRollMinMax(-90, 90, Vector3.left);   // twist
            t_LeftUpperArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm] = t_LeftUpperArm;
            SimpleROM t_LeftForeArm = new SimpleROM();
			t_LeftForeArm.SetPitchMinMax(0, 0, Vector3.left);   // 
			t_LeftForeArm.SetYawMinMax(-5, 150, Vector3.up);  // flex 
			t_LeftForeArm.SetRollMinMax(-150, 20, Vector3.forward);   // 
            t_LeftForeArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm] = t_LeftForeArm;


            SimpleROM t_RightThigh = new SimpleROM();
			t_RightThigh.SetPitchMinMax(-90, 50, Vector3.right); // up/down
			t_RightThigh.SetYawMinMax(-60, 60, Vector3.up);  //right/left  
			t_RightThigh.SetRollMinMax(-60, 60, Vector3.forward);   // twist
            t_RightThigh.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh] = t_RightThigh;

            SimpleROM t_RightCalf = new SimpleROM();
			t_RightCalf.SetPitchMinMax(0,150, Vector3.right);   // 
			t_RightCalf.SetYawMinMax(-20, 20, Vector3.up);  // flex 
			t_RightCalf.SetRollMinMax(0, 0, Vector3.forward);   // twist
            t_RightCalf.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf] = t_RightCalf;

            SimpleROM t_LeftThigh = new SimpleROM();
			t_LeftThigh.SetPitchMinMax(-90, 50, Vector3.right); // up/down
			t_LeftThigh.SetYawMinMax(-60, 60, Vector3.up);  //right/left
			t_LeftThigh.SetRollMinMax(-60, 60, Vector3.forward);   // twist
            t_LeftThigh.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh] = t_LeftThigh;
            SimpleROM t_LeftCalf = new SimpleROM();
			t_LeftCalf.SetPitchMinMax(0,150, Vector3.right);   // twist
			t_LeftCalf.SetYawMinMax(-20, 20, Vector3.up);  // flex
			t_LeftCalf.SetRollMinMax(0, 0, Vector3.forward);   // twist
            t_LeftCalf.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf);
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf] = t_LeftCalf;
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
        private bool ConstraintRoll(AngleConstraint a_RollConst, ref Quaternion a_quat, BodyStructureMap.SubSegmentTypes a_subType)
        {
            Vector3 localOrthoAxe;
            float t_RollAngle;
            //special case for arms
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm ||
                a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm ||
                a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm || 
                a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm  )
            {
                localOrthoAxe = a_quat * Vector3.forward;
                localOrthoAxe = Vector3.ProjectOnPlane(localOrthoAxe, a_quat * Vector3.right);
                //localOrthoAxe.Normalize();
                t_RollAngle = Mathf.Atan2(localOrthoAxe.y, localOrthoAxe.x) * Mathf.Rad2Deg;
            }
            else
            {
                localOrthoAxe = a_quat * Vector3.right;
                t_RollAngle = Mathf.Atan2(localOrthoAxe.y, localOrthoAxe.x) * Mathf.Rad2Deg;
            }

            float dif = 0;
            if (a_RollConst != null)
            {
                if (t_RollAngle < a_RollConst.minAngle)
                {
                    dif = a_RollConst.minAngle - t_RollAngle;
                }
                else if (t_RollAngle > a_RollConst.maxAngle)
                {
                    dif = a_RollConst.maxAngle - t_RollAngle;
                }
            }
            a_quat = a_quat * Quaternion.Euler(0, 0, dif);
            return Mathf.Abs(dif) < 1;
        }
        private bool ConstraintPitch(AngleConstraint a_PitchConst, ref Quaternion a_quat, BodyStructureMap.SubSegmentTypes a_subType)
        {
            //Vector3 localOrthoAxe = a_quat * Vector3.up;
            Vector3 localOrthoAxe;// = Vector3.zero;
            //special case for arms
            if (a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm ||
                a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm ||
                a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm ||
                a_subType == BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm)
            {
                localOrthoAxe = a_quat * Vector3.right;
            }
            else
            {
                localOrthoAxe = a_quat * Vector3.up;
            }
            float t_PitchAngle = Mathf.Atan2(localOrthoAxe.z, localOrthoAxe.y) * Mathf.Rad2Deg;
            float dif = 0;
            if (a_PitchConst != null)
            {
                if (t_PitchAngle < a_PitchConst.minAngle)
                {
                    dif = a_PitchConst.minAngle - t_PitchAngle;
                }
                else if (t_PitchAngle > a_PitchConst.maxAngle)
                {
                    dif = a_PitchConst.maxAngle - t_PitchAngle;
                }
            }
            a_quat = a_quat * Quaternion.Euler(dif,0 ,0);
            return Mathf.Abs(dif) < 1;
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
//        public void capIMURotation(BodyStructureMap.SegmentTypes aSegType, BodyStructureMap.SubSegmentTypes aSubType, BodySubSegment aSubSeg, Vector3 Eulers, Quaternion refquat)
//        {
//            Quaternion qcomp = Quaternion.Euler(Eulers);
//            float angle = Quaternion.Angle(refquat, qcomp);
//            if (Mathf.Abs(angle) > 1.5f)
//            {
//                Debug.LogError("not same angles, dif is: " + angle);
//            }

//            AngleConstraint Xconstraint = squeletteRom[(int)aSubType].PitchMinMax.NegateForIMU(Vector3.right);
//            AngleConstraint Yconstraint = squeletteRom[(int)aSubType].RollMinMax.NegateForIMU(Vector3.up);
//            AngleConstraint Zconstraint = squeletteRom[(int)aSubType].YawMinMax;


//            ClampAngles(ref Eulers, Xconstraint, Yconstraint, Zconstraint);
//            Quaternion tQuat = Quaternion.Euler(-Eulers.x, -Eulers.z, Eulers.y);

////             if (Reference != null)
////             {
////                 BodySubSegment refsubSeg = Reference.BodySegments.Find(x => x.SegmentType == aSegType).BodySubSegmentsDictionary[(int)aSubType];
////                 refsubSeg.UpdateSubsegmentOrientation(tQuat, 0, true);
////             }
//        }

        public void capRotation(BodyStructureMap.SegmentTypes aSegType, BodyStructureMap.SubSegmentTypes aSubType, BodySubSegment aSubSeg, ref Quaternion aQuat, bool local = false)
        {
            capRotation(aSegType, aSubType, aSubSeg.GetSubSegmentTransform(), ref aQuat, local);
        }
        public Quaternion capRotation(BodyStructureMap.SegmentTypes aSegType, BodyStructureMap.SubSegmentTypes aSubType, Transform tSubSeg, ref Quaternion aQuat, bool local = false)
		{
			if (viewerCam == null)
			{
				TempCameraSettings();
			}

            if (//aSubType != BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm 
                // && 
                aSubType != BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm )
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
            //Vector3 tEuler = tQuat.eulerAngles;
            //if (mFlags.IsAdjustingSegmentAxis)
            //{
            //    ExtractAngles(tQuat, ref tAngles);
            //    ClampAngles(ref tAngles, Xconstraint, Yconstraint, Zconstraint);
            //    tQuat = Quaternion.Euler(tAngles.x, tAngles.y, tAngles.z);
            //}
            //else
            //{
            ExtractUsingConstraint(ref tQuat, PitchConstraint, YawConstraint, RollConstraint, aSubType);
            //}


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

            if (Reference != null)
            {
                BodySubSegment refsubSeg = Reference.BodySegments.Find(x => x.SegmentType == aSegType).BodySubSegmentsDictionary[(int)aSubType];
                refsubSeg.UpdateSubsegmentOrientation(tQuat, 2/*local ? 0 : 3*/, false);
            }

            return tQuat;
		}
	}
}
