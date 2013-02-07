using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IWorker
    {
        Task<byte[]> ReceiveAsync();
        Task SendAsync(byte[] data);
        void Close();
        bool Closed { get; }
    }
}