using System;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.cameraSubControls
{
    public abstract class CameraSubControl : AbstractSubControl
    {
        internal Camera Camera;
        [SerializeField]
        internal Transform Target;
        internal RenderedBody TargetRenderedBody;
        private BodyStructureMap.SubSegmentTypes mLookAtSubsegmentType = BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine;
        internal LayerMask TargetsLayer;
        protected Guid Id = Guid.NewGuid();
        protected Ray Ray;
        protected RaycastHit RaycastHit;
        [SerializeField]
        protected InputState CurrentState = InputState.Idle;
        [SerializeField]
        private bool mIsEnabled;
        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; }
        }
        public override SubControlType SubControlType
        {
            get { throw new System.NotImplementedException(); }
        }

        public void SetTarget(Transform vTarget)
        {
            Target = vTarget;
        }
        public BodyStructureMap.SubSegmentTypes LookAtSubsegmentType
        {
            get { return mLookAtSubsegmentType; }
            set
            {

                mLookAtSubsegmentType = value;
                //change the target subsegment
                Target = TargetRenderedBody.GetSubSegmentTransform(mLookAtSubsegmentType);

            }
        }

        public override void Disable()
        {
            IsEnabled = false;
        }

        public override void Enable()
        {
            IsEnabled = true;
        }

    }
    public enum InputState
    {
        Idle,
        HoveringOverTarget,
        InputDown,
        InputUp
    }
}