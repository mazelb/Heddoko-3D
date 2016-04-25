using System;
using System.Collections.Generic;
using UnityEngine; 

namespace Assets.Scripts.Body_Data.View.Anaylsis
{
    /// <summary>
    /// Settings for RULA rules, includes static functions 
    /// </summary>
    public static class RulaSettings
    {
        private static Dictionary<AnaylsisFeedBackContainer.PosturePosition, List<RulaPointStructure>>
            sGAngleRangeAssociation;
        private static Dictionary<AnaylsisFeedBackContainer.PosturePosition, float> sGMaskRangeMap;
        private static Dictionary<string, Color32> sGColorMapping;

        /// <summary>
        /// Statically defined set of action mappings
        /// </summary>
        private static Dictionary<AnaylsisFeedBackContainer.PosturePosition, Action<RulaVisualAngleAnalysis>>
            sGActionMap;

        /// <summary>
        /// Return a float value indicating that range of the mask to be applied onto the pie chart
        /// note will return 0 if the position wasn't registered in the map
        /// </summary>
        /// <param name="vPosition">the</param>
        /// <returns></returns>
        public static float GetRangeMapValue(AnaylsisFeedBackContainer.PosturePosition vPosition)
        {
            float vReturn = 0;
            if (sGMaskRangeMap == null)
            {
                InitializePointMaskMap();
            }
            if (sGMaskRangeMap.ContainsKey(vPosition))
            {
                vReturn = sGMaskRangeMap[vPosition];
            }
            return vReturn;
        }

        private static void InitializePointMaskMap()
        {
            sGMaskRangeMap = new Dictionary<AnaylsisFeedBackContainer.PosturePosition, float>();
            sGMaskRangeMap.Add(AnaylsisFeedBackContainer.PosturePosition.TrunkRotation, 220f);
            sGMaskRangeMap.Add(AnaylsisFeedBackContainer.PosturePosition.TrunkFlexionExtension, 220f);
        }

        /// <summary>
        /// Statically defined set of action mappings
        /// Note: this will return null in the case that no actions for the specified Positions were found
        /// </summary>
        public static Action<RulaVisualAngleAnalysis> GetActionMap(
            AnaylsisFeedBackContainer.PosturePosition vPosturePosition)
        {
            if (sGActionMap == null)
            {
                InializeActionMapping();
            }
            if (sGActionMap.ContainsKey(vPosturePosition))
            {
                return sGActionMap[vPosturePosition];
            }
            else
            {
                return null;
            }
        }


        private static void InializeActionMapping()
        {
            sGActionMap = new Dictionary<AnaylsisFeedBackContainer.PosturePosition, Action<RulaVisualAngleAnalysis>>();
            sGActionMap.Add(AnaylsisFeedBackContainer.PosturePosition.TrunkFlexionExtension, SpineAngleCoronalPlane);
            sGActionMap.Add(AnaylsisFeedBackContainer.PosturePosition.TrunkRotation, SpineAngleTransversePlane);

        }

        /// <summary>
        /// get the point structure list for the specified position
        /// </summary>
        /// <param name="vPosturePosition"></param>
        /// <returns></returns>
        public static List<RulaPointStructure> PointStructureList(
            AnaylsisFeedBackContainer.PosturePosition vPosturePosition)
        {
            if (sGAngleRangeAssociation == null)
            {
                InitPointStructureList();
            }

            // ReSharper disable once PossibleNullReferenceException
            return sGAngleRangeAssociation[vPosturePosition];

        }

