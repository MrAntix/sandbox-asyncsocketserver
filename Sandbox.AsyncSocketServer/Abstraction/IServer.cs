using System;
using System.Collections.Generic;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IServer : IDisposable
    {
        void Add(ServerProcess process);
        void Remove(ServerProcess process);

        IEnumerable<ServerProcess> Processes { get; }

        Action<ServerProcess, Exception> NotifyException { get; set; }
        Action<ServerProcess, string> NotifyLog { get; set; }
    }
}