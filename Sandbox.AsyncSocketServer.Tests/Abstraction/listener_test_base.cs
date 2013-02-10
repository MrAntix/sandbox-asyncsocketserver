using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Sandbox.AsyncSocketServer.Abstraction;
using Sandbox.AsyncSocketServer.Buffering;
using Sandbox.AsyncSocketServer.Sockets;

namespace Sandbox.AsyncSocketServer.Tests.Abstraction
{
    public abstract class listener_test_base : IDisposable
    {
        protected class TestSettings
        {
            public int MaxConnections = 1;
            public int BufferSize = 1024;

            public TimeSpan Timeout = TimeSpan.FromSeconds(2);
        }

        protected readonly TestSettings Settings;

        static IPAddress _ipAddress;
        static int _port = 8088;

        IListener _listener;
        readonly IBufferManager _manager;

        protected listener_test_base(
            TestSettings settings = null)
        {
            Settings = settings ?? new TestSettings();

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            _ipAddress = ipHostInfo.AddressList[1];

            _manager = new BufferManager(Settings.MaxConnections, Settings.BufferSize);
            var workerFactory = new WorkerManager(_manager, Settings.Timeout);

            _listener = new Listener(
                new ListenerSettings(_ipAddress, ++_port),
                s => workerFactory.Get(new WorkerSocket(s)));
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
                    ServerWorker = acceptTask.Result
                };
        }

        protected class ClientServer : IDisposable
        {
            public Socket Client;
            public IWorker ServerWorker;

            #region dispose

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed) return;

                if (disposing)
                {
                    ServerWorker.Close();
                    Client.Shutdown(SocketShutdown.Both);
                    Client.Dispose();
                }

                _disposed = true;
            }

            ~ClientServer()
            {
                Dispose(false);
            }

            bool _disposed;

            #endregion
        }

        #region dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_listener != null)
                {
                    _listener.Dispose();
                    _listener = null;
                }
            }

            _disposed = true;
        }

        ~listener_test_base()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion
    }
}