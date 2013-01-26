using System;
using System.Collections.Generic;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IServer : IDisposable
    {
        void Add(ServerProcess process);
        void Remove(ServerProcess process);

        IEnumerable<ServerProcess> Processes { get; }
        void Error(ServerProcess process, Exception exception);
        Action<string> LogAction { get; set; }
    }
}