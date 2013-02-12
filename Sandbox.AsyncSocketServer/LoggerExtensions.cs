using System;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public static class LoggerExtensions
    {
        public static Action<string> NullLog = null;

        static void Log<T>(
            ILogger logger,
            LogLevel level,
            T sender, Func<string> getText)
        {
            if (logger != null)
            {
                if (logger.Level >= level)
                {
                    var text = string.Format("{0}: {1}", sender, getText());

#pragma warning disable 612,618
                    logger.Log(level, typeof (T).Name, text);
#pragma warning restore 612,618
                }
            }

            else if (NullLog != null)
            {
                NullLog(
                    string.Format("[{0}:{1}] {2}: {3}", level, typeof (T).Name, sender, getText())
                    );
            }
        }

        public static void Error<T>(
            this ILogger logger,
            T sender, Func<string> getText)
        {
            Log(logger, LogLevel.Error, sender, getText);
        }

        public static void Error<T>(
            this ILogger logger,
            T sender, Exception exception)
        {
            Log(logger, LogLevel.Error, sender, exception.ToString);
        }

        public static void System<T>(
            this ILogger logger,
            T sender, Func<string> getText)
        {
            Log(logger, LogLevel.System, sender, getText);
        }

        public static void Information<T>(
            this ILogger logger,
            T sender, Func<string> getText)
        {
            Log(logger, LogLevel.Information, sender, getText);
        }

        public static void Diagnostic<T>(
            this ILogger logger,
            T sender, Func<string> getText)
        {
            Log(logger, LogLevel.Diagnostic, sender, getText);
        }
    }
}