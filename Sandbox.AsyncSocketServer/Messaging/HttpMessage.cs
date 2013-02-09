using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Messaging
{
    public class HttpMessage
    {
        const string Terminator = "\r\n\r\n";
        readonly List<byte> _data;
        int _headerIndex;

        public HttpMessage()
        {
            _data = new List<byte>();
            _headerIndex = -1;
        }

        public async Task WriteAsync(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");

            if (data.Length == 0) return;

            // get the place to start looking for the terminator
            var terminatorIndexStart = _data.Count > Terminator.Length
                                           ? _data.Count - Terminator.Length
                                           : 0;

            // add data
            _data.AddRange(data);

            if (_headerIndex == -1)
                // look for a header
                _headerIndex = GetTerminatorIndex(data, terminatorIndexStart);
        }

        public bool HasHeader
        {
            get { return _headerIndex > -1; }
        }

        public static int GetTerminatorIndex(
            IReadOnlyList<byte> data, int startIndex)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            for (var dataIndex = startIndex; dataIndex < data.Count; dataIndex++)
            {
                if (Enumerable
                    .Range(0, Terminator.Length)
                    .All(
                        terminatorIndex =>
                        dataIndex + terminatorIndex < data.Count
                        && data[dataIndex + terminatorIndex] == Terminator[terminatorIndex]))
                    return dataIndex;
            }

            return -1;
        }
    }
}