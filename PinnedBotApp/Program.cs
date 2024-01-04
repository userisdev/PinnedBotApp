using System;
using System.Linq;
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
            var token = args.FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("token error.");
                Environment.Exit(1);
            }

            var bot = new PinnedBot(token);
            await bot.RunAsync();
            Environment.Exit(0);
        }
    }
}
