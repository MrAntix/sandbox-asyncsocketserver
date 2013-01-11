using System;
using System.Net;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer
{
    public interface IListener : IDisposable
    {
        Task<IDataSocket> AcceptAsync();
    }

    public interface IListeningSocket
    {
        void Start(IPAddress ipAddress, int port);
    }
}