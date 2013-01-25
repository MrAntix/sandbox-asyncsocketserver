﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Sandbox.AsyncSocketServer.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class process_test
    {
        [Fact]
        public void dispose_removes_from_server()
        {
            var sut = new ServerProcess(
                GetListenerMock().Object, GetMessageHandlerMock().Object);

            var serverMock = new Mock<IServer>();
            serverMock.Setup(o => o.Remove(It.IsAny<ServerProcess>())).Verifiable();

            sut.Server = serverMock.Object;

            sut.Dispose();

            serverMock.Verify();
        }

        [Fact]
        public async Task start_with_no_server()
        {
            var sut = new ServerProcess(
                GetListenerMock().Object, GetMessageHandlerMock().Object);

            Assert.DoesNotThrow(sut.Start);
        }

        [Fact]
        public async Task cannot_start_one_already_started()
        {
            var sut = new ServerProcess(
                GetListenerMock().Object, GetMessageHandlerMock().Object);

            sut.Start();

            Assert.Throws<InvalidOperationException>(() => sut.Start());
        }

        [Fact]
        public async Task can_restart_a_process()
        {
            var sut = new ServerProcess(
                GetListenerMock().Object, GetMessageHandlerMock().Object);

            sut.Start();
            Assert.True(sut.IsStarted);

            sut.Stop();
            Assert.False(sut.IsStarted);

            sut.Start();
            Assert.True(sut.IsStarted);
        }

        [Fact]
        public async Task on_error_IsStarted_is_false()
        {
            var listenerMock = GetListenerMock();
            listenerMock
                .Setup(o => o.AcceptAsync())
                .Callback(() => { throw new Exception(); });

            var sut = new ServerProcess(
                listenerMock.Object, GetMessageHandlerMock().Object);

            sut.Start();
            Thread.Sleep(100);

            Assert.False(sut.IsStarted);
        }

        [Fact]
        public async Task on_error_Exception_is_set()
        {
            var exception = new Exception();

            var listenerMock = new Mock<IListener>();
            listenerMock
                .Setup(o => o.AcceptAsync())
                .Callback(() => { throw exception; });

            var sut = new ServerProcess(
                listenerMock.Object, GetMessageHandlerMock().Object);

            sut.Start();
            Thread.Sleep(100);

            Assert.Equal(exception, sut.Exception);
        }

        [Fact]
        public async Task on_error_server_recieves_exception()
        {
            var exception = new Exception();

            var listenerMock = GetListenerMock();
            listenerMock
                .Setup(o => o.AcceptAsync())
                .Callback(() => { throw exception; });

            var sut = new ServerProcess(
                listenerMock.Object, GetMessageHandlerMock().Object);

            var serverMock = new Mock<IServer>();
            serverMock.Setup(o => o.Error(sut, exception)).Verifiable();

            sut.Server = serverMock.Object;

            sut.Start();
            Thread.Sleep(100);

            serverMock.Verify();
        }

        static Mock<IListener> GetListenerMock(Mock<IWorker> workerMock = null)
        {
            workerMock = workerMock ?? GetWorkerMock();
            var mock = new Mock<IListener>();
            mock
                .Setup(o => o.AcceptAsync())
                .Returns(Task<IWorker>.Factory.StartNew(
                () =>
                    {
                        Thread.Sleep(10);
                        return workerMock.Object;
                    }));

            return mock;
        }

        static Mock<IWorker> GetWorkerMock()
        {
            var mock = new Mock<IWorker>();
            mock.Setup(o => o.ReceiveAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new byte[] { }));
            mock.Setup(o => o.SendAsync(It.IsAny<byte[]>()))
                .Returns(Task.FromResult(default(object)));

            return mock;
        }

        static Mock<IMessageHandler> GetMessageHandlerMock()
        {
            var mock = new Mock<IMessageHandler>();
            mock.Setup(o => o.ProcessAsync(It.IsAny<byte[]>()))
                .Returns(Task.FromResult(new byte[] { }));

            return mock;
        }
    }
}