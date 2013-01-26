using System;
using System.Threading.Tasks;
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
        public Exception Exception { get; set; }

        public IServer Server { get; set; }

        public void Start()
        {
            if (IsStarted)
                throw new InvalidOperationException();

            IsStarted = true;
            Server.Log("Started");

            Task.Run((Func<Task>) AcceptLoop);
        }

        public void Stop()
        {
            if (!IsStarted)
                throw new InvalidOperationException();

            IsStarted = false;
            Exception = null;

            Server.Log("Stopped");
        }

        async Task AcceptLoop()
        {
            try
            {
                if (IsStarted) Server.Log("Running");
                while (IsStarted)
                {
                    // get the next connection
                    var worker = await _listener.AcceptAsync();

                    new Task(() => Process(worker)).Start();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        async Task Process(IWorker worker)
        {
            try
            {
                Server.Log("Process");

                // recieve any data, process it and send a response
                var data = await worker.ReceiveAsync(_handler.Terminator);
                Server.Log("Received data");
                var processedData = await _handler.ProcessAsync(data);
                Server.Log("Process data");
                await worker.SendAsync(processedData);
                Server.Log("Sent data");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        void HandleException(Exception ex)
        {
            IsStarted = false;
            Exception = ex;

            if (Server != null)
                Server.Error(this, ex);

            Server.Log("Stopped on exception");
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
            Server.Log("Disposed");
        }

        ~ServerProcess()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion
    }
}