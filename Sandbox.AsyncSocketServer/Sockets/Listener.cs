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
        Socket _socket;
        SocketAwaitable _awaitable;

        readonly ListenerSettings _settings;
        readonly Func<Socket, IWorker> _createWorker;

        public Listener(
            ListenerSettings settings,
            Func<Socket, IWorker> createWorker)
        {
            _settings = settings;
            _createWorker = createWorker;

            // create awaitable
            _awaitable = new SocketAwaitable(Timeout.InfiniteTimeSpan);
        }

        public void Start()
        {
            if (_socket != null)
                throw new InvalidOperationException("Listener already started");

            _socket = CreateBoundSocket(
                new IPEndPoint(_settings.IPAddress, _settings.Port));
            _socket.Listen(_settings.Backlog);
        }

        public async Task<IWorker> AcceptAsync()
        {
            if (_socket == null)
                throw new InvalidOperationException("Listener not started");

            _awaitable.Reset();
            if (!_socket.AcceptAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;

            if (_awaitable.IsCanceled)
            {

                return null;
            }

            var socket = _awaitable.EventArgs.AcceptSocket;
            _awaitable.EventArgs.AcceptSocket = null;

            return _createWorker(socket);
        }

        public void Stop()
        {
            _awaitable.Cancel();

            _socket.Close();
            _socket = null;
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
                _socket.Close();
                _socket.Dispose();
                _socket = null;

                _awaitable.Dispose();
                _awaitable = null;
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