using System;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public class ServerProcess : IDisposable
    {
        readonly IListener _listener;
        readonly Func<IMessageHandler> _createHandler;
        readonly ILogger _logger;

        public ServerProcess(
            IListener listener, Func<IMessageHandler> createHandler,
            ILogger logger)
        {
            if (listener == null) throw new ArgumentNullException("listener");
            if (createHandler == null) throw new ArgumentNullException("createHandler");

            _listener = listener;
            _createHandler = createHandler;
            _logger = logger;

            Name = Guid.NewGuid().ToString();
        }

        public string Name { get; set; }
        public bool IsStarted { get; private set; }
        public Exception Exception { get; set; }

        public IServer Server { get; set; }

        public void Start()
        {
            if (IsStarted)
                throw new InvalidOperationException();

            IsStarted = true;
            _logger.System(this, () => "Started");

            Task.Run(() => AcceptLoop());
        }

        public void Stop()
        {
            if (!IsStarted)
                throw new InvalidOperationException();

            IsStarted = false;
            Exception = null;

            _logger.System(this, () => "Stopped");
        }

        async void AcceptLoop()
        {
            try
            {
                if (IsStarted) _logger.Information(this, () => "Running");
                while (IsStarted)
                {
                    // get the next connection
                    var worker = await _listener.AcceptAsync();

                    Process(worker);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        async void Process(IWorker worker)
        {
            try
            {
                _logger.Information(this, () => "Process");

                var handler = _createHandler();

                while (!worker.Closed)
                {
                    // recieve any data, process it and send a response
                    var request = await worker.ReceiveAsync();
                    if (request == null)
                    {
                        _logger.Information(this, () => "Process Connection Closed");

                        return;
                    }

                    var response = await handler.ProcessAsync(request);

                    if (response == null) continue;

                    await worker.SendAsync(response);
                    break;
                }

                _logger.Information(this, () => "Process Complete");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                worker.Close();
            }
        }

        void HandleException(Exception ex)
        {
            IsStarted = false;
            Exception = ex;

            _logger.System(this, () => "Stopped on exception");
        }

        public override string ToString()
        {
            return Name;
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

            _logger.Information(this, () => "Disposed");
        }

        ~ServerProcess()
        {
            Dispose(false);
        }

        bool _disposed;

        #endregion
    }
}