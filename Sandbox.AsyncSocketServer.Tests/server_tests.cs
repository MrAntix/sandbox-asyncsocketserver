using System.Linq;
using Moq;
using Sandbox.AsyncSocketServer.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class server_tests
    {
        [Fact]
        public void add_process_sets_server()
        {
            using (var server = GetServer())
            {
                var process = new ServerProcess(GetListener(), GetHandler, null);
                server.Add(process);

                Assert.Equal(server, process.Server);
            }
        }

        [Fact]
        public void remove_process_sets_server_to_null()
        {
            using (var server = GetServer())
            {
                var process = new ServerProcess(GetListener(), GetHandler, null);
                server.Add(process);

                server.Remove(process);
                Assert.Null(process.Server);
            }
        }

        [Fact]
        public void disposed_process_is_removed()
        {
            using (var server = GetServer())
            {
                server.Add(new ServerProcess(GetListener(), GetHandler, null));

                var process = new ServerProcess(GetListener(), GetHandler, null);
                server.Add(process);

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

            server.Add(new ServerProcess(GetListener(), GetHandler, null));
            server.Add(new ServerProcess(GetListener(), GetHandler, null));

            Assert.Equal(2, server.Processes.Count());

            server.Dispose();

            Assert.Equal(0, server.Processes.Count());
        }

        static IServer GetServer()
        {
            return new Server(null);
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