

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnStartRecordingClicked(object sender, EventArgs e)
        {
            StartRecording(SubmissionMode.Building,false);
        }

    }

}
