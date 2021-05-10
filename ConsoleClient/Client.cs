using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace Server
{
    public class Client : DataProcesser
    {
        readonly string CONNECTION_CLOSED_MESSAGE = "Server closed the connection...";
        IPEndPoint endPoint;
        CancellationTokenSource autoReceivingCancelationToken;
        public Client(string ip = "127.0.0.1", int port = 8080)
        {
            socket = GetSocket();
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            autoReceivingCancelationToken = new CancellationTokenSource();
        }

        ~Client()
        {
            Disconnect();
        }

        public void Connect()
        {
            if (IsConnected())
            {
                return;
            }
            socket = GetSocket();
            socket.Connect(endPoint);
            autoReceivingCancelationToken = new CancellationTokenSource();
        }

        public async Task ConnectAsync()
        {
            if (IsConnected())
            {
                return;
            }
            socket = GetSocket();
            await socket.ConnectAsync(endPoint);
            autoReceivingCancelationToken = new CancellationTokenSource();
        }

        public void Disconnect()
        {
            autoReceivingCancelationToken.Cancel();
            try
            {
                if (IsConnected())
                {
                    SendAsync(CONNECTION_CLOSED);
                }
            }
            finally
            {
                socket.Close();
            }
        }

        public async Task SendAsync(string message)
        {
            if (!IsConnected())
            {
                Connect();
            }
            await base.SendAsync(message);
        }

        public void Send(string message)
        {
            if (!IsConnected())
            {
                Connect();
            }
            base.Send(message);
        }

        public async Task<string> RecieveAsync(CancellationToken token = default)
        {
            string response = await base.RecieveAsync(socket, token);
            if (response == CONNECTION_CLOSED)
            {
                StopReceiving();
            }
            return response;
        }

        public string Recieve()
        {
            string response = base.Recieve(socket);
            if (response == CONNECTION_CLOSED)
            {
                return "";
            }
            return response;
        }

        public async Task StartReceiving(Action<string> callback, CancellationTokenSource token)
        {
            autoReceivingCancelationToken = token;
            while (true)
            {
                if (autoReceivingCancelationToken.Token.IsCancellationRequested)
                {
                    break;
                }
                string result = await RecieveAsync(autoReceivingCancelationToken.Token);
                callback(result);
            }
        }

        public void StopReceiving()
        {
            autoReceivingCancelationToken.Cancel();
        }

        public async Task<string> SendRecieveAsync(string message)
        {
            try
            {
                SendAsync(message);
            }
            catch (Exception e)
            {
                Log($"WARNING: server have not received the message because of the following error:\n{e.Message}");
                return "";
            }
            return await RecieveAsync();
        }

        Socket GetSocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveTimeout = 6000;
            socket.SendTimeout = 6000;

            return socket;
        }
    }
}
