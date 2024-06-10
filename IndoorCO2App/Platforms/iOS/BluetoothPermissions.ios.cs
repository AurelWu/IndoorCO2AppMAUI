using Microsoft.Maui.ApplicationModel;
using CoreBluetooth;
using CoreLocation;
using Foundation;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    /// <summary>
    /// MAUI Bluetooth Permission Declaration (for iOS).
    /// </summary>
    /// <remarks>
    /// IMPORTANT: Put this file in `Platforms/iOS/` directory.
    /// </remarks>
    public partial class BluetoothPermissions : Permissions.BasePlatformPermission
    {
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            var status = CBManager.Authorization == CBManagerAuthorization.AllowedAlways
                ? PermissionStatus.Granted
                : PermissionStatus.Denied;
            return Task.FromResult(status);
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            if (CBManager.Authorization == CBManagerAuthorization.AllowedAlways)
                return PermissionStatus.Granted;

            var locationManager = new CLLocationManager();
            locationManager.RequestWhenInUseAuthorization();
            locationManager.RequestAlwaysAuthorization();

            var tcs = new TaskCompletionSource<PermissionStatus>();

            locationManager.AuthorizationChanged += (sender, e) =>
            {
                if (e.Status == CLAuthorizationStatus.AuthorizedAlways || e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
                {
                    tcs.TrySetResult(PermissionStatus.Granted);
                }
                else
                {
                    tcs.TrySetResult(PermissionStatus.Denied);
                }
            };

            // Trigger the Bluetooth permission request.
            var centralManager = new CBCentralManager();
            centralManager.UpdatedState += (sender, e) =>
            {
                if (centralManager.State == CBManagerState.PoweredOn)
                {
                    tcs.TrySetResult(PermissionStatus.Granted);
                }
                else
                {
                    tcs.TrySetResult(PermissionStatus.Denied);
                }
            };

            return await tcs.Task;
        }

        public override void EnsureDeclared()
        {
            // Ensure that the necessary keys are in Info.plist
            if (!NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString("NSBluetoothAlwaysUsageDescription")) ||
                !NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString("NSBluetoothPeripheralUsageDescription")))
            {
                throw new PermissionException("Bluetooth permission descriptions are not set in Info.plist.");
            }

            if (!NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")) ||
                !NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString("NSLocationAlwaysUsageDescription")))
            {
                throw new PermissionException("Location permission descriptions are not set in Info.plist.");
            }
        }
    }
}
