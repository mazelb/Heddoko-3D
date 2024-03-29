﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Option: missing-value detection (*Specified/ShouldSerialize*/Reset*) enabled
    
// Generated from: ProcessedFrame.proto
namespace Heddoko
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Vector4")]
  public partial class Vector4 : global::ProtoBuf.IExtensible
  {
    public Vector4() {}
    
    private float _x;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"x", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float x
    {
      get { return _x; }
      set { _x = value; }
    }
    private float _y;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"y", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float y
    {
      get { return _y; }
      set { _y = value; }
    }
    private float _z;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"z", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float z
    {
      get { return _z; }
      set { _z = value; }
    }
    private float _w;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"w", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float w
    {
      get { return _w; }
      set { _w = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MappedOrientation")]
  public partial class MappedOrientation : global::ProtoBuf.IExtensible
  {
    public MappedOrientation() {}
    
    private Heddoko.SensorPosition _sensorPosition;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"sensorPosition", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public Heddoko.SensorPosition sensorPosition
    {
      get { return _sensorPosition; }
      set { _sensorPosition = value; }
    }
    private Heddoko.Vector4 _vector4;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"vector4", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public Heddoko.Vector4 vector4
    {
      get { return _vector4; }
      set { _vector4 = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ProcessedFrame")]
  public partial class ProcessedFrame : global::ProtoBuf.IExtensible
  {
    public ProcessedFrame() {}
    
    private readonly global::System.Collections.Generic.List<Heddoko.MappedOrientation> _mappedOrientation = new global::System.Collections.Generic.List<Heddoko.MappedOrientation>();
    [global::ProtoBuf.ProtoMember(1, Name=@"mappedOrientation", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Heddoko.MappedOrientation> mappedOrientation
    {
      get { return _mappedOrientation; }
    }
  
    private ulong _TimeStamp;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"TimeStamp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public ulong TimeStamp
    {
      get { return _TimeStamp; }
      set { _TimeStamp = value; }
    }
    private uint _UserID;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"UserID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint UserID
    {
      get { return _UserID; }
      set { _UserID = value; }
    }
    private string _KitID;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"KitID", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string KitID
    {
      get { return _KitID; }
      set { _KitID = value; }
    }
    private double _Longitude;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"Longitude", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public double Longitude
    {
      get { return _Longitude; }
      set { _Longitude = value; }
    }
    private double _Latitute;
    [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name=@"Latitute", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public double Latitute
    {
      get { return _Latitute; }
      set { _Latitute = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"SensorPosition")]
    public enum SensorPosition
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_UpperSpine", Value=0)]
      SP_UpperSpine = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_LowerSpine", Value=1)]
      SP_LowerSpine = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_RightUpperArm", Value=2)]
      SP_RightUpperArm = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_RightForeArm", Value=3)]
      SP_RightForeArm = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_LeftUpperArm", Value=4)]
      SP_LeftUpperArm = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_LeftForeArm", Value=5)]
      SP_LeftForeArm = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_RightThigh", Value=6)]
      SP_RightThigh = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_RightCalf", Value=7)]
      SP_RightCalf = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_LeftThigh", Value=8)]
      SP_LeftThigh = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_LeftCalf", Value=9)]
      SP_LeftCalf = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_RightElbow", Value=10)]
      SP_RightElbow = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_LeftElbow", Value=11)]
      SP_LeftElbow = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_RightKnee", Value=12)]
      SP_RightKnee = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SP_LeftKnee", Value=13)]
      SP_LeftKnee = 13
    }
  
}