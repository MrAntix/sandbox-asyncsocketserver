using System;
using System.Collections.Generic;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IServer : IDisposable
    {
        ServerProcess Start(IListener listener, IMessageHandler handler);
        void Stop(ServerProcess process);

        IEnumerable<ServerProcess> Processes { get; }
    }
}