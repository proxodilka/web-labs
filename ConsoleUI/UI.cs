using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI
{
    public class UI
    {
        public static Tuple<string, int> GetServerParameters()
        {
            Console.Clear();
            Dictionary<string, string> serverParams = new Dictionary<string, string>()
            {
                ["server IP"] = "127.0.0.1",
                ["server port"] = "8080"
            };

            Dictionary<string, string> preferences = new Dictionary<string, string>();

            foreach (var kw in serverParams)
            {
                Console.WriteLine($"Enter {kw.Key} (keep empty to use default: {kw.Value})");
                string param = Console.ReadLine().Trim();

                if (param != "")
                {
                    preferences[kw.Key] = param;
                }
            }

            foreach (var kw in preferences)
            {
                serverParams[kw.Key] = kw.Value;
            }

            string ip = serverParams["server IP"];
            int port = int.Parse(serverParams["server port"]);

            Console.Clear();
            return new Tuple<string, int>(ip, port);
        }
    }
}
