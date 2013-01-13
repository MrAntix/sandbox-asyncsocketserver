﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class full_cycle_test : IDisposable
    {
        public static ManualResetEvent AllDone = new ManualResetEvent(false);
        readonly Socket _client;
        IDataSocket _server;
        static int _port = 8089;

        const string DataToSend = "Hello World";
        const string Terminator = "\n\n";

        public full_cycle_test()
        {
            Trace.WriteLine("Created Socket");

            AllDone.Reset();

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[1];
            var port = _port++;

            var bufferManager = new BufferManager(1, 1024);
            var dataSocketFactory = new DataSocketFactory(bufferManager, Terminator);

            var listener = new Listener(
                new ListenerSettings(ipAddress, port),
                dataSocketFactory.Create);

            // set the socket on continue to allow connection to be made below
            listener.AcceptAsync()
                    .ContinueWith(t =>
                        {
                            _server = t.Result;
                            AllDone.Set();
                        });

            // Create a socket and connect and send data
            _client = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _client.ConnectAsync(
                new SocketAsyncEventArgs
                    {
                        RemoteEndPoint = new IPEndPoint(ipAddress, port)
                    });

            // wait will connection is made
            AllDone.WaitOne();
        }

        [Fact]
        public async Task data_receieved()
        {
            _client.Send(
                Encoding.ASCII.GetBytes(DataToSend + Terminator));

            // see what we get
            var result = await _server.ReceiveAsync();
            var actual = Encoding.ASCII.GetString(result);

            Assert.Equal(DataToSend, actual);
        }

        [Fact]
        public async Task data_sent()
        {
            await _server.SendAsync(
                Encoding.ASCII.GetBytes(DataToSend));

            var buffer = new byte[1024];
            var bytesReceived = _client.Receive(buffer);

            var actual = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

            Assert.Equal(DataToSend, actual);
        }

        public void Dispose()
        {
            _server.Dispose();
            try
            {
                _client.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
            }
            _client.Close();
        }
    }
}