using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Messaging
{
    public interface IMessage
    {
        Task WriteAsync(byte[] data);
    }

    public class Message : IMessage
    {
        readonly List<byte> _data;
        int _headerIndex;

        public Message()
        {
            _data = new List<byte>();
            _headerIndex = -1;
        }

        public async Task WriteAsync(byte[] data)
        {
            _data.AddRange(data);
        }


    }
}