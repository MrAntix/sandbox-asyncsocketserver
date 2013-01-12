﻿using System;
using System.Runtime.Serialization;

namespace Sandbox.AsyncSocketServer
{
    [Serializable]
    public class BufferMaximumAllocationsExceededException : Exception
    {
        public BufferMaximumAllocationsExceededException()
        {
        }

        protected BufferMaximumAllocationsExceededException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}