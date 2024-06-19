
#if IOS
using CoreBluetooth;
using Foundation;
using UIKit;

#endif

namespace IndoorCO2App
{
    internal class BluetoothService
    {
        internal bool IsBluetoothEnabled()
        {
#if IOS
            return BluetoothManagerSingleton.CentralManager.State == CBManagerState.PoweredOn;
#endif
            return false;
        }

        internal async Task<bool> ShowEnableBluetoothDialogAsync()
        {
            bool result = await App.Current.MainPage.DisplayAlert(
                "Enable Bluetooth",
                "Bluetooth is currently disabled. Would you like to enable it?",
                "Yes",
                "No");
#if IOS
            if (result)
            {

                var url = new NSUrl("App-Prefs:root=Bluetooth");
                if (UIApplication.SharedApplication.CanOpenUrl(url))
                {
                    UIApplication.SharedApplication.OpenUrl(url);
                }
            }
#endif

            return result;
        }
    }


    public static class BluetoothManagerSingleton
    {
#if IOS
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
#endif
    }
}