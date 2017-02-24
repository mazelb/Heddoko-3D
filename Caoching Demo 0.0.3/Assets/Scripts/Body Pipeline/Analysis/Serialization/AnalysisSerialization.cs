/**
* @file AnalysisSerialization.cs
* @brief Contains the AnalysisSerialization
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using Newtonsoft.Json;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// A custom attribute to denote whether an analysis segment serialization is to be ignored, if not, what is its custom Attribute name and its order
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class AnalysisSerialization : System.Attribute, IComparable
    {

        [JsonIgnore] public bool IgnoreAttribute; 
        [JsonProperty] public string AttributeName; 
        [JsonProperty] public int Order; 


        /// <summary>
        /// sets the sign of the analysis attribute
        /// </summary>
        [JsonProperty] public bool IsSignNegative;

        public int CompareTo(object obj)
        {
            int vValue = -1;
            if (obj.GetType() == typeof(AnalysisSerialization))
            {
                var vCompare = (AnalysisSerialization)obj;
                {
                    vValue = Order.CompareTo(vCompare.Order);
                }
            }
            return vValue;
        }
    }


}
