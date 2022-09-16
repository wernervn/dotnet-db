using System;

namespace DotnetSql.Services
{
    public static class EnvironmentHelper
    {
        public static string GetUserVariable(string variable)
            => Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);

        public static void SetUserVariable(string variable, string value)
            => Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
    }
}
