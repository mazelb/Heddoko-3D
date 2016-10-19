// /**
// * @file ErrorController.cs
// * @brief Contains the ErrorController class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;

namespace Assets.Scripts.ErrorHandling
{
    /// <summary>
    /// A an error controller class that handles errors
    /// </summary> 
    public abstract class ErrorController
    {
         public abstract void AddErrorHandler(string vKey, IErrorHandler vHandler);
        
        public abstract void RemoveErrorHandler(string vKey, IErrorHandler vHandler);

        public abstract void Notify(string vKey);

        public abstract void Clear();
        public abstract void ClearErrorHandlerKey(string vKey);
    }
}