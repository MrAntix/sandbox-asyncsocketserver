using System.Net;
using System.Net.Sockets;
using System.Threading;
using Moq;
using Sandbox.AsyncSocketServer.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class listener_test
    {
        public static ManualResetEvent AllDone = new ManualResetEvent(false);

        [Fact]
        public void listener_awaits()
        {
            AllDone.Reset();

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[1];
            const int port = 8088;

            var listener = new Listener(
                new ListenerSettings(ipAddress, port),
                s => Mock.Of<IDataSocket>());
            IDataSocket socket = null;

            listener.AcceptAsync()
                    .ContinueWith(t =>
                        {
                            socket = t.Result;

                            AllDone.Set();
                        });

            Assert.Null(socket);

            // Create a socket and connect
            using (var client = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.ConnectAsync(
                    new SocketAsyncEventArgs
                        {
                            RemoteEndPoint = new IPEndPoint(ipAddress, port)
                        });
            }

            AllDone.WaitOne();

            Assert.NotNull(socket);
        }
    }
}