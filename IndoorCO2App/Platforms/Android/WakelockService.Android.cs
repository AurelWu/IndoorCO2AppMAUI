using Android.Content;
using Android.OS;
using Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    public class WakeLockServiceAndroid : IWakeLockService
    {
        private PowerManager.WakeLock _wakeLock;

        public void AcquireWakeLock()
        {
            var powerManager = (PowerManager)Android.App.Application.Context.GetSystemService(Context.PowerService);
            _wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "MyApp::WakeLockTag");
            _wakeLock.Acquire();
        }

        public void ReleaseWakeLock()
        {
            _wakeLock?.Release();
            _wakeLock = null;
        }
    }
}
