using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Sockets
{
    public class Listener : IListener
    {
        readonly Socket _socket;
        readonly SocketAwaitable _awaitable;

        readonly Func<Socket, IWorker> _createWorker;

        public Listener(
            ListenerSettings settings,
            Func<Socket, IWorker> createWorker)
        {
            _createWorker = createWorker;

            // create awaitable
            _awaitable = new SocketAwaitable(Timeout.InfiniteTimeSpan);

            _socket = CreateBoundSocket(
                new IPEndPoint(settings.IPAddress, settings.Port));
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

            return _createWorker(socket);
        }

        static Socket CreateBoundSocket(IPEndPoint endpoint)
        {
            // demand permission
            var permission = new SocketPermission(
                NetworkAccess.Connect, TransportType.Tcp,
                endpoint.Address.ToString(), endpoint.Port);
            permission.Demand();

            // create a tcp socket to listen
            var socket = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(endpoint);

            return socket;
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
                _socket.Dispose();
                _awaitable.Dispose();
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