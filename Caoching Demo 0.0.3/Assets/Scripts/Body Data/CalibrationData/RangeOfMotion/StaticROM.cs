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
        private bool ConstraintYaw(AngleConstraint a_YawConst, Vector3 a_SegmentAxe, Vector3 a_upwards, ref Quaternion a_quat, BodyStructureMap.SubSegmentTypes a_subType)
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
        private bool ConstraintRoll(AngleConstraint a_RollAC, Vector3 a_SegmentAxe, Vector3 a_upwards, ref Quaternion a_quat, BodyStructureMap.SubSegmentTypes a_subType)
        {
            //Vector3 localOrthoAxe;
            float t_RollAngle;
            //Vector3 t_upwards = a_RollAC.GetUpVector();

            Quaternion localWithoutRoll = Quaternion.LookRotation(a_quat * a_RollAC.axe, a_upwards);
            Quaternion localWithRoll = Quaternion.LookRotation(a_quat * a_RollAC.axe, a_quat * a_upwards);

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


        public class PitchResult
        {
            public Color color = Color.clear;
            public Vector3 proj = Vector3.zero;
            public float radius = 0.0f;
            public bool inBound = false;
        }
        public PitchResult m_Res = new PitchResult();

        private PitchResult PitchBothPosBounds(Vector3 constraintUp, Vector3 tvert, Vector3 tFlat, float m_RadiusConeMin, float m_RadiusConeMax, float m_RadiusProjOnConeMin, float m_RadiusProjOnConeMax)
        {
            PitchResult tRes = new PitchResult();

            Vector3 t_AxeProjOnConeMax = tvert + tFlat.normalized * m_RadiusProjOnConeMax;
            Vector3 t_AxeProjOnConeMin = tvert + tFlat.normalized * m_RadiusProjOnConeMin;

            if (tvert.normalized != constraintUp)
            {
                tRes.color = Color.magenta;
                tRes.proj = -tvert + tFlat.normalized * m_RadiusProjOnConeMin;
                tRes.radius = m_RadiusProjOnConeMin;
                tRes.inBound = false;
            }
            else
            {
                if (m_RadiusProjOnConeMax > m_RadiusConeMax)
                {
                    tRes.color = Color.red;
                    tRes.proj = t_AxeProjOnConeMax;
                    tRes.radius = m_RadiusProjOnConeMax;
                    tRes.inBound = false;
                }
                else if (m_RadiusProjOnConeMin < m_RadiusConeMin)
                {
                    tRes.color = Color.black;
                    tRes.proj = t_AxeProjOnConeMin;
                    tRes.radius = m_RadiusProjOnConeMin;
                    tRes.inBound = false;
                }
                else
                {
                    tRes.color = Color.cyan;
                    tRes.proj = t_AxeProjOnConeMax;
                    tRes.radius = m_RadiusProjOnConeMax;
                    tRes.inBound = true;
                }
            }
            return tRes;
        }

        private PitchResult PitchHalfPosBounds(Vector3 constraintUp, Vector3 tvert, Vector3 tFlat, float a_RadiusConeMin, float a_RadiusConeMax, float a_RadiusProjOnConeMin, float a_RadiusProjOnConeMax)
        {
            PitchResult tRes = new PitchResult();
            Vector3 t_AxeProjOnConeMax = tvert + tFlat.normalized * a_RadiusProjOnConeMax;
            Vector3 t_AxeProjOnConeMin = tvert - tFlat.normalized * a_RadiusProjOnConeMin;

            if (tvert.normalized != constraintUp) // bottom side
            {
                tRes.proj = t_AxeProjOnConeMin;
                tRes.radius = a_RadiusProjOnConeMin;
                if (a_RadiusProjOnConeMin < a_RadiusConeMin)
                {
                    tRes.color = Color.black;
                    tRes.inBound = false;
                }
                else
                {
                    tRes.color = Color.green;
                    tRes.inBound = true;
                }
            }
            else // upper side
            {
                tRes.proj = t_AxeProjOnConeMax;
                tRes.radius = a_RadiusProjOnConeMax;
                if (a_RadiusProjOnConeMax > a_RadiusConeMax)
                {
                    tRes.color = Color.red;
                    tRes.inBound = false;
                }
                else
                {
                    tRes.color = Color.cyan;
                    tRes.inBound = true;
                }
            }
            return tRes;
        }

        private PitchResult PitchBothNegBounds(Vector3 constraintUp, Vector3 tvert, Vector3 tFlat, float m_RadiusConeMin, float m_RadiusConeMax, float m_RadiusProjOnConeMin, float m_RadiusProjOnConeMax)
        {
            PitchResult tRes = new PitchResult();
            if (tvert.normalized == constraintUp) // upper side always false as both constraints are negatives
            {
                tRes.color = Color.magenta;
                tRes.proj = -tvert - tFlat.normalized * m_RadiusProjOnConeMax;
                tRes.radius = m_RadiusProjOnConeMax;
                tRes.inBound = false;
            }
            else
            {
                if (m_RadiusProjOnConeMax > m_RadiusConeMax) // negative part above max
                {
                    tRes.color = Color.red;
                    tRes.proj = tvert - tFlat.normalized * m_RadiusProjOnConeMax;
                    tRes.radius = m_RadiusProjOnConeMax;
                    tRes.inBound = false;
                }
                else if (m_RadiusProjOnConeMin < m_RadiusConeMin) // negative part under min
                {
                    tRes.color = Color.black;
                    tRes.proj = tvert - tFlat.normalized * m_RadiusProjOnConeMin;
                    tRes.radius = m_RadiusProjOnConeMin;
                    tRes.inBound = false;
                }
                else
                {
                    tRes.color = Color.cyan;
                    tRes.proj = tvert - tFlat.normalized * m_RadiusProjOnConeMin;
                    tRes.radius = m_RadiusProjOnConeMin;
                    tRes.inBound = true;

                }
            }
            return tRes;
        }


        // return true if in bounds, meaning no constraint applied
        private bool ConstraintPitch(AngleConstraint a_PitchAC, Vector3 a_SegmentAxe, Vector3 a_upwards, ref Quaternion a_quat, BodyStructureMap.SubSegmentTypes a_subType)
        {
            Vector3 localAxe = a_quat * a_SegmentAxe;
            localAxe.Normalize();
            Vector3 constraintUp = a_upwards;

            float t_maxZ = a_PitchAC.maxAngle;
            float t_minZ = a_PitchAC.minAngle;

            Vector3 tFlat = Vector3.ProjectOnPlane(localAxe, constraintUp);
            Vector3 tvert = localAxe - tFlat;

            //Vector3 t_ConeMax = Quaternion.Euler(0, 0, t_maxZ) * a_SegmentAxe;
            //Vector3 t_ConeMin = Quaternion.Euler(0, 0, t_minZ) * a_SegmentAxe;

            float t_RadiusConeMax = (t_maxZ > 0 ? 1 : -1) * Mathf.Sin((90 - t_maxZ) * Mathf.Deg2Rad);
            float t_RadiusConeMin = (t_minZ > 0 ? 1 : -1) * Mathf.Sin((90 - t_minZ) * Mathf.Deg2Rad);

            float t_RadiusProjOnConeMax = tvert.magnitude * Mathf.Tan((90 - t_maxZ) * Mathf.Deg2Rad);
            float t_RadiusProjOnConeMin = tvert.magnitude * Mathf.Tan((90 - t_minZ) * Mathf.Deg2Rad);

            if (t_maxZ > 0 && t_minZ >= 0)
            {
                m_Res = PitchBothPosBounds(constraintUp, tvert, tFlat, t_RadiusConeMin, t_RadiusConeMax, t_RadiusProjOnConeMin, t_RadiusProjOnConeMax);
            }
            else if (t_maxZ > 0 && t_minZ < 0)
            {
                m_Res = PitchHalfPosBounds(constraintUp, tvert, tFlat, t_RadiusConeMin, t_RadiusConeMax, t_RadiusProjOnConeMin, t_RadiusProjOnConeMax);
            }
            else if (t_maxZ <= 0 && t_minZ < 0)
            {
                m_Res = PitchBothNegBounds(constraintUp, tvert, tFlat, t_RadiusConeMin, t_RadiusConeMax, t_RadiusProjOnConeMin, t_RadiusProjOnConeMax);
            }

            if (!m_Res.inBound && m_Res.proj != Vector3.zero)
                a_quat = Quaternion.LookRotation(m_Res.proj.normalized, a_quat * Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(a_SegmentAxe, a_upwards));


            return m_Res.inBound;
        }

        private void ExtractUsingConstraint(ref Quaternion tQuatLocal,/* AngleConstraint tpitch, AngleConstraint tYaw, AngleConstraint tRoll,*/ BodyStructureMap.SubSegmentTypes a_subType)
        {
            AngleConstraint PitchConstraint = null;
            AngleConstraint YawConstraint = null;
            AngleConstraint RollConstraint = null;

            SimpleROM t_segmentROM = squeletteRom[(int)a_subType];
            PitchConstraint = squeletteRom[(int)a_subType].PitchMinMax;
            YawConstraint = squeletteRom[(int)a_subType].YawMinMax;
            RollConstraint = squeletteRom[(int)a_subType].RollMinMax;

            if (mFlags.Pitch)
                ConstraintPitch(PitchConstraint, t_segmentROM.SegmentAxe, t_segmentROM.GetUpVector(), ref tQuatLocal, a_subType);
            if(mFlags.Roll)
                ConstraintRoll(RollConstraint, t_segmentROM.SegmentAxe, t_segmentROM.GetUpVector(), ref tQuatLocal, a_subType);
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

            //AngleConstraint PitchConstraint = null;
            //AngleConstraint YawConstraint = null;
            //AngleConstraint RollConstraint = null;

            //SimpleROM t_segmentROM = squeletteRom[(int)aSubType];
//             PitchConstraint = squeletteRom[(int)aSubType].PitchMinMax;
//             YawConstraint = squeletteRom[(int)aSubType].YawMinMax;
//             RollConstraint = squeletteRom[(int)aSubType].RollMinMax;
            //Vector3 tAngles = Vector3.zero;
  
            ExtractUsingConstraint(ref tQuat, aSubType);


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
            t_LowerSpine.SegmentAxe = Vector3.up;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine] = t_LowerSpine;
        }
        private void ResetUpperSpine()
        {
            SimpleROM t_UpperSpine = new SimpleROM();
            t_UpperSpine.SetPitchMinMax(-40, 40, Vector3.right);
            t_UpperSpine.SetYawMinMax(-40, 40, Vector3.up);
            t_UpperSpine.SetRollMinMax(-40, 40, Vector3.forward);
            t_UpperSpine.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine);
            t_UpperSpine.SegmentAxe = Vector3.up;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine] = t_UpperSpine;
        }
        private void ResetRightUpperArm()
        {
            SimpleROM t_RightUpperArm = new SimpleROM();
            t_RightUpperArm.SetPitchMinMax(-60, 100, Vector3.forward); // up/down
            t_RightUpperArm.SetYawMinMax(-100, 100, Vector3.up);  // front/back  
            t_RightUpperArm.SetRollMinMax(-90, 90, Vector3.right);   // twist
            t_RightUpperArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm);
            t_RightUpperArm.SegmentAxe = Vector3.right;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm] = t_RightUpperArm;
        }
        private void ResetRightForeArm()
        {
            SimpleROM t_RightForeArm = new SimpleROM();
            t_RightForeArm.SetPitchMinMax(0, 0, Vector3.forward);   //
            t_RightForeArm.SetYawMinMax(-150, 5, Vector3.up);  // flex
            t_RightForeArm.SetRollMinMax(-150, 20, Vector3.right);   // 
            t_RightForeArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm);
            t_RightForeArm.SegmentAxe = Vector3.right;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm] = t_RightForeArm;
        }
        private void ResetLeftUpperArm()
        {

            SimpleROM t_LeftUpperArm = new SimpleROM();
            t_LeftUpperArm.SetPitchMinMax(-60, 100, Vector3.forward); // up/down
            t_LeftUpperArm.SetYawMinMax(-100, 100, Vector3.up);  // front/back  
            t_LeftUpperArm.SetRollMinMax(-90, 90, Vector3.right);   // twist
            t_LeftUpperArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm);
            t_LeftUpperArm.SegmentAxe = Vector3.left;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm] = t_LeftUpperArm;
        }
        private void ResetLeftForeArm()
        {
            SimpleROM t_LeftForeArm = new SimpleROM();
            t_LeftForeArm.SetPitchMinMax(0, 0, Vector3.forward);   // 
            t_LeftForeArm.SetYawMinMax(-5, 150, Vector3.up);  // flex 
            t_LeftForeArm.SetRollMinMax(-150, 20, Vector3.right);   // 
            t_LeftForeArm.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm);
            t_LeftForeArm.SegmentAxe = Vector3.right;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm] = t_LeftForeArm;
        }
        private void ResetRightThigh()
        {
            SimpleROM t_RightThigh = new SimpleROM();
            t_RightThigh.SetPitchMinMax(-90, 50, Vector3.right); // up/down
            t_RightThigh.SetYawMinMax(-60, 60, Vector3.forward);   // twist
            t_RightThigh.SetRollMinMax(-60, 60, Vector3.up);  //right/left  
            t_RightThigh.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh);
            t_RightThigh.SegmentAxe = Vector3.down;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh] = t_RightThigh;
        }
        private void ResetRightCalf()
        {
            SimpleROM t_RightCalf = new SimpleROM();
            t_RightCalf.SetPitchMinMax(0, 150, Vector3.right);   // 
            t_RightCalf.SetYawMinMax(0, 0, Vector3.forward);   // twist
            t_RightCalf.SetRollMinMax(-30, 90, Vector3.up);  // flex 
            t_RightCalf.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf);
            t_RightCalf.SegmentAxe = Vector3.down;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf] = t_RightCalf;
        }
        private void ResetLeftThigh()
        {
            SimpleROM t_LeftThigh = new SimpleROM();
            t_LeftThigh.SetPitchMinMax(-90, 50, Vector3.right);     // up/down
            t_LeftThigh.SetYawMinMax(-60, 60, Vector3.forward);     //right/left
            t_LeftThigh.SetRollMinMax(-60, 60, Vector3.up);         // twist
            t_LeftThigh.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh);
            t_LeftThigh.SegmentAxe = Vector3.down;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh] = t_LeftThigh;
        }
        private void ResetLeftCalf()
        {
            SimpleROM t_LeftCalf = new SimpleROM();
            t_LeftCalf.SetPitchMinMax(0, 150, Vector3.right);
            t_LeftCalf.SetYawMinMax(0, 0, Vector3.forward);
            t_LeftCalf.SetRollMinMax(-90, 30, Vector3.up);
            t_LeftCalf.Name = System.Enum.GetName(typeof(BodyStructureMap.SubSegmentTypes), BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf);
            t_LeftCalf.SegmentAxe = Vector3.down;
            squeletteRom[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf] = t_LeftCalf;
        }
    }
}
