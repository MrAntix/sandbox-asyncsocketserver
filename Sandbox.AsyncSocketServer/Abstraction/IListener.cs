using System;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IListener : IDisposable
    {
        void Start();
        Task<IWorker> AcceptAsync();
        void Stop();
    }
}