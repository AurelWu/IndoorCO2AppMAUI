

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async Task OnRequestBluetoothPermissionsDialogAsync(object sender, EventArgs e)
        {
            var status = await bluetoothHelper.RequestAsync();
        }

    }

}
