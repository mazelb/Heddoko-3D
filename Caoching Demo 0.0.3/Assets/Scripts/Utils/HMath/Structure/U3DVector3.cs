/** 
* @file U3DVector3.cs
* @brief Contains the U3DVector3  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/ 
using UnityEngine;

namespace Assets.Scripts.Utils.HMath
{
    /// <summary>
    /// A implementation of HVector3 to be used within the Unity 3D platform
    /// </summary>
    public class U3DVector3 : HVector3
    {
        internal Vector3 Instance;

        public U3DVector3(float x, float y, float z) : base(x, y, z)
        {
            X = x;
            Y = y;
            Z = z;

        }
        /// <summary>
        /// Copies components of the passed in vector. 
        /// </summary>
        /// <param name="vVector"></param>
        public U3DVector3(Vector3 vVector):base(vVector.x, vVector.y, vVector.z)
        {
            X = vVector.x;
            Y = vVector.y;
            Z = vVector.z;
        }

        public U3DVector3(float x, float y) : base(x, y)
        {

        }
        /// <summary>
        /// X component
        /// </summary>
        public override float X
        {
            get { return Instance.x; }
            set { Instance.x = value; }
        }
        /// <summary>
        /// Y Component
        /// </summary>
        public override float Y {
            get { return Instance.y; }
            set { Instance.y = value; }
        }


        /// <summary>
        /// Get and set the Z component 
        /// </summary>
        public override float Z
        {
            get { return Instance.z; }
            set { Instance.z= value; }
        }

        /// <summary>
        /// The vector's magnitude
        /// </summary>
        public override float Magnitude
        {
            get { return Instance.magnitude; }
        }

        /// <summary>
        /// The vector's magnitude squared. 
        /// </summary>
        public override float SqrMagnitude
        {
            get { return Instance.sqrMagnitude; }
        }

        /// <summary>
        /// Normalizes the vector
        /// </summary>
        /// <returns></returns>
        public override void Normalize()
        {
          Instance.Normalize();
        }

        /// <summary>
        /// Returns the vector as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Instance.ToString();
        }

        /// <summary>
        /// returns the vector as a string with a specified format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public override string ToString(string format)
        {
            return Instance.ToString(format);
        }
        /// <summary>
        /// Adds vector b to the instance and returns a new vector
        /// </summary> 
        /// <returns>this + b</returns>
        protected override HVector3 Plus(HVector3 b)
        {
            return new U3DVector3(Instance+ new Vector3(b.X,b.Y,b.Z)); 
        }

        protected override HVector3 Minus(HVector3 b)
        {
            return new U3DVector3(Instance - new Vector3(b.X, b.Y, b.Z));
        }

        protected override HVector3 ScalarMultiply(float d)
        {
            return new U3DVector3(Instance * d);
        }
    }
}
