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



        [SerializeField]
        private BodyStructureMap.BodyTypes mCurrentBodyType;
        private Dictionary<BodyStructureMap.SubSegmentTypes, Transform> mTransformMapping = new Dictionary<BodyStructureMap.SubSegmentTypes, Transform>(10);
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
                foreach (var vKvPair in mTransformMapping)
                {
                    vKvPair.Value.gameObject.layer = mCurrLayerMask;
                }
                if (LayerCopyListeners != null)
                {
                    for (int i = 0; i < LayerCopyListeners.Length; i ++)
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
            mCurrentBodyType = vType;
            Debug.Log("create subsegments per body type. Everything is set to fullbody now");
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf, LowerLeftLeg);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh, UpperLeftLeg);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf, LowerRightLeg);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh, UpperRightLeg);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine, UpperSpine);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine, Hips);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm, LowerLeftArm);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm, UpperLeftArm);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm, LowerRightArm);
            mTransformMapping.Add(BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm, UpperRightArm);

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
        public RulaVisualAngleAnalysis GetRulaVisualAngleAnalysis( AnaylsisFeedBackContainer.PosturePosition vPosturePosition,bool vShow=false)
        {
             return AnaylsisFeedBackContainer.RequestRulaVisualAngleAnalysis(vPosturePosition,CurrentLayerMask,vShow);
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
        public Transform GetSubSegment(BodyStructureMap.SubSegmentTypes sstype)
        {
            return mTransformMapping[sstype];
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
