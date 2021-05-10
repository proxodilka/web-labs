using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using ClientHandler.UI;
using ServerHandler.UI;

namespace net_labs
{
    class Program
    {
        struct Option
        {
            public string option;
            public Func<Task> action;
        }

        static readonly List<Option> menu = new List<Option>() {
            new Option() {option="Host server", action=ServerUI.HostServer },
            new Option() {option="Connect to the server", action=() => ClientUI.GetInstance().ConnectToServer(ClientUI.CONSOLE_TYPE.SEQUENTIEL) },
            new Option() {option="Exit", action=async () => System.Environment.Exit(0) }
        };

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                for (int i = 0; i < menu.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {menu[i].option}");
                }

                int choise = int.MaxValue;
                do
                {
                    Console.Write("> ");
                    try
                    {
                        choise = int.Parse(Console.ReadLine()) - 1;
                        if (choise > menu.Count)
                        {
                            throw new Exception("Invalid choise");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input! Try again.");
                    }
                } while (choise > menu.Count);
                menu[choise].action().GetAwaiter().GetResult();
            }
        }
    }
}
