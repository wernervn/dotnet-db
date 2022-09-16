namespace DotnetSql.Commands
{
    public static class ParameterDescriptions
    {
        public const string StartPathDescription = "The path to a directory containing C# files. If none is specified, the current directory will be used.";
        public const string SetServerDescription = "Sets the Server component of the connection. Defaults to '(LocalDB)\\MSSQLLocalDB'";
        public const string SetDBDescription = "Sets the Initial Catalog component of the connection.";
        public const string ShowConnectionDescription = "Shows the connection string that will be used to execute scripts.";
        public const string ScriptFileDescription = "The SQL script that will be executed. The full path is only required if executed from a different folder.";
        public const string BatchFileDescription = "A text file containing paths to Sql script files. The full path is only required if located in a different folder.";
    }
}
