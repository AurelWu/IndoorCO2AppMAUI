

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnStartManualRecordingClicked(object sender, EventArgs e)
        {
            try
            {
                string msg = "Only use this recording mode if the location is not in the List above or if receiving Locations does not work currently. Recordings in this mode are not put into the map instantly but manually looked at and then added to the Map if the location can be validated. Using this mode the exact GPS Coordinates taken during the recording duration will be submitted at the end!";

                bool result = await DisplayAlert("Manual Recording Mode", msg, "Understood", "Cancel");
                if (result == true)
                {
                    await StartRecording(SubmissionMode.BuildingManual, false);
                }
            }
            catch (Exception ex) 
            {
                Logger.WriteToLog($"Error when calling OnStartManualRecordingClicked: {ex}", false);
            }
            
        }
    }

}
