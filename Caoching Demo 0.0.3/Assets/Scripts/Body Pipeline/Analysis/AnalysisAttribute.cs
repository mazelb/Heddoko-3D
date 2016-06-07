// /**
// * @file AnalysisAttribute.cs
// * @brief Contains the AnalysisAttribute
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// A custom attribute to denote whether an analysis segment is to be ignored, if not, what is its custom Attribute name. 
    /// </summary>
    public class AnalysisAttribute : System.Attribute
    {
        public bool IgnoreAttribute { get; set; }
        public string AttributeName { get; set; }
    }
}