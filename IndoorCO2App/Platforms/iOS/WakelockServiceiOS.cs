using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    public class WakeLockServiceiOS : IWakeLockService
    {
        public void AcquireWakeLock()
        {
            // Dummy implementation for iOS, as there's no equivalent to Android's wake lock
            Console.WriteLine("Acquiring wake lock on iOS does nothing.");
        }

        public void ReleaseWakeLock()
        {
            // Dummy implementation for iOS, as there's no equivalent to Android's wake lock
            Console.WriteLine("Releasing wake lock on iOS does nothing.");
        }
    }
}
