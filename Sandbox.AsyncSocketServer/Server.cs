using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class Server : IServer
    {
        readonly HashSet<ServerProcess> _bag = new HashSet<ServerProcess>();
        static readonly object LockObject = new Object();

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

        protected virtual void Dispose(bool disposing)
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