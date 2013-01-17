using System;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IWorker : IDisposable
    {
        Task<byte[]> ReceiveAsync();

        Task SendAsync(byte[] data);

        bool Disposed { get; }
    }
}