// In Platforms/Android/LocationServiceAndroid.cs

using CoreBluetooth;
using Foundation;
using UIKit;

namespace IndoorCO2App
{
    internal class BluetoothServiceiOS : BluetoothService
    {
        internal override bool IsBluetoothEnabled()
        {
            return BluetoothManagerSingleton.CentralManager.State == CBManagerState.PoweredOn;
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
                var url = new NSUrl("App-Prefs:root=Bluetooth");
                if (UIApplication.SharedApplication.CanOpenUrl(url))
                {
                    UIApplication.SharedApplication.OpenUrl(url);
                }
            }

            return result;
        }
    }

    
    public static class BluetoothManagerSingleton
    {
        private static CBCentralManager _centralManager;

        public static CBCentralManager CentralManager
        {
            get
            {
                if (_centralManager == null)
                {
                    _centralManager = new CBCentralManager();
                }
                return _centralManager;
            }
        }
    }
    

}