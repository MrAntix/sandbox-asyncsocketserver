using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public sealed class Server : IServer
    {
        readonly DelegateLogger _logger;
        readonly HashSet<ServerProcess> _bag = new HashSet<ServerProcess>();
        static readonly object LockObject = new Object();

        public Server(DelegateLogger logger)
        {
            _logger = logger;
        }

        IEnumerable<ServerProcess> IServer.Processes
        {
            get { return _bag; }
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

        void Dispose(bool disposing)
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