using System;
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
        readonly Action _release;

        public DataSocket(
            Socket socket, SocketAwaitable awaitable,
            Action release)
        {
            _socket = socket;
            _awaitable = awaitable;
            _release = release;
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

                data.Append(
                    Encoding.ASCII.GetString(
                        _awaitable.EventArgs.Buffer,
                        _awaitable.EventArgs.Offset, bytesReceived));
            }

            return Encoding.ASCII.GetBytes(data.ToString());
        }

        public Task SendAsync(byte[] data)
        {
            throw new NotImplementedException();
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