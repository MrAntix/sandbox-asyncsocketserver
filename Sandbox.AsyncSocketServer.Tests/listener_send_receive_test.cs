using System.Text;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class listener_send_receive_test : listener_test_base
    {
        const string DataToSend = "Hello World";

        [Fact]
        public async Task data_receieved()
        {
            IDataSocket serverSocket;

            using (var client = CreateClient(out serverSocket))
            {
                client.Send(
                    Encoding.ASCII.GetBytes(DataToSend + Terminator));

                // see what we get
                var result = await serverSocket.ReceiveAsync();
                var actual = Encoding.ASCII.GetString(result);

                Assert.Equal(DataToSend, actual);
            }
        }

        [Fact]
        public async Task data_receieved_client_left_connected()
        {
            IDataSocket serverSocket;

            var client = CreateClient(out serverSocket);
            client.Send(
                Encoding.ASCII.GetBytes(DataToSend + Terminator));

            // see what we get
            var result = await serverSocket.ReceiveAsync();
            var actual = Encoding.ASCII.GetString(result);

            Assert.Equal(DataToSend, actual);
        }

        [Fact]
        public async Task data_sent()
        {
            IDataSocket serverSocket;

            using (var client = CreateClient(out serverSocket))
            {
                await serverSocket.SendAsync(
                    Encoding.ASCII.GetBytes(DataToSend));

                var buffer = new byte[1024];
                var bytesReceived = client.Receive(buffer);

                var actual = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                Assert.Equal(DataToSend, actual);
            }
        }
    }
}