using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Messaging
{
    public interface IHttpMessage
    {
        Task WriteAsync(byte[] data);

        bool HasHeader { get; }
    }
}