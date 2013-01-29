using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Sockets
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
                // create the timer object, disabled
                _timer = new Timer(
                    s => ((SocketAwaitable) s).TimedOut(),
                    this, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
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
            _continuation = null;
            IsCompleted = false;
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
                throw new SocketException((int) EventArgs.SocketError);
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

        void TimedOut()
        {
            IsTimedOut = true;
            IsCompleted = true;
            StopTimer();
        }

        #endregion
    }
}