using System;
using System.IO;
using System.Text;

namespace MySchool.Classes
{
    /// <summary>
    /// Centralized logging utility for the MySchool application.
    /// Logs are written to both Debug output and a persistent log file.
    /// </summary>
    internal static class Logger
    {
        private static readonly object _logLock = new object();
        private static string? _logFilePath;

        private static string LogFilePath
        {
            get
            {
                if (_logFilePath == null)
                {
                    string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string logsDir = Path.Combine(appData, "MySchool", "logs");

                    try
                    {
                        if (!Directory.Exists(logsDir))
                        {
                            Directory.CreateDirectory(logsDir);
                        }

                        // Use date-based log file name for easier management
                        string logFileName = $"myschool_{DateTime.Now:yyyy-MM-dd}.log";
                        _logFilePath = Path.Combine(logsDir, logFileName);

                        // Clean up old log files (keep last 7 days)
                        CleanupOldLogs(logsDir);
                    }
                    catch
                    {
                        // Fallback to temp directory if AppData is not accessible
                        _logFilePath = Path.Combine(Path.GetTempPath(), "myschool.log");
                    }
                }
                return _logFilePath;
            }
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        public static void Info(string category, string message)
        {
            WriteLog("INFO", category, message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void Warning(string category, string message, Exception? exception = null)
        {
            var fullMessage = exception != null
            ? $"{message}\nException: {exception.GetType().Name}\nMessage: {exception.Message}"
                     : message;

            WriteLog("WARN", category, fullMessage);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public static void Error(string category, string message, Exception? exception = null)
        {
            var fullMessage = exception != null
                  ? $"{message}\nException: {exception.GetType().Name}\nMessage: {exception.Message}\nStack Trace: {exception.StackTrace}"
                          : message;

            WriteLog("ERROR", category, fullMessage);
        }

        /// <summary>
        /// Logs a debug message (only written to Debug output, not to file).
        /// </summary>
        public static void Debug(string category, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] [{category}] {message}");
        }

        private static void WriteLog(string level, string category, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] [{level}] [{category}] {message}";

            // Write to Debug output
            System.Diagnostics.Debug.WriteLine(logEntry);

            // Write to file
            lock (_logLock)
            {
                try
                {
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
                }
                catch
                {
                    // If logging fails, don't crash the app
                    System.Diagnostics.Debug.WriteLine($"Failed to write to log file: {LogFilePath}");
                }
            }
        }

        private static void CleanupOldLogs(string logsDirectory)
        {
            try
            {
                var logFiles = Directory.GetFiles(logsDirectory, "myschool_*.log");
                var cutoffDate = DateTime.Now.AddDays(-7);

                foreach (var logFile in logFiles)
                {
                    var fileInfo = new FileInfo(logFile);
                    if (fileInfo.LastWriteTime < cutoffDate)
                    {
                        try
                        {
                            File.Delete(logFile);
                            System.Diagnostics.Debug.WriteLine($"Deleted old log file: {logFile}");
                        }
                        catch
                        {
                            // Ignore errors when deleting old logs
                        }
                    }
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        /// <summary>
        /// Gets the path to the current log file.
        /// </summary>
        public static string GetLogFilePath()
        {
            return LogFilePath;
        }
    }
}
