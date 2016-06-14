/**
* @file IQuaternionMathServiceProvider.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed@heddoko.com)
* @date 05 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using Assets.Scripts.Utils.HMath.Structure;

namespace Assets.Scripts.Utils.HMath.Service_Provider
{
    public interface IQuaternionMathServiceProvider
    {
        HQuaternion Identity { get; }
        float Dot(HQuaternion vHQuaternion, HQuaternion vHQuaternion1);
        HQuaternion AngleAxis(float vAngle,   HVector3 vAxis);
         HQuaternion FromToRotation(HVector3 vFromDirection, HVector3 vToDirection);
        HQuaternion LookRotation(  HVector3 vForward,   HVector3 vUpwards);
        HQuaternion Slerp(HQuaternion vFrom, HQuaternion vTo, float vF);
        HQuaternion Lerp(HQuaternion vFrom, HQuaternion vTo, float vF);
        HQuaternion RotateTowards(HQuaternion vFrom, HQuaternion vTo, float vMaxDegreesDelta);
        HQuaternion Inverse(HQuaternion vRotation);
        float Angle(HQuaternion vHQuaternion, HQuaternion vHQuaternion1);
        HQuaternion Euler(float vF, float vF1, float vF2);
        HQuaternion Euler(HVector3 vEuler);
        HQuaternion Multiply(HQuaternion vLhs, HQuaternion vRhs);
        HVector3 Multiply(HQuaternion vRotation, HVector3 vPoint);
    }
}