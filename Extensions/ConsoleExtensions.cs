using System.Xml.Linq;

namespace ChartAPI.Extensions
{
    public static class ConsoleExtensions
    {
        public static void WriteLineWithTime<T>(T value)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {value}");
        }
        public static void WriteWithTime<T>(T value)
        {
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {value}");
        }
    }
}
