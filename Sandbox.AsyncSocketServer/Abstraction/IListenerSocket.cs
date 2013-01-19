using System;
using System.Net;
using System.Net.Sockets;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IListenerSocket : IDisposable
    {
        void Listen(IPEndPoint endpoint, int backlog);
        bool AcceptAsync(SocketAsyncEventArgs eventArgs);
    }
}