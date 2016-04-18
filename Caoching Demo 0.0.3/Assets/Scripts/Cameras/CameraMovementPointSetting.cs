
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved 
*/


using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Data model which holds settings for different view points, from which a camera controller will reference as a camera approaches a LookAtTarget
    /// </summary>
    [Serializable]
    public class CameraMovementPointSetting : MonoBehaviour
    {
        private static Dictionary<FeedbackTextCategory, Dictionary<PlaneNormalFromTransformType, Feedback>> sgFeedbackTextMapping =
          new Dictionary<FeedbackTextCategory, Dictionary<PlaneNormalFromTransformType, Feedback>>();

        private static bool sgFeedbackTextMapComplete=false;
        public Sprite Good;
        public Sprite DownArrow;
        public Sprite UpArrow;
        public Sprite RightArrow;
        public Sprite LeftArrow;
        //Offset from the transform
        public Vector3 Offset;
        public float OrthographicSize = 1f;
        public Transform LookAtTarget;
        public bool FollowLookAtTarget;

        //the Arcsprite's angle at this position.
        public Sprite ArcSprite;

        /// <summary>
        /// A transform from which a plane will orient itself with.
        /// </summary>
        public Transform PlaneOrientationTransform;

        //How far is the object away from the 
        public Vector3 TransformOffset;
        [SerializeField]
        private PlaneNormalFromTransformType PlaneNormalType;
        public float multi = 1f;
        public float OffsetMultiplier=1f;


        void Awake()
        {
            if (!sgFeedbackTextMapComplete)
            {
                //xy plane
                Feedback vMoveElbowUp = new Feedback(UpArrow, "RAISE ELBOW");  
                Feedback vMoveElbowDown = new Feedback(DownArrow, "LOWER ELBOW");

                //xz plane
                Feedback vMoveElbowBack = new Feedback(LeftArrow, "MOVE ELBOW BACKWARDS");
                Feedback vMoveElbowForward = new Feedback(RightArrow, "MOVE ELBOW FORWARD");

                //yz plane
                Feedback vBadSagitalFeedback = new Feedback(DownArrow, "Bad position on sagital plane ");

                //Good feedback
                Feedback vGoodFeedback = new Feedback(Good, "GOOD");

                Dictionary<PlaneNormalFromTransformType, Feedback> vLessThanFeedbackMapping = new Dictionary<PlaneNormalFromTransformType, Feedback>(3);
                vLessThanFeedbackMapping.Add(PlaneNormalFromTransformType.Right, vBadSagitalFeedback);
                vLessThanFeedbackMapping.Add(PlaneNormalFromTransformType.Up , vMoveElbowBack);
                vLessThanFeedbackMapping.Add(PlaneNormalFromTransformType.Forward, vMoveElbowDown);

                Dictionary<PlaneNormalFromTransformType, Feedback> vGreaterThanFeedbackMapping = new Dictionary<PlaneNormalFromTransformType, Feedback>(3);
                vGreaterThanFeedbackMapping.Add(PlaneNormalFromTransformType.Right, vBadSagitalFeedback);
                vGreaterThanFeedbackMapping.Add(PlaneNormalFromTransformType.Up , vMoveElbowForward);
                vGreaterThanFeedbackMapping.Add(PlaneNormalFromTransformType.Forward, vMoveElbowUp);

                Dictionary<PlaneNormalFromTransformType, Feedback> vGoodFeedbackMapping = new Dictionary<PlaneNormalFromTransformType, Feedback>(3);
                vGoodFeedbackMapping.Add(PlaneNormalFromTransformType.Right, vGoodFeedback);
                vGoodFeedbackMapping.Add(PlaneNormalFromTransformType.Forward, vGoodFeedback);
                vGoodFeedbackMapping.Add(PlaneNormalFromTransformType.Up, vGoodFeedback);

                sgFeedbackTextMapping.Add(FeedbackTextCategory.LessThan, vLessThanFeedbackMapping);
                sgFeedbackTextMapping.Add(FeedbackTextCategory.GreaterThan, vGreaterThanFeedbackMapping);
                sgFeedbackTextMapping.Add(FeedbackTextCategory.Good, vGoodFeedbackMapping);
                sgFeedbackTextMapComplete = true;
            }
           
        }

        /// <summary>
        /// Getter that returns feedback text at current point
        /// </summary>
        public Feedback GetFeedBackTextAtCurrentPoint(FeedbackTextCategory vTextCategory)
        {
            return sgFeedbackTextMapping[vTextCategory][PlaneNormalType];
        }


        public Vector3 PlaneNormal
        {
            get
            {
                Vector3 vToBeReturned = Vector3.zero;
                switch (PlaneNormalType)
                {
                    case (PlaneNormalFromTransformType.Up):
                        vToBeReturned = -PlaneOrientationTransform.up;
                        break;
                    case (PlaneNormalFromTransformType.Forward):
                        vToBeReturned = PlaneOrientationTransform.forward;
                        break;
                    case (PlaneNormalFromTransformType.Right):
                        vToBeReturned = PlaneOrientationTransform.right;
                        break;
                }
                return vToBeReturned;
            }
        }

        

        void LateUpdate()
        {
            if (FollowLookAtTarget)
            {
                transform.position = LookAtTarget.position  + (((LookAtTarget.forward  )*  1f * multi )) + Offset  ;
            }
        }
        public enum PlaneNormalFromTransformType
        {
            Up,
            Forward,
            Right
        }

        public enum FeedbackTextCategory
        {
            LessThan,
            Good,
            GreaterThan
        }

        /// <summary>
        /// A structure to hold feedback strings with their respective sprites
        /// </summary>
        public struct Feedback
        {
            public string FeedbackMSG;
            public Sprite FeedbackImg;

            public Feedback(Sprite vFeedbackImage, string vFeedbackMsg)
            {
                FeedbackImg = vFeedbackImage;
                FeedbackMSG = vFeedbackMsg;
            }
        }
    }
}
