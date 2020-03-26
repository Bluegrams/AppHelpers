using System;
using System.IO;

namespace Bluegrams.Application
{
    /// <summary>
    /// A simple logger that logs into a text file in the temp folder.
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// The default logger instance.
        /// </summary>
        public static Logger Default { get; } = new Logger();

        private readonly string file;

        private Logger()
        {
            file = Path.Combine(Path.GetTempPath(), AppInfo.ProductName.ToLower() + ".log");
        }

        /// <summary>
        /// Logs a new exception.
        /// </summary>
        /// <param name="message">The logging message.</param>
        /// <param name="ex">The exception to be logged.</param>
        public void Log(string message, Exception ex)
        {
            string logEntry = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}] {message}";
            File.AppendAllLines(file, new[] { logEntry, ex.ToString() });
        }
    }
}
