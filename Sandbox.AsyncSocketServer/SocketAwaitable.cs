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

        public SocketAwaitable(TimeSpan timeout)
        {
            _timeout = timeout;
            if (_timeout.Ticks != 0
                || _timeout == Timeout.InfiniteTimeSpan)
            {
                _timer = new Timer(s =>
                {
                    var awaitable = (SocketAwaitable)s;
                    awaitable.IsTimedOut = true;
                    awaitable.IsCompleted = true;
                    awaitable.StopTimer();
                }, this, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }

            EventArgs = new SocketAsyncEventArgs();
            EventArgs.Completed += delegate
            {
                if (_timer != null)
                {
                    StopTimer();
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
            StopTimer();
        }

        public SocketAwaitable GetAwaiter()
        {
            if (IsCompleted
                || _timeout == Timeout.InfiniteTimeSpan) return this;

            if (_timeout.Ticks == 0)
            {
                IsTimedOut = true;
                IsCompleted = true;
            }
            else
            {
                StartTimer();
            }

            return this;
        }

        public bool IsCompleted { get; internal set; }
        public bool IsTimedOut { get; private set; }

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
                throw new SocketException((int)EventArgs.SocketError);
        }

        #region timer

        readonly TimeSpan _timeout;
        readonly Timer _timer;

        void StartTimer()
        {
            _timer.Change(_timeout, Timeout.InfiniteTimeSpan);
        }

        void StopTimer()
        {
            if (_timer != null)
                _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        #endregion
    }
}