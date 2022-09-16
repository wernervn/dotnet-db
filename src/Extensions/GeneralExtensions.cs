namespace DotnetSql.Extensions
{
    public static class GeneralExtensions
    {
        public static string StripQuotes(this string text)
        {
            return text.Replace("\"", string.Empty).Replace("'", string.Empty);
        }
    }
}
