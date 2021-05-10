using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server;
using ConsoleUI;

namespace ClientHandler.UI
{
    public class ClientUI
    {
        protected ClientUI()
        {
            var serverParams = ConsoleUI.UI.GetServerParameters();
            client = new Client(serverParams.Item1, serverParams.Item2);
        }
        public enum CONSOLE_TYPE
        {
            ASYNC,
            SEQUENTIEL
        }

        protected static Client client;
        protected static CancellationTokenSource cancellationToken;
        protected static ClientUI instance;

        public static ClientUI GetInstance()
        {
            if (instance == null)
            {
                instance = new ClientUI();
            }
            return instance;
        }

        public async Task ConnectToServer(CONSOLE_TYPE type = CONSOLE_TYPE.SEQUENTIEL)
        {
            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't connect to the server because of the following error:\n{e.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            switch (type)
            {
                case CONSOLE_TYPE.ASYNC:
                    await StartAsyncClient();
                    break;
                case CONSOLE_TYPE.SEQUENTIEL:
                default:
                    await StartSequentialClient();
                    break;
            }
        }

        async Task StartSequentialClient()
        {
            while (true)
            {
                if (!client.IsConnected())
                {
                    Console.WriteLine("ERROR: Server closed the connection.\nPress any key to continue...");
                    Console.ReadKey();
                    break;
                }
                Console.Write("\nEnter a message to send to the server:\n> ");
                string message = Console.ReadLine();

                if (message == null)
                {
                    break;
                }

                string response = await client.SendRecieveAsync(message);
                Console.WriteLine($"Server responded: \n'''\n{response}\n'''");
            }
            client.Disconnect();
        }

        protected async virtual Task StartAsyncClient()
        {
            cancellationToken = new CancellationTokenSource();

            AsyncConsole.Init();
            var OnInputTask = AsyncConsole.ReadLine(OnInput, cancellationToken.Token);
            await client.StartReceiving(OnRecieve, cancellationToken);

            AsyncConsole.WriteLine("ERROR: Server closed the connection.\nPress Enter to continue...");
            await OnInputTask;
            client.Disconnect();
        }

        protected virtual void OnRecieve(string message)
        {
            AsyncConsole.WriteLine(message);
            if (!client.IsConnected())
            {
                cancellationToken.Cancel();
                return;
            }
        }

        protected virtual void OnInput(string message)
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
