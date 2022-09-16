using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotnetSql.Commands;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace DotnetSql
{
    static class Program
    {
        static int Main(string[] args)
        {
            var quitOptions = new string[] { "exit", "x", "quit", "q" };
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            if (Debugger.IsAttached)
            {
                while (true)
                {
                    var input = Prompt.GetString("Enter arguments> ");
                    var inputSplit = input?.Split(' ') ?? new string[1];

                    if (inputSplit[0] == "clear")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (quitOptions.Contains(inputSplit[0], StringComparer.OrdinalIgnoreCase))
                    {
                        // Exit out
                        return 0;
                    }

                    _ = RunApp(inputSplit).GetAwaiter().GetResult();
                }
            }
            else
            {
                return RunApp(args).GetAwaiter().GetResult();
            }
        }

        private static async Task<int> RunApp(string[] args)
            => await CommandLineApplication.ExecuteAsync<DbCommand>(args).ConfigureAwait(false);
    }

}
