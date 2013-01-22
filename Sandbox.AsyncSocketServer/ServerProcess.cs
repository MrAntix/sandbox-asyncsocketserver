using System;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class ServerProcess : IDisposable
    {
        readonly IListener _listener;
        readonly IMessageHandler _handler;
        Action<ServerProcess> _release;

        public ServerProcess(
            IListener listener, IMessageHandler handler
            )
        {
            _listener = listener;
            _handler = handler;
        }

        public async void Start(Action<ServerProcess> release)
        {
            if (release == null) throw new ArgumentNullException("release");

            if (_release != null)
                throw new InvalidOperationException();
            _release = release;

            while (_release != null)
            {
                // get the next connection
                var worker = await _listener.AcceptAsync();

                // recieve any data, process it and send a response
                var data = await worker.ReceiveAsync(_handler.Terminator);
                var processedData = await _handler.ProcessAsync(data);
                await worker.SendAsync(processedData);
            }
        }

        public void Stop()
        {
            if (_release == null)
                throw new InvalidOperationException();

            _release(this);
            _release = null;
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
                Stop();
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