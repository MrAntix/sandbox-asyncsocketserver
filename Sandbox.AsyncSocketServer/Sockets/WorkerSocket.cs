using System;
using System.Net.Sockets;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Sockets
{
    public class WorkerSocket : IWorkerSocket
    {
        Socket _socket;

        public WorkerSocket(Socket socket)
        {
            _socket = socket;
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
                    try
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception)
                    {
                    }
                    _socket.Close();
                    _socket = null;
                }
            }

            _disposed = true;
        }

        ~WorkerSocket()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion

        public bool ReceiveAsync(SocketAsyncEventArgs eventArgs)
        {
            return _socket.ReceiveAsync(eventArgs);
        }

        public bool SendAsync(SocketAsyncEventArgs eventArgs)
        {
            return _socket.SendAsync(eventArgs);
        }
    }
}