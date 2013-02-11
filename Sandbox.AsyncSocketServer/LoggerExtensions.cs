using System;
using System.Diagnostics;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public static class LoggerExtensions
    {
        static void Log(
            ILogger logger,
            LogEntryType entryType, string filter, string message)
        {
            if (logger == null)
                Debug.WriteLine(message, filter);
#pragma warning disable 612,618
            else logger.Log(entryType, filter, message);
#pragma warning restore 612,618
        }

        public static void Diagnostic<T>(
            this ILogger logger,
            T sender, string message)
        {
            Log(logger,
                LogEntryType.Diagnostic,
                typeof (T).Name,
                string.Format("{0}: {1}", sender, message));
        }

        public static void Information<T>(
            this ILogger logger,
            T sender, string message)
        {
            Log(logger,
                LogEntryType.Information,
                typeof (T).Name,
                string.Format("{0}: {1}", sender, message));
        }

        public static void Error<T>(
            this ILogger logger,
            T sender, Exception exception)
        {
            Log(logger,
                LogEntryType.Error,
                typeof (T).Name,
                string.Format("{0}: {1}", sender, exception));
        }

        public static void Error<T>(
            this ILogger logger,
            T sender, string message)
        {
            Log(logger,
                LogEntryType.Error,
                typeof (T).Name,
                string.Format("{0}: {1}", sender, message));
        }
    }
}