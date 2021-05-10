using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUI
{
    public class AsyncConsole
    {
        static int lines = 0;

        static void LineReader(string[] result)
        {
            result[0] = Console.ReadLine();
            DrawInputSection();
        }

        static void KeyWaiter()
        {
            Console.ReadKey();
            DrawInputSection();
        }

        public async static Task ReadLine(Action<string> callback, CancellationToken token = default)
        {
            while (true)
            {
                string[] result = new string[1];
                var inputTask = new Task(() => LineReader(result), token);
                inputTask.Start();
                await inputTask;
                if (token.IsCancellationRequested)
                {
                    break;
                }
                callback(result[0]);
            }
        }

        public async static Task WaitKey()
        {
            var inputTask = new Task(KeyWaiter);
            inputTask.Start();
            await inputTask;
        }

        static void DrawInputSection()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.Write(new string(' ', Console.WindowWidth * 2));
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            string separator = new string('-', Console.WindowWidth);
            Console.Write($"{separator}\n> ");
            Console.SetCursorPosition(0, 0);
            Console.SetCursorPosition(2, Console.WindowHeight - 1);
        }

        public static void Init()
        {
            Console.Clear();
            DrawInputSection();
            lines = 0;
        }

        static void WriteSingleLine(string line)
        {
            int leftpos = Console.CursorLeft;
            int toppos = Console.CursorTop;
            if (lines == Console.WindowHeight - 2)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(new string(' ', Console.WindowWidth * (Console.WindowHeight - 2)));
                lines = 0;
            }
            Console.SetCursorPosition(0, lines);
            Console.WriteLine(line);
            lines++;
            Console.SetCursorPosition(leftpos, toppos);
        }

        static List<string> SplitByLines(string lines)
        {
            string[] _lines = lines.Split('\n');
            List<string> output = new List<string>(_lines.Length);

            foreach (string line in _lines)
            {
                for (int i = 0; i < line.Length; i += Console.WindowWidth)
                {
                    output.Add(line.Substring(i, Math.Min(line.Length - i, Console.WindowWidth)));
                }
            }

            return output;
        }

        public static void WriteLine(string line)
        {
            List<string> lines = SplitByLines(line);
            foreach (var _line in lines)
            {
                WriteSingleLine(_line);
            }
        }
    }
}
