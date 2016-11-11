// /**
// * @file ProbuffMessageViewController.cs
// * @brief Contains the 
// * @author Mohammed Haider(mohammed@heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Communication.View;
using Assets.Scripts.Utils;
using heddoko;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    /// <summary>
    /// A view controller: displays incoming protobuff messages
    /// </summary>
    public class ProbuffMessageViewController : MonoBehaviour
    {
        public ProtoFrameDataListView ListView;
        public Text BrainpackMessagePerSec;
        public Text AverageMessagePerSec;
        private BrainpackMessagePerSecond mBrainpackMessagePerSecond = new BrainpackMessagePerSecond();
        public Body Body;
        //private Dictionary<int, bool> mSelectedIntegers = new Dictionary<int, bool>();
        private bool[] mSelectedIntegers = new bool[9];
        public Vector3 FirstVector;
        public Vector3 SecondVector;
        public int vVal;
        public bool IsPaused = false;
        void Update()
        {
            BodySegment.FirstVector = FirstVector;
            BodySegment.SecondVector = SecondVector;
            BodyFrame.vVal = vVal;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsPaused = !IsPaused;
            }
        }
        public BrainpackMessagePerSecond BrainpackMessagePerSecond
        {
            get
            {
                return mBrainpackMessagePerSecond;
            }
        }
        void Start()
        {
            ResetList();
        }

        public void UpdateBody(Body vBody)
        {
            if (Body != null)
            {
                vBody.RenderedBody.SensorTransformContainer.SensorSelected -= RemoveAllButIndex;
                vBody.RenderedBody.SensorTransformContainer.SensorDeselected -= DisplayAllItems;
            }
            Body = vBody;
            Body.RenderedBody.SensorTransformContainer.SensorSelected += RemoveAllButIndex;
            Body.RenderedBody.SensorTransformContainer.SensorDeselected += DisplayAllItems;
        }

        private void DisplayAllItems(int vObj)
        {
            ListView.DataSource.Clear();
            ResetList();
            ListView.UpdateItems();
        }

        /// <summary>
        /// Resets the list
        /// </summary>
        private void ResetList()
        {
            //reset the list
            for (int i = 0; i < 9; i++)
            {
                var vItem = new ProtoFrameDataViewDescription();
                vItem.Index = i;
                vItem.MappedEuler = string.Format("{0},{1},{2}", 0, 0, 0);
                vItem.RawEuler = string.Format("{0},{1},{2}", 0, 0, 0);
                vItem.MappedQuat = string.Format("{0},{1},{2},{3}", 0, 0, 0, 0);
                vItem.RawQuat = string.Format("{0},{1},{2},{3}", 0, 0, 0, 0);
                ListView.Add(vItem);
                mSelectedIntegers[i] = true;
            }
            ListView.UpdateItems();
        }
        /// <summary>
        /// removes all the items from the list except the passed in index
        /// </summary>
        /// <param name="vObj"></param>
        private void RemoveAllButIndex(int vObj)
        {
            ListView.DataSource.Clear();
            var vItem = new ProtoFrameDataViewDescription();
            vItem.Index = vObj;
            vItem.MappedEuler = string.Format("{0},{1},{2}", 0, 0, 0);
            vItem.RawEuler = string.Format("{0},{1},{2}", 0, 0, 0);
            vItem.MappedQuat = string.Format("{0},{1},{2},{3}", 0, 0, 0, 0);
            vItem.RawQuat = string.Format("{0},{1},{2},{3}", 0, 0, 0, 0);
            ListView.Add(vItem);

            for (int i = 0; i < mSelectedIntegers.Length; i++)
            {
                mSelectedIntegers[i] = i == vObj;

            }
            ListView.UpdateItems();
        }


        public void ProcessMessage(Packet vPacket)
        {
            if (IsPaused)
            {
                return;
            }
            int vCount = vPacket.fullDataFrame.imuDataFrame.Count;
            var vImuDataFrame = vPacket.fullDataFrame.imuDataFrame;

            for (int vI = 0; vI < vCount; vI++)
            {
                if (!mSelectedIntegers[vI])
                {
                    continue;
                }

                int vID = (int)vImuDataFrame[vI].imuId;
                float vMx = vImuDataFrame[vI].Mag_x;
                float vMy = vImuDataFrame[vI].Mag_y;
                float vMz = vImuDataFrame[vI].Mag_z;

                float vAccelx = vImuDataFrame[vI].Accel_x;
                float vAccely = vImuDataFrame[vI].Accel_y;
                float vAccelz = vImuDataFrame[vI].Accel_z;

                float vQx = vImuDataFrame[vI].quat_x_yaw;
                float vQy = vImuDataFrame[vI].quat_y_pitch;
                float vQz = vImuDataFrame[vI].quat_z_roll;
                float vQw = vImuDataFrame[vI].quat_w;

                float vYaw = 57.29578f * Mathf.Atan2((2.0f * vQx * vQy + 2.0f * vQw * vQz), (2.0f * vQw * vQw + 2.0f * vQx - 1.0f));
                float vPitch = 57.29578f * Mathf.Asin(-(2.0f * vQx * vQz - 2.0f * vQw * vQy));
                float vRoll = 57.29578f * Mathf.Atan2((2.0f * vQy * vQz + 2.0f * vQw * vQx), (float)(2.0 * vQw * vQw + 2.0 * vQw * vQw - 1.0f));
                if (vYaw < 0f)
                {
                    vYaw += 360f;
                }
                int vIndex = ListView.DataSource.Count < 9 ? 0 : vID;
                var vDescription = ListView.DataSource[vIndex];
                BodyStructureMap.SubSegmentTypes vType = BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine;
                if (vI != 0)
                {
                    vType = (BodyStructureMap.SubSegmentTypes)(vI + 1);
                }

                Transform vTransform = Body.RenderedBody.GetSubSegmentTransform(vType);
                var vQuatRot = vTransform.rotation;
                var vEulerRot = vTransform.eulerAngles;
                vIndex = ListView.DataSource.Count < 9 ? 0 : vID;
                ListView.DataSource[vIndex].RawQuat = string.Format("{0} , {1} , {2}, {3} ", FormatFloatTo2Decimals(vQx), FormatFloatTo2Decimals(vQy), FormatFloatTo2Decimals(vQz), FormatFloatTo2Decimals(vQw));
                ListView.DataSource[vIndex].RawEuler = string.Format("{0} , {1} , {2} ", FormatFloatTo2Decimals(vYaw), FormatFloatTo2Decimals(vPitch), FormatFloatTo2Decimals(vRoll));
                vDescription.MappedEuler = string.Format("{0} , {1} , {2} ", FormatFloatTo2Decimals(vEulerRot.y), FormatFloatTo2Decimals(vEulerRot.x), FormatFloatTo2Decimals(vEulerRot.z));
                vDescription.MappedQuat = string.Format("{0} , {1} , {2} , {3} ", FormatFloatTo2Decimals(vQuatRot.x), FormatFloatTo2Decimals(vQuatRot.y), FormatFloatTo2Decimals(vQuatRot.z), FormatFloatTo2Decimals(vQuatRot.w));
                vDescription.Magnetometer = string.Format("{0} , {1} , {2}   ", FormatFloatTo2Decimals(vMx), FormatFloatTo2Decimals(vMy), FormatFloatTo2Decimals(vMz));
                vDescription.Acceleration = string.Format("{0} , {1} , {2}  ", FormatFloatTo2Decimals(vAccelx), FormatFloatTo2Decimals(vAccely), FormatFloatTo2Decimals(vAccelz));

                UpdateSensors(vIndex, vImuDataFrame[vI]);

            }
            ListView.UpdateItems();
            Body.RenderedBody.SensorTransformContainer.UpdateSensorOrientation(vPacket);
        }

        private void UpdateSensors(int vIndex, ImuDataFrame vImuDataFrame)
        {
            if (Body != null)
            {
                var vCalStableStatus = (vImuDataFrame.sensorMask >> 19) & 0x01;
                var vMagTransient = (vImuDataFrame.sensorMask >> 20) & 0x01;
                Body.RenderedBody.SensorTransformContainer.ChangeState(vIndex, vCalStableStatus == 1, vMagTransient == 1);

            }
        }
        private string FormatFloatTo2Decimals(float vValue)
        {
            return vValue.ToString("0.00");
        }

        public void ProcessMessage(object vVsender, object vVargs)
        {
            Action vAction = () =>
            {
                Packet vPacket = (Packet)vVargs;
                ProcessMessage(vPacket);
                if (!mBrainpackMessagePerSecond.Initialized)
                {
                    mBrainpackMessagePerSecond.Init(vPacket);
                    return;
                }
                mBrainpackMessagePerSecond.CountFrame(vPacket);
                BrainpackMessagePerSec.text = mBrainpackMessagePerSecond.BmPs + "";
                AverageMessagePerSec.text = mBrainpackMessagePerSecond.AverageBmPs + "";
            };
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);
        }


    }
}