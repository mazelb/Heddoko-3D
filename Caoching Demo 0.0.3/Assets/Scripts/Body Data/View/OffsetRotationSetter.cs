// /**
// * @file OffsetRotationSetter.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 12 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    public class OffsetRotationSetter : MonoBehaviour
    {
        public RenderedBody RenderedBody;

        public Quaternion[] OffsetRotations = new Quaternion[9];
        private Dictionary<BodyStructureMap.SubSegmentTypes, Quaternion> OffsetRotationMap = new Dictionary<BodyStructureMap.SubSegmentTypes, Quaternion>();

        private void Initialize()
        {
            var vBodyTypes = EnumUtil.GetValues<BodyStructureMap.SubSegmentTypes>();
            foreach (var vSubSegmentTypes in vBodyTypes)
            {
                OffsetRotationMap.Add(vSubSegmentTypes, Quaternion.identity);
            }
        }

        void Start()
        {
            Initialize();
            //apply to the rendered body
        }
        public void SetOffsetRotation(BodyStructureMap.SubSegmentTypes vType)
        {
            //Find the associated Segment from the subsegmetn type
            var vSegmentMap = BodyStructureMap.Instance.SegmentToSubSegmentMap;
            var vSegmentType = vSegmentMap.First(x => x.Value.Contains(vType)).Key;
            var vSegment = RenderedBody.AssociatedBodyView.AssociatedBody.GetSegmentFromSegmentType(vSegmentType);
           
        }


    }
}