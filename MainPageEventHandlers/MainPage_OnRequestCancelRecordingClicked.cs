

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async Task OnRequestCancelRecordingClickedAsync(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Cancel Recording", "Are you sure you want to cancel the recording?", "Yes", "No");
            if (result == true)
            {
                CancelRecordingAsync();
            }
        }

    }

}
