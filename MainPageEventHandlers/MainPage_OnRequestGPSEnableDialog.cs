

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private async void OnRequestGPSEnableDialog(object sender, EventArgs e)
        {
            bool isActive = SpatialManager.CheckIfGpsIsEnabled();
            if (isActive) return; // won't do anything already active
            bool result = await SpatialManager.ShowEnableGPSDialogAsync();
        }

    }
}
