using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Antix.Testing;
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
                    Encoding.ASCII.GetBytes(DataToSend));

                // see what we get
                var result = await clientServer.Server.ReceiveAsync();
                var actual = Encoding.ASCII.GetString(result);

                Assert.Equal(DataToSend, actual);
            }
        }

        [Fact]
        public async Task client_sends_to_server()
        {
            var clientServer = CreateClientServer();

            clientServer.Client.Send(
                Encoding.ASCII.GetBytes(DataToSend));

            // see what we get
            var result = await clientServer.Server.ReceiveAsync();
            var actual = Encoding.ASCII.GetString(result);

            Assert.Equal(DataToSend, actual);
        }

        [Fact]
        public async Task client_sends_to_server_large_amount_of_data()
        {
            var clientServer = CreateClientServer();
            using (clientServer.Client)
            {
                var expected = TestData.Text
                    .WithLetters()
                    .WithRange(1000000, 1000000)
                    .Build();

                clientServer.Client.Send(
                    Encoding.ASCII.GetBytes(expected));
                clientServer.Client.Close();

                // a process loop, break when the result is null
                var result = new List<byte>();
                byte[] chunk;
                while ((chunk = await clientServer.Server.ReceiveAsync()) != null)
                {
                    result.AddRange(chunk);
                }

                var actual = Encoding.ASCII.GetString(result.ToArray());

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
                        Encoding.ASCII.GetBytes(expected));

                    // see what we get
                    var result = await clientServer.Server.ReceiveAsync();
                    var actual = Encoding.ASCII.GetString(result);

                    Assert.Equal(expected, actual);
                }
            }
        }
    }
}