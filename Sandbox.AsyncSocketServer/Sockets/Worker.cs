using System;
using System.Collections.Generic;
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

        public async Task<byte[]> ReceiveAsync(string terminator)
        {
            var data = new List<byte>();

            while (!Disposed)
            {
                _awaitable.Reset();
                if (!_socket.ReceiveAsync(_awaitable.EventArgs))
                    _awaitable.IsCompleted = true;

                await _awaitable;

                if (_awaitable.IsTimedOut)
                {
                    Dispose();
                }
                else
                {
                    // get the place to start looking for the terminator
                    var terminatorIndexStart = data.Count > terminator.Length
                                                   ? data.Count - terminator.Length
                                                   : 0;

                    // check the number of bytes recieved
                    var bytesReceived = _awaitable.EventArgs.BytesTransferred;
                    if (bytesReceived <= 0) break;

                    // add the received data
                    data.AddRange(_awaitable.EventArgs
                                            .Buffer.Skip(_awaitable.EventArgs.Offset).Take(bytesReceived));

                    // look for a terminator
                    var terminatorIndex = GetTerminatorIndex(data, terminator, terminatorIndexStart);
                    if (terminatorIndex > -1)
                    {
                        // terminator found, return all data up to it
                        return data.Take(terminatorIndex).ToArray();
                    }
                }
            }

            // client closed connection
            return data.ToArray();
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

        int GetTerminatorIndex(IReadOnlyList<byte> data, string terminator, int startIndex)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            for (var dataIndex = startIndex; dataIndex < data.Count; dataIndex++)
            {
                if (Enumerable
                    .Range(0, terminator.Length)
                    .All(
                        terminatorIndex =>
                        dataIndex + terminatorIndex < data.Count
                        && data[dataIndex + terminatorIndex] == terminator[terminatorIndex]))
                    return dataIndex;
            }

            return -1;
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