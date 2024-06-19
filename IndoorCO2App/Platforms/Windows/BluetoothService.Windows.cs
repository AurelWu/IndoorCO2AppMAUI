// In Platforms/Android/LocationServiceAndroid.cs


namespace IndoorCO2App
{
    internal class BluetoothServiceWindows : BluetoothService
    {
        internal override bool IsBluetoothEnabled()
        {
            return false;
        }

        internal override Task<bool> ShowEnableBluetoothDialogAsync()
        {
            throw new NotImplementedException();
        }
    }
}