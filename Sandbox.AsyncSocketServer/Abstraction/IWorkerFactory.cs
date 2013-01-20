namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IWorkerFactory
    {
        IWorker Create(IWorkerSocket socket);
    }
}