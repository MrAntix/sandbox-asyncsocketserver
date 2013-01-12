using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class DataSocketFactory : IDataSocketFactory
    {
        readonly IBufferManager _bufferManager;
        readonly Stack<SocketAwaitable> _awaitablesPool;

        public DataSocketFactory(
            IBufferManager bufferManager)
        {
            _bufferManager = bufferManager;

            // create event arg pool
            _awaitablesPool = new Stack<SocketAwaitable>(
                Enumerable.Range(0, 1000)
                          .Select(i => new SocketAwaitable()));
        }

        public IDataSocket Create(Socket socket)
        {
            return new DataSocket(socket);
        }
    }
}