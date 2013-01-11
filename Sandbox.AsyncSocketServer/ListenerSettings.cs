using System.Net;

namespace Sandbox.AsyncSocketServer
{
    public class ListenerSettings
    {
        readonly IPAddress _ipAddress;
        readonly int _port;

        public ListenerSettings(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public IPAddress IPAddress
        {
            get { return _ipAddress; }
        }

        public int Port
        {
            get { return _port; }
        }
    }
}