using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Sockets
{
    public class Worker : IWorker
    {
        IWorkerSocket _socket;
        readonly SocketAwaitable _awaitable;
        readonly Action _release;

        public Worker(
            IWorkerSocket socket, SocketAwaitable awaitable,
            Action release)
        {
            _socket = socket;
            _awaitable = awaitable;
            _release = release;
        }

        public async Task<byte[]> ReceiveAsync()
        {
            _awaitable.Reset();

            if (!_socket.ReceiveAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;

            if (_awaitable.IsTimedOut)
            {
                Close();
                throw new WorkerTimeoutException();
            }

            // check the number of bytes recieved, 
            // if 0, the remote end has closed the connection
            var bytesReceived = _awaitable.EventArgs.BytesTransferred;
            if (bytesReceived == 0) return null;

            return _awaitable
                .EventArgs
                .Buffer
                .Skip(_awaitable.EventArgs.Offset).Take(bytesReceived)
                .ToArray();
        }

        public async Task SendAsync(byte[] data)
        {
            _awaitable.Reset();
            _awaitable.EventArgs.SetBuffer(data, 0, data.Length);

            if (!_socket.SendAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;
        }

        public void Close()
        {
            if (_socket == null) return;

            _socket.Dispose();
            _socket = null;

            _release();
        }

        #region dispose

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            Close();
            Disposed = true;
        }

        ~Worker()
        {
            Dispose(false);
        }

        #endregion
    }
}