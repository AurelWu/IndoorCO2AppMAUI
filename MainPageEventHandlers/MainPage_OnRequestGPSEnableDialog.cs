

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async Task OnRequestGPSEnableDialogAsync(object sender, EventArgs e)
        {
            bool isActive = SpatialManager.CheckIfGpsIsEnabled();
            if (isActive) return; // won't do anything already active
            bool result = await SpatialManager.ShowEnableGPSDialogAsync();
        }

    }
}
