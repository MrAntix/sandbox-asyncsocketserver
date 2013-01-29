using System.Collections.Concurrent;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Buffering
{
    public class BufferManager : IBufferManager
    {
        readonly byte[] _buffers;

        readonly int _allocatedBufferSize;
        readonly int _maximumAllocations;

        int _allocationIndex;
        readonly ConcurrentStack<BufferAllocation> _allocationPool;

        public BufferManager(int maximumAllocations, int allocatedBufferSize)
        {
            _buffers = new byte[maximumAllocations*allocatedBufferSize];

            _maximumAllocations = maximumAllocations;
            _allocatedBufferSize = allocatedBufferSize;

            _allocationIndex = 0;
            _allocationPool = new ConcurrentStack<BufferAllocation>();
        }

        public int MaximumAllocations
        {
            get { return _maximumAllocations; }
        }

        public int AllocatedBufferSize
        {
            get { return _allocatedBufferSize; }
        }

        public BufferAllocation Allocate()
        {
            BufferAllocation allocation;

            if (!_allocationPool.TryPop(out allocation))
            {
                // create allocation upto the max allowed
                if (_allocationIndex == MaximumAllocations)
                    throw new BufferMaximumAllocationsExceededException();

                allocation = new BufferAllocation(
                    _buffers,
                    _allocationIndex*AllocatedBufferSize,
                    AllocatedBufferSize);

                _allocationIndex++;

                return allocation;
            }

            return allocation;
        }

        public void Deallocate(BufferAllocation allocation)
        {
            _allocationPool.Push(allocation);
        }
    }
}