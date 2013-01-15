using System.Net.Sockets;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IWorkerFactory
    {
        IWorker Create(Socket socket);
    }
}