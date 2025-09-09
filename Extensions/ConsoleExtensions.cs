using System.Xml.Linq;

namespace ChartAPI.Extensions
{
    public static class ConsoleExtensions
    {
        public static void WritewithTime(this Console console, string value)
        {
            Console.Write($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {value}\n");
        }
    }
}
