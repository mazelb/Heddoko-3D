
/** 
* @file BodyStructureMap.cs
* @brief Contains the BodyStructureMap class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System.Collections.Generic;
using Newtonsoft.Json;
using Assets.Scripts.Utils;
using System.IO;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class BodyStructureMap
{
    #region Singleton definition
    private static readonly BodyStructureMap instance = new BodyStructureMap();

    //has the bodystructuremap been initiated?
    private bool isInitialized;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static BodyStructureMap()
    {
    }

    private BodyStructureMap()
    {
    }

    public static BodyStructureMap Instance
    {
        get
        {
            if (!instance.isInitialized)
            {
                instance.CreateBodyToSegmentMap();
                instance.CreateSegmentToSensorPosMap();
                instance.CreateSegmentToSubSegmentMap();
                instance.CreateSensorPosToSensorIDMap();
                instance.CreateSensorPosToSensorTypeMap();
                instance.isInitialized = true;
            }
            return instance;
        }
    }
    #endregion


    public struct TrackingStructure
    {
        public BodyFrame.Vect4 InitRawEuler;
        public BodyFrame.Vect4 CurrRawEuler;
        public CalibrationStructure CalibrationData;
    };

    public struct CalibrationStructure
    {
        public int HistoryCount;
        public BodyFrame.Vect4 MinRawEuler;
        public BodyFrame.Vect4 MaxRawEuler;
        public BodyFrame.Vect4 AvgRawEulerDiff;
        public List<BodyFrame.Vect4> PrevRawEulers;
    };

    //Body Types
    public enum BodyTypes
    {
        BodyType_FullBody = 0,
        BodyType_UpperBody = 1,
        BodyType_LowerBody = 2,
        BodyType_Limbs = 3,
        BodyType_Count
    };

    //Segment Types
    public enum SegmentTypes
    {
        SegmentType_Trunk = 0,
        SegmentType_RightArm = 1,
        SegmentType_LeftArm = 2,
        SegmentType_RightLeg = 3,
        SegmentType_LeftLeg = 4,
        SegmentType_Hips = 5,
        SegmentType_Count
    };

    //Sub Segment Types 
    public enum SubSegmentTypes
    {
        SubsegmentType_UpperSpine = 0,
        SubsegmentType_LowerSpine = 1,
        SubsegmentType_RightUpperArm = 2,
        SubsegmentType_RightForeArm = 3,
        SubsegmentType_LeftUpperArm = 4,
        SubsegmentType_LeftForeArm = 5,
        SubsegmentType_RightThigh = 6,
        SubsegmentType_RightCalf = 7,
        SubsegmentType_LeftThigh = 8,
        SubsegmentType_LeftCalf = 9,
        SubsegmentType_Count
    };

    //Sensors positions
    public enum SensorPositions
    {
        SP_UpperSpine = 0,
        SP_LowerSpine = 1,
        SP_RightUpperArm = 2,
        SP_RightForeArm = 3,
        SP_LeftUpperArm = 4,
        SP_LeftForeArm = 5,
        SP_RightThigh = 6,
        SP_RightCalf = 7,
        SP_LeftThigh = 8,
        SP_LeftCalf = 9,
        SP_RightElbow = 10,
        SP_LeftElbow = 11,
        SP_RightKnee = 12,
        SP_LeftKnee = 13,
        SP_SensorPositionCount
    }

    //Sensors types
    public enum SensorTypes
    {
        ST_Default = 0, //No data in SensorData
        ST_Biomech = 1, //Data: Yaw, Pitch, Roll
        ST_Flexcore = 2, //Data: Flex Value
        ST_HeartRate = 3, //Data (TBD)
        ST_SensorTypeCount
    }

    //  //Body structure maps
    [JsonProperty]
    public Dictionary<BodyTypes, List<SegmentTypes>> BodyToSegmentMap = new Dictionary<BodyTypes, List<SegmentTypes>>();
    [JsonProperty]
    public Dictionary<SegmentTypes, List<SubSegmentTypes>> SegmentToSubSegmentMap = new Dictionary<SegmentTypes, List<SubSegmentTypes>>();
    [JsonProperty]
    public Dictionary<SegmentTypes, List<SensorPositions>> SegmentToSensorPosMap = new Dictionary<SegmentTypes, List<SensorPositions>>();
    [JsonProperty]
    public Dictionary<SensorPositions, SensorTypes> SensorPosToSensorTypeMap = new Dictionary<SensorPositions, SensorTypes>();
    [JsonProperty]
    public Dictionary<SensorPositions, int> SensorPosToSensorIDMap = new Dictionary<SensorPositions, int>();

    //Build body structure maps
    public void ReadBodyStructureFile()
    {
        string vPath = FilePathReferences.LocalSavedDataPath("body_structure_map.json");
        BodyStructureMap vBSm = null;
        try
        {
            vBSm = JsonUtilities.JsonFileToObject<BodyStructureMap>(vPath);
        }
        //in case the file isn't found, write a new one with generic values
        catch (FileNotFoundException)
        {
            CreateBodyToSegmentMap();
            CreateSegmentToSensorPosMap();
            CreateSegmentToSubSegmentMap();
            CreateSensorPosToSensorIDMap();
            CreateSensorPosToSensorTypeMap();
            JsonUtilities.ConvertObjectToJson(vPath, this);
            return;
        }

        this.BodyToSegmentMap = vBSm.BodyToSegmentMap;
        this.SegmentToSubSegmentMap = vBSm.SegmentToSubSegmentMap;
        this.SegmentToSensorPosMap = vBSm.SegmentToSensorPosMap;
        this.SensorPosToSensorTypeMap = vBSm.SensorPosToSensorTypeMap;
        this.SensorPosToSensorIDMap = vBSm.SensorPosToSensorIDMap;
        vBSm = null; //discard bsm
    }

    /// <summary>
    /// Writes a bodystructure map to file
    /// </summary>
    public void WriteBodyStructureFile()
    {
        string path = FilePathReferences.LocalSavedDataPath("body_structure_map.json");
        JsonUtilities.ConvertObjectToJson(path, this);
    }

    public void CreateBodyToSegmentMap()
    {
        //BodyToSegmentMap = new Dictionary<BodyTypes, List<SegmentTypes>>();

        var vBodyTypes = EnumUtil.GetValues<BodyTypes>();
        foreach (BodyTypes vBodyType in vBodyTypes)
        {
            switch (vBodyType)
            {
                case BodyTypes.BodyType_FullBody:
                    {
                        List<SegmentTypes> vFullBodySegments = new List<SegmentTypes>();
                        var vSegmentTypes = EnumUtil.GetValues<SegmentTypes>();

                        foreach (SegmentTypes vSegmentType in vSegmentTypes)
                        {
                            if (vSegmentType != SegmentTypes.SegmentType_Count)
                            {
                                vFullBodySegments.Add(vSegmentType);
                            }
                        }

                        BodyToSegmentMap.Add(BodyTypes.BodyType_FullBody, vFullBodySegments);
                    }
                    break;
                case BodyTypes.BodyType_UpperBody:
                    {
                        List<SegmentTypes> vUpperBodySegments = new List<SegmentTypes>();
                        vUpperBodySegments.Add(SegmentTypes.SegmentType_Trunk);
                        vUpperBodySegments.Add(SegmentTypes.SegmentType_RightArm);
                        vUpperBodySegments.Add(SegmentTypes.SegmentType_LeftArm);
                        BodyToSegmentMap.Add(BodyTypes.BodyType_UpperBody, vUpperBodySegments);
                    }
                    break;
                case BodyTypes.BodyType_LowerBody:
                    {
                        List<SegmentTypes> vLowerBodySegments = new List<SegmentTypes>();
                        vLowerBodySegments.Add(SegmentTypes.SegmentType_RightLeg);
                        vLowerBodySegments.Add(SegmentTypes.SegmentType_LeftLeg);
                        BodyToSegmentMap.Add(BodyTypes.BodyType_LowerBody, vLowerBodySegments);
                    }
                    break;
                case BodyTypes.BodyType_Limbs:
                    {
                        List<SegmentTypes> vLimbsBodySegments = new List<SegmentTypes>();
                        vLimbsBodySegments.Add(SegmentTypes.SegmentType_RightArm);
                        vLimbsBodySegments.Add(SegmentTypes.SegmentType_LeftArm);
                        vLimbsBodySegments.Add(SegmentTypes.SegmentType_RightLeg);
                        vLimbsBodySegments.Add(SegmentTypes.SegmentType_LeftLeg);
                        BodyToSegmentMap.Add(BodyTypes.BodyType_Limbs, vLimbsBodySegments);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void CreateSegmentToSubSegmentMap()
    {
        var vSegmentTypes = EnumUtil.GetValues<SegmentTypes>();
        foreach (SegmentTypes vSegmentType in vSegmentTypes)
        {
            switch (vSegmentType)
            {
                case SegmentTypes.SegmentType_Trunk:
                    {
                        List<SubSegmentTypes> vTorsoSubSegments = new List<SubSegmentTypes>();
                        vTorsoSubSegments.Add(SubSegmentTypes.SubsegmentType_LowerSpine);
                        vTorsoSubSegments.Add(SubSegmentTypes.SubsegmentType_UpperSpine);
                        SegmentToSubSegmentMap.Add(SegmentTypes.SegmentType_Trunk, vTorsoSubSegments);
                    }
                    break;
                case SegmentTypes.SegmentType_Hips:
                    {
                        List<SubSegmentTypes> vHipSubSegments = new List<SubSegmentTypes>();
                        vHipSubSegments.Add(SubSegmentTypes.SubsegmentType_LowerSpine);
                        vHipSubSegments.Add(SubSegmentTypes.SubsegmentType_UpperSpine);
                        SegmentToSubSegmentMap.Add(SegmentTypes.SegmentType_Hips, vHipSubSegments);
                    }
                    break;
                case SegmentTypes.SegmentType_RightArm:
                    {
                        List<SubSegmentTypes> vRightArmSubSegments = new List<SubSegmentTypes>();
                        vRightArmSubSegments.Add(SubSegmentTypes.SubsegmentType_UpperSpine);
                        vRightArmSubSegments.Add(SubSegmentTypes.SubsegmentType_RightUpperArm);
                        vRightArmSubSegments.Add(SubSegmentTypes.SubsegmentType_RightForeArm);
                        SegmentToSubSegmentMap.Add(SegmentTypes.SegmentType_RightArm, vRightArmSubSegments);
                    }
                    break;
                case SegmentTypes.SegmentType_LeftArm:
                    {
                        List<SubSegmentTypes> vLeftArmSubSegments = new List<SubSegmentTypes>();
                        vLeftArmSubSegments.Add(SubSegmentTypes.SubsegmentType_UpperSpine);
                        vLeftArmSubSegments.Add(SubSegmentTypes.SubsegmentType_LeftUpperArm);
                        vLeftArmSubSegments.Add(SubSegmentTypes.SubsegmentType_LeftForeArm);
                        SegmentToSubSegmentMap.Add(SegmentTypes.SegmentType_LeftArm, vLeftArmSubSegments);
                    }
                    break;
                case SegmentTypes.SegmentType_RightLeg:
                    {
                        List<SubSegmentTypes> vRightLegSubSegments = new List<SubSegmentTypes>();
                        vRightLegSubSegments.Add(SubSegmentTypes.SubsegmentType_LowerSpine);
                        vRightLegSubSegments.Add(SubSegmentTypes.SubsegmentType_RightThigh);
                        vRightLegSubSegments.Add(SubSegmentTypes.SubsegmentType_RightCalf);
                        SegmentToSubSegmentMap.Add(SegmentTypes.SegmentType_RightLeg, vRightLegSubSegments);
                    }
                    break;
                case SegmentTypes.SegmentType_LeftLeg:
                    {
                        List<SubSegmentTypes> vLeftLegSubSegments = new List<SubSegmentTypes>();
                        vLeftLegSubSegments.Add(SubSegmentTypes.SubsegmentType_LowerSpine);
                        vLeftLegSubSegments.Add(SubSegmentTypes.SubsegmentType_LeftThigh);
                        vLeftLegSubSegments.Add(SubSegmentTypes.SubsegmentType_LeftCalf);
                        SegmentToSubSegmentMap.Add(SegmentTypes.SegmentType_LeftLeg, vLeftLegSubSegments);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void CreateSegmentToSensorPosMap()
    {
        var vSegmentTypes = EnumUtil.GetValues<SegmentTypes>();
        foreach (SegmentTypes vSegmentType in vSegmentTypes)
        {
            switch (vSegmentType)
            {
                case SegmentTypes.SegmentType_Trunk:
                    {
                        List<SensorPositions> vTorsoSensorPos = new List<SensorPositions>();
                        vTorsoSensorPos.Add(SensorPositions.SP_UpperSpine);
                        vTorsoSensorPos.Add(SensorPositions.SP_LowerSpine);
                        SegmentToSensorPosMap.Add(SegmentTypes.SegmentType_Trunk, vTorsoSensorPos);
                    }
                    break;
                case SegmentTypes.SegmentType_Hips:
                    {
                        List<SensorPositions> vHipSensorPos = new List<SensorPositions>();
                        vHipSensorPos.Add(SensorPositions.SP_UpperSpine);
                        vHipSensorPos.Add(SensorPositions.SP_LowerSpine);
                        SegmentToSensorPosMap.Add(SegmentTypes.SegmentType_Hips, vHipSensorPos);
                    }
                    break;
                case SegmentTypes.SegmentType_RightArm:
                    {
                        List<SensorPositions> vRightArmSensorPos = new List<SensorPositions>();
                        vRightArmSensorPos.Add(SensorPositions.SP_UpperSpine);
                        vRightArmSensorPos.Add(SensorPositions.SP_RightUpperArm);
                        vRightArmSensorPos.Add(SensorPositions.SP_RightForeArm);
                        vRightArmSensorPos.Add(SensorPositions.SP_RightElbow);
                        SegmentToSensorPosMap.Add(SegmentTypes.SegmentType_RightArm, vRightArmSensorPos);
                    }
                    break;
                case SegmentTypes.SegmentType_LeftArm:
                    {
                        List<SensorPositions> vLeftArmSensorPos = new List<SensorPositions>();
                        vLeftArmSensorPos.Add(SensorPositions.SP_UpperSpine);
                        vLeftArmSensorPos.Add(SensorPositions.SP_LeftUpperArm);
                        vLeftArmSensorPos.Add(SensorPositions.SP_LeftForeArm);
                        vLeftArmSensorPos.Add(SensorPositions.SP_LeftElbow);
                        SegmentToSensorPosMap.Add(SegmentTypes.SegmentType_LeftArm, vLeftArmSensorPos);
                    }
                    break;
                case SegmentTypes.SegmentType_RightLeg:
                    {
                        List<SensorPositions> vRightLegSensorPos = new List<SensorPositions>();
                        vRightLegSensorPos.Add(SensorPositions.SP_UpperSpine);
                        vRightLegSensorPos.Add(SensorPositions.SP_LowerSpine);
                        vRightLegSensorPos.Add(SensorPositions.SP_RightThigh);
                        vRightLegSensorPos.Add(SensorPositions.SP_RightCalf);
                        vRightLegSensorPos.Add(SensorPositions.SP_RightKnee);
                        SegmentToSensorPosMap.Add(SegmentTypes.SegmentType_RightLeg, vRightLegSensorPos);
                    }
                    break;
                case SegmentTypes.SegmentType_LeftLeg:
                    {
                        List<SensorPositions> vLeftLegSensorPos = new List<SensorPositions>();
                        vLeftLegSensorPos.Add(SensorPositions.SP_UpperSpine);
                        vLeftLegSensorPos.Add(SensorPositions.SP_LowerSpine);
                        vLeftLegSensorPos.Add(SensorPositions.SP_LeftThigh);
                        vLeftLegSensorPos.Add(SensorPositions.SP_LeftCalf);
                        vLeftLegSensorPos.Add(SensorPositions.SP_LeftKnee);
                        SegmentToSensorPosMap.Add(SegmentTypes.SegmentType_LeftLeg, vLeftLegSensorPos);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void CreateSensorPosToSensorTypeMap()
    {
        var vSensorPositions = EnumUtil.GetValues<SensorPositions>();
        foreach (SensorPositions vSensorPos in vSensorPositions)
        {
            switch (vSensorPos)
            {
                case SensorPositions.SP_UpperSpine:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_UpperSpine, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_LowerSpine:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_LowerSpine, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_RightUpperArm:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_RightUpperArm, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_RightForeArm:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_RightForeArm, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_RightElbow:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_RightElbow, SensorTypes.ST_Flexcore);
                    }
                    break;
                case SensorPositions.SP_LeftUpperArm:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_LeftUpperArm, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_LeftForeArm:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_LeftForeArm, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_LeftElbow:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_LeftElbow, SensorTypes.ST_Flexcore);
                    }
                    break;
                case SensorPositions.SP_RightThigh:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_RightThigh, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_RightCalf:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_RightCalf, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_RightKnee:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_RightKnee, SensorTypes.ST_Flexcore);
                    }
                    break;
                case SensorPositions.SP_LeftThigh:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_LeftThigh, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_LeftCalf:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_LeftCalf, SensorTypes.ST_Biomech);
                    }
                    break;
                case SensorPositions.SP_LeftKnee:
                    {
                        SensorPosToSensorTypeMap.Add(SensorPositions.SP_LeftKnee, SensorTypes.ST_Flexcore);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    public void CreateSensorPosToSensorIDMap()
    {
        var vSensorPositions = EnumUtil.GetValues<SensorPositions>();
        foreach (SensorPositions vSensorPos in vSensorPositions)
        {
            switch (vSensorPos)
            {
                case SensorPositions.SP_UpperSpine:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_UpperSpine, 0);
                    }
                    break;
                case SensorPositions.SP_LowerSpine:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_LowerSpine, 9);
                    }
                    break;
                case SensorPositions.SP_RightUpperArm:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_RightUpperArm, 1);
                    }
                    break;
                case SensorPositions.SP_RightForeArm:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_RightForeArm, 2);
                    }
                    break;
                case SensorPositions.SP_RightElbow:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_RightElbow, 0);
                    }
                    break;
                case SensorPositions.SP_LeftUpperArm:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_LeftUpperArm, 3);
                    }
                    break;
                case SensorPositions.SP_LeftForeArm:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_LeftForeArm, 4);
                    }
                    break;
                case SensorPositions.SP_LeftElbow:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_LeftElbow, 1);
                    }
                    break;
                case SensorPositions.SP_RightThigh:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_RightThigh, 5);
                    }
                    break;
                case SensorPositions.SP_RightCalf:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_RightCalf, 6);
                    }
                    break;
                case SensorPositions.SP_RightKnee:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_RightKnee, 2);
                    }
                    break;
                case SensorPositions.SP_LeftThigh:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_LeftThigh, 7);
                    }
                    break;
                case SensorPositions.SP_LeftCalf:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_LeftCalf, 8);
                    }
                    break;
                case SensorPositions.SP_LeftKnee:
                    {
                        SensorPosToSensorIDMap.Add(SensorPositions.SP_LeftKnee, 3);
                    }
                    break;

                default:
                    break;
            }
        }
    }
    /**
    * SubSegmentOrientationType enum
    * @brief The type of subsegment's orientation
    */
    public enum SubSegmentOrientationType
    {
        Fused = 0,
        NonFused = 1,
        MappedTransformation
    }
}