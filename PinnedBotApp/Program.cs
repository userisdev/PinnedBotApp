using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PinnedBotApp
{
    /// <summary> Program class. </summary>
    internal static class Program
    {
        /// <summary> Defines the entry point of the application. </summary>
        /// <param name="args"> The arguments. </param>
        private static async Task Main(string[] args)
        {
            string token = args.FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("token error.");
                Environment.Exit(1);
            }

            string execPath = Assembly.GetExecutingAssembly().Location;
            string dirPath = Path.GetDirectoryName(execPath);
            string execNameWithoutExtension = Path.GetFileNameWithoutExtension(execPath);
            string logFilePath = Path.Combine(dirPath, $"{execNameWithoutExtension}.log");
            Trace.Listeners.Clear();
            _ = Trace.Listeners.Add(new TextWriterTraceListener(logFilePath, "LogFile"));
            _ = Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.AutoFlush = true;

            PinnedBot bot = new PinnedBot(token);
            await bot.RunAsync();
            Environment.Exit(0);
        }
    }
}
