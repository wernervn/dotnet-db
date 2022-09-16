using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using Serilog;

namespace DotnetSql.Services
{
    public class SqlExecuteService
    {
        public const string DOTNET_DB_SERVER = nameof(DOTNET_DB_SERVER);
        public const string DOTNET_DB_DATABASE = nameof(DOTNET_DB_DATABASE);

        public SqlExecuteService()
        {
            var server = EnvironmentHelper.GetUserVariable(DOTNET_DB_SERVER);
            var db = EnvironmentHelper.GetUserVariable(DOTNET_DB_DATABASE);
            ConnectionString = $"Data Source={server};Initial Catalog={db};Integrated Security=True;";
        }

        public string ConnectionString { get; }

        #region Helpers
        private async Task<SqlConnection> ConnectionAsync()
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            return connection;
        }
        #endregion Helpers

        public async Task<bool> IsValidConnection()
        {
            try
            {
                using var connection = await ConnectionAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task Execute(string scriptFile)
        {
            using var connection = await ConnectionAsync().ConfigureAwait(false);
            try
            {
                foreach (var sql in await GetCommands(scriptFile))
                {
                    await connection.ExecuteAsync(sql, commandType: CommandType.Text, commandTimeout: 60).ConfigureAwait(false);
                }
                Log.Information("Statement from {File} executed", scriptFile);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Statement from {File} failed", scriptFile);
            }
        }

        public async Task ExecuteBatch(string batchFile)
        {
            foreach (var script in await File.ReadAllLinesAsync(batchFile))
            {
                var scriptFile = Path.IsPathRooted(script) ? script : Path.Combine(Path.GetDirectoryName(batchFile), script);
                await Execute(scriptFile);
            }
        }

        private async Task<IEnumerable<string>> GetCommands(string scriptFile)
        {
            var text = await File.ReadAllTextAsync(scriptFile);
            return text.Split("GO", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}
