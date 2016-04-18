/** 
* @file AnaylsisFeedBackContainer.cs
* @brief Container for analysis feedback, depends on Body
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View.Anaylsis
{
    /// <summary>
    /// A container of feedback objects that are dependent on the body
    /// </summary>
    public class AnaylsisFeedBackContainer : MonoBehaviour
    {
        [SerializeField]
        private RenderedBody mRenderedBody;
        public GameObject JointAngleContainer;
        public Transform SpineUp;
        public Transform HipsFwd;
        public RulaVisualAngleAnalysis DefaultItem;

        private static Dictionary<PosturePosition, PostureStartEndStructure> sGPostureStartAssociation;
        private readonly Dictionary<PosturePosition, RulaVisualAngleAnalysis> mRulaVisualAngleContainer = new Dictionary<PosturePosition, RulaVisualAngleAnalysis>();

        public RenderedBody RenderedBody
        {
            get { return mRenderedBody; }
            set { mRenderedBody = value; }
        }

        /// <summary>
        /// Predefined set of Associative mapping of Posture start end structure 
        /// </summary>
        public Dictionary<PosturePosition, PostureStartEndStructure> PostureStartAssociation
        {
            get
            {
                if (sGPostureStartAssociation == null)
                {
                    sGPostureStartAssociation = new Dictionary<PosturePosition, PostureStartEndStructure>();
                    Init();
                }
                return sGPostureStartAssociation;
            }
        }

        /// <summary>
        /// Request an visual angle analysis for the current view. 
        /// </summary>
        /// <param name="vPosturePos"></param>
        /// <param name="vLayerMask"></param>
        /// <param name="vShow"></param>
        public RulaVisualAngleAnalysis RequestRulaVisualAngleAnalysis(PosturePosition vPosturePos, LayerMask vLayerMask, bool vShow= false)
        {
            RulaVisualAngleAnalysis vNewAnalysis = null;
            if (!mRulaVisualAngleContainer.ContainsKey(vPosturePos))
            {
                vNewAnalysis = Instantiate(DefaultItem);
                vNewAnalysis.transform.SetParent(JointAngleContainer.transform, false);
                PostureStartEndStructure vStructure = PostureStartAssociation[vPosturePos];
                vNewAnalysis.Init(vPosturePos, vStructure.Center, vStructure.End, vShow );
                mRulaVisualAngleContainer.Add(vPosturePos, vNewAnalysis);
            }
            else
            {
                PostureStartEndStructure vStructure = PostureStartAssociation[vPosturePos];
                vNewAnalysis = mRulaVisualAngleContainer[vPosturePos];
                vNewAnalysis.Init(vPosturePos, vStructure.Center, vStructure.End ,vShow);
            }
            vNewAnalysis.UpdateMask(vLayerMask);

            return vNewAnalysis;
        }



        /// <summary>
        ///Initialize the container
        /// </summary>
        private void Init()
        {
            PostureStartEndStructure vTrunkFlexionExtensionStruct = new PostureStartEndStructure
            {
                Center = RenderedBody.Hips,
                End = SpineUp
            };
            sGPostureStartAssociation.Add(PosturePosition.TrunkFlexionExtension, vTrunkFlexionExtensionStruct);

            PostureStartEndStructure vTrunkRotation = new PostureStartEndStructure()
            {
                Center = RenderedBody.Hips,
                End = HipsFwd
            };
            sGPostureStartAssociation.Add(PosturePosition.TrunkRotation, vTrunkRotation);
        }


        public enum PosturePosition
        {
            Null,
            TrunkFlexionExtension,
            TrunkRotation,
            TrunkSideBending,
            LeftElbow,
            RightElbow,
            LeftShoulder,
            RightShoulder,
            LeftKnee,
            RightKnee,
            LeftHip,
            RightHip
        }

    

        /// <summary>
        /// Updates all layer masks of elements in the container
        /// </summary>
        /// <param name="vNewLayerMask">the new layer mask to update to</param>
        public void UpdateAllVisualLayers(LayerMask vNewLayerMask)
        {
            foreach (var vKeyPair in mRulaVisualAngleContainer)
            {
                vKeyPair.Value.UpdateMask(vNewLayerMask);
            }
        }
        public enum AnatomicalPlane
        {
            /// <summary>
            /// XY plane
            /// </summary>
            Sagital,
            /// <summary>
            /// XZ plane
            /// </summary>
            Transverse,
            /// <summary>
            /// YZ plane
            /// </summary>
            Coronal
        }
    }

    public class PostureStartEndStructure
    {
        public Transform Center;
        public Transform End;
    }
}