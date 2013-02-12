using System;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public sealed class DelegateLogger : ILogger
    {
        readonly Action<LogLevel, string, string> _action;

        public DelegateLogger(
            Action<LogLevel, string, string> action, LogLevel level = LogLevel.Information)
        {
            if (action == null) throw new ArgumentNullException("action");

            _action = action;
            Level = level;
        }

        public LogLevel Level { get; set; }

        void ILogger.Log(LogLevel level, string filter, string text)
        {
            _action(level, filter, text);
        }
    }
}