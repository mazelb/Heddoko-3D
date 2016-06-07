﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Option: missing-value detection (*Specified/ShouldSerialize*/Reset*) enabled

// Generated from: heddokoPacket.proto
namespace HeddokoLib.heddokoProtobuff
{
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"ImuDataFrame")]
        public partial class ImuDataFrame : global::ProtoBuf.IExtensible
        {
            public ImuDataFrame() { }

            private uint _imuId;
            [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"imuId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public uint imuId
            {
                get { return _imuId; }
                set { _imuId = value; }
            }
            private uint? _sensorMask;
            [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"sensorMask", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public uint sensorMask
            {
                get { return _sensorMask ?? default(uint); }
                set { _sensorMask = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool sensorMaskSpecified
            {
                get { return this._sensorMask != null; }
                set { if (value == (this._sensorMask == null)) this._sensorMask = value ? this.sensorMask : (uint?)null; }
            }
            private bool ShouldSerializesensorMask() { return sensorMaskSpecified; }
            private void ResetsensorMask() { sensorMaskSpecified = false; }

            private float? _quat_x_yaw;
            [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"quat_x_yaw", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            public float quat_x_yaw
            {
                get { return _quat_x_yaw ?? default(float); }
                set { _quat_x_yaw = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool quat_x_yawSpecified
            {
                get { return this._quat_x_yaw != null; }
                set { if (value == (this._quat_x_yaw == null)) this._quat_x_yaw = value ? this.quat_x_yaw : (float?)null; }
            }
            private bool ShouldSerializequat_x_yaw() { return quat_x_yawSpecified; }
            private void Resetquat_x_yaw() { quat_x_yawSpecified = false; }

            private float? _quat_y_pitch;
            [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"quat_y_pitch", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            public float quat_y_pitch
            {
                get { return _quat_y_pitch ?? default(float); }
                set { _quat_y_pitch = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool quat_y_pitchSpecified
            {
                get { return this._quat_y_pitch != null; }
                set { if (value == (this._quat_y_pitch == null)) this._quat_y_pitch = value ? this.quat_y_pitch : (float?)null; }
            }
            private bool ShouldSerializequat_y_pitch() { return quat_y_pitchSpecified; }
            private void Resetquat_y_pitch() { quat_y_pitchSpecified = false; }

            private float? _quat_z_roll;
            [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"quat_z_roll", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            public float quat_z_roll
            {
                get { return _quat_z_roll ?? default(float); }
                set { _quat_z_roll = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool quat_z_rollSpecified
            {
                get { return this._quat_z_roll != null; }
                set { if (value == (this._quat_z_roll == null)) this._quat_z_roll = value ? this.quat_z_roll : (float?)null; }
            }
            private bool ShouldSerializequat_z_roll() { return quat_z_rollSpecified; }
            private void Resetquat_z_roll() { quat_z_rollSpecified = false; }

            private float? _quat_w;
            [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name = @"quat_w", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            public float quat_w
            {
                get { return _quat_w ?? default(float); }
                set { _quat_w = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool quat_wSpecified
            {
                get { return this._quat_w != null; }
                set { if (value == (this._quat_w == null)) this._quat_w = value ? this.quat_w : (float?)null; }
            }
            private bool ShouldSerializequat_w() { return quat_wSpecified; }
            private void Resetquat_w() { quat_wSpecified = false; }

            private int? _Mag_x;
            [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"Mag_x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Mag_x
            {
                get { return _Mag_x ?? default(int); }
                set { _Mag_x = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Mag_xSpecified
            {
                get { return this._Mag_x != null; }
                set { if (value == (this._Mag_x == null)) this._Mag_x = value ? this.Mag_x : (int?)null; }
            }
            private bool ShouldSerializeMag_x() { return Mag_xSpecified; }
            private void ResetMag_x() { Mag_xSpecified = false; }

            private int? _Mag_y;
            [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name = @"Mag_y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Mag_y
            {
                get { return _Mag_y ?? default(int); }
                set { _Mag_y = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Mag_ySpecified
            {
                get { return this._Mag_y != null; }
                set { if (value == (this._Mag_y == null)) this._Mag_y = value ? this.Mag_y : (int?)null; }
            }
            private bool ShouldSerializeMag_y() { return Mag_ySpecified; }
            private void ResetMag_y() { Mag_ySpecified = false; }

            private int? _Mag_z;
            [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"Mag_z", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Mag_z
            {
                get { return _Mag_z ?? default(int); }
                set { _Mag_z = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Mag_zSpecified
            {
                get { return this._Mag_z != null; }
                set { if (value == (this._Mag_z == null)) this._Mag_z = value ? this.Mag_z : (int?)null; }
            }
            private bool ShouldSerializeMag_z() { return Mag_zSpecified; }
            private void ResetMag_z() { Mag_zSpecified = false; }

            private int? _Accel_x;
            [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name = @"Accel_x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Accel_x
            {
                get { return _Accel_x ?? default(int); }
                set { _Accel_x = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Accel_xSpecified
            {
                get { return this._Accel_x != null; }
                set { if (value == (this._Accel_x == null)) this._Accel_x = value ? this.Accel_x : (int?)null; }
            }
            private bool ShouldSerializeAccel_x() { return Accel_xSpecified; }
            private void ResetAccel_x() { Accel_xSpecified = false; }

            private int? _Accel_y;
            [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name = @"Accel_y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Accel_y
            {
                get { return _Accel_y ?? default(int); }
                set { _Accel_y = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Accel_ySpecified
            {
                get { return this._Accel_y != null; }
                set { if (value == (this._Accel_y == null)) this._Accel_y = value ? this.Accel_y : (int?)null; }
            }
            private bool ShouldSerializeAccel_y() { return Accel_ySpecified; }
            private void ResetAccel_y() { Accel_ySpecified = false; }

            private int? _Accel_z;
            [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name = @"Accel_z", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Accel_z
            {
                get { return _Accel_z ?? default(int); }
                set { _Accel_z = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Accel_zSpecified
            {
                get { return this._Accel_z != null; }
                set { if (value == (this._Accel_z == null)) this._Accel_z = value ? this.Accel_z : (int?)null; }
            }
            private bool ShouldSerializeAccel_z() { return Accel_zSpecified; }
            private void ResetAccel_z() { Accel_zSpecified = false; }

            private int? _Rot_x;
            [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name = @"Rot_x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Rot_x
            {
                get { return _Rot_x ?? default(int); }
                set { _Rot_x = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Rot_xSpecified
            {
                get { return this._Rot_x != null; }
                set { if (value == (this._Rot_x == null)) this._Rot_x = value ? this.Rot_x : (int?)null; }
            }
            private bool ShouldSerializeRot_x() { return Rot_xSpecified; }
            private void ResetRot_x() { Rot_xSpecified = false; }

            private int? _Rot_y;
            [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name = @"Rot_y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Rot_y
            {
                get { return _Rot_y ?? default(int); }
                set { _Rot_y = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Rot_ySpecified
            {
                get { return this._Rot_y != null; }
                set { if (value == (this._Rot_y == null)) this._Rot_y = value ? this.Rot_y : (int?)null; }
            }
            private bool ShouldSerializeRot_y() { return Rot_ySpecified; }
            private void ResetRot_y() { Rot_ySpecified = false; }

            private int? _Rot_z;
            [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name = @"Rot_z", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int Rot_z
            {
                get { return _Rot_z ?? default(int); }
                set { _Rot_z = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool Rot_zSpecified
            {
                get { return this._Rot_z != null; }
                set { if (value == (this._Rot_z == null)) this._Rot_z = value ? this.Rot_z : (int?)null; }
            }
            private bool ShouldSerializeRot_z() { return Rot_zSpecified; }
            private void ResetRot_z() { Rot_zSpecified = false; }

            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }

        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"FullDataFrame")]
        public partial class FullDataFrame : global::ProtoBuf.IExtensible
        {
            public FullDataFrame() { }

            private uint _timeStamp;
            [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"timeStamp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public uint timeStamp
            {
                get { return _timeStamp; }
                set { _timeStamp = value; }
            }
            private readonly global::System.Collections.Generic.List<ImuDataFrame> _imuDataFrame = new global::System.Collections.Generic.List<ImuDataFrame>();
            [global::ProtoBuf.ProtoMember(2, Name = @"imuDataFrame", DataFormat = global::ProtoBuf.DataFormat.Default)]
            public global::System.Collections.Generic.List<ImuDataFrame> imuDataFrame
            {
                get { return _imuDataFrame; }
            }

            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }

        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"Packet")]
        public partial class Packet : global::ProtoBuf.IExtensible
        {
            public Packet() { }

            private PacketType _type;
            [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public PacketType type
            {
                get { return _type; }
                set { _type = value; }
            }
            private Packet.BrainPackState? _state;
            [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"state", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public Packet.BrainPackState state
            {
                get { return _state ?? Packet.BrainPackState.Idle; }
                set { _state = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool stateSpecified
            {
                get { return this._state != null; }
                set { if (value == (this._state == null)) this._state = value ? this.state : (Packet.BrainPackState?)null; }
            }
            private bool ShouldSerializestate() { return stateSpecified; }
            private void Resetstate() { stateSpecified = false; }

            private string _brainPackVersion;
            [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"brainPackVersion", DataFormat = global::ProtoBuf.DataFormat.Default)]
            public string brainPackVersion
            {
                get { return _brainPackVersion ?? ""; }
                set { _brainPackVersion = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool brainPackVersionSpecified
            {
                get { return this._brainPackVersion != null; }
                set { if (value == (this._brainPackVersion == null)) this._brainPackVersion = value ? this.brainPackVersion : (string)null; }
            }
            private bool ShouldSerializebrainPackVersion() { return brainPackVersionSpecified; }
            private void ResetbrainPackVersion() { brainPackVersionSpecified = false; }

            private int? _batteryCharge;
            [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"batteryCharge", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int batteryCharge
            {
                get { return _batteryCharge ?? default(int); }
                set { _batteryCharge = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool batteryChargeSpecified
            {
                get { return this._batteryCharge != null; }
                set { if (value == (this._batteryCharge == null)) this._batteryCharge = value ? this.batteryCharge : (int?)null; }
            }
            private bool ShouldSerializebatteryCharge() { return batteryChargeSpecified; }
            private void ResetbatteryCharge() { batteryChargeSpecified = false; }

            private FullDataFrame _fullDataFrame = null;
            [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"fullDataFrame", DataFormat = global::ProtoBuf.DataFormat.Default)]
            [global::System.ComponentModel.DefaultValue(null)]
            public FullDataFrame fullDataFrame
            {
                get { return _fullDataFrame; }
                set { _fullDataFrame = value; }
            }
            private string _recordingName;
            [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name = @"recordingName", DataFormat = global::ProtoBuf.DataFormat.Default)]
            public string recordingName
            {
                get { return _recordingName ?? ""; }
                set { _recordingName = value; }
            }
            [global::System.Xml.Serialization.XmlIgnore]
            [global::System.ComponentModel.Browsable(false)]
            public bool recordingNameSpecified
            {
                get { return this._recordingName != null; }
                set { if (value == (this._recordingName == null)) this._recordingName = value ? this.recordingName : (string)null; }
            }
            private bool ShouldSerializerecordingName() { return recordingNameSpecified; }
            private void ResetrecordingName() { recordingNameSpecified = false; }

            [global::ProtoBuf.ProtoContract(Name = @"BrainPackState")]
            public enum BrainPackState
            {

                [global::ProtoBuf.ProtoEnum(Name = @"Idle", Value = 0)]
                Idle = 0,

                [global::ProtoBuf.ProtoEnum(Name = @"Recording", Value = 1)]
                Recording = 1,

                [global::ProtoBuf.ProtoEnum(Name = @"Reset", Value = 2)]
                Reset = 2,

                [global::ProtoBuf.ProtoEnum(Name = @"Error", Value = 4)]
                Error = 4
            }

            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }

        [global::ProtoBuf.ProtoContract(Name = @"PacketType")]
        public enum PacketType
        {

            [global::ProtoBuf.ProtoEnum(Name = @"StateRequest", Value = 0)]
            StateRequest = 0,

            [global::ProtoBuf.ProtoEnum(Name = @"StateResponse", Value = 1)]
            StateResponse = 1,

            [global::ProtoBuf.ProtoEnum(Name = @"DataFrame", Value = 2)]
            DataFrame = 2,

            [global::ProtoBuf.ProtoEnum(Name = @"BatteryChargeRequest", Value = 3)]
            BatteryChargeRequest = 3,

            [global::ProtoBuf.ProtoEnum(Name = @"BatteryChargeResponse", Value = 4)]
            BatteryChargeResponse = 4,

            [global::ProtoBuf.ProtoEnum(Name = @"BrainPackVersionRequest", Value = 5)]
            BrainPackVersionRequest = 5,

            [global::ProtoBuf.ProtoEnum(Name = @"BrainPackVersionResponse", Value = 6)]
            BrainPackVersionResponse = 6,

            [global::ProtoBuf.ProtoEnum(Name = @"SetRecordingName", Value = 7)]
            SetRecordingName = 7,

            [global::ProtoBuf.ProtoEnum(Name = @"GetRecordingName", Value = 8)]
            GetRecordingName = 8
        }

    }
