

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnRequestBluetoothPermissionsDialog(object sender, EventArgs e)
        {
            try
            {
                var status = await bluetoothHelper.RequestAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnRequestBluetoothPermissionsDialog {ex}", false);
            }
            
        }

    }

}
