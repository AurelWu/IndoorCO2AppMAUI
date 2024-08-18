using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Core.Content;
using System.Threading.Tasks;
using Permission = Android.Content.PM.Permission;
using Android.OS;

namespace IndoorCO2App_Android
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            // Notify the permission result handler
            PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public static class PermissionsHandler
    {
        private static TaskCompletionSource<PermissionStatus> _tcs;

        public static void RequestPermissions(TaskCompletionSource<PermissionStatus> tcs)
        {
            _tcs = tcs;

            var permissions = new[]
            {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessFineLocation,
        };

            // Add BluetoothConnect and BluetoothScan permissions if on Android 13 or higher
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                permissions = permissions.Concat(new[]
                {
                Manifest.Permission.BluetoothConnect,
                Manifest.Permission.BluetoothScan
            }).ToArray();
            }

            Platform.CurrentActivity.RequestPermissions(permissions, requestCode: 1000);
        }

        public static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == 1000)
            {
                if (grantResults.Length > 0)
                {
                    // Determine if all permissions were granted
                    bool allPermissionsGranted = grantResults.All(result => result == Permission.Granted);

                    _tcs?.TrySetResult(allPermissionsGranted ? PermissionStatus.Granted : PermissionStatus.Denied);
                }
                else
                {
                    _tcs?.TrySetResult(PermissionStatus.Denied);
                }
            }
        }
    }
}
