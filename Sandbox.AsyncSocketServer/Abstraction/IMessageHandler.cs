using System.Threading.Tasks;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    /// <summary>
    ///     <para>Message handler interface</para>
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        ///     <para>Process a message coming from the worker</para>
        /// </summary>
        Task<byte[]> ProcessAsync(byte[] request);

        /// <summary>
        ///     <para>A terminator for the incomming data</para>
        /// </summary>
        string Terminator { get; }
    }
}