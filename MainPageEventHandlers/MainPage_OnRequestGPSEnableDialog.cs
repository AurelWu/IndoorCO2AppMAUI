

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnRequestGPSEnableDialog(object sender, EventArgs e)
        {
            try
            {
                bool isActive = SpatialManager.CheckIfGpsIsEnabled();
                if (isActive) return; // won't do anything already active
                bool result = await SpatialManager.ShowEnableGPSDialogAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnRequestGPSEnableDialog {ex}", false);
            }
            
        }

    }
}
