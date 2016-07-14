 /**
 * @file AnalysisSerialization.cs
 * @brief Contains the AnalysisSerialization
 * @author Mohammed Haider( mohammed@heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */
namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// A custom attribute to denote whether an analysis segment is to be ignored, if not, what is its custom Attribute name. 
    /// </summary>
    public class AnalysisSerialization : System.Attribute
    {
        public bool IgnoreAttribute { get; set; }
        public string AttributeName { get; set; }
    }
}