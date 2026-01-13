using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayClient.Host.Cli.Helpers
{
    public class ConsoleHelper
    {
        public static void WriteLineSuccess(string str)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(str);
            Console.ForegroundColor = c;
        }

        public static void WriteLineInfo(string str)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(str);
            Console.ForegroundColor = c;
        }

        public static void WriteLineHighlight(string str)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(str);
            Console.ForegroundColor = c;
        }

        public static void WriteLineError(string str)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(str);
            Console.ForegroundColor = c;
        }

        public static void WriteLineWarning(string str)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(str);
            Console.ForegroundColor = c;
        }
    }
}
