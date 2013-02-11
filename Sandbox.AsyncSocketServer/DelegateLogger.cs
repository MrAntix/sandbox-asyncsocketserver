using System;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public sealed class DelegateLogger : ILogger
    {
        readonly Action<LogEntryType, string, string> _action;

        public DelegateLogger(
            Action<LogEntryType, string, string> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            _action = action;
        }

        void ILogger.Log(LogEntryType entryType, string filter, string text)
        {
            _action(entryType, filter, text);
        }
    }
}