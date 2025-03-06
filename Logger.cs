using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndoorCO2App_Android;
using Microsoft.VisualStudio.Utilities;

namespace IndoorCO2App_Multiplatform
{
    public static class Logger
    {
        public static CircularBuffer<string> circularBuffer = new CircularBuffer<string>(500000);

        public static void WriteToLog(string text, bool persistent)
        {
            DateTime dateTime = DateTime.Now;
            string textWithTimeStamp = text + " | " + dateTime.ToString();
            circularBuffer.Add(textWithTimeStamp);
            if(persistent)
            {
                LogPersistent(textWithTimeStamp);
            }
        }

        public static void CopyLogToClipboard()
        {
            // Convert CircularBuffer contents to a single string
            var content = string.Join(Environment.NewLine, circularBuffer);

            // Copy the content to clipboard
            Clipboard.SetTextAsync(content);
        }

        public static void LogError(string source, Exception? ex)
        {
            if (ex != null)
            {
                string logPath = Path.Combine(FileSystem.Current.AppDataDirectory, "persistentLog.txt");
                const int maxFileSize = 100 * 1024; // 100 KB max size

                string errorLog = $"{DateTime.UtcNow}: [{source}] {ex.Message}\n{ex.StackTrace}\n\n";

                // If the file exists and is too large, trim it
                if (File.Exists(logPath) && new FileInfo(logPath).Length > maxFileSize)
                {
                    string oldLogs = File.ReadAllText(logPath);
                    oldLogs = oldLogs.Substring(oldLogs.Length / 2); // Keep only last half
                    File.WriteAllText(logPath, oldLogs);
                }
                File.AppendAllText(logPath, " ");
                File.AppendAllText(logPath,"Crash Entry Start");
                File.AppendAllText(logPath, errorLog);
                File.AppendAllText(logPath,"Crash Entry End\r\n");
                File.AppendAllText(logPath, " ");
            }
        }

        private static void LogPersistent(string text)
        {
            string logPath = Path.Combine(FileSystem.Current.AppDataDirectory, "persistentLog.txt");
            const int maxFileSize = 100 * 1024; // 100 KB max size

            // If the file exists and is too large, trim it
            if (File.Exists(logPath) && new FileInfo(logPath).Length > maxFileSize)
            {
                string oldLogs = File.ReadAllText(logPath);
                oldLogs = oldLogs.Substring(oldLogs.Length / 2); // Keep only last half
                File.WriteAllText(logPath, oldLogs);
            }

            File.AppendAllText(logPath, text + System.Environment.NewLine);
        }
    }
}
