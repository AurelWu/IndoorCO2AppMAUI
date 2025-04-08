#if IOS
using CoreBluetooth;
using CoreLocation;
using Foundation;
using Microsoft.Maui.ApplicationModel;
using System.Threading.Tasks;
using UIKit;

namespace IndoorCO2App_Multiplatform
{
    public class BluetoothHelperApple : IBluetoothHelper
    {
        CBCentralManager bluetoothManager;
        CLLocationManager locationManager;

        public BluetoothHelperApple()
        {
            bluetoothManager = new CBCentralManager();
            locationManager = new CLLocationManager();
        }

        public bool CheckStatus()
        {
            var status = CBManager.Authorization == CBManagerAuthorization.AllowedAlways
            ? PermissionStatus.Granted
            : PermissionStatus.Denied;

            if (status == PermissionStatus.Granted) return true;
            else return false;
        }

        public async Task<PermissionStatus> RequestAsync()
        {
            // Check if Bluetooth and Location permissions are already granted
            if (CheckStatus())
                return PermissionStatus.Granted;

            // Request Location permission
            locationManager.RequestWhenInUseAuthorization();

            // Wait for authorization
            var tcs = new TaskCompletionSource<PermissionStatus>();
            locationManager.AuthorizationChanged += (sender, args) =>
            {
                if (args.Status == CLAuthorizationStatus.AuthorizedWhenInUse || args.Status == CLAuthorizationStatus.AuthorizedAlways)
                {
                    tcs.SetResult(PermissionStatus.Granted);
                }
                else
                {
                    tcs.SetResult(PermissionStatus.Denied);
                }
            };

            return await tcs.Task;
        }

        public void EnsureDeclared()
        {
            // Ensure Bluetooth and Location permissions are declared in Info.plist
            bool hasBluetoothUsageDescription = NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString("NSBluetoothAlwaysUsageDescription"));
            bool hasLocationWhenInUseUsageDescription = NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription"));

            if (!hasBluetoothUsageDescription || !hasLocationWhenInUseUsageDescription)
            {
                throw new PermissionException("Bluetooth and/or Location permissions are not set in Info.plist.");
            }
        }

        public async void RequestBluetoothEnable()
        {
            if (MainPage.bluetoothHelper.CheckIfBTEnabled()) return;
            // Show the dialog asking if the user wants to enable Bluetooth
            bool result = await App.Current.MainPage.DisplayAlert(
                "Enable Bluetooth",
                "Bluetooth is currently disabled. Would you like to enable it?",
                "Yes",
                "No");


            // If the user clicks "Yes", redirect them to the Bluetooth settings
            if (result)
            {
                var url = new NSUrl("App-Prefs:root=Bluetooth");
                if (UIApplication.SharedApplication.CanOpenUrl(url))
                {
                    UIApplication.SharedApplication.OpenUrl(url);                    
                }
            }
        }

        public bool CheckIfBTEnabled()
        {
            // Check if Bluetooth is enabled
            return bluetoothManager.State == CBManagerState.PoweredOn;
        }

        public bool HasPermissionInManifest(string permission)
        {           
            // Ensure Bluetooth and Location permissions are declared in Info.plist
            bool hasBluetoothUsageDescription = NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString("NSBluetoothAlwaysUsageDescription"));
            bool hasLocationWhenInUseUsageDescription = NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription"));

            if (!hasBluetoothUsageDescription || !hasLocationWhenInUseUsageDescription)
            {
                return false;
            }
            else return true;
        }
    }
}
#endif
