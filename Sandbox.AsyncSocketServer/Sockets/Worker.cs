using System;
using System.Linq;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;
using Sandbox.AsyncSocketServer.Buffering;

namespace Sandbox.AsyncSocketServer.Sockets
{
    public class Worker : IWorker
    {
        readonly ILogger _logger;
        readonly SocketAwaitable _awaitable;
        IWorkerSocket _socket;
        Action _release;

        public Worker(TimeSpan timeout, ILogger logger)
        {
            _logger = logger;
            _awaitable = new SocketAwaitable(timeout);
        }

        public void Set(
            IWorkerSocket socket, BufferAllocation bufferAllocation,
            Action release)
        {
            _socket = socket;
            _release = release;

            _awaitable.EventArgs
                      .SetBuffer(
                          bufferAllocation.Buffer,
                          bufferAllocation.Offset, bufferAllocation.Size);

            Closed = false;
        }

        public async Task<byte[]> ReceiveAsync()
        {
            _logger.Diagnostic(this, () => "Receiving");

            _awaitable.Reset();

            if (!_socket.ReceiveAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;

            if (_awaitable.IsTimedOut)
            {
                _logger.Error(this, () => "Timed out");

                return null;
            }

            // check the number of bytes recieved, 
            // if 0, the remote end has closed the connection
            var bytesReceived = _awaitable.EventArgs.BytesTransferred;
            _logger.Diagnostic(this, () => string.Format("Received: {0}", bytesReceived));

            if (bytesReceived == 0) return null;

            return _awaitable
                .EventArgs
                .Buffer
                .Skip(_awaitable.EventArgs.Offset).Take(bytesReceived)
                .ToArray();
        }

        public async Task SendAsync(byte[] data)
        {
            _logger.Diagnostic(this, () => string.Format("Sending: {0}", data.Length));

            _awaitable.Reset();
            _awaitable.EventArgs.SetBuffer(data, 0, data.Length);

            if (!_socket.SendAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;

            _logger.Diagnostic(this, () => string.Format("Sent: {0}", data.Length));
        }

        public void Close()
        {
            if (_socket == null) return;

            _socket.Dispose();
            _socket = null;

            Closed = true;

            _release();

            _logger.Diagnostic(this, () => "Worker Closed");
        }

        public bool Closed { get; private set; }
    }
}