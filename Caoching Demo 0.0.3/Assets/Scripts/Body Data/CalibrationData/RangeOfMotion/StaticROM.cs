using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion
{
    public class StaticROM
    {
        public Dictionary<BodyStructureMap.SubSegmentTypes, SimpleROM> squeletteRom = new Dictionary<BodyStructureMap.SubSegmentTypes, SimpleROM>();

        public StaticROM()
        {
            SimpleROM t_UpperSpine = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine] ;
            t_UpperSpine.SetPitchMinMax(-40,40);
            t_UpperSpine.SetYawMinMax(-40,40);
            t_UpperSpine.SetRollMinMax(-40, 40);
            SimpleROM t_LowerSpine = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];
            t_LowerSpine.SetPitchMinMax(-40, 40);
            t_LowerSpine.SetYawMinMax(-40, 40);
            t_LowerSpine.SetRollMinMax(-40, 40);

            SimpleROM t_RightUpperArm = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];
            t_RightUpperArm.SetPitchMinMax(-60, 100); // up/down
            t_RightUpperArm.SetYawMinMax(-100, 100);  // front/back  
            t_RightUpperArm.SetRollMinMax(-90, 90);   // twist
            SimpleROM t_RightForeArm = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];
            t_RightForeArm.SetYawMinMax(-80, 80);  // flex
            t_RightForeArm.SetRollMinMax(-20, 150);   // twist

            SimpleROM t_LeftUpperArm = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm];
            t_LeftUpperArm.SetPitchMinMax(-60, 100); // up/down
            t_LeftUpperArm.SetYawMinMax(-100, 100);  // front/back  
            t_LeftUpperArm.SetRollMinMax(-90, 90);   // twist
            SimpleROM t_LeftForeArm = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm];
            t_LeftForeArm.SetYawMinMax(-80, 80);  // flex 
            t_LeftForeArm.SetRollMinMax(-20, 150);   // twist

            SimpleROM t_RightThigh = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh];
            t_RightThigh.SetPitchMinMax(-90, 50); // up/down
            t_RightThigh.SetYawMinMax(-60, 60);  //right/left  
            t_RightThigh.SetRollMinMax(-60, 60);   // twist
            SimpleROM t_RightCalf = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf];
            t_RightCalf.SetYawMinMax(-80, 80);  // flex 
            t_RightCalf.SetRollMinMax(-90, 90);   // twist

            SimpleROM t_LeftThigh = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh];
            t_LeftThigh.SetPitchMinMax(-90, 50); // up/down
            t_LeftThigh.SetYawMinMax(-60, 60);  //right/left  
            t_LeftThigh.SetRollMinMax(-60, 60);   // twist
            SimpleROM t_LeftCalf = squeletteRom[BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf];
            t_LeftCalf.SetYawMinMax(-80, 80);  // flex 
            t_LeftCalf.SetRollMinMax(-90, 90);   // twist
        }

        public void capFrame()
        {

        }
    }
}
