using System.Text;

namespace Project3Vitour.Helpers
{
    public static class TranslationDebugLogger
    {
        private static readonly string LogPath = @"C:\Users\TUFAN\source\repos\Project3Vitour\Project3Vitour\translation-debug.log";
        private static readonly object Sync = new();

        public static void Log(string message)
        {
            try
            {
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}";

                lock (Sync)
                {
                    File.AppendAllText(LogPath, line, Encoding.UTF8);
                }
            }
            catch
            {
            }
        }

        public static string GetLogPath()
        {
            return LogPath;
        }
    }
}
