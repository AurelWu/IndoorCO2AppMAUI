using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Utilities;

namespace IndoorCO2App_Multiplatform
{
    internal static class Logger
    {
        public static CircularBuffer<string> circularBuffer = new CircularBuffer<string>(50000);

        public static void CopyLogToClipboard()
        {
            // Convert CircularBuffer contents to a single string
            var content = string.Join(Environment.NewLine, circularBuffer);

            // Copy the content to clipboard
            Clipboard.SetTextAsync(content);
        }
    }
}
