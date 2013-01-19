using System;
using System.Net;
using System.Net.Sockets;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IListenerSocket:IDisposable
    {
        void Listen(IPEndPoint endpoint, int backlog);
        bool AcceptAsync(SocketAsyncEventArgs eventArgs);
    }

    public class ListenerSocket : IListenerSocket
    {
        Socket _socket;

        public void Listen(IPEndPoint endpoint, int backlog)
        {
            // demand permission
            var permission = new SocketPermission(
                NetworkAccess.Connect, TransportType.Tcp,
                endpoint.Address.ToString(), endpoint.Port);
            permission.Demand();

            // create a tcp socket to listen
            _socket = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(endpoint);
            _socket.Listen(backlog);
        }

        public bool AcceptAsync(SocketAsyncEventArgs eventArgs)
        {
            return _socket.AcceptAsync(eventArgs);
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
                    _socket = null;
                }
            }

            _disposed = true;
        }

        ~ListenerSocket()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion

    }
}