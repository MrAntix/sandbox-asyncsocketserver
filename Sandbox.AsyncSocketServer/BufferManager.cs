using System.Collections.Generic;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class BufferManager : IBufferManager
    {
        readonly byte[] _buffers;

        readonly int _allocatedBufferSize;
        readonly int _maximumAllocations;

        int _allocationIndex;
        readonly Stack<BufferAllocation> _allocationPool;

        public BufferManager(int maximumAllocations, int allocatedBufferSize)
        {
            _buffers = new byte[maximumAllocations*allocatedBufferSize];

            _maximumAllocations = maximumAllocations;
            _allocatedBufferSize = allocatedBufferSize;

            _allocationIndex = 0;
            _allocationPool = new Stack<BufferAllocation>();
        }


        public BufferAllocation Allocate()
        {
            if (_allocationPool.Count == 0)
            {
                // create allocation upto the max allowed
                if (_allocationIndex == _maximumAllocations)
                    throw new BufferMaximumAllocationsExceededException();

                var allocation = new BufferAllocation(
                    _buffers,
                    _allocationIndex*_allocatedBufferSize,
                    _allocatedBufferSize);

                _allocationIndex++;

                return allocation;
            }

            // reuse previously created
            return _allocationPool.Pop();
        }

        public void Deallocate(BufferAllocation allocation)
        {
            _allocationPool.Push(allocation);
        }
    }
}