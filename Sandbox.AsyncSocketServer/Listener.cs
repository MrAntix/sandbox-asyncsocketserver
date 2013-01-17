using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class Listener : IListener
    {
        readonly Socket _socket;
        readonly SocketAwaitable _awaitable;

        readonly Func<Socket, IWorker> _createDataSocket;

        public Listener(
            ListenerSettings settings,
            Func<Socket, IWorker> createDataSocket)
        {
            _createDataSocket = createDataSocket;

            // demand permission
            var permission = new SocketPermission(
                NetworkAccess.Connect, TransportType.Tcp,
                settings.IPAddress.ToString(), settings.Port);
            permission.Demand();

            // create awaitable
            _awaitable = new SocketAwaitable(Timeout.InfiniteTimeSpan);

            // create a tcp socket to listen
            _socket = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(new IPEndPoint(settings.IPAddress, settings.Port));
            _socket.Listen(settings.Backlog);
        }

        public async Task<IWorker> AcceptAsync()
        {
            _awaitable.Reset();
            if (!_socket.AcceptAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;

            var socket = _awaitable.EventArgs.AcceptSocket;
            _awaitable.EventArgs.AcceptSocket = null;

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
                if (_socket != null)
                {
                    _socket.Dispose();
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