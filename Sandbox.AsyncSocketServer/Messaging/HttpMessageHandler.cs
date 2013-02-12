using System.Text;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer.Messaging
{
    public class HttpMessageHandler :
        IMessageHandler
    {
        readonly HttpMessage _message;
        readonly ILogger _logger;

        public HttpMessageHandler(
            ILogger logger)
        {
            _logger = logger;
            _message = new HttpMessage();
        }

        public async Task<byte[]> ProcessAsync(byte[] request)
        {
            _logger.Diagnostic(
                this, () => string.Format("Processing: {0}", Encoding.ASCII.GetString(request)));

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

                _logger.Diagnostic(
                    this, () => string.Format("Found Header: {0}", responseString));

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