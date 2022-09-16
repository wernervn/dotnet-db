using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;
using DotnetSql.Services;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using static DotnetSql.Commands.ParameterDescriptions;

namespace DotnetSql.Commands
{
    [Command(
    Name = "dotnet sql",
    FullName = "dotnet-sql",
    Description = ".Net global tool to execute SQL scripts on the command-line."
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    internal class DbCommand
    {
        private static string GetVersion()
        => typeof(DbCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        #region Arguments
        [Argument(0, Description = StartPathDescription)]
        public string StartPath { get; set; }

        [Option("--server", SetServerDescription, CommandOptionType.SingleValue,
            ShortName = "s", LongName = "server")]
        public string ServerName { get; set; }

        [Option("--database", SetDBDescription, CommandOptionType.SingleValue,
            ShortName = "d", LongName = "database")]
        public string Database { get; set; }

        [Option("--show-connection", ShowConnectionDescription, CommandOptionType.NoValue,
        ShortName = "c", LongName = "show-connection")]
        public bool ShowConnection { get; set; } = false;

        [Option(CommandOptionType.SingleValue, Description = ScriptFileDescription,
            ShortName = "r", LongName = "run-file")]
        public string ScriptFile { get; set; }

        [Option(CommandOptionType.SingleValue, Description = BatchFileDescription,
            ShortName = "b", LongName = "batch-file")]
        public string BatchFile { get; set; }
        #endregion

        private async Task OnExecuteAsync()
        {
            if (string.IsNullOrEmpty(StartPath))
            {
                StartPath = GetCurrentDirectory();
            }

            var console = PhysicalConsole.Singleton;

            if (!console.IsOutputRedirected)
            {
                ClearCurrentConsoleLine();
            }
            else
            {
                console.WriteLine();
            }

            if (IsParametersValid())
            {
                var service = new SqlExecuteService();

                if (!string.IsNullOrWhiteSpace(ServerName))
                {
                    EnvironmentHelper.SetUserVariable(SqlExecuteService.DOTNET_DB_SERVER, ServerName);
                    Log.Information("Connection string: {Connection}", service.ConnectionString);
                }

                if (!string.IsNullOrWhiteSpace(Database))
                {
                    EnvironmentHelper.SetUserVariable(SqlExecuteService.DOTNET_DB_DATABASE, Database);
                    Log.Information("Connection string: {Connection}", service.ConnectionString);
                }

                if (ShowConnection)
                {
                    Log.Information("Connection string: {Connection}", service.ConnectionString);
                }

                if (File.Exists(ScriptFile))
                {
                    if (await service.IsValidConnection())
                    {
                        await service.Execute(ScriptFile).ConfigureAwait(false);
                    }
                    else
                    {
                        Log.Error("Invalid connection: {@Connection}", service.ConnectionString);
                    }
                    return;
                }

                if (File.Exists(BatchFile))
                {
                    if (await service.IsValidConnection())
                    {
                        await service.ExecuteBatch(BatchFile).ConfigureAwait(false);
                    }
                    else
                    {
                        Log.Error("Invalid connection: {@Connection}", service.ConnectionString);
                    }
                    return;
                }

                //console.WriteLine("\r\nDONE!!");
                WriteHelp();
            }
            else
            {
                WriteHelp();
            }
        }

        #region Helpers
        // Start path should either be the path to a solution file, or a directory to search for C# (.cs) files
        private bool IsParametersValid()
        {
            if (!string.IsNullOrEmpty(ScriptFile) && !Path.IsPathRooted(ScriptFile))
            {
                ScriptFile = Path.Combine(StartPath, ScriptFile);
                if (!File.Exists(ScriptFile))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(BatchFile) && !Path.IsPathRooted(BatchFile))
            {
                BatchFile = Path.Combine(StartPath, BatchFile);
                if (!File.Exists(BatchFile))
                {
                    return false;
                }
            }

            return true;
        }

        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private static string GetCurrentDirectory()
        {
            var fs = new FileSystem();
            return fs.Directory.GetCurrentDirectory();
        }

        #endregion

        private static void WriteHelp()
        {
            var help = $@"
Usage: dotnet global-funcs <Path>

Arguments:
  Path                                       {StartPathDescription}

Options:
  --version                                  Show version information.
  -?|-h|--help                               Show this help message.
  -s|--server <SERVER>                       {SetServerDescription}
  -d|--database <DATABASE>                   {SetDBDescription}
  -c|--show-connection                       {ShowConnectionDescription}
  -f|--script-file                           {ScriptFileDescription}
";
            Console.Write(help);
        }
    }
}
