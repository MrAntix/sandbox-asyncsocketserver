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
        public async Task data_receieved()
        {
            var clientServer = CreateClient();
            using (clientServer.Client)
            {
                clientServer.Client.Send(
                    Encoding.ASCII.GetBytes(DataToSend + Settings.Terminator));

                // see what we get
                var result = await clientServer.Server.ReceiveAsync();
                var actual = Encoding.ASCII.GetString(result);

                Assert.Equal(DataToSend, actual);
            }
        }

        [Fact]
        public async Task data_receieved_client_left_connected()
        {
            var clientServer = CreateClient();

            clientServer.Client.Send(
                Encoding.ASCII.GetBytes(DataToSend + Settings.Terminator));

            // see what we get
            var result = await clientServer.Server.ReceiveAsync();
            var actual = Encoding.ASCII.GetString(result);

            Assert.Equal(DataToSend, actual);
        }

        [Fact]
        public async Task data_sent()
        {
            using (var clientServer = CreateClient())
            {
                await clientServer.Server.SendAsync(
                    Encoding.ASCII.GetBytes(DataToSend));

                var buffer = new byte[1024];
                var bytesReceived = clientServer.Client.Receive(buffer);

                var actual = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                Assert.Equal(DataToSend, actual);
            }
        }
    }
}