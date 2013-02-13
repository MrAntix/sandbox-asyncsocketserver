using System;
using System.Collections.Concurrent;
using System.Linq;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Sockets
{
    public class WorkerManager : IWorkerManager
    {
        readonly IBufferManager _bufferManager;
        readonly ConcurrentStack<Worker> _workerPool;

        public WorkerManager(
            IBufferManager bufferManager,
            TimeSpan timeout)
        {
            _bufferManager = bufferManager;

            // create worker pool
            _workerPool = new ConcurrentStack<Worker>(
                Enumerable.Range(0, bufferManager.MaximumAllocations)
                          .Select(i => new Worker(timeout)));
        }

        public IWorker Get(IWorkerSocket socket)
        {
            Worker worker;
            if (!_workerPool.TryPop(out worker))
                throw new WorkerFactoryException();

            var bufferAllocation = _bufferManager.Allocate();

            worker.Set(
                socket, bufferAllocation,
                () =>
                    {
                        _workerPool.Push(worker);
                        _bufferManager.Deallocate(bufferAllocation);
                    });

            return worker;
        }
    }
}