using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class Listener : IListener
    {
        readonly Socket _listener;
        readonly Stack<SocketAwaitable> _awaitablesPool;

        readonly Func<Socket, IDataSocket> _createDataSocket;

        public Listener(
            ListenerSettings settings, 
            Func<Socket, IDataSocket> createDataSocket)
        {
            _createDataSocket = createDataSocket;

            // demand permission
            var permission = new SocketPermission(
                NetworkAccess.Connect, TransportType.Tcp,
                settings.IPAddress.ToString(), settings.Port);
            permission.Demand();

            // create event arg pool
            _awaitablesPool = new Stack<SocketAwaitable>(
                Enumerable.Range(0, 10)
                          .Select(i => new SocketAwaitable()));

            // create a real socket and wrap it up
            _listener = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _listener.Bind(new IPEndPoint(settings.IPAddress, settings.Port));
            _listener.Listen(100);
        }

        public async Task<IDataSocket> AcceptAsync()
        {
            var awaitable = _awaitablesPool.Pop();

            awaitable.Reset();
            if (!_listener.AcceptAsync(awaitable.EventArgs))
                awaitable.IsCompleted = true;

            await awaitable;

            var socket = awaitable.EventArgs.AcceptSocket;
            awaitable.EventArgs.AcceptSocket = null;
            _awaitablesPool.Push(awaitable);

            return _createDataSocket(socket);
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
                if (_listener != null)
                {
                    _listener.Dispose();
                }
            }

            _disposed = true;
        }

        ~Listener()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion
    }
}