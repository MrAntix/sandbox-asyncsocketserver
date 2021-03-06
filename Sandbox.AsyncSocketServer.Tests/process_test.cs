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
        public process_test()
        {
            LoggerExtensions.NullLog = Console.WriteLine;
        }

        [Fact]
        public void dispose_removes_from_server()
        {
            var sut = new ServerProcess(
                GetListenerMock().Object, () => GetMessageHandlerMock().Object, null);

            var serverMock = new Mock<IServer>();
            serverMock.Setup(o => o.Remove(It.IsAny<ServerProcess>())).Verifiable();

            sut.Server = serverMock.Object;

            sut.Dispose();

            serverMock.Verify();

            GC.Collect();
        }

        [Fact]
        public void start_with_no_server()
        {
            using (var sut = new ServerProcess(
                GetListenerMock().Object, () => GetMessageHandlerMock().Object, null))
            {
                // ReSharper disable AccessToDisposedClosure
                // executed right away, not disposed at this point
                Assert.DoesNotThrow(() => sut.Start());
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void cannot_start_one_already_started()
        {
            using (var sut = new ServerProcess(
                GetListenerMock().Object, () => GetMessageHandlerMock().Object, null))
            {
                sut.Start();

                // ReSharper disable AccessToDisposedClosure
                // executed right away, not disposed at this point
                Assert.Throws<InvalidOperationException>(() => sut.Start());
                // ReSharper restore AccessToDisposedClosure

                Thread.Sleep(50);
            }
        }

        [Fact]
        public void can_restart_a_process()
        {
            using (var sut = new ServerProcess(
                GetListenerMock().Object, () => GetMessageHandlerMock().Object, null))
            {
                sut.Start();
                Assert.True(sut.IsStarted);

                sut.Stop();
                Assert.False(sut.IsStarted);

                sut.Start();
                Assert.True(sut.IsStarted);

                Thread.Sleep(50);
            }
        }

        [Fact]
        public void on_error_IsStarted_is_false()
        {
            var listenerMock = GetListenerMock();
            listenerMock
                .Setup(o => o.AcceptAsync())
                .Callback(() => { throw new Exception(); });

            using (var sut = new ServerProcess(
                listenerMock.Object, () => GetMessageHandlerMock().Object, null))
            {
                sut.Start();
                Thread.Sleep(50);

                Assert.False(sut.IsStarted);
            }
        }

        [Fact]
        public void on_error_Exception_is_set()
        {
            var exception = new Exception();

            var listenerMock = new Mock<IListener>();
            listenerMock
                .Setup(o => o.AcceptAsync())
                .Callback(() => { throw exception; });

            using (var sut = new ServerProcess(
                listenerMock.Object, () => GetMessageHandlerMock().Object, null))
            {
                sut.Start();
                Thread.Sleep(50);

                Assert.Equal(exception, sut.Exception);
            }
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
                            Thread.Sleep(5);
                            return workerMock.Object;
                        }));

            return mock;
        }

        static Mock<IWorker> GetWorkerMock()
        {
            var mock = new Mock<IWorker>();
            mock.Setup(o => o.ReceiveAsync())
                .Returns(Task<byte[]>.Factory.StartNew(
                    () =>
                        {
                            Thread.Sleep(10);
                            return new byte[] {};
                        }));
            mock.Setup(o => o.SendAsync(It.IsAny<byte[]>()))
                .Returns(Task.Factory.StartNew(
                    () => Thread.Sleep(10)));

            return mock;
        }

        static Mock<IMessageHandler> GetMessageHandlerMock()
        {
            var mock = new Mock<IMessageHandler>();
            mock.Setup(o => o.ProcessAsync(It.IsAny<byte[]>()))
                .Returns(Task.FromResult(new byte[] {}));

            return mock;
        }
    }
}