        /// <summary>
        /// Updates the spine's angular fill with respect to the coronal plane
        /// </summary> 
        private static void SpineAngleCoronalPlane(RulaVisualAngleAnalysis vAnalysis)
        {

            Vector3 vUpVector = vAnalysis.CenteredObject.right;

            //Get the projection of the perfect Vector
            Vector3 vPerfectVectProjection = Vector3.ProjectOnPlane(vAnalysis.TransformComparison.up, vUpVector);
            float vAngle = Vector3.Angle(vAnalysis.CenteredObject.up, vPerfectVectProjection);
            Quaternion vRot = Quaternion.LookRotation(vAnalysis.TransformComparison.right,
                vAnalysis.TransformComparison.up);



            Vector3 vOffset = (0.2f * vAnalysis.CenteredObject.up);
            vAnalysis.PieGraph.transform.position = vAnalysis.CenteredObject.position + vOffset;
            vAnalysis.MaskingImage.transform.position = vAnalysis.CenteredObject.position + vOffset;


            Vector3 vCross = Vector3.Cross(vAnalysis.CenteredObject.up, vPerfectVectProjection);
            float vSign = Mathf.Sign(Vector3.Dot(vUpVector, vCross));

            float vFill = vAngle / 360f;

            //set the image fill from the angles between two vectors

            Quaternion vChildRotation = vRot;

            float vAngleToCompute = vSign < 0 ? 360 - vAngle : vAngle;
            vAnalysis.Point = GetScore(vAnalysis.CurrentPosturePos, vAngleToCompute);

            if (vAnalysis.MaskOnlyRange)
            {
                Quaternion vInv = Quaternion.AngleAxis(vAngle, Vector3.forward);
                vFill = (GetRangeMapValue(vAnalysis.CurrentPosturePos)) / 360f;
                Quaternion vMaskQuat = vRot * Quaternion.Inverse(vInv);
                vAnalysis.MaskingImage.transform.rotation = vMaskQuat;
                //rotate mask by the total range of available motion /2 
                float vAngRot = GetRangeMapValue(vAnalysis.CurrentPosturePos);
                //  Quaternion vAngleRot = Quaternion.AngleAxis(vAngRot, Vector3.forward);
                vAnalysis.MaskingImage.transform.Rotate(Vector3.forward, vAngRot / 2f);


                vChildRotation = vRot * Quaternion.Inverse(vInv);
                vAnalysis.PieGraph.transform.rotation = vChildRotation;
                vAnalysis.VectorAxisGo.transform.up = vAnalysis.TransformComparison.up;


            }
            else
            {
                if (vSign < 0)
                {
                    vAnalysis.MaskingImage.fillClockwise = true;
                }

                else
                {
                    vAnalysis.MaskingImage.fillClockwise = false;
                }
                vAnalysis.MaskingImage.transform.rotation = vRot;
            }


            //set the image fill from the angles between two vectors
            if (!vAnalysis.IsAnimating)
            {
                vAnalysis.MaskingImage.fillAmount = vFill;
            }

        }


        /// <summary>
        /// Updates the RulaVisualAngleAnalysis' angular fill with respect to the tranverse plane of the spine
        /// </summary>
        /// <param name="vAnalysis"></param>

        private static void SpineAngleTransversePlane(RulaVisualAngleAnalysis vAnalysis)
        {
            // Vector3 vUpVector = vAnalysis.TransformComparison.up;
            Vector3 vUpVector = vAnalysis.TransformComparison.up;
            //Get the projection of the perfect Vector
            Vector3 vPerfectVectProjection = Vector3.ProjectOnPlane(vAnalysis.TransformComparison.right,
                vAnalysis.CenteredObject.up);
            float vAngle = Vector3.Angle(vAnalysis.CenteredObject.right, vPerfectVectProjection);


            Vector3 vCross = Vector3.Cross(vAnalysis.CenteredObject.right, vPerfectVectProjection);
            float vSign = Mathf.Sign(Vector3.Dot(vUpVector, vCross));


            float vFill = vAngle / 360f;
            Vector3 vNewPos = vAnalysis.CenteredObject.transform.position + vAnalysis.CenteredObject.up * 0.4f;
            vAnalysis.MaskingImage.transform.position = vNewPos;
            //compute point

            float vAngleToCompute = vSign < 0 ? 360 - vAngle : vAngle;
            vAnalysis.Point = GetScore(vAnalysis.CurrentPosturePos, vAngleToCompute);

            // Quaternion vRot = Quaternion.LookRotation(vAnalysis.TransformComparison.up, vAnalysis.TransformComparison.forward);

            Quaternion vRot = Quaternion.LookRotation(Vector3.up, Vector3.forward);


            if (vAnalysis.MaskOnlyRange)
            {
                vAnalysis.MaskingImage.transform.rotation = vRot;
                vAnalysis.MaskingImage.transform.Rotate(Vector3.forward, 91);
                vFill = GetRangeMapValue(vAnalysis.CurrentPosturePos) / 360f;
                vAnalysis.PieGraph.transform.rotation = vRot;
                Vector3 vEuler = Vector3.zero;
                vEuler.z = vAngle * vSign;
                vAnalysis.VectorAxisGo.transform.localRotation = Quaternion.Euler(vEuler);
                vAnalysis.VectorAxisGo.transform.position = vNewPos + Vector3.up * 0.1f;

            }
            else
            {
                if (vSign < 0)
                {
                    vAnalysis.MaskingImage.fillClockwise = true;
                }

                else
                {
                    vAnalysis.MaskingImage.fillClockwise = false;
                }
                vAnalysis.MaskingImage.transform.rotation = vRot;
            }

            //Set the fill amount if the analyzer isn't animating
            if (!vAnalysis.IsAnimating)
            {
                vAnalysis.MaskingImage.fillAmount = vFill;
            }
        }

        /// <summary>
        /// Get the point color for the rula point
        /// </summary>
        /// <param name="vVar"></param>
        /// <returns></returns>
        public static Color GetPointColorOfRulaPoint(string vVar)
        {
            if (sGColorMapping == null)
            {
                InitColorMapping();
            }
            // ReSharper disable once PossibleNullReferenceException
            return sGColorMapping[vVar];
        }

