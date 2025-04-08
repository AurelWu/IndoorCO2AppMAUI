

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async Task OnRequestGPSPermissionDialogAsync(object sender, EventArgs e)
        {

            bool granted = await SpatialManager.IsLocationPermissionGrantedAsync();
            if (granted) return; // won't do anything if we already got permission;

        }
    }
}

