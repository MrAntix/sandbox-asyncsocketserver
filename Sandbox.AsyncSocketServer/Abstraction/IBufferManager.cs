namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface IBufferManager
    {
        BufferAllocation Allocate();
        void Deallocate(BufferAllocation allocation);
    }
}