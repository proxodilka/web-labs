using Newtonsoft.Json;
using Server;
using ServerHandler;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatConsoleVersion
{
    public class ChatClient
    {
        readonly public Socket socket;
        readonly public string name;
        readonly public string color;

        public ChatClient(Socket socket, string name, string color = "Red")
        {
            this.socket = socket;
            this.name = name;
            this.color = color;
        }
    }
    public class ChatServer : EchoServer
    {
        HashSet<ChatClient> clients;
        public readonly static string CONNECTION_ESTABLISHED = "__CONNECTION_ESTABLISHED__";
        public readonly static string MESSAGE_RECEIVED = "__MESSAGE_RECEIVED__";
        public enum NotificationType
        {
            UNKNOWN,
            JOIN,
            EXIT,
            MESSAGE,
            SERVER_MESSAGE
        }
        public ChatServer(string ip = "127.0.0.1", int port = 8080) : base(ip, port)
        {
            clients = new HashSet<ChatClient>();
        }

        public async Task<ChatClient> RegisterClient(Socket client)
        {
            string register_data = await RecieveAsync(client);
            if (register_data == CONNECTION_CLOSED)
            {
                return null;
            }

            Dictionary<string, string> credentials;
            try
            {
                credentials = JsonConvert.DeserializeObject<Dictionary<string, string>>(register_data);
            }
            catch (Exception e)
            {
                Log($"ERROR: An error occured during handshake, credentials has an invalid format:\n{e.Message}");
                return null;
            }

            ChatClient new_client;
            try
            {
                string color = "Red";
                credentials.TryGetValue("color", out color);
                new_client = new ChatClient(client, credentials["name"], color);
            }
            catch (Exception e)
            {
                Log($"ERROR: An error occured during handshake, credentials has an invalid format:\n{e.Message}");
                return null;
            }
            SendAsync(CONNECTION_ESTABLISHED, client);
            NotifyClients(new_client, NotificationType.JOIN);
            clients.Add(new_client);
            return new_client;
        }

        public void NotifyClients(string message)
        {
            foreach (var client in clients)
            {
                SendAsync(message, client.socket);
            }
        }

        public string GetNotification(NotificationType type, List<string> arguments = null, ChatClient invoker = null)
        {
            Dictionary<string, string> notification = new Dictionary<string, string>()
            {
                ["type"] = type.ToString(),
                ["invoker_name"] = invoker == null? "null" : invoker.name,
                ["invoker_color"] = invoker == null? "null" : invoker.color,
                ["message"] = ""
            };
            switch (type)
            {
                case NotificationType.JOIN:
                    break;
                case NotificationType.EXIT:
                    break;
                case NotificationType.SERVER_MESSAGE:
                case NotificationType.MESSAGE:
                    notification["message"] = arguments[0];
                    break;
                default:
                    Log($"ERROR: client {invoker.socket.RemoteEndPoint} has invoked an unknown event: {type}");
                    break;
            }
            return JsonConvert.SerializeObject(notification);
        }

        public void NotifyClients(ChatClient invoker, NotificationType type, List<string> arguments = null)
        {
            string message = GetNotification(type, arguments, invoker);
            NotifyClients(message);
        }

        public override async void HandleConnection(Socket client, CancellationTokenSource token, bool do_async = true)
        {
            if (!do_async)
            {
                Log($"WARNING: only asynchronous mode is supported, parameter 'do_async = {do_async}' is ignored.");
            }

            ChatClient _client = await RegisterClient(client);
            if (_client == null)
            {
                token.Cancel();
                return;
            }
            while (true)
            {
                string request;
                request = await RecieveAsync(client);

                string response;

                Log($"Received data from {client.RemoteEndPoint}: '{request}'");

                Command command = ParseCommand(request);
                switch (command.command)
                {
                    case SERVER_COMMANDS.QUIT:
                        response = CONNECTION_CLOSED;
                        break;
                    case SERVER_COMMANDS.HELP:
                        response = GetNotification(NotificationType.SERVER_MESSAGE, new List<string>() { GetHelp() });
                        break;
                    case SERVER_COMMANDS.SEND:
                        NotifyClients(_client, NotificationType.MESSAGE, command.arguments);
                        continue;
                    case SERVER_COMMANDS.UNKNOWN:
                    default:
                        response = GetNotification(
                            NotificationType.SERVER_MESSAGE,
                            new List<string>() { $"ERROR: unknown command: '{command.raw_request}'" + GetHelp() }
                        );
                        break;
                }

                SendAsync(response, client);

                if (response == CONNECTION_CLOSED)
                {
                    break;
                }
            }
            token.Cancel();
            client.Shutdown(SocketShutdown.Both);
            Log($"Connection closed with: {client.RemoteEndPoint}");
            client.Close();
            clients.Remove(_client);
            NotifyClients(_client, NotificationType.EXIT);
        }

    }
}
