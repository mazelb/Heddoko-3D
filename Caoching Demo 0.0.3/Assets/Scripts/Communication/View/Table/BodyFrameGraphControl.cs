/**
* @file BodyFrameGraphControl.cs
* @brief Contains the BodyFrameGraphControl
* @author Mohammed Haider( mohammed @heddoko.com)
* @date May 2016 
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using Assets.Scripts.UI.Metrics;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Communication.View.Table
{
    /// <summary>
    /// Contains a control that hooks into a body and updates three DataSourceObj objects
    /// </summary>
    public class BodyFrameGraphControl : MonoBehaviour
    {
        public DataSourceObj YawObj;
        public DataSourceObj PitchObj;
        public DataSourceObj RollObj;
        private Dictionary<int , DropdownMenuStruct> mSensorPositions;
        private Body mBody;
        public Dropdown DropdownMenu;
        /// <summary>
        /// The current sensor position being tracked
        /// </summary>
        private BodyStructureMap.SensorPositions mCurrTrackingPos = BodyStructureMap.SensorPositions.SP_UpperSpine;

        void Start()
        {
            var vTrunkSensorStr = "0 - Trunk Sensor";
            var vRUArmSensorStr = "1 - Right Upper Arm";
            var vRLArmSensorStr = "2 - Right Forearm";
            var vLUArmSensorStr = "3 - Left Upper Arm";
            var vLLArmSensorStr = "4 - Left Forearm";
            var vRightThighStr = "5 - Right Thigh";
            var vRightCalfStr = "6 - Right Calf";
            var vLeftThighStr = "7 -Left Right Thigh";
            var vLeftCalfStr = "8 - Left Calf";

            mSensorPositions = new Dictionary<int, DropdownMenuStruct>();
            mSensorPositions.Add(0, new DropdownMenuStruct(vTrunkSensorStr, BodyStructureMap.SensorPositions.SP_UpperSpine));
            mSensorPositions.Add(1, new DropdownMenuStruct(vRUArmSensorStr, BodyStructureMap.SensorPositions.SP_RightUpperArm));
            mSensorPositions.Add(2, new DropdownMenuStruct(vRLArmSensorStr, BodyStructureMap.SensorPositions.SP_RightForeArm));
            mSensorPositions.Add(3, new DropdownMenuStruct(vLUArmSensorStr, BodyStructureMap.SensorPositions.SP_LeftUpperArm));
            mSensorPositions.Add(4, new DropdownMenuStruct(vLLArmSensorStr, BodyStructureMap.SensorPositions.SP_LeftForeArm));
            mSensorPositions.Add(5, new DropdownMenuStruct(vRightThighStr, BodyStructureMap.SensorPositions.SP_RightThigh));
            mSensorPositions.Add(6, new DropdownMenuStruct(vRightCalfStr, BodyStructureMap.SensorPositions.SP_RightCalf));
            mSensorPositions.Add(7, new DropdownMenuStruct(vLeftThighStr, BodyStructureMap.SensorPositions.SP_LeftThigh));
            mSensorPositions.Add(8, new DropdownMenuStruct(vLeftCalfStr, BodyStructureMap.SensorPositions.SP_LeftCalf));

            List<Dropdown.OptionData> vOptionData = new List<Dropdown.OptionData>();
            vOptionData.Add(new Dropdown.OptionData( vTrunkSensorStr));
            vOptionData.Add(new Dropdown.OptionData(vRUArmSensorStr));
            vOptionData.Add(new Dropdown.OptionData(vRLArmSensorStr));
            vOptionData.Add(new Dropdown.OptionData(vLUArmSensorStr));
            vOptionData.Add(new Dropdown.OptionData(vLLArmSensorStr));
            vOptionData.Add(new Dropdown.OptionData(vRightThighStr));
            vOptionData.Add(new Dropdown.OptionData(vRightCalfStr));
            vOptionData.Add(new Dropdown.OptionData(vLeftThighStr));
            vOptionData.Add(new Dropdown.OptionData(vLeftCalfStr));
            DropdownMenu.AddOptions(vOptionData);
            DropdownMenu.value = 0;
            DropdownMenu.onValueChanged.AddListener(OnDropdownValueChange);
        }

        /// <summary>
        /// On dropdown value selected
        /// </summary>
        private void OnDropdownValueChange(int vValue)
        {
            DropdownMenu.value = vValue;
            mCurrTrackingPos = mSensorPositions[vValue].Position;
        }

        /// <summary>
        /// A dropdown menu structure that maps an integer to a sensor position
        /// </summary>
        private struct DropdownMenuStruct
        {
            public string  Id;
            public BodyStructureMap.SensorPositions Position;

            public DropdownMenuStruct(string vId, BodyStructureMap.SensorPositions vPosition)
            {
                this.Id = vId;
                this.Position = vPosition;
            }
        }

        /// <summary>
        /// When a body frame has been created
        /// </summary>
        /// <param name="vFrame"></param>
        public void OnBodyFrameUpdateHandler(BodyFrame vFrame)
        {
            YawObj.UpdatableVariable = vFrame.FrameData[mCurrTrackingPos].x;
            PitchObj.UpdatableVariable = vFrame.FrameData[mCurrTrackingPos].y;
            RollObj.UpdatableVariable = vFrame.FrameData[mCurrTrackingPos].z;
        }

        /// <summary>
        /// Set the data source body
        /// </summary>
        /// <param name="vNewBody"></param>
        public void SetBody(Body vNewBody)
        {
            if (mBody != null)
            {
                mBody.View.BodyFrameUpdatedEvent -= OnBodyFrameUpdateHandler;
            }
            mBody = vNewBody;
            mBody.View.BodyFrameUpdatedEvent += OnBodyFrameUpdateHandler;
        }

    }
}