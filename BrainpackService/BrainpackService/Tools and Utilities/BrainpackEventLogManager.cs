 
 

namespace BrainpackService.Tools_and_Utilities
{
   public  class BrainpackEventLogManager
   {
       public  delegate void ExceptionHandlingDelegate(string vMsg);

        public delegate void ExceptionThrowingDelegate(string vMsg);

       public delegate void EventLogDelegate(string vMsg);
        private static event ExceptionThrowingDelegate ExceptionThrowingDelegateHandler;
#pragma warning disable 67
        private static event ExceptionHandlingDelegate ExceptionHandlingDelegateHandler;
#pragma warning restore 67
       private static event EventLogDelegate EventLogHandling;

       public static void RegisterExceptionHandlingDelegate(ExceptionHandlingDelegate vEhd)
       {
           ExceptionHandlingDelegateHandler += vEhd;
       }

       public static void RegisterExceptionThrowingDelegate(ExceptionThrowingDelegate vEtd)
       {
           ExceptionThrowingDelegateHandler += vEtd;
       }

        public static void DeRegisterExceptionHandlingDelegate(ExceptionHandlingDelegate vEhd)
        {
            ExceptionHandlingDelegateHandler -= vEhd;
        }

        public static void DeRegisterExceptionThrowingDelegate(ExceptionThrowingDelegate vEtd)
        {
            ExceptionThrowingDelegateHandler -= vEtd;
        }

       public static void RegisterEventLogMessage(EventLogDelegate vEld)
       {
           EventLogHandling += vEld;
       }

        public static void DeRegisterEventLogMessage(EventLogDelegate vEld)
        {
            EventLogHandling -= vEld;
        }
        public static void InvokeExceptionThrowingDelegate(string vMsg)
       {
           if (ExceptionThrowingDelegateHandler != null)
           {
               ExceptionThrowingDelegateHandler(vMsg);
           }
       }

       public static void InvokeEventLogMessage(string vMsg)
       {
           if (EventLogHandling != null)
           {
               EventLogHandling(vMsg);
           }
       }

       public static void InvokeEventLogError(string vMsg)
       {
            if (EventLogHandling != null)
            {
                EventLogHandling(vMsg);
            }
        }

       public static void InvokeNetworkingException(string vMsg)
       {
            if (EventLogHandling != null)
            {
                EventLogHandling(vMsg);
            }
        }
    }
}
