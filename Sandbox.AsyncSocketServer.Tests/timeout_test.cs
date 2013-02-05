using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Antix.Testing;
using Sandbox.AsyncSocketServer.Sockets;
using Sandbox.AsyncSocketServer.Tests.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class timeout_test : listener_test_base
    {
        const string DataToSend = "Hello World\n";

        public timeout_test() :
            base(new TestSettings
                {
                    MaxConnections = 1,
                    Timeout = TimeSpan.FromMilliseconds(5)
                })
        {
        }

        [Fact]
        public void client_times_out_if_no_communication_sent()
        {
            var clientServer = CreateClientServer();

            var server = clientServer.Server;
            new Thread(() =>
                {
                    while (!server.Disposed)
                    {
                        var result = server.ReceiveAsync().Result;
                        Debug.Write(Encoding.ASCII.GetString(result));
                    }
                }).Start();

            Assert.Throws<SocketException>(() =>
                {
                    var loop = 1;
                    while (loop++ < 20)
                    {
                        Thread.Sleep(loop);
                        clientServer.Client.Send(
                            Encoding.ASCII.GetBytes(DataToSend));
                    }
                });
        }

        [Fact]
        public async Task server_times_out_if_no_communication_sent()
        {
            var clientServer = CreateClientServer();
            using (clientServer.Client)
            {
                try
                {
                    await clientServer.Server.ReceiveAsync();
                    Assert.True(false, "(No exception was thrown)");
                }
                catch (Exception ex)
                {
                    Assert.IsType<WorkerTimeoutException>(ex);
                }
            }
        }
    }
}