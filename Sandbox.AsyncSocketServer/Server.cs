using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public sealed class Server : IServer
    {
        readonly HashSet<ServerProcess> _bag = new HashSet<ServerProcess>();
        static readonly object LockObject = new Object();

        readonly Action<ServerProcess, string> _notifyLog;
        readonly Action<ServerProcess, Exception> _notifyException;

        public Server(
            Action<ServerProcess, string> notifyLog,
            Action<ServerProcess, Exception> notifyException)
        {
            _notifyLog = notifyLog;
            _notifyException = notifyException;
        }

        IEnumerable<ServerProcess> IServer.Processes
        {
            get { return _bag; }
        }

        public Action<ServerProcess, string> NotifyLog
        {
            get { return _notifyLog; }
        }

        public Action<ServerProcess, Exception> NotifyException
        {
            get { return _notifyException; }
        }

        void IServer.Add(ServerProcess process)
        {
            lock (LockObject) _bag.Add(process);
            process.Server = this;
        }

        void IServer.Remove(ServerProcess process)
        {
            lock (LockObject) _bag.Remove(process);
            process.Server = null;
        }

        #region dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                foreach (var process in _bag.ToArray())
                    process.Dispose();
            }

            _disposed = true;
        }

        ~Server()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion
    }
}