using System.Net.Sockets;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IDataSocketFactory
    {
        IDataSocket Create(Socket socket);
    }
}