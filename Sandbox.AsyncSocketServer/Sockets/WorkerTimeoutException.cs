using System;
using System.Runtime.Serialization;

namespace Sandbox.AsyncSocketServer.Sockets
{
    [Serializable]
    public class WorkerTimeoutException : Exception
    {
        public WorkerTimeoutException()
        {
        }

        protected WorkerTimeoutException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}