using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class EchoServer : DataProcesser
    {
        public enum SERVER_COMMANDS
        {
            UNKNOWN,
            QUIT,
            SEND,
            HELP
        }

        public struct Command
        {
            readonly public SERVER_COMMANDS command;
            readonly public string raw_request;
            readonly public List<string> arguments;

            public Command(SERVER_COMMANDS command, string raw_request, List<string> arguments = null)
            {
                this.command = command;
                this.raw_request = raw_request;
                this.arguments = arguments == null ? new List<string>() : arguments;
            }
        }

        protected struct HelpUnit
        {
            readonly public string name;
            readonly public SERVER_COMMANDS command;
            readonly public string description;
            readonly public List<string> arguments;

            public HelpUnit(string name, SERVER_COMMANDS command, string description, List<string> arguments = null)
            {
                this.name = name;
                this.command = command;
                this.description = description;
                this.arguments = arguments == null ? new List<string>() : arguments;
            }
        }

        public readonly static int MAX_BACKLOG = 10;

        protected static Dictionary<string, HelpUnit> help;

        public EchoServer(string ip = "127.0.0.1", int port = 8080)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.Bind(endPoint);
            socket.Listen(MAX_BACKLOG);

            InitHelp();
        }

        protected virtual void InitHelp()
        {
            help = new Dictionary<string, HelpUnit>()
            {
                ["quit"] = new HelpUnit("quit", SERVER_COMMANDS.QUIT, "disconnect from the server"),
                ["send"] = new HelpUnit("send", SERVER_COMMANDS.SEND, "send message to the server", new List<string>() { "message" })
            };
        }

        ~EchoServer()
        {
            socket.Close();
        }

        public async Task ListenAsync()
        {
            Log($"Server is running at: {socket.LocalEndPoint}");
            while (true)
            {
                Socket client = await socket.AcceptAsync();
                Log($"New connection established: {client.RemoteEndPoint}");
                var token = new CancellationTokenSource();
                HandleConnection(client, token);
            }
        }

        async Task Pinger(Socket client, CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                SendAsync("Ping...", client, token);
                await Task.Delay(3000);
            }
        }

        public void Listen()
        {
            Log($"Server is running at: {socket.LocalEndPoint}");
            while (true)
            {
                Socket client = socket.Accept();
                Log($"New connection established: {client.RemoteEndPoint}");
                var token = new CancellationTokenSource();
                new Thread(() => HandleConnection(client, token, false)).Start();
            }
        }

        public virtual async void HandleConnection(Socket client, CancellationTokenSource token, bool do_async = true)
        {
            while (true)
            {
                string request;
                if (do_async)
                {
                    request = await RecieveAsync(client);
                }
                else
                {
                    request = Recieve(client);
                }

                string response;

                Log($"Received data from {client.RemoteEndPoint}: '{request}'");

                Command command = ParseCommand(request);
                switch (command.command)
                {
                    case SERVER_COMMANDS.QUIT:
                        response = CONNECTION_CLOSED;
                        break;
                    case SERVER_COMMANDS.HELP:
                        response = GetHelp();
                        break;
                    case SERVER_COMMANDS.SEND:
                        response = ProcessMessage(command.arguments[0]);
                        break;
                    case SERVER_COMMANDS.UNKNOWN:
                    default:
                        response = $"ERROR: unknown command: '{command.raw_request}'" + GetHelp();
                        break;
                }

                if (do_async)
                {
                    SendAsync(response, client);
                }
                else
                {
                    Send(response, client);
                }

                if (response == CONNECTION_CLOSED)
                {
                    break;
                }
            }
            token.Cancel();
            client.Shutdown(SocketShutdown.Both);
            Log($"Connection closed with: {client.RemoteEndPoint}");
            client.Close();
        }

        public virtual string ProcessMessage(string data)
        {
            return $"Got your message: '{data}'\nP.S. your server <3";
        }

        public string GetHelp()
        {
            string result = "\n=========== Available commands ===========\n";
            foreach (var kw in help)
            {
                result += $"/{kw.Key}";
                foreach (var arg in kw.Value.arguments)
                {
                    result += $" <{arg}>";
                }
                result += $"\t{kw.Value.description}\n";
            }
            return result;
        }

        public virtual Command ParseCommand(string data)
        {
            if (data == CONNECTION_CLOSED)
            {
                return new Command(SERVER_COMMANDS.QUIT, data);
            }

            if (data.Length == 0 || (data.Length > 0 && data[0] != '/'))
            {
                return new Command(SERVER_COMMANDS.SEND, data, new List<string>() { data });
            }

            string[] command_components = data.Substring(1, data.Length - 1).Split();
            SERVER_COMMANDS command;
            try
            {
                command = Enum.Parse<SERVER_COMMANDS>(command_components[0], true);
            }
            catch
            {
                command = SERVER_COMMANDS.UNKNOWN;
            }

            List<string> arguments = null;
            if (command_components.Length > 1)
            {
                arguments = new List<string>(
                    new ArraySegment<string>(command_components, 1, command_components.Length - 1).ToArray()
                );
            }

            return new Command(command, data, arguments);
        }

        protected override void Log(string message)
        {
            string timestamp = DateTime.Now.ToLongTimeString();
            Console.WriteLine($"<{timestamp}>: {message}");
        }
    }
}
