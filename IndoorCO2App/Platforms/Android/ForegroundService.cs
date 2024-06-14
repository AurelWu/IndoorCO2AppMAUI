using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using AndroidX.Core.App;
    using System;
    using System.Threading.Tasks;

    [Service]
    public class ForegroundService : Service
    {
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        private PeriodicTimer _timer;
        private DateTime _timeOfLastGPSUpdate = DateTime.MinValue;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var notification = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                .SetContentTitle("CO2 Monitoring Service")
                .SetContentText("Service is running in the background")                
                .SetOngoing(true)
                .Build();

            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);

            Task.Run(() => RunService());

            return StartCommandResult.Sticky;
        }

        private async Task RunService()
        {
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await _timer.WaitForNextTickAsync())
            {
                BluetoothManager.Update();
                // Update any required status or logs here
                var currentCO2Reading = BluetoothManager.currentCO2Reading;

                if (DateTime.Now - _timeOfLastGPSUpdate > TimeSpan.FromSeconds(15))
                {
                    SpatialManager.UpdateLocation();
                    _timeOfLastGPSUpdate = DateTime.Now;
                }
            }
        }

        public override IBinder OnBind(Intent intent) => null;

        public override void OnDestroy()
        {
            _timer.Dispose();
            base.OnDestroy();
        }
    }
}
