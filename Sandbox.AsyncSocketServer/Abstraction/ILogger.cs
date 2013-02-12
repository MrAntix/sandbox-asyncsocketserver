using System;
using System.ComponentModel;

namespace Sandbox.AsyncSocketServer.Abstraction
{
    public interface ILogger
    {
        /// <summary>
        ///     <para>Create a log entry</para>
        ///     <para>This method should not be called directly, use the extension methods of ILogger</para>
        ///     <para>The extension methods allow for a null logger and redirects the entry to .net diagnostics</para>
        /// </summary>
        /// <param name="level">
        ///     Type of entry <see cref="LogLevel" />
        /// </param>
        /// <param name="filter">Filter string</param>
        /// <param name="text">Log text</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use the extension methods of ILogger")]
        void Log(LogLevel level, string filter, string text);

        /// <summary>
        /// <para>Level to log at</para>
        /// </summary>
        LogLevel Level { get; set; }
    }
}