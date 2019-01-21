using System;
using System.Diagnostics;

namespace ResiLab.MailFilter.Infrastructure {
    public static class Logger {
        static Logger() {
            Trace.Listeners.Add(new EventLogTraceListener("ResiLab.MailFilter"));

            if (Environment.UserInteractive) {
                Trace.Listeners.Add(new ConsoleTraceListener());
            }
        }

        public static void Info(string message) {
            Trace.TraceInformation(message);
        }

        public static void Warn(string message) {
            Trace.TraceWarning(message);
        }

        public static void Error(string message, Exception exception = null) {
            Trace.TraceError("{0}{1}{2}", message, Environment.NewLine, exception);
        }
    }
}