using System;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IListener : IDisposable
    {
        Task<IDataSocket> AcceptAsync();
    }
}