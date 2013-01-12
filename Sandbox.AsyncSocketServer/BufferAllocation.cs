namespace Sandbox.AsyncSocketServer
{
    public class BufferAllocation
    {
        readonly byte[] _buffer;
        readonly int _offset;
        readonly int _size;

        public BufferAllocation(byte[] buffer, int offset, int size)
        {
            _buffer = buffer;
            _offset = offset;
            _size = size;
        }

        public byte[] Buffer
        {
            get { return _buffer; }
        }

        public int Offset
        {
            get { return _offset; }
        }

        public int Size
        {
            get { return _size; }
        }
    }
}