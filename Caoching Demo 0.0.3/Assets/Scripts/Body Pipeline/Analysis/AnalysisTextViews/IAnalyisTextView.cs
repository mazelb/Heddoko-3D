/** 
* @file IAnalyisTextView.cs
* @brief Contains the IAnalyisTextView  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using JetBrains.Annotations;

namespace Assets.Scripts.Body_Data.View.Anaylsis
{
    /// <summary>
    /// An interface for Text views
    /// </summary>
    public interface IAnalyisTextView
    {
         string LabelName { get;  }
        Body BodyToAnalyze { get; set; }

        void Hide();
        void Show();
    }
}