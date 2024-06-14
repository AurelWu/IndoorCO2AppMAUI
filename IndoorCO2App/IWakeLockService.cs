using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    public interface IWakeLockService
    {
        void AcquireWakeLock();
        void ReleaseWakeLock();
    }
}
