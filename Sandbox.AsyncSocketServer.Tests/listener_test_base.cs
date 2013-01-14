using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class listener_test_base : IDisposable
    {
        protected const string Terminator = "\n\n";
        protected const int MaxBuffers = 100;

        static IPAddress _ipAddress;
        static int _port = 8088;

        IListener _listener;

        protected listener_test_base()
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            _ipAddress = ipHostInfo.AddressList[1];

            var bufferManager = new BufferManager(MaxBuffers, 1024);
            var dataSocketFactory = new DataSocketFactory(bufferManager, Terminator);

            _listener = new Listener(
                new ListenerSettings(_ipAddress, ++_port),
                dataSocketFactory.Create);
        }

        protected Socket CreateClient(out IDataSocket serverSocket)
        {
            IDataSocket outServerSocket = null;
            _listener.AcceptAsync()
                     .ContinueWith(t =>
                         {
                             if (t.Exception != null) throw t.Exception;
                             outServerSocket = t.Result;
                         });

            // Create a socket and connect and send data
            var client = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(_ipAddress, _port));

            while (outServerSocket == null) Thread.Sleep(0);

            serverSocket = outServerSocket;
            return client;
        }

        protected async Task<Socket> CreateClient()
        {
            // Create a socket and connect and send data
            var client = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.Connect(new IPEndPoint(_ipAddress, _port));
            await _listener.AcceptAsync();

            return client;
        }

        public void Dispose()
        {
            _listener.Dispose();
            _listener = null;
        }
    }
}