using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace ServerHandler.UI
{
    public class ServerUI
    {
        public async static Task HostServer()
        {
            var serverParams = ConsoleUI.UI.GetServerParameters();
            EchoServer server = new EchoServer(serverParams.Item1, serverParams.Item2);
            await server.ListenAsync();
        }
    }
}
