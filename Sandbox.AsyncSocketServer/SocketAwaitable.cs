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
        Exception _exception;
        readonly TimeSpan _timeout;
        Timer _timer;

        public SocketAwaitable(TimeSpan timeout)
        {
            _timeout = timeout;

            EventArgs = new SocketAsyncEventArgs();
            EventArgs.Completed += delegate
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

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
            _timer = null;
        }

        public SocketAwaitable GetAwaiter()
        {
            if (IsCompleted
                || _timeout == Timeout.InfiniteTimeSpan) return this;

            if (_timeout.Ticks == 0)
            {
                _exception = new TimeoutException();
                IsCompleted = true;
            }

            _timer = new Timer(s =>
                {
                    var awaitable = (SocketAwaitable) s;
                    awaitable._exception = new TimeoutException();
                    awaitable.IsCompleted = true;
                }, this, _timeout, Timeout.InfiniteTimeSpan);

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
            if (_exception != null) throw _exception;

            if (EventArgs.SocketError != SocketError.Success)
                throw new SocketException((int) EventArgs.SocketError);
        }
    }
}