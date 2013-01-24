using System;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class ServerProcess : IDisposable
    {
        readonly IListener _listener;
        readonly IMessageHandler _handler;

        public ServerProcess(
            IListener listener, IMessageHandler handler
            )
        {
            if (listener == null) throw new ArgumentNullException("listener");
            if (handler == null) throw new ArgumentNullException("handler");

            _listener = listener;
            _handler = handler;
        }

        public bool IsStarted { get; private set; }

        public IServer Server { get; set; }

        public void Start()
        {
            if (IsStarted)
                throw new InvalidOperationException();

            IsStarted = true;
            Run();
        }

        public void Stop()
        {
            if (!IsStarted)
                throw new InvalidOperationException();

            IsStarted = false;
        }

        async void Run()
        {
            while (IsStarted)
            {
                // get the next connection
                var worker = await _listener.AcceptAsync();

                // recieve any data, process it and send a response
                var data = await worker.ReceiveAsync(_handler.Terminator);
                var processedData = await _handler.ProcessAsync(data);
                await worker.SendAsync(processedData);
            }
        }

        #region dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (IsStarted) Stop();
                if (Server != null) Server.Remove(this);
            }

            _disposed = true;
        }

        ~ServerProcess()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion
    }
}