        /// <summary>
        /// Initialize values
        /// </summary>
        private static void InitPointStructureList()
        {
            sGAngleRangeAssociation =
                new Dictionary<AnaylsisFeedBackContainer.PosturePosition, List<RulaPointStructure>>(9);
            List<RulaPointStructure> vTrunkExtFlex = new List<RulaPointStructure>();
            vTrunkExtFlex.Add(new RulaPointStructure(350f + float.Epsilon, 360f, RulaPoint.OnePointTrExtBuffer));
            vTrunkExtFlex.Add(new RulaPointStructure(340f + float.Epsilon, 350f, RulaPoint.TwoPoint));
            vTrunkExtFlex.Add(new RulaPointStructure(120f + float.Epsilon, 340f, RulaPoint.Null));
            vTrunkExtFlex.Add(new RulaPointStructure(60f + float.Epsilon, 120f, RulaPoint.FourPoint));
            vTrunkExtFlex.Add(new RulaPointStructure(20f + float.Epsilon, 60f, RulaPoint.ThreePoint));
            vTrunkExtFlex.Add(new RulaPointStructure(10f + float.Epsilon, 20f, RulaPoint.TwoPoint));
            vTrunkExtFlex.Add(new RulaPointStructure(float.Epsilon, 10f, RulaPoint.OnePointTrExtBuffer));

            sGAngleRangeAssociation.Add(AnaylsisFeedBackContainer.PosturePosition.TrunkFlexionExtension, vTrunkExtFlex);

            List<RulaPointStructure> vTrunkRot = new List<RulaPointStructure>();
            vTrunkRot.Add(new RulaPointStructure(float.Epsilon, 20f + float.Epsilon, RulaPoint.ZeroPointTrunkTwistBuff));
            vTrunkRot.Add(new RulaPointStructure(20f, 100f + float.Epsilon, RulaPoint.OnePoint));
            vTrunkRot.Add(new RulaPointStructure(100f, 260f + float.Epsilon, RulaPoint.Null));
            vTrunkRot.Add(new RulaPointStructure(260f, 340f + float.Epsilon, RulaPoint.OnePoint));
            vTrunkRot.Add(new RulaPointStructure(340f, 360f, RulaPoint.ZeroPointTrunkTwistBuff));
            sGAngleRangeAssociation.Add(AnaylsisFeedBackContainer.PosturePosition.TrunkRotation, vTrunkRot);



        }

        public static int GetScore(AnaylsisFeedBackContainer.PosturePosition vPosition, float vRotation)
        {

            if (sGAngleRangeAssociation.ContainsKey(vPosition))
            {
                foreach (var vPointStructure in sGAngleRangeAssociation[vPosition])
                {
                    if (vRotation > vPointStructure.StartTheta && vRotation <= vPointStructure.EndTheta)
                    {
                        return RulaPoint.GetPoint(vPointStructure.PointName);
                    }
                }

            }
            return 0;

        }

        private static void InitColorMapping()
        {
            sGColorMapping = new Dictionary<string, Color32>();
            Color vOnePointTrunkFlexionExtBuffer = new Color(255f / 255, 186f / 255, 70f / 255f, 0.6f);
            Color vZeroPointTrunkTwistBuffer = new Color(249f / 255, 69f / 255, 97f / 255f, 0.60f);
            Color vOnePointC = new Color(249f / 255, 69f / 255, 97f / 255f, 0.60f);
            Color vTwoPointC = new Color(255f / 255, 186f / 255, 70f / 255f, 0.6f);
            Color vThreePointC = new Color(250f / 255, 141f / 255, 57f / 255f, 0.6f);
            Color vFourPointC = new Color(249f / 255, 69f / 255, 97f / 255f, 0.60f);
            Color vZeroPointC = new Color(4f / 255, 69f / 255, 97f / 255f, 0.6f);
            Color vClear = new Color(1, 1, 1, 0);

            sGColorMapping.Add(RulaPoint.OnePointTrExtBuffer, vOnePointTrunkFlexionExtBuffer);
            sGColorMapping.Add(RulaPoint.ZeroPointTrunkTwistBuff, vZeroPointTrunkTwistBuffer);
            sGColorMapping.Add(RulaPoint.Null, vClear);
            sGColorMapping.Add(RulaPoint.ZeroPoint, vZeroPointC);
            sGColorMapping.Add(RulaPoint.OnePoint, vOnePointC);
            sGColorMapping.Add(RulaPoint.TwoPoint, vTwoPointC);
            sGColorMapping.Add(RulaPoint.ThreePoint, vThreePointC);
            sGColorMapping.Add(RulaPoint.FourPoint, vFourPointC);

        }
    }
 
}


