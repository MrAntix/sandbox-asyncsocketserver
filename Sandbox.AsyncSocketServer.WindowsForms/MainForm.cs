using System;
using System.Net;
using System.Windows.Forms;
using Sandbox.AsyncSocketServer.Abstraction;
using Sandbox.AsyncSocketServer.Buffering;
using Sandbox.AsyncSocketServer.Messaging;
using Sandbox.AsyncSocketServer.Sockets;

namespace Sandbox.AsyncSocketServer.WindowsForms
{
    public partial class MainForm : Form
    {
        readonly Server _server;
        readonly DelegateLogger _logger;

        public MainForm()
        {
            InitializeComponent();

            _logger = new DelegateLogger(
                (t, f, m) =>
                    {
                        if (InvokeRequired)
                            Invoke(new Action<LogLevel, string, string>(Log), t, f, m);
                        else Log(t, f, m);
                    },
                LogLevel.System);

            _server = new Server(_logger);

            LogLevelControl.DataSource = Enum.GetValues(typeof (LogLevel));
            LogLevelControl.DataBindings.Add("SelectedItem", _logger, "Level");
            LogLevelControl.SelectionChangeCommitted += (e, s) => { _logger.Level = (LogLevel)LogLevelControl.SelectedItem; };
        }

        protected override void OnShown(EventArgs e)
        {
            var workerFactory = new WorkerManager(
                new BufferManager(10, 2048), TimeSpan.FromTicks(1));

            var listener = new Listener(
                new ListenerSettings(IPAddress.Any, 8088),
                s => workerFactory.Get(new WorkerSocket(s))
                );

            var process = new ServerProcess(
                listener, () => new HttpMessageHandler(_logger),
                _logger)
                {
                    Name = "sandbox",
                    Server = _server
                };

            process.Start();

            StartStopButton.Click += (s, ce) =>
                {
                    if (process.IsStarted)
                    {
                        process.Stop();
                        StartStopButton.Text = "Start";
                    }
                    else
                    {
                        process.Start();
                        StartStopButton.Text = "Stop";
                    }
                };
        }

        void Log(LogLevel level, string title, string message)
        {
            if (!LogTextBox.IsDisposed)
                LogTextBox.AppendText(
                    string.Format("[{0}] {1}: {2}\r\n", level, title, message));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _server.Dispose();

            base.OnFormClosing(e);
        }

        void button1_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            GC.Collect();
        }

        private void StartStopButton_Click(object sender, EventArgs e)
        {
        }

    }
}