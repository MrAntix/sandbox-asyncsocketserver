﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sandbox.AsyncSocketServer.Abstraction;

namespace Sandbox.AsyncSocketServer
{
    public static class Extensions
    {
        public static async Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
                await task;
            else
                throw new TimeoutException();
        }
    }
    public class Worker : IWorker
    {
        readonly Socket _socket;
        readonly SocketAwaitable _awaitable;
        readonly string _terminator;
        readonly Action _release;

        public Worker(
            Socket socket, SocketAwaitable awaitable,
            string terminator,
            Action release)
        {
            _socket = socket;
            _awaitable = awaitable;
            _release = release;
            _terminator = terminator;
        }

        public async Task<byte[]> ReceiveAsync()
        {
            var data = new List<byte>();

            try
            {
                while (true)
                {
                    _awaitable.Reset();
                    if (!_socket.ReceiveAsync(_awaitable.EventArgs))
                        _awaitable.IsCompleted = true;

                    await _awaitable;

                    // get the place to start looking for the terminator
                    var terminatorIndexStart = data.Count > _terminator.Length
                                                   ? data.Count - _terminator.Length
                                                   : 0;

                    // check the number of bytes recieved
                    var bytesReceived = _awaitable.EventArgs.BytesTransferred;
                    if (bytesReceived <= 0) break;

                    // add the received data
                    data.AddRange(_awaitable.EventArgs
                                            .Buffer.Skip(_awaitable.EventArgs.Offset).Take(bytesReceived));

                    // look for a terminator
                    var terminatorIndex = GetTerminatorIndex(data, terminatorIndexStart);
                    if (terminatorIndex > -1)
                    {
                        // terminator found, return all data up to it
                        return data.Take(terminatorIndex).ToArray();
                    }
                }
                
                // client closed connection
                return data.ToArray();
            }
            catch (TimeoutException)
            {
                Dispose();
                throw;
            }
        }

        public async Task SendAsync(byte[] data)
        {
            _awaitable.Reset();
            _awaitable.EventArgs.SetBuffer(data, 0, data.Length);

            if (!_socket.SendAsync(_awaitable.EventArgs))
                _awaitable.IsCompleted = true;

            await _awaitable;
        }

        int GetTerminatorIndex(IReadOnlyList<byte> data, int startIndex)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            for (var dataIndex = startIndex; dataIndex < data.Count; dataIndex++)
            {
                if (Enumerable
                    .Range(0, _terminator.Length)
                    .All(
                        terminatorIndex =>
                        dataIndex + terminatorIndex < data.Count
                        && data[dataIndex + terminatorIndex] == _terminator[terminatorIndex]))
                    return dataIndex;
            }

            return -1;
        }

        #region dispose

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    Debug.WriteLine("socket shutdown");
                }
                catch (Exception)
                {
                }
                _socket.Close();
            }

            _release();
            Disposed = true;
        }

        ~Worker()
        {
            Dispose(false);
        }


        #endregion
    }
}