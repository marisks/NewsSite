using System;
using System.Diagnostics.Tracing;

namespace ImportDataProcessor
{
    sealed class ImportEventSourceWriter : EventSource
    {
        public static ImportEventSourceWriter Log = new ImportEventSourceWriter();

        public void Error(string message, Exception ex)
        {
            if (IsEnabled())
            {
                var msg = string.Format("{0} \n Exception: {1} \n Stack Trace: {2}", message, ex.Message, ex.StackTrace);
                WriteEvent(2, msg);
            }
        }
    }
}