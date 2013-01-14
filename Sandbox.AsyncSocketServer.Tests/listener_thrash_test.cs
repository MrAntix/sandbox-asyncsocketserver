using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class listener_thrash_test : listener_test_base
    {
        [Fact]
        public async Task open_clients_to_max()
        {
            var clients =
                Enumerable
                    .Range(0, MaxBuffers)
                    .Select(async i => await CreateClient())
                    .Select(t => t.Result);

            var message = Encoding.ASCII.GetBytes("Hello World" + Terminator);

            var totalBytes = clients.Sum(c => c.Send(message));

            Assert.Equal(message.Length*MaxBuffers, totalBytes);
        }

        [Fact]
        public async Task open_clients_over_max()
        {
            for (var i = 0; i < MaxBuffers; i++)
            {
                await CreateClient();
            }
            try
            {
                await CreateClient();
                Assert.True(false);
            }
            catch (Exception ex)
            {
                Assert.IsType<DataSocketFactoryException>(ex);
            }
        }
    }
}