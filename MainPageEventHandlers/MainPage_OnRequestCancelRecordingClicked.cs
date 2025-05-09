﻿

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnRequestCancelRecordingClicked(object sender, EventArgs e)
        {
            try
            {

                bool result = await DisplayAlert("Cancel Recording", "Are you sure you want to cancel the recording?", "Yes", "No");
                if (result == true)
                {
                    CancelRecording();
                }
            }
            catch (Exception ex) 
            {
                Logger.WriteToLog($"Error when calling OnRequestCancelRecordingClicked: {ex}", false);
            }
        }

    }

}
