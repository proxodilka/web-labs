using ClientHandler.UI;
using ConsoleUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatConsoleVersion
{
    public class ChatClientUI : ClientUI
    {

        protected ChatClientUI() : base() { }

        public static new ChatClientUI GetInstance()
        {
            if (instance == null)
            {
                instance = new ChatClientUI();
            }
            return instance as ChatClientUI;
        }

        protected async override Task StartAsyncClient()
        {
            Console.Write("Enter your nickname:\n> ");
            string name = Console.ReadLine();

            Dictionary<string, string> credentianls = new Dictionary<string, string>()
            {
                ["name"] = name
            };

            string connectionResult = await client.SendRecieveAsync(JsonConvert.SerializeObject(credentianls));
            if (connectionResult != ChatServer.CONNECTION_ESTABLISHED)
            {
                Console.WriteLine("ERROR: Can't connect to the remote server.\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            cancellationToken = new CancellationTokenSource();

            AsyncConsole.Init();
            AsyncConsole.WriteLine("========= Welcome to the server =========");
            var OnInputTask = AsyncConsole.ReadLine(OnInput, cancellationToken.Token);
            await client.StartReceiving(OnRecieve, cancellationToken);

            AsyncConsole.WriteLine("ERROR: Server closed the connection.\nPress Enter to continue...");
            await OnInputTask;
            client.Disconnect();
        }

        public static string ParseResponse(string response)
        {
            if (response == ChatServer.CONNECTION_CLOSED)
            {
                return "You have been kicked from the server";
            }
            Dictionary<string, string> notification;
            try
            {
                notification = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            }
            catch (Exception e)
            {
                throw new Exception($"ERROR: bad json:\n{response}\n{e.Message}");
            }

            if (notification == null)
            {
                return response;
            }

            ChatServer.NotificationType type;

            try
            {
                type = Enum.Parse<ChatServer.NotificationType>(notification["type"], true);
            } 
            catch
            {
                type = ChatServer.NotificationType.UNKNOWN; 
            }

            string to_print;
            switch (type)
            {
                case ChatServer.NotificationType.JOIN:
                    to_print = $"{notification["invoker_name"]} has entered the chat!";
                    break;
                case ChatServer.NotificationType.EXIT:
                    to_print = $"{notification["invoker_name"]} has left the chat.";
                    break;
                case ChatServer.NotificationType.MESSAGE:
                    to_print = $"{notification["invoker_name"]}: {notification["message"]}";
                    break;
                case ChatServer.NotificationType.SERVER_MESSAGE:
                default:
                    to_print = notification["message"];
                    break;
            }

            return to_print;
        }

        protected override void OnRecieve(string message)
        {
            try
            {
                message = ParseResponse(message);
            } 
            catch(Exception e)
            {
                AsyncConsole.WriteLine(e.Message);
                return;
            }

            AsyncConsole.WriteLine(message);

            if (!client.IsConnected())
            {
                cancellationToken.Cancel();
                return;
            }
        }

        protected override void OnInput(string message)
        {
            if (!client.IsConnected())
            {
                cancellationToken.Cancel();
                return;
            }
            client.SendAsync(message);
        }
    }
}
