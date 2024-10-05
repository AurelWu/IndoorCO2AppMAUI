

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnRequestBluetoothEnableDialog(object sender, EventArgs e)
        {
            bluetoothHelper.RequestBluetoothEnable();
        }

    }

}
