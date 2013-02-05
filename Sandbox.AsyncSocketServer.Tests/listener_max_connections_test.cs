using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.AsyncSocketServer.Sockets;
using Sandbox.AsyncSocketServer.Tests.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class listener_max_connections_test : listener_test_base
    {
        public listener_max_connections_test() :
            base(new TestSettings
                {
                    MaxConnections = 5000, // ephemeral ports http://support.microsoft.com/kb/196271
                    BufferSize = 2
                })
        {
        }

        [Fact]
        public void open_clients_to_max()
        {
            var clientServers = new List<ClientServer>();

            for (var i = 0; i < Settings.MaxConnections; i++)
                clientServers.Add(CreateClientServer());

            var message = Encoding.ASCII.GetBytes("Hello World");

            var totalBytes = clientServers.Sum(c => c.Client.Send(message));

            Assert.Equal(message.Length*Settings.MaxConnections, totalBytes);
        }

        [Fact]
        public void open_clients_over_max()
        {
            var clientServers = new List<ClientServer>();

            for (var i = 0; i < Settings.MaxConnections; i++)
                clientServers.Add(CreateClientServer());

            try
            {
                CreateClientServer();

                throw new Exception("expected exception");
            }
            catch (Exception ex)
            {
                Assert.IsType<WorkerFactoryException>(ex);
            }
        }
    }
}