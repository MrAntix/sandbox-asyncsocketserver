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
        readonly SocketAwaitable _awaitable;

        readonly IListenerSocket _listenerSocket;
        readonly Func<Socket, IWorker> _createWorker;

        public Listener(
            ListenerSettings settings, 
            IListenerSocket listenerSocket,
            Func<Socket, IWorker> createWorker)
        {
            _createWorker = createWorker;
            _listenerSocket = listenerSocket;

            // create awaitable
            _awaitable = new SocketAwaitable(Timeout.InfiniteTimeSpan);

            _listenerSocket
                .Listen(
                    new IPEndPoint(settings.IPAddress, settings.Port),
                    settings.Backlog);
        }

        public async Task<IWorker> AcceptAsync()
        {
            _awaitable.Reset();
            if (!_listenerSocket.AcceptAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;

            var socket = _awaitable.EventArgs.AcceptSocket;
            _awaitable.EventArgs.AcceptSocket = null;

            return _createWorker(socket);
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
                if (_listenerSocket != null)
                {
                    _listenerSocket.Dispose();
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