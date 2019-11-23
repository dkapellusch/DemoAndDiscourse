using System.Text;

namespace DemoAndDiscourse.RocksDb.Extensions
{
    public static class StringExtensions
    {
        public static string Pad(this string input, int padTo, char padChar)
        {
            var inputBuilder = new StringBuilder(input ?? string.Empty);

            while (inputBuilder.Length < padTo)
            {
                inputBuilder.Append(padChar);
            }

            return inputBuilder.ToString();
        }
    }
}