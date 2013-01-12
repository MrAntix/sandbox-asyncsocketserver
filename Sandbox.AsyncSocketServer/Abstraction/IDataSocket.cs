using System;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IDataSocket : IDisposable
    {
        Task<byte[]> ReceiveAsync();

        Task SendAsync(byte[] data);
    }
}