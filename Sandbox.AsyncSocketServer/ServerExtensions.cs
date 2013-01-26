using System;
using System.Diagnostics;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public static class ServerExtensions
    {
        public static void Log(
            this IServer server,
            ServerProcess process, string message)
        {
            if (server == null || server.NotifyLog == null)
                Debug.WriteLine(message, process.Name);
            else
                server.NotifyLog(process, message);
        }

        public static void Exception(
            this IServer server, 
            ServerProcess process, Exception exception)
        {
            if (server == null || server.NotifyException == null)
                Debug.WriteLine(exception.ToString(), process.Name);
            else
                server.NotifyException(process, exception);
        }
    }
}