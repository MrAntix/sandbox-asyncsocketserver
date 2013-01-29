using System;
using System.Collections.Concurrent;
using System.Linq;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Sockets
{
    public class WorkerFactory : IWorkerFactory
    {
        readonly IBufferManager _bufferManager;
        readonly ConcurrentStack<SocketAwaitable> _awaitablesPool;

        public WorkerFactory(
            IBufferManager bufferManager,
            TimeSpan timeout)
        {
            _bufferManager = bufferManager;

            // create event arg pool
            _awaitablesPool = new ConcurrentStack<SocketAwaitable>(
                Enumerable.Range(0, bufferManager.MaximumAllocations)
                          .Select(i => new SocketAwaitable(timeout)));
        }

        public IWorker Create(IWorkerSocket socket)
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
                () =>
                    {
                        _awaitablesPool.Push(awaitable);
                        _bufferManager.Deallocate(bufferAllocation);
                    });
        }
    }
}