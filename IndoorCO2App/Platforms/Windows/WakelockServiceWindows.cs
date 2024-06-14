using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    public class WakeLockServiceWindows : IWakeLockService
    {
        public void AcquireWakeLock()
        {
            // Dummy implementation for windows, as there's no equivalent to Android's wake lock
            Console.WriteLine("Acquiring wake lock on Windows does nothing.");
        }

        public void ReleaseWakeLock()
        {
            // Dummy implementation for windows, as there's no equivalent to Android's wake lock
            Console.WriteLine("Releasing wake lock on Windows does nothing.");
        }
    }
}
