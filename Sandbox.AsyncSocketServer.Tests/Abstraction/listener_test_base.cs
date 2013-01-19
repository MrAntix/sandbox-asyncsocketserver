using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Tests.Abstraction
{
    public abstract class listener_test_base : IDisposable
    {
        protected class TestSettings
        {
            public string Terminator = "\n\n";
            public int MaxConnections = 1;
            public int BufferSize = 1024;

            public TimeSpan Timeout = TimeSpan.FromSeconds(2);
        }

        protected readonly TestSettings Settings;

        static IPAddress _ipAddress;
        static int _port = 8088;

        readonly IListener _listener;
        readonly IBufferManager _manager;

        protected listener_test_base(
            TestSettings settings = null)
        {
            Settings = settings ?? new TestSettings();

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            _ipAddress = ipHostInfo.AddressList[1];

            _manager = new BufferManager(Settings.MaxConnections, Settings.BufferSize);
            var dataSocketFactory = new WorkerFactory(
                _manager, Settings.Terminator, Settings.Timeout);

            _listener = new Listener(
                new ListenerSettings(_ipAddress, ++_port),
                new ListenerSocket(),
                dataSocketFactory.Create);
        }

        protected ClientServer CreateClientServer()
        {
            var acceptTask = _listener.AcceptAsync();

            // Create a socket and connect and send data
            var client = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(_ipAddress, _port));

            while (!acceptTask.IsCompleted) Thread.Sleep(0);

            if (acceptTask.Exception != null)
                throw acceptTask.Exception.InnerException;

            return new ClientServer
                {
                    Client = client,
                    Server = acceptTask.Result
                };
        }

        protected class ClientServer : IDisposable
        {
            public Socket Client;
            public IWorker Server;

            public void Dispose()
            {
                Server.Dispose();
                Client.Shutdown(SocketShutdown.Both);
                Client.Dispose();
            }
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}