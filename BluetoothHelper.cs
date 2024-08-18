#if ANDROID
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;
using System.Threading.Tasks;
using Application = Android.App.Application;
#endif

namespace IndoorCO2App_Android
{

    public class BluetoothHelper
    {
        public static bool CheckStatus()
        {
#if ANDROID
            // Check if the permissions for Bluetooth and Location are granted
            var bluetoothPermission = ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.Bluetooth) == Permission.Granted;
            var bluetoothAdminPermission = ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.BluetoothAdmin) == Permission.Granted;
            var locationPermission = ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.AccessFineLocation) == Permission.Granted;

            // For Android 13 and later, check the additional Bluetooth permissions
            bool bluetoothConnectPermission = Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu
            ? ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.BluetoothConnect) == Permission.Granted
            : true; // BluetoothConnect is not required on earlier versions

            bool bluetoothScanPermission = Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu
                ? ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.BluetoothScan) == Permission.Granted
                : true; // BluetoothScan is not required on earlier versions

            return bluetoothPermission && bluetoothAdminPermission && locationPermission && bluetoothConnectPermission && bluetoothScanPermission;
#endif
            return false;
        }

        public static async Task<PermissionStatus> RequestAsync()
        {
            // Check if the permissions are already granted
            if (CheckStatus())
                return PermissionStatus.Granted;

            // Request permissions
            var tcs = new TaskCompletionSource<PermissionStatus>();
#if ANDROID
            PermissionsHandler.RequestPermissions(tcs);
#endif
            return await tcs.Task;
        }

        public void EnsureDeclared()
        {
#if ANDROID
            // Check if the necessary permissions are declared in the AndroidManifest.xml
            bool hasBluetoothPermission = HasPermissionInManifest(Android.Manifest.Permission.Bluetooth);
            bool hasBluetoothAdminPermission = HasPermissionInManifest(Android.Manifest.Permission.BluetoothAdmin);
            bool hasLocationPermission = HasPermissionInManifest(Android.Manifest.Permission.AccessFineLocation);
            bool hasBluetoothConnectPermission = Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu
                ? HasPermissionInManifest(Android.Manifest.Permission.BluetoothConnect)
                : true; // BluetoothConnect is not required on earlier versions
            bool hasBluetoothScanPermission = Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu
                ? HasPermissionInManifest(Android.Manifest.Permission.BluetoothScan)
                : true; // BluetoothScan is not required on earlier versions

            if (!hasBluetoothPermission ||
                !hasBluetoothAdminPermission ||
                !hasLocationPermission ||
                !hasBluetoothConnectPermission ||
                !hasBluetoothScanPermission)
            {
                throw new PermissionException("Bluetooth and/or Location permissions are not set in AndroidManifest.xml.");
            }
#endif
        }

        private bool HasPermissionInManifest(string permission)
        {
#if ANDROID
            var packageInfo = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, PackageInfoFlags.Permissions);
            foreach (var perm in packageInfo.RequestedPermissions)
            {
                if (perm.Equals(permission, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
#endif
            return false;
        }

        public static void RequestBluetoothEnable()
        {
#if ANDROID
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (bluetoothAdapter == null)
            {
                // Device does not support Bluetooth
                return;
            }

            if (!bluetoothAdapter.IsEnabled)
            {
                // Bluetooth is not enabled, show a dialog to ask the user to enable it
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                Platform.CurrentActivity.StartActivityForResult(enableBtIntent, 1);
            }
            else
            {
                // Bluetooth is already enabled
            }
#endif
        }

        public static bool CheckIfBTEnabled()
        {
        #if ANDROID
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if(bluetoothAdapter == null)
            {
                // Device does not support Bluetooth
                return false;
            }

            if (!bluetoothAdapter.IsEnabled)
            {
                return false;
            }
            else
            {
                return true;
            }
        #endif
            return false;
        
        }


    }
}













