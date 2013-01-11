using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer
{
    public sealed class SocketAwaitable : INotifyCompletion
    {
        static readonly Action Sentinel = () => { };

        Action _continuation;

        public SocketAwaitable()
        {
            EventArgs = new SocketAsyncEventArgs();
            EventArgs.Completed += delegate
                {
                    var prev = _continuation
                               ?? Interlocked
                                      .CompareExchange(ref _continuation, Sentinel, null);

                    if (prev != null) prev();
                };
        }

        internal void Reset()
        {
            IsCompleted = false;
            _continuation = null;
        }

        public SocketAwaitable GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted { get; internal set; }
        public SocketAsyncEventArgs EventArgs { get; private set; }

        public void OnCompleted(Action continuation)
        {
            if (_continuation == Sentinel
                || Interlocked
                       .CompareExchange(ref _continuation, continuation, null) == Sentinel)
            {
                Task.Run(continuation);
            }
        }

        public void GetResult()
        {
            if (EventArgs.SocketError != SocketError.Success)
                throw new SocketException((int) EventArgs.SocketError);
        }
    }
}