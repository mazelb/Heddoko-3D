//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Option: missing-value detection (*Specified/ShouldSerialize*/Reset*) enabled
    
// Generated from: AnalysisFrame.proto
namespace Heddoko
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AnalysisFrame")]
  public partial class AnalysisFrame : global::ProtoBuf.IExtensible
  {
    public AnalysisFrame() {}
    
    private float _TrunkFlexionAngle;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"TrunkFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TrunkFlexionAngle
    {
      get { return _TrunkFlexionAngle; }
      set { _TrunkFlexionAngle = value; }
    }
    private float _TrunkLateralAngle;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"TrunkLateralAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TrunkLateralAngle
    {
      get { return _TrunkLateralAngle; }
      set { _TrunkLateralAngle = value; }
    }
    private float _TrunkRotationAngle;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"TrunkRotationAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TrunkRotationAngle
    {
      get { return _TrunkRotationAngle; }
      set { _TrunkRotationAngle = value; }
    }
    private float _TrunkLateralSignedAngle;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"TrunkLateralSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TrunkLateralSignedAngle
    {
      get { return _TrunkLateralSignedAngle; }
      set { _TrunkLateralSignedAngle = value; }
    }
    private float _TrunkRotationSignedAngle;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"TrunkRotationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TrunkRotationSignedAngle
    {
      get { return _TrunkRotationSignedAngle; }
      set { _TrunkRotationSignedAngle = value; }
    }
    private float _TrunkFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name=@"TrunkFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TrunkFlexionSignedAngle
    {
      get { return _TrunkFlexionSignedAngle; }
      set { _TrunkFlexionSignedAngle = value; }
    }
    private float _TorsoFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(7, IsRequired = true, Name=@"TorsoFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TorsoFlexionAngularAcceleration
    {
      get { return _TorsoFlexionAngularAcceleration; }
      set { _TorsoFlexionAngularAcceleration = value; }
    }
    private float _TorsoFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(8, IsRequired = true, Name=@"TorsoFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TorsoFlexionAngularVelocity
    {
      get { return _TorsoFlexionAngularVelocity; }
      set { _TorsoFlexionAngularVelocity = value; }
    }
    private float _TorsoLateralAngularAcceleration;
    [global::ProtoBuf.ProtoMember(9, IsRequired = true, Name=@"TorsoLateralAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TorsoLateralAngularAcceleration
    {
      get { return _TorsoLateralAngularAcceleration; }
      set { _TorsoLateralAngularAcceleration = value; }
    }
    private float _TorsoLateralAngularVelocity;
    [global::ProtoBuf.ProtoMember(10, IsRequired = true, Name=@"TorsoLateralAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TorsoLateralAngularVelocity
    {
      get { return _TorsoLateralAngularVelocity; }
      set { _TorsoLateralAngularVelocity = value; }
    }
    private float _TorsoRotationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(11, IsRequired = true, Name=@"TorsoRotationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TorsoRotationAngularAcceleration
    {
      get { return _TorsoRotationAngularAcceleration; }
      set { _TorsoRotationAngularAcceleration = value; }
    }
    private float _TorsoRotationAngularVelocity;
    [global::ProtoBuf.ProtoMember(12, IsRequired = true, Name=@"TorsoRotationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float TorsoRotationAngularVelocity
    {
      get { return _TorsoRotationAngularVelocity; }
      set { _TorsoRotationAngularVelocity = value; }
    }
    private float _RightElbowFlexionAngle;
    [global::ProtoBuf.ProtoMember(13, IsRequired = true, Name=@"RightElbowFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightElbowFlexionAngle
    {
      get { return _RightElbowFlexionAngle; }
      set { _RightElbowFlexionAngle = value; }
    }
    private float _RightElbowFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(14, IsRequired = true, Name=@"RightElbowFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightElbowFlexionSignedAngle
    {
      get { return _RightElbowFlexionSignedAngle; }
      set { _RightElbowFlexionSignedAngle = value; }
    }
    private float _RightForeArmPronationSignedAngle;
    [global::ProtoBuf.ProtoMember(15, IsRequired = true, Name=@"RightForeArmPronationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightForeArmPronationSignedAngle
    {
      get { return _RightForeArmPronationSignedAngle; }
      set { _RightForeArmPronationSignedAngle = value; }
    }
    private float _RightShoulderFlexionAngle;
    [global::ProtoBuf.ProtoMember(17, IsRequired = true, Name=@"RightShoulderFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderFlexionAngle
    {
      get { return _RightShoulderFlexionAngle; }
      set { _RightShoulderFlexionAngle = value; }
    }
    private float _RightShoulderFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(18, IsRequired = true, Name=@"RightShoulderFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderFlexionSignedAngle
    {
      get { return _RightShoulderFlexionSignedAngle; }
      set { _RightShoulderFlexionSignedAngle = value; }
    }
    private float _RightShoulderVertAbductionAngle;
    [global::ProtoBuf.ProtoMember(19, IsRequired = true, Name=@"RightShoulderVertAbductionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderVertAbductionAngle
    {
      get { return _RightShoulderVertAbductionAngle; }
      set { _RightShoulderVertAbductionAngle = value; }
    }
    private float _RightShoulderVerticalAbductionSignedAngle;
    [global::ProtoBuf.ProtoMember(20, IsRequired = true, Name=@"RightShoulderVerticalAbductionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderVerticalAbductionSignedAngle
    {
      get { return _RightShoulderVerticalAbductionSignedAngle; }
      set { _RightShoulderVerticalAbductionSignedAngle = value; }
    }
    private float _RightShoulderHorAbductionAngle;
    [global::ProtoBuf.ProtoMember(21, IsRequired = true, Name=@"RightShoulderHorAbductionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderHorAbductionAngle
    {
      get { return _RightShoulderHorAbductionAngle; }
      set { _RightShoulderHorAbductionAngle = value; }
    }
    private float _RightShoulderHorizontalAbductionSignedAngle;
    [global::ProtoBuf.ProtoMember(22, IsRequired = true, Name=@"RightShoulderHorizontalAbductionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderHorizontalAbductionSignedAngle
    {
      get { return _RightShoulderHorizontalAbductionSignedAngle; }
      set { _RightShoulderHorizontalAbductionSignedAngle = value; }
    }
    private float _RightShoulderRotationSignedAngle;
    [global::ProtoBuf.ProtoMember(23, IsRequired = true, Name=@"RightShoulderRotationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderRotationSignedAngle
    {
      get { return _RightShoulderRotationSignedAngle; }
      set { _RightShoulderRotationSignedAngle = value; }
    }
    private float _RightElbowFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(25, IsRequired = true, Name=@"RightElbowFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightElbowFlexionAngularVelocity
    {
      get { return _RightElbowFlexionAngularVelocity; }
      set { _RightElbowFlexionAngularVelocity = value; }
    }
    private float _RightElbowFlexionPeakAngularVelocity;
    [global::ProtoBuf.ProtoMember(26, IsRequired = true, Name=@"RightElbowFlexionPeakAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightElbowFlexionPeakAngularVelocity
    {
      get { return _RightElbowFlexionPeakAngularVelocity; }
      set { _RightElbowFlexionPeakAngularVelocity = value; }
    }
    private float _RightElbowFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(27, IsRequired = true, Name=@"RightElbowFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightElbowFlexionAngularAcceleration
    {
      get { return _RightElbowFlexionAngularAcceleration; }
      set { _RightElbowFlexionAngularAcceleration = value; }
    }
    private float _RightForeArmPronationAngularVelocity;
    [global::ProtoBuf.ProtoMember(28, IsRequired = true, Name=@"RightForeArmPronationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightForeArmPronationAngularVelocity
    {
      get { return _RightForeArmPronationAngularVelocity; }
      set { _RightForeArmPronationAngularVelocity = value; }
    }
    private float _RightForeArmPronationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(29, IsRequired = true, Name=@"RightForeArmPronationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightForeArmPronationAngularAcceleration
    {
      get { return _RightForeArmPronationAngularAcceleration; }
      set { _RightForeArmPronationAngularAcceleration = value; }
    }
    private float _RightShoulderFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(30, IsRequired = true, Name=@"RightShoulderFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderFlexionAngularVelocity
    {
      get { return _RightShoulderFlexionAngularVelocity; }
      set { _RightShoulderFlexionAngularVelocity = value; }
    }
    private float _RightShoulderFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(31, IsRequired = true, Name=@"RightShoulderFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderFlexionAngularAcceleration
    {
      get { return _RightShoulderFlexionAngularAcceleration; }
      set { _RightShoulderFlexionAngularAcceleration = value; }
    }
    private float _RightShoulderVertAbductionAngularVelocity;
    [global::ProtoBuf.ProtoMember(32, IsRequired = true, Name=@"RightShoulderVertAbductionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderVertAbductionAngularVelocity
    {
      get { return _RightShoulderVertAbductionAngularVelocity; }
      set { _RightShoulderVertAbductionAngularVelocity = value; }
    }
    private float _RightShoulderVertAbductionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(33, IsRequired = true, Name=@"RightShoulderVertAbductionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderVertAbductionAngularAcceleration
    {
      get { return _RightShoulderVertAbductionAngularAcceleration; }
      set { _RightShoulderVertAbductionAngularAcceleration = value; }
    }
    private float _RightShoulderHorAbductionAngularVelocity;
    [global::ProtoBuf.ProtoMember(34, IsRequired = true, Name=@"RightShoulderHorAbductionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderHorAbductionAngularVelocity
    {
      get { return _RightShoulderHorAbductionAngularVelocity; }
      set { _RightShoulderHorAbductionAngularVelocity = value; }
    }
    private float _RightShoulderHorAbductionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(35, IsRequired = true, Name=@"RightShoulderHorAbductionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderHorAbductionAngularAcceleration
    {
      get { return _RightShoulderHorAbductionAngularAcceleration; }
      set { _RightShoulderHorAbductionAngularAcceleration = value; }
    }
    private float _RightShoulderRotationAngularVelocity;
    [global::ProtoBuf.ProtoMember(36, IsRequired = true, Name=@"RightShoulderRotationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderRotationAngularVelocity
    {
      get { return _RightShoulderRotationAngularVelocity; }
      set { _RightShoulderRotationAngularVelocity = value; }
    }
    private float _RightShoulderRotationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(37, IsRequired = true, Name=@"RightShoulderRotationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightShoulderRotationAngularAcceleration
    {
      get { return _RightShoulderRotationAngularAcceleration; }
      set { _RightShoulderRotationAngularAcceleration = value; }
    }
    private float _LeftElbowFlexionAngle;
    [global::ProtoBuf.ProtoMember(38, IsRequired = true, Name=@"LeftElbowFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftElbowFlexionAngle
    {
      get { return _LeftElbowFlexionAngle; }
      set { _LeftElbowFlexionAngle = value; }
    }
    private float _LeftElbowFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(39, IsRequired = true, Name=@"LeftElbowFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftElbowFlexionSignedAngle
    {
      get { return _LeftElbowFlexionSignedAngle; }
      set { _LeftElbowFlexionSignedAngle = value; }
    }
    private float _LeftForeArmPronationSignedAngle;
    [global::ProtoBuf.ProtoMember(40, IsRequired = true, Name=@"LeftForeArmPronationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftForeArmPronationSignedAngle
    {
      get { return _LeftForeArmPronationSignedAngle; }
      set { _LeftForeArmPronationSignedAngle = value; }
    }
    private float _LeftShoulderFlexionAngle;
    [global::ProtoBuf.ProtoMember(42, IsRequired = true, Name=@"LeftShoulderFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderFlexionAngle
    {
      get { return _LeftShoulderFlexionAngle; }
      set { _LeftShoulderFlexionAngle = value; }
    }
    private float _LeftShoulderFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(43, IsRequired = true, Name=@"LeftShoulderFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderFlexionSignedAngle
    {
      get { return _LeftShoulderFlexionSignedAngle; }
      set { _LeftShoulderFlexionSignedAngle = value; }
    }
    private float _LeftShoulderVertAbductionAngle;
    [global::ProtoBuf.ProtoMember(44, IsRequired = true, Name=@"LeftShoulderVertAbductionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderVertAbductionAngle
    {
      get { return _LeftShoulderVertAbductionAngle; }
      set { _LeftShoulderVertAbductionAngle = value; }
    }
    private float _LeftShoulderVerticalAbductionSignedAngle;
    [global::ProtoBuf.ProtoMember(45, IsRequired = true, Name=@"LeftShoulderVerticalAbductionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderVerticalAbductionSignedAngle
    {
      get { return _LeftShoulderVerticalAbductionSignedAngle; }
      set { _LeftShoulderVerticalAbductionSignedAngle = value; }
    }
    private float _LeftShoulderHorAbductionAngle;
    [global::ProtoBuf.ProtoMember(46, IsRequired = true, Name=@"LeftShoulderHorAbductionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderHorAbductionAngle
    {
      get { return _LeftShoulderHorAbductionAngle; }
      set { _LeftShoulderHorAbductionAngle = value; }
    }
    private float _LeftShoulderHorizontalAbductionSignedAngle;
    [global::ProtoBuf.ProtoMember(47, IsRequired = true, Name=@"LeftShoulderHorizontalAbductionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderHorizontalAbductionSignedAngle
    {
      get { return _LeftShoulderHorizontalAbductionSignedAngle; }
      set { _LeftShoulderHorizontalAbductionSignedAngle = value; }
    }
    private float _LeftShoulderRotationSignedAngle;
    [global::ProtoBuf.ProtoMember(48, IsRequired = true, Name=@"LeftShoulderRotationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderRotationSignedAngle
    {
      get { return _LeftShoulderRotationSignedAngle; }
      set { _LeftShoulderRotationSignedAngle = value; }
    }
    private float _LeftElbowFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(50, IsRequired = true, Name=@"LeftElbowFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftElbowFlexionAngularVelocity
    {
      get { return _LeftElbowFlexionAngularVelocity; }
      set { _LeftElbowFlexionAngularVelocity = value; }
    }
    private float _LeftElbowFlexionPeakAngularVelocity;
    [global::ProtoBuf.ProtoMember(51, IsRequired = true, Name=@"LeftElbowFlexionPeakAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftElbowFlexionPeakAngularVelocity
    {
      get { return _LeftElbowFlexionPeakAngularVelocity; }
      set { _LeftElbowFlexionPeakAngularVelocity = value; }
    }
    private float _LeftElbowFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(52, IsRequired = true, Name=@"LeftElbowFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftElbowFlexionAngularAcceleration
    {
      get { return _LeftElbowFlexionAngularAcceleration; }
      set { _LeftElbowFlexionAngularAcceleration = value; }
    }
    private float _LeftForeArmPronationAngularVelocity;
    [global::ProtoBuf.ProtoMember(53, IsRequired = true, Name=@"LeftForeArmPronationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftForeArmPronationAngularVelocity
    {
      get { return _LeftForeArmPronationAngularVelocity; }
      set { _LeftForeArmPronationAngularVelocity = value; }
    }
    private float _LeftElbowPronationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(54, IsRequired = true, Name=@"LeftElbowPronationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftElbowPronationAngularAcceleration
    {
      get { return _LeftElbowPronationAngularAcceleration; }
      set { _LeftElbowPronationAngularAcceleration = value; }
    }
    private float _LeftShoulderFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(55, IsRequired = true, Name=@"LeftShoulderFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderFlexionAngularVelocity
    {
      get { return _LeftShoulderFlexionAngularVelocity; }
      set { _LeftShoulderFlexionAngularVelocity = value; }
    }
    private float _LeftShoulderFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(56, IsRequired = true, Name=@"LeftShoulderFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderFlexionAngularAcceleration
    {
      get { return _LeftShoulderFlexionAngularAcceleration; }
      set { _LeftShoulderFlexionAngularAcceleration = value; }
    }
    private float _LeftShoulderVertAbductionAngularVelocity;
    [global::ProtoBuf.ProtoMember(57, IsRequired = true, Name=@"LeftShoulderVertAbductionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderVertAbductionAngularVelocity
    {
      get { return _LeftShoulderVertAbductionAngularVelocity; }
      set { _LeftShoulderVertAbductionAngularVelocity = value; }
    }
    private float _LeftShoulderVertAbductionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(58, IsRequired = true, Name=@"LeftShoulderVertAbductionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderVertAbductionAngularAcceleration
    {
      get { return _LeftShoulderVertAbductionAngularAcceleration; }
      set { _LeftShoulderVertAbductionAngularAcceleration = value; }
    }
    private float _LeftShoulderHorAbductionAngularVelocity;
    [global::ProtoBuf.ProtoMember(59, IsRequired = true, Name=@"LeftShoulderHorAbductionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderHorAbductionAngularVelocity
    {
      get { return _LeftShoulderHorAbductionAngularVelocity; }
      set { _LeftShoulderHorAbductionAngularVelocity = value; }
    }
    private float _LeftShoulderHorAbductionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(60, IsRequired = true, Name=@"LeftShoulderHorAbductionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderHorAbductionAngularAcceleration
    {
      get { return _LeftShoulderHorAbductionAngularAcceleration; }
      set { _LeftShoulderHorAbductionAngularAcceleration = value; }
    }
    private float _LeftShoulderRotationAngularVelocity;
    [global::ProtoBuf.ProtoMember(61, IsRequired = true, Name=@"LeftShoulderRotationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderRotationAngularVelocity
    {
      get { return _LeftShoulderRotationAngularVelocity; }
      set { _LeftShoulderRotationAngularVelocity = value; }
    }
    private float _LeftShoulderRotationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(62, IsRequired = true, Name=@"LeftShoulderRotationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftShoulderRotationAngularAcceleration
    {
      get { return _LeftShoulderRotationAngularAcceleration; }
      set { _LeftShoulderRotationAngularAcceleration = value; }
    }
    private float _RightKneeFlexionAngle;
    [global::ProtoBuf.ProtoMember(63, IsRequired = true, Name=@"RightKneeFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightKneeFlexionAngle
    {
      get { return _RightKneeFlexionAngle; }
      set { _RightKneeFlexionAngle = value; }
    }
    private float _RightKneeFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(64, IsRequired = true, Name=@"RightKneeFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightKneeFlexionSignedAngle
    {
      get { return _RightKneeFlexionSignedAngle; }
      set { _RightKneeFlexionSignedAngle = value; }
    }
    private float _RightKneeRotationSignedAngle;
    [global::ProtoBuf.ProtoMember(65, IsRequired = true, Name=@"RightKneeRotationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightKneeRotationSignedAngle
    {
      get { return _RightKneeRotationSignedAngle; }
      set { _RightKneeRotationSignedAngle = value; }
    }
    private float _RightHipFlexionAngle;
    [global::ProtoBuf.ProtoMember(66, IsRequired = true, Name=@"RightHipFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipFlexionAngle
    {
      get { return _RightHipFlexionAngle; }
      set { _RightHipFlexionAngle = value; }
    }
    private float _RightHipFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(67, IsRequired = true, Name=@"RightHipFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipFlexionSignedAngle
    {
      get { return _RightHipFlexionSignedAngle; }
      set { _RightHipFlexionSignedAngle = value; }
    }
    private float _RightHipAbductionAngle;
    [global::ProtoBuf.ProtoMember(68, IsRequired = true, Name=@"RightHipAbductionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipAbductionAngle
    {
      get { return _RightHipAbductionAngle; }
      set { _RightHipAbductionAngle = value; }
    }
    private float _RightHipAbductionSignedAngle;
    [global::ProtoBuf.ProtoMember(69, IsRequired = true, Name=@"RightHipAbductionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipAbductionSignedAngle
    {
      get { return _RightHipAbductionSignedAngle; }
      set { _RightHipAbductionSignedAngle = value; }
    }
    private float _RightHipRotationSignedAngle;
    [global::ProtoBuf.ProtoMember(70, IsRequired = true, Name=@"RightHipRotationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipRotationSignedAngle
    {
      get { return _RightHipRotationSignedAngle; }
      set { _RightHipRotationSignedAngle = value; }
    }
    private float _RightKneeFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(71, IsRequired = true, Name=@"RightKneeFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightKneeFlexionAngularVelocity
    {
      get { return _RightKneeFlexionAngularVelocity; }
      set { _RightKneeFlexionAngularVelocity = value; }
    }
    private float _RightKneeFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(72, IsRequired = true, Name=@"RightKneeFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightKneeFlexionAngularAcceleration
    {
      get { return _RightKneeFlexionAngularAcceleration; }
      set { _RightKneeFlexionAngularAcceleration = value; }
    }
    private float _RightKneeRotationAngularVelocity;
    [global::ProtoBuf.ProtoMember(73, IsRequired = true, Name=@"RightKneeRotationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightKneeRotationAngularVelocity
    {
      get { return _RightKneeRotationAngularVelocity; }
      set { _RightKneeRotationAngularVelocity = value; }
    }
    private float _RightKneeRotationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(74, IsRequired = true, Name=@"RightKneeRotationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightKneeRotationAngularAcceleration
    {
      get { return _RightKneeRotationAngularAcceleration; }
      set { _RightKneeRotationAngularAcceleration = value; }
    }
    private float _RightHipFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(75, IsRequired = true, Name=@"RightHipFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipFlexionAngularVelocity
    {
      get { return _RightHipFlexionAngularVelocity; }
      set { _RightHipFlexionAngularVelocity = value; }
    }
    private float _RightHipFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(76, IsRequired = true, Name=@"RightHipFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipFlexionAngularAcceleration
    {
      get { return _RightHipFlexionAngularAcceleration; }
      set { _RightHipFlexionAngularAcceleration = value; }
    }
    private float _RightHipAbductionAngularVelocity;
    [global::ProtoBuf.ProtoMember(77, IsRequired = true, Name=@"RightHipAbductionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipAbductionAngularVelocity
    {
      get { return _RightHipAbductionAngularVelocity; }
      set { _RightHipAbductionAngularVelocity = value; }
    }
    private float _RightHipAbductionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(78, IsRequired = true, Name=@"RightHipAbductionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipAbductionAngularAcceleration
    {
      get { return _RightHipAbductionAngularAcceleration; }
      set { _RightHipAbductionAngularAcceleration = value; }
    }
    private float _RightHipRotationAngularVelocity;
    [global::ProtoBuf.ProtoMember(79, IsRequired = true, Name=@"RightHipRotationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipRotationAngularVelocity
    {
      get { return _RightHipRotationAngularVelocity; }
      set { _RightHipRotationAngularVelocity = value; }
    }
    private float _RightHipRotationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(80, IsRequired = true, Name=@"RightHipRotationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightHipRotationAngularAcceleration
    {
      get { return _RightHipRotationAngularAcceleration; }
      set { _RightHipRotationAngularAcceleration = value; }
    }
    private float _RightLegHeight;
    [global::ProtoBuf.ProtoMember(81, IsRequired = true, Name=@"RightLegHeight", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightLegHeight
    {
      get { return _RightLegHeight; }
      set { _RightLegHeight = value; }
    }
    private float _RightInitThighHeight;
    [global::ProtoBuf.ProtoMember(82, IsRequired = true, Name=@"RightInitThighHeight", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightInitThighHeight
    {
      get { return _RightInitThighHeight; }
      set { _RightInitThighHeight = value; }
    }
    private float _RightInitTibiaHeight;
    [global::ProtoBuf.ProtoMember(83, IsRequired = true, Name=@"RightInitTibiaHeight", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float RightInitTibiaHeight
    {
      get { return _RightInitTibiaHeight; }
      set { _RightInitTibiaHeight = value; }
    }
    private float _LeftKneeFlexionAngle;
    [global::ProtoBuf.ProtoMember(84, IsRequired = true, Name=@"LeftKneeFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftKneeFlexionAngle
    {
      get { return _LeftKneeFlexionAngle; }
      set { _LeftKneeFlexionAngle = value; }
    }
    private float _LeftKneeFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(85, IsRequired = true, Name=@"LeftKneeFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftKneeFlexionSignedAngle
    {
      get { return _LeftKneeFlexionSignedAngle; }
      set { _LeftKneeFlexionSignedAngle = value; }
    }
    private float _LeftKneeRotationSignedAngle;
    [global::ProtoBuf.ProtoMember(86, IsRequired = true, Name=@"LeftKneeRotationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftKneeRotationSignedAngle
    {
      get { return _LeftKneeRotationSignedAngle; }
      set { _LeftKneeRotationSignedAngle = value; }
    }
    private float _LeftHipFlexionAngle;
    [global::ProtoBuf.ProtoMember(87, IsRequired = true, Name=@"LeftHipFlexionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipFlexionAngle
    {
      get { return _LeftHipFlexionAngle; }
      set { _LeftHipFlexionAngle = value; }
    }
    private float _LeftHipFlexionSignedAngle;
    [global::ProtoBuf.ProtoMember(88, IsRequired = true, Name=@"LeftHipFlexionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipFlexionSignedAngle
    {
      get { return _LeftHipFlexionSignedAngle; }
      set { _LeftHipFlexionSignedAngle = value; }
    }
    private float _LeftHipAbductionAngle;
    [global::ProtoBuf.ProtoMember(89, IsRequired = true, Name=@"LeftHipAbductionAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipAbductionAngle
    {
      get { return _LeftHipAbductionAngle; }
      set { _LeftHipAbductionAngle = value; }
    }
    private float _LeftHipAbductionSignedAngle;
    [global::ProtoBuf.ProtoMember(90, IsRequired = true, Name=@"LeftHipAbductionSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipAbductionSignedAngle
    {
      get { return _LeftHipAbductionSignedAngle; }
      set { _LeftHipAbductionSignedAngle = value; }
    }
    private float _LeftHipRotationSignedAngle;
    [global::ProtoBuf.ProtoMember(91, IsRequired = true, Name=@"LeftHipRotationSignedAngle", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipRotationSignedAngle
    {
      get { return _LeftHipRotationSignedAngle; }
      set { _LeftHipRotationSignedAngle = value; }
    }
    private float _LeftKneeFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(92, IsRequired = true, Name=@"LeftKneeFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftKneeFlexionAngularVelocity
    {
      get { return _LeftKneeFlexionAngularVelocity; }
      set { _LeftKneeFlexionAngularVelocity = value; }
    }
    private float _LeftKneeFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(93, IsRequired = true, Name=@"LeftKneeFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftKneeFlexionAngularAcceleration
    {
      get { return _LeftKneeFlexionAngularAcceleration; }
      set { _LeftKneeFlexionAngularAcceleration = value; }
    }
    private float _LeftKneeRotationAngularVelocity;
    [global::ProtoBuf.ProtoMember(94, IsRequired = true, Name=@"LeftKneeRotationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftKneeRotationAngularVelocity
    {
      get { return _LeftKneeRotationAngularVelocity; }
      set { _LeftKneeRotationAngularVelocity = value; }
    }
    private float _LeftKneeRotationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(95, IsRequired = true, Name=@"LeftKneeRotationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftKneeRotationAngularAcceleration
    {
      get { return _LeftKneeRotationAngularAcceleration; }
      set { _LeftKneeRotationAngularAcceleration = value; }
    }
    private float _LeftHipFlexionAngularVelocity;
    [global::ProtoBuf.ProtoMember(96, IsRequired = true, Name=@"LeftHipFlexionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipFlexionAngularVelocity
    {
      get { return _LeftHipFlexionAngularVelocity; }
      set { _LeftHipFlexionAngularVelocity = value; }
    }
    private float _LeftHipFlexionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(97, IsRequired = true, Name=@"LeftHipFlexionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipFlexionAngularAcceleration
    {
      get { return _LeftHipFlexionAngularAcceleration; }
      set { _LeftHipFlexionAngularAcceleration = value; }
    }
    private float _LeftHipAbductionAngularVelocity;
    [global::ProtoBuf.ProtoMember(98, IsRequired = true, Name=@"LeftHipAbductionAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipAbductionAngularVelocity
    {
      get { return _LeftHipAbductionAngularVelocity; }
      set { _LeftHipAbductionAngularVelocity = value; }
    }
    private float _LeftHipAbductionAngularAcceleration;
    [global::ProtoBuf.ProtoMember(99, IsRequired = true, Name=@"LeftHipAbductionAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipAbductionAngularAcceleration
    {
      get { return _LeftHipAbductionAngularAcceleration; }
      set { _LeftHipAbductionAngularAcceleration = value; }
    }
    private float _LeftHipRotationAngularVelocity;
    [global::ProtoBuf.ProtoMember(100, IsRequired = true, Name=@"LeftHipRotationAngularVelocity", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipRotationAngularVelocity
    {
      get { return _LeftHipRotationAngularVelocity; }
      set { _LeftHipRotationAngularVelocity = value; }
    }
    private float _LeftHipRotationAngularAcceleration;
    [global::ProtoBuf.ProtoMember(101, IsRequired = true, Name=@"LeftHipRotationAngularAcceleration", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftHipRotationAngularAcceleration
    {
      get { return _LeftHipRotationAngularAcceleration; }
      set { _LeftHipRotationAngularAcceleration = value; }
    }
    private float _LeftLegHeight;
    [global::ProtoBuf.ProtoMember(102, IsRequired = true, Name=@"LeftLegHeight", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftLegHeight
    {
      get { return _LeftLegHeight; }
      set { _LeftLegHeight = value; }
    }
    private float _LeftInitThighHeight;
    [global::ProtoBuf.ProtoMember(103, IsRequired = true, Name=@"LeftInitThighHeight", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftInitThighHeight
    {
      get { return _LeftInitThighHeight; }
      set { _LeftInitThighHeight = value; }
    }
    private float _LeftInitTibiaHeight;
    [global::ProtoBuf.ProtoMember(104, IsRequired = true, Name=@"LeftInitTibiaHeight", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float LeftInitTibiaHeight
    {
      get { return _LeftInitTibiaHeight; }
      set { _LeftInitTibiaHeight = value; }
    }
    private float _ErgoScore;
    [global::ProtoBuf.ProtoMember(105, IsRequired = true, Name=@"ErgoScore", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public float ErgoScore
    {
      get { return _ErgoScore; }
      set { _ErgoScore = value; }
    }
    private uint _TimeStamp;
    [global::ProtoBuf.ProtoMember(106, IsRequired = true, Name=@"TimeStamp", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    public uint TimeStamp
    {
      get { return _TimeStamp; }
      set { _TimeStamp = value; }
    }
    private uint _UserId;
    [global::ProtoBuf.ProtoMember(107, IsRequired = true, Name=@"UserId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint UserId
    {
      get { return _UserId; }
      set { _UserId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}