using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace IndoorCO2App_Android
{
    public class PersistentLogHelper
    {
        public static async Task CopyCrashLogToClipboardAsync()
        {
            string logPath = Path.Combine(FileSystem.AppDataDirectory, "persistentLog.txt");

            if (File.Exists(logPath))
            {
                string logContent = await File.ReadAllTextAsync(logPath);
                await Clipboard.SetTextAsync(logContent);
            }
        }
    }
}
