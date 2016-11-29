/** 
* @file RenderedBody.cs
* @brief The rendering component of a Body
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Body_Data.View.Anaylsis;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    /// <summary>
    /// The class that models the in-scene avatar, referencing it's visual and movement Components. 
    /// </summary>
    public class RenderedBody : MonoBehaviour
    {
        public BodyView AssociatedBodyView;
        public GameObject Root;
        public SkinnedMeshRenderer Joints;
        public SkinnedMeshRenderer Torso;
        public SkinnedMeshRenderer Limbs;
        public AnaylsisFeedBackContainer AnaylsisFeedBackContainer;

        public Shader NormalShader;
        public Shader XRayShader;
        public Transform UpperLeftArm;
        public Transform LowerLeftArm;
        public Transform UpperRightArm;
        public Transform LowerRightArm;
        public Transform UpperLeftLeg;
        public Transform LowerLeftLeg;
        public Transform UpperRightLeg;
        public Transform LowerRightLeg;
        public Transform Hips;
        public Transform UpperSpine;
        public GameObject[] LayerCopyListeners;

        public CalibrationData.RangeOfMotion.StaticROM ROM;



		[SerializeField]
        private BodyStructureMap.BodyTypes mCurrentBodyType;
        internal Dictionary<BodyStructureMap.SubSegmentTypes, SegmentInteractibleObjects> TransformMapping = new Dictionary<BodyStructureMap.SubSegmentTypes, SegmentInteractibleObjects>(10);
        private LayerMask mCurrLayerMask;
        /// <summary>
        /// Getter and setter property: assigns the layer mask to the associated SkinnedMeshes
        /// </summary>
        public LayerMask CurrentLayerMask
        {
            get
            {
                mCurrLayerMask = gameObject.layer;
                return mCurrLayerMask;
            }
            set
            {
                mCurrLayerMask = value;
                Joints.gameObject.layer = mCurrLayerMask;
                Limbs.gameObject.layer = mCurrLayerMask;
                Torso.gameObject.layer = mCurrLayerMask;
                gameObject.layer = mCurrLayerMask;
                foreach (var vKvPair in TransformMapping)
                {
                    vKvPair.Value.Transform.gameObject.layer = mCurrLayerMask;
                }
                if (LayerCopyListeners != null)
                {
                    for (int i = 0; i < LayerCopyListeners.Length; i++)
                    {
                        LayerCopyListeners[i].layer = mCurrLayerMask;
                    }
                }
            }
        }



        /// <summary>
        /// Applies a transformation to the skin based on the body type, defaulted to full body
        /// </summary>
        /// <param name="vTypes"></param>
        public void Init(BodyStructureMap.BodyTypes vType = BodyStructureMap.BodyTypes.BodyType_FullBody)
        {
            ROM = new CalibrationData.RangeOfMotion.StaticROM();
            mCurrentBodyType = vType; 
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf, new SegmentInteractibleObjects(LowerLeftLeg, 6, Limbs));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh, new SegmentInteractibleObjects(UpperLeftLeg, 4, Limbs));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf, new SegmentInteractibleObjects(LowerRightLeg, 7, Limbs));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh, new SegmentInteractibleObjects(UpperRightLeg, 5, Limbs));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine, new SegmentInteractibleObjects(UpperSpine,0, Torso));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine, new SegmentInteractibleObjects(Hips, 0, Torso));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm, new SegmentInteractibleObjects(LowerLeftArm, 0, Limbs));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm, new SegmentInteractibleObjects(UpperLeftArm, 2, Limbs));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm, new SegmentInteractibleObjects(LowerRightArm, 1, Limbs));
            TransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm, new SegmentInteractibleObjects(UpperRightArm, 3, Limbs));
            foreach (var vKvPair in TransformMapping)
            {
                vKvPair.Value.Selectable = vKvPair.Value.Transform.gameObject.AddComponent<SelectableSegment>();
                if (vKvPair.Key != BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine ||
                  vKvPair.Key != BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine)
                {
                    vKvPair.Value.SubsegmentVisibility = vKvPair.Value.Transform.gameObject.AddComponent<SubsegmentVisibility>();
                    vKvPair.Value.SubsegmentVisibility.Initialize(vKvPair.Value.MaterialIndex, vKvPair.Value.AssociatedMeshRenderer);
                    vKvPair.Value.Selectable.SegmentHeldDownEvent += vKvPair.Value.SubsegmentVisibility.ToggleVisiblity;
                }
            }
        }

        /// <summary>
        /// Updates the current body type to the passed in body type
        /// </summary>
        /// <param name="vType"></param>
        public void UpdateBodyType(BodyStructureMap.BodyTypes vType)
        {
            mCurrentBodyType = vType;
        }


        /// <summary>
        /// Hides the segment based on the segment passed in
        /// </summary>
        /// <param name="vSegment"></param>
        public void HideSegment(BodyStructureMap.SegmentTypes vSegment)
        {

        }

        /// <summary>
        /// Request a RulaVisualAngleAnalysis for the current rendered body
        /// </summary>
        /// <param name="vPosturePosition">The posture position</param>
        /// <param name="vShow">Show after initialization: default to false</param>
        /// <returns></returns>
        public RulaVisualAngleAnalysis GetRulaVisualAngleAnalysis(AnaylsisFeedBackContainer.PosturePosition vPosturePosition, bool vShow = false)
        {
            return AnaylsisFeedBackContainer.RequestRulaVisualAngleAnalysis(vPosturePosition, CurrentLayerMask, vShow);
        }

        /// <summary>
        /// Changes a segment's color based on the segment
        /// </summary>
        /// <param name="vSegment"></param>
        /// <param name="vNewColor"></param>
        public void ChangeSegmentColor(BodyStructureMap.SegmentTypes vSegment, Color32 vNewColor)
        {

        }

        /// <summary>
        /// Valides the segment change based on the current body type
        /// </summary>
        /// <param name="vSegment"></param>
        /// <returns></returns>
        private bool ValidateSegmentChange(BodyStructureMap.SegmentTypes vSegment)
        {
            return true;
        }

        /// <summary>
        /// deactivates the current rendered body and cleans up resources
        /// </summary>
        public void Cleanup()
        {

        }

        /// <summary>
        /// get the associated subsegment
        /// </summary>
        /// <param name="sstype"></param>
        /// <returns></returns>
        public Transform GetSubSegmentTransform(BodyStructureMap.SubSegmentTypes sstype)
        {
            return TransformMapping[sstype].Transform;
        }

        /// <summary>
        /// Reset the visibility of the RenderedBody
        /// </summary>
        public void ResetVisiblity()
        {
            foreach (var vKeyPair in TransformMapping)
            {
                vKeyPair.Value.SubsegmentVisibility.IsVisible = true;
            }
        }
    }

    public class SegmentInteractibleObjects
    {
        public Transform Transform;
        public int MaterialIndex;
        public SkinnedMeshRenderer AssociatedMeshRenderer;
        public SubsegmentVisibility SubsegmentVisibility;
        public SelectableSegment Selectable;

        public SegmentInteractibleObjects(Transform vTransform, int vMaterialIndex,
            SkinnedMeshRenderer vSkinnedMeshRenderer)
        {
            Transform = vTransform;
            MaterialIndex = vMaterialIndex;
            AssociatedMeshRenderer = vSkinnedMeshRenderer;


        }
    }
    [Serializable]
    public class InvalidSegmentChangeRequestException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidSegmentChangeRequestException()
        {
        }

        public InvalidSegmentChangeRequestException(string message) : base(message)
        {
        }

        public InvalidSegmentChangeRequestException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidSegmentChangeRequestException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
