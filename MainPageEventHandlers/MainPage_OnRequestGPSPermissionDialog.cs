

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private async void OnRequestGPSPermissionDialog(object sender, EventArgs e)
        {

            bool granted = await SpatialManager.IsLocationPermissionGrantedAsync();
            if (granted) return; // won't do anything if we already got permission;

        }
    }
}

