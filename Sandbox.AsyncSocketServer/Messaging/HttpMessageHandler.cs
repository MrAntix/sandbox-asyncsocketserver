using System;
using System.Text;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Messaging
{
    public class HttpMessageHandler :
        IMessageHandler
    {
        public async Task<byte[]> ProcessAsync(byte[] request)
        {
            var requestString = Encoding.UTF8.GetString(request);

            var responseString = "HTTP/1.1 200 Hiya\r\n\r\nHello World\r\n\r\n";

            return Encoding.UTF8.GetBytes(responseString);
        }

        public string Terminator
        {
            get { return "\r\n\r\n"; }
        }
    }
}