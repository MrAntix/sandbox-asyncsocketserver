namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IWorkerManager
    {
        IWorker Get(IWorkerSocket socket);
    }
}