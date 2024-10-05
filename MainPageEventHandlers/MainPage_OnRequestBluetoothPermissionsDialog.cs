

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnRequestBluetoothPermissionsDialog(object sender, EventArgs e)
        {
            var status = await bluetoothHelper.RequestAsync();
        }

    }

}
