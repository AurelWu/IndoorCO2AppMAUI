

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnRequestGPSPermissionDialog(object sender, EventArgs e)
        {
            try
            {
                bool granted = await SpatialManager.IsLocationPermissionGrantedAsync();
                if (granted) return; // won't do anything if we already got permission;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnRequestGPSEnableDialogAsync: {ex}", false);
            }

        }
    }
}

