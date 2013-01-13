using System.Net;

namespace Sandbox.AsyncSocketServer
{
    public class ListenerSettings
    {
        readonly IPAddress _ipAddress;
        readonly int _port;
        readonly int _backlog;

        public ListenerSettings(
            IPAddress ipAddress, int port, 
            int backlog = 100)
        {
            _ipAddress = ipAddress;
            _port = port;
            _backlog = backlog;
        }

        public IPAddress IPAddress
        {
            get { return _ipAddress; }
        }

        public int Port
        {
            get { return _port; }
        }

        public int Backlog
        {
            get { return _backlog; }
        }
    }
}