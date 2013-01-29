using Sandbox.AsyncSocketServer.Buffering;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class buffer_manager_test
    {
        [Fact]
        public void allocation_is_created_when_available()
        {
            var sut = new BufferManager(1, 10);

            Assert.DoesNotThrow(() => sut.Allocate());
        }

        [Fact]
        public void exception_is_thrown_when_allocation_is_not_available()
        {
            var sut = new BufferManager(0, 10);

            Assert.Throws<BufferMaximumAllocationsExceededException>(() => sut.Allocate());
        }

        [Fact]
        public void deallocated_can_be_used_again()
        {
            var sut = new BufferManager(1, 10);

            var allocation = sut.Allocate();

            sut.Deallocate(allocation);

            var allocation2 = sut.Allocate();

            Assert.Same(allocation, allocation2);
        }
    }
}