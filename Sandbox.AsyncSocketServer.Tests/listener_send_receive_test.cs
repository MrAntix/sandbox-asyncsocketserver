using System;
using System.Text;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Tests.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class listener_send_receive_test : listener_test_base
    {
        const string DataToSend = "Hello World";

        public listener_send_receive_test() :
            base(new TestSettings
                {
                    MaxConnections = 1
                })
        {
        }

        [Fact]
        public async Task client_sends_to_server_and_connection_closed()
        {
            var clientServer = CreateClientServer();
            using (clientServer.Client)
            {
                clientServer.Client.Send(
                    Encoding.ASCII.GetBytes(DataToSend + Settings.Terminator));

                // see what we get
                var result = await clientServer.Server.ReceiveAsync(Settings.Terminator);
                var actual = Encoding.ASCII.GetString(result);

                Assert.Equal(DataToSend, actual);
            }
        }

        [Fact]
        public async Task client_sends_to_server_with_terminator()
        {
            var clientServer = CreateClientServer();

            clientServer.Client.Send(
                Encoding.ASCII.GetBytes(DataToSend + Settings.Terminator));

            // see what we get
            var result = await clientServer.Server.ReceiveAsync(Settings.Terminator);
            var actual = Encoding.ASCII.GetString(result);

            Assert.Equal(DataToSend, actual);
        }

        [Fact]
        public async Task client_sends_to_server_large_amount_of_data()
        {
            var clientServer = CreateClientServer();
            using (clientServer.Client)
            {
                var expected = new String('x', 10000000);

                clientServer.Client.Send(
                    Encoding.ASCII.GetBytes(expected + Settings.Terminator));

                // see what we get
                var result = await clientServer.Server.ReceiveAsync(Settings.Terminator);
                var actual = Encoding.ASCII.GetString(result);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async Task server_sends_to_client()
        {
            using (var clientServer = CreateClientServer())
            {
                await clientServer.Server.SendAsync(
                    Encoding.ASCII.GetBytes(DataToSend));

                var buffer = new byte[1024];
                var bytesReceived = clientServer.Client.Receive(buffer);

                var actual = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                Assert.Equal(DataToSend, actual);
            }
        }

        [Fact]
        public async Task client_sends_to_server_a_number_of_times_on_the_same_connection()
        {
            using (var clientServer = CreateClientServer())
            {
                for (var i = 0; i < 10000; i++)
                {
                    var expected = string.Concat(DataToSend, i);

                    clientServer.Client.Send(
                        Encoding.ASCII.GetBytes(expected + Settings.Terminator));

                    // see what we get
                    var result = await clientServer.Server.ReceiveAsync(Settings.Terminator);
                    var actual = Encoding.ASCII.GetString(result);

                    Assert.Equal(expected, actual);
                }
            }
        }
    }
}