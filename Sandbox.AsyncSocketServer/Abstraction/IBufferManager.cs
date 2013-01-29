using Sandbox.AsyncSocketServer.Buffering;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IBufferManager
    {
        BufferAllocation Allocate();
        void Deallocate(BufferAllocation allocation);

        int MaximumAllocations { get; }
        int AllocatedBufferSize { get; }
    }
}