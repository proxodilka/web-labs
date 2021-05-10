using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace Server
{
    public class DataProcesser
    {
        public readonly static string CONNECTION_CLOSED = $"__CONNECTION_CLOSED__";
        protected Socket socket;
        public async Task<string> RecieveAsync(Socket socket = null, CancellationToken cancellationToken = default)
        {
            socket = socket == null ? this.socket : socket;

            byte[] buffer = new byte[1024];
            string data = "";

            do
            {
                int size = 0;
                try
                {
                    size = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
                }
                catch (Exception e)
                {
                    Log($"Can't receive data from remote because of the following error:\n{e.Message}");
                    return CONNECTION_CLOSED;
                }
                data += Encoding.UTF8.GetString(buffer, 0, size);
            } while (socket.Available > 0);

            return data;
        }

        public string Recieve(Socket socket = null)
        {
            socket = socket == null ? this.socket : socket;

            byte[] buffer = new byte[1024];
            string data = "";

            do
            {
                int size = 0;
                try
                {
                    size = socket.Receive(buffer);
                }
                catch (Exception e)
                {
                    Log($"Can't receive data from remote because of the following error:\n{e.Message}");
                    return CONNECTION_CLOSED;
                }
                data += Encoding.UTF8.GetString(buffer, 0, size);
            } while (socket.Available > 0);

            return data;
        }

        public async Task SendAsync(string message, Socket socket = null, CancellationToken cancellationToken = default)
        {
            socket = socket == null ? this.socket : socket;

            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                await socket.SendAsync(data, SocketFlags.None, cancellationToken);
            }
            catch (Exception e)
            {
                Log(
                    $"WARNING: remote {socket.RemoteEndPoint} have not received the message because of the following error:\n{e.Message}"
                );
            }
        }

        public void Send(string message, Socket socket = null)
        {
            socket = socket == null ? this.socket : socket;

            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                socket.Send(data);
            }
            catch (Exception e)
            {
                Log(
                    $"WARNING: remote {socket.RemoteEndPoint} have not received the message because of the following error:\n{e.Message}"
                );
            }

        }

        public bool IsConnected(Socket socket = null)
        {
            socket = socket == null ? this.socket : socket;

            if (socket == null)
            {
                return false;
            }
            if (!socket.Connected)
            {
                return false;
            }
            bool cond1 = socket.Poll(1000, SelectMode.SelectRead);
            bool cond2 = socket.Available == 0;
            return !cond1 || !cond2;

        }
        protected virtual void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
