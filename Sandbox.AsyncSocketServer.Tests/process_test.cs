using System;
using Moq;
using Sandbox.AsyncSocketServer.Abstraction;
using Xunit;

namespace Sandbox.AsyncSocketServer.Tests
{
    public class process_test
    {
        [Fact]
        public void start_with_no_server()
        {
            var sut = new ServerProcess(
                Mock.Of<IListener>(), Mock.Of<IMessageHandler>());

            Assert.DoesNotThrow(sut.Start);
        }

        [Fact]
        public void cannot_start_one_already_started()
        {
            var sut = new ServerProcess(
                Mock.Of<IListener>(), Mock.Of<IMessageHandler>());

            sut.Start();

            Assert.Throws<InvalidOperationException>(() => sut.Start());
        }

        [Fact]
        public void dispose_removes_from_server()
        {
            var sut = new ServerProcess(
                Mock.Of<IListener>(), Mock.Of<IMessageHandler>());

            var serverMock = new Mock<IServer>();
            serverMock.Setup(o => o.Remove(It.IsAny<ServerProcess>())).Verifiable();

            sut.Server = serverMock.Object;

            sut.Dispose();

            serverMock.Verify();
        }

        [Fact]
        public void can_restart_a_process()
        {
            var sut = new ServerProcess(
                Mock.Of<IListener>(), Mock.Of<IMessageHandler>());

            sut.Start();
            Assert.True(sut.IsStarted);

            sut.Stop();
            Assert.False(sut.IsStarted);

            sut.Start();
            Assert.True(sut.IsStarted);
        }

        [Fact]
        public void on_error()
        {
        }
    }
}