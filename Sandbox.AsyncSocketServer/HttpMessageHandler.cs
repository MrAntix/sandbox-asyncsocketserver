using System;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class HttpMessageHandler :
        IMessageHandler
    {
        public Task<byte[]> ProcessAsync(byte[] request)
        {
            throw new NotImplementedException();
        }

        public string Terminator
        {
            get { return "\n\n"; }
        }
    }
}