using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;

namespace Assets.Scripts.Tests.Investigations
{
    public class BodySegmentHipsCheck : MonoBehaviour
    {
        public RecordingPlayerView Pview;
        public Body Body;
        public float ForwardAngle;
        public float UpAngle;
        public int IndexToCheck = 6;
        public Vector3 GlobalHipsUp;
        public Vector3 HipsForward;
        public Vector3 EulerHipRot;
        public Quaternion HipQuat;
        /// <summary>
        /// Set an index between 6 and 500.
        /// </summary>
        public void SetIndexToCheck()
        {
            int vRandom = Random.Range(6, 500);
        }
        // Use this for initialization
        void Start()
        {
            Pview.RecordingPlayerViewLayoutCreatedEvent += SetBody;
        }

        private void SetBody(RecordingPlayerView vVview)
        {
            Body = vVview.CurrBody;
            Body.View.BodyFrameUpdatedEvent += CheckIndexValue;
        }

        private void CheckIndexValue(BodyFrame vVnewframe)
        {
            if (vVnewframe.Index == IndexToCheck)
            {
                ForwardAngle = BodySegment.ForwardHipsAngle;
                UpAngle = BodySegment.UpHipsAngle;
                GlobalHipsUp = BodySegment.GlobalVectorUp;
                HipsForward = BodySegment.HipForwardDirection;
            }
            HipQuat = BodySegment.HipRot;
            EulerHipRot = HipQuat.eulerAngles;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                BodySegment.SetToUpVector = !BodySegment.SetToUpVector;
            }
        }
    }
}
