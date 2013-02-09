using System.Text;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Messaging
{
    public class HttpMessageHandler :
        IMessageHandler
    {
        readonly HttpMessage _message;

        public HttpMessageHandler()
        {
            _message = new HttpMessage();
        }

        public async Task<byte[]> ProcessAsync(byte[] request)
        {
            await _message.WriteAsync(request);

            if (_message.HasHeader)
            {
                var responseString =
                    string.Concat(
                        "HTTP/1.1 200", Terminator,
                        "<pre>",
                        Encoding.UTF8.GetString(request),
                        "</pre>"
                        );

                return Encoding.UTF8.GetBytes(responseString);
            }

            return null;
        }

        public string Terminator
        {
            get { return "\r\n\r\n"; }
        }
    }
}