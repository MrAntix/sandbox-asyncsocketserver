using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class WorkerFactory : IWorkerFactory
    {
        readonly IBufferManager _bufferManager;
        readonly ConcurrentStack<SocketAwaitable> _awaitablesPool;
        readonly string _terminator;

        public WorkerFactory(
            IBufferManager bufferManager,
            string terminator)
        {
            _bufferManager = bufferManager;
            _terminator = terminator;

            // create event arg pool
            _awaitablesPool = new ConcurrentStack<SocketAwaitable>(
                Enumerable.Range(0, bufferManager.MaximumAllocations)
                          .Select(i => new SocketAwaitable()));
        }

        public IWorker Create(Socket socket)
        {
            SocketAwaitable awaitable;
            if (!_awaitablesPool.TryPop(out awaitable))
                throw new WorkerFactoryException();

            var bufferAllocation = _bufferManager.Allocate();

            awaitable.EventArgs
                     .SetBuffer(bufferAllocation.Buffer,
                                bufferAllocation.Offset, bufferAllocation.Size);

            return new Worker(
                socket, awaitable,
                _terminator,
                () =>
                    {
                        _awaitablesPool.Push(awaitable);
                        _bufferManager.Deallocate(bufferAllocation);
                    });
        }
    }
}