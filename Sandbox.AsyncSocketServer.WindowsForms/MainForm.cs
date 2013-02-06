using System;
using System.Net;
using System.Windows.Forms;
using Sandbox.AsyncSocketServer.Buffering;
using Sandbox.AsyncSocketServer.Messaging;
using Sandbox.AsyncSocketServer.Sockets;

namespace Sandbox.AsyncSocketServer.WindowsForms
{
    public partial class MainForm : Form
    {
        readonly Server _server;

        public MainForm()
        {
            InitializeComponent();

            Action<string> log =
                m =>
                    {
                        if (InvokeRequired) Invoke(new Action<string>(Log), m);
                        else Log(m);
                    };

            _server = new Server(
                (p, m) => log(string.Format("{0}: {1}\r\n", p.Name, m)),
                (p, ex) => Log(string.Format("!ERROR! {0}: {1}\r\n", p.Name, ex.ToString()))
                );
        }

        protected override void OnShown(EventArgs e)
        {
            var workerFactory = new WorkerFactory(
                new BufferManager(100, 2048), TimeSpan.FromMilliseconds(1));

            var listener = new Listener(
                new ListenerSettings(IPAddress.Any, 8088),
                s => workerFactory.Create(new WorkerSocket(s))
                );

            var process = new ServerProcess(
                listener, () => new HttpMessageHandler(new HttpMessage()))
                {
                    Name = "sandbox",
                    Server = _server
                };

            process.Start();
        }

        void Log(string message)
        {
            if (!LogTextBox.IsDisposed)
                LogTextBox.AppendText(message);
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
    }
}