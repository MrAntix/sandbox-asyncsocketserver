using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class DataSocket : IDataSocket
    {
        readonly Socket _socket;
        readonly SocketAwaitable _awaitable;
        readonly string _terminator;
        readonly Action _release;

        public DataSocket(
            Socket socket, SocketAwaitable awaitable,
            string terminator,
            Action release)
        {
            _socket = socket;
            _awaitable = awaitable;
            _release = release;
            _terminator = terminator;
        }

        public async Task<byte[]> ReceiveAsync()
        {
            var data = new StringBuilder();

            while (true)
            {
                _awaitable.Reset();
                if (!_socket.ReceiveAsync(_awaitable.EventArgs))
                    _awaitable.IsCompleted = true;

                await _awaitable;

                var bytesReceived = _awaitable.EventArgs.BytesTransferred;
                if (bytesReceived <= 0) break;

                var received = Encoding.ASCII.GetString(
                    _awaitable.EventArgs.Buffer,
                    _awaitable.EventArgs.Offset, bytesReceived);

                data.Append(received);

                var terminatorIndex = GetTerminatorIndex(data);
                if (terminatorIndex > -1)
                {
                    // terminator found, return all data up to it
                    return Encoding.ASCII.GetBytes(data.ToString(0, terminatorIndex));
                }
            }

            // client closed connection
            return Encoding.ASCII.GetBytes(data.ToString());
        }

        public async Task SendAsync(byte[] data)
        {
            _awaitable.Reset();
            _awaitable.EventArgs.SetBuffer(data, 0, data.Length);

            if (!_socket.SendAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;
        }

        int GetTerminatorIndex(StringBuilder data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            for (var dataIndex = 0; dataIndex < data.Length; dataIndex++)
            {
                if (Enumerable
                    .Range(0, _terminator.Length)
                    .All(
                        terminatorIndex =>
                        dataIndex + terminatorIndex < data.Length
                        && data[dataIndex + terminatorIndex] == _terminator[terminatorIndex]))
                    return dataIndex;
            }

            return -1;
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
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                }
                _socket.Close();
            }

            _release();
            _disposed = true;
        }

        ~DataSocket()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion
    }
}