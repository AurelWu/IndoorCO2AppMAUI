// In Platforms/Android/LocationServiceAndroid.cs


using Android.Bluetooth;
using Android.Content;

namespace IndoorCO2App
{
    internal class BluetoothServiceAndroid : BluetoothService
    {
        internal override bool IsBluetoothEnabled()
        {
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            return bluetoothAdapter?.IsEnabled ?? false;
        }

        internal override async Task<bool> ShowEnableBluetoothDialogAsync()
        {
            bool result = await App.Current.MainPage.DisplayAlert(
                "Enable Bluetooth",
                "Bluetooth is currently disabled. Would you like to enable it?",
                "Yes",
                "No");

            if (result)
            {
                var intent = new Intent(BluetoothAdapter.ActionRequestEnable);
                Platform.CurrentActivity.StartActivityForResult(intent, 1);
            }

            return result;
        }
    }
}