using System;
using System.Collections.Generic;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class Server : IServer
    {
        readonly List<ServerProcess> _bag = new List<ServerProcess>();
        static readonly object LockObject = new Object();

        IEnumerable<ServerProcess> IServer.Processes
        {
            get { return _bag; }
        }

        ServerProcess IServer.Start(
            IListener listener,
            IMessageHandler handler)
        {
            var process = new ServerProcess(listener, handler);
            lock (LockObject) _bag.Add(process);

            process.Start(Stop);

            return process;
        }

        public void Stop(ServerProcess process)
        {
            lock (LockObject) _bag.Remove(process);
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
                foreach (var process in _bag.ToArray()) process.Dispose();
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