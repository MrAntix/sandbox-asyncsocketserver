using System.Linq;
using Moq;
using Sandbox.AsyncSocketServer.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class server_tests
    {
        [Fact]
        public void disposed_process_is_removed()
        {
            using (var server = GetServer())
            {
                server.Start(GetListener(), GetHandler());

                var process = server
                    .Start(GetListener(), GetHandler());

                Assert.Equal(2, server.Processes.Count());

                process.Dispose();

                Assert.Equal(1, server.Processes.Count());
                Assert.DoesNotContain(process, server.Processes);
            }
        }

        [Fact]
        public void all_processes_disposed_with_server()
        {
            var server = GetServer();

            server.Start(GetListener(), GetHandler());
            server.Start(GetListener(), GetHandler());

            Assert.Equal(2, server.Processes.Count());

            server.Dispose();

            Assert.Equal(0, server.Processes.Count());
        }

        static IServer GetServer()
        {
            return new Server();
        }

        static IListener GetListener()
        {
            var mock = new Mock<IListener>();

            return mock.Object;
        }

        static IMessageHandler GetHandler()
        {
            var mock = new Mock<IMessageHandler>();

            return mock.Object;
        }
    }
}