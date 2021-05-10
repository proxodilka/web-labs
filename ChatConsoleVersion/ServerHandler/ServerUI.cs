using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatConsoleVersion
{
    public class ServerUI
    {
        public async static Task HostServer()
        {
            var serverParams = ConsoleUI.UI.GetServerParameters();
            ChatServer server = new ChatServer(serverParams.Item1, serverParams.Item2);
            await server.ListenAsync();
        }
    }
}
