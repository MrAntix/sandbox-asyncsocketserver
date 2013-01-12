using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class DataSocket : IDataSocket
    {
        readonly Socket _socket;

        public DataSocket(Socket socket)
        {
            _socket = socket;
        }

        public Task<byte[]> ReceiveAsync()
        {
            throw new NotImplementedException();
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
                //if (MEMBER != null)
                //{
                //    MEMBER.Dispose();
                //    MEMBER = null;
                //}
            }

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