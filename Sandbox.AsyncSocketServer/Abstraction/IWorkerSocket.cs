using System;
using System.Net.Sockets;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IWorkerSocket : IDisposable
    {
        bool ReceiveAsync(SocketAsyncEventArgs eventArgs);
        bool SendAsync(SocketAsyncEventArgs eventArgs);
    }
}