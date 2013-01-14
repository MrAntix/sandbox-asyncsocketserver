using System;
using System.Runtime.Serialization;

namespace Sandbox.AsyncSocketServer
{
    [Serializable]
    public class DataSocketFactoryException : Exception
    {
        public DataSocketFactoryException()
        {
        }

        protected DataSocketFactoryException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}