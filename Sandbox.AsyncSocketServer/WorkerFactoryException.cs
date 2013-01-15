using System;
using System.Runtime.Serialization;

namespace Sandbox.AsyncSocketServer
{
    [Serializable]
    public class WorkerFactoryException : Exception
    {
        public WorkerFactoryException()
        {
        }

        protected WorkerFactoryException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}