/** 
* @file U3DVector3.cs
* @brief Contains the U3DVector3  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.Utils.HMath.Structure
{
    /// <summary>
    /// A implementation of HVector3 to be used within the Unity 3D platform
    /// </summary>
    public class U3DVector3 : HVector3
    {
        internal Vector3 mVector3;

        public U3DVector3(float x, float y, float z) : base(x, y, z)
        {

        }
        /// <summary>
        /// Copies components of the passed in vector. 
        /// </summary>
        /// <param name="vVector"></param>
        public U3DVector3(Vector3 vVector) : base(vVector.x, vVector.y, vVector.z)
        {
        }

        public U3DVector3(float x, float y) : base(x, y)
        {

        }
        /// <summary>
        /// X component
        /// </summary>
        public override float X
        {
            get { return mVector3.x; }
            set { mVector3.x = value; }
        }
        /// <summary>
        /// Y Component
        /// </summary>
        public override float Y
        {
            get { return mVector3.y; }
            set { mVector3.y = value; }
        }


        /// <summary>
        /// Get and set the Z component 
        /// </summary>
        public override float Z
        {
            get { return mVector3.z; }
            set { mVector3.z = value; }
        }



        /// <summary>
        /// Normalizes the vector
        /// </summary>
        /// <returns></returns>
        public override void Normalize()
        {
            mVector3.Normalize();
        }

        /// <summary>
        /// Returns the vector as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mVector3.ToString();
        }

        public override void Cross(HVector3 vVector)
        {
            mVector3 = Vector3.Cross(mVector3, ((U3DVector3)vVector).mVector3);
        }

        /// <summary>
        /// returns the vector as a string with a specified format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public override string ToString(string format)
        {
            return mVector3.ToString(format);
        }
        /// <summary>
        /// Adds vector b to the instance and returns a new vector
        /// </summary> 
        /// <returns>this + b</returns>
        protected override HVector3 Plus(HVector3 b)
        {
            return new U3DVector3(mVector3 + new Vector3(b.X, b.Y, b.Z));
        }

        protected override HVector3 Minus(HVector3 b)
        {
            return new U3DVector3(mVector3 - new Vector3(b.X, b.Y, b.Z));
        }

        protected override HVector3 ScalarMultiply(float d)
        {
            return new U3DVector3(mVector3 * d);
        }
    }
}
