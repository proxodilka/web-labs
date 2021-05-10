using Server;
using System;
using System.Diagnostics;
using System.Threading;

namespace ServerBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip;
            int port;

            if (args.Length > 1)
            {
                ip = args[0];
                port = int.Parse(args[1]);
            }
            else
            {
                ip = "127.0.0.1";
                port = 8080;
            }
            
            var client = new Client(ip, port);

            int NREQ = 100;

            var globalSW = new Stopwatch();
            var localSW = new Stopwatch();
            long[] reses = new long[NREQ];

            globalSW.Start();
            for (int i=0; i< NREQ; i++)
            {
                localSW.Reset();
                localSW.Start();
                client.Send($"My message {i}/{NREQ}");
                var response = client.Recieve();
                localSW.Stop();
                reses[i] = localSW.ElapsedMilliseconds;
                Thread.Sleep(10);
            }
            globalSW.Stop();

            for (int i = 0; i < NREQ; i++)
            {
                Console.WriteLine(reses[i]);
            }
        }
    }
}
