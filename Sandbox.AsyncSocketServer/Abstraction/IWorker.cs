using System;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IWorker : IDisposable
    {
        Task<byte[]> ReceiveAsync(string terminator);
        Task SendAsync(byte[] data);
        void Close();

        bool Disposed { get; }
    }
}