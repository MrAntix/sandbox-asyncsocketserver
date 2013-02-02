using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                m => Invoke(new Action<string>(Log), m);

            _server = new Server(
                (p, m) => log(string.Format("{0}: {1}\r\n", p.Name, m)),
                (p, ex) => Log(string.Format("!ERROR! {0}: {1}\r\n", p.Name, ex.ToString()))
                );

        }

        protected override void OnShown(EventArgs e)
        {
            var workerFactory = new WorkerFactory(
                new BufferManager(100, 2048), Timeout.InfiniteTimeSpan);

            var listener = new Listener(
                new ListenerSettings(IPAddress.Any, 8088),
                s => workerFactory.Create(new WorkerSocket(s))
                );

            var process = new ServerProcess(
                listener, new HttpMessageHandler())
            {
                Name = "sandbox",
                Server = _server
            };

            process.Start();
        }

        void Log(string message)
        {
            LogTextBox.AppendText(message);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _server.Dispose();
        }
    }
}
