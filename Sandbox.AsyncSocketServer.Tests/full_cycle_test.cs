using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Sandbox.AsyncSocketServer.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class full_cycle_test
    {
        public static ManualResetEvent AllDone = new ManualResetEvent(false);

        [Fact]
        public async Task data_receieved()
        {
            AllDone.Reset();

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[1];
            const int port = 8088;

            var bufferManager = new BufferManager(1, 1024);
            var dataSocketFactory = new DataSocketFactory(bufferManager);


            var listener = new Listener(
                new ListenerSettings(ipAddress, port),
                dataSocketFactory.Create);

            IDataSocket socket = null;
            const string expected = "Hello World";

            listener.AcceptAsync()
                    .ContinueWith(t =>
                        {
                            socket = t.Result;
                            AllDone.Set();
                        });

            // Create a socket and connect
            using (var client = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.ConnectAsync(
                    new SocketAsyncEventArgs
                        {
                            RemoteEndPoint = new IPEndPoint(ipAddress, port)
                        });

                client.Send(
                    Encoding.ASCII.GetBytes(expected));
            }

            AllDone.WaitOne();

            var result = await socket.ReceiveAsync();
            var actual = Encoding.ASCII.GetString(result);

            Assert.Equal(expected, actual);
        }
    }
}