

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnResumeRecordingClicked(object sender, EventArgs e)
        {
            StartRecording(false, true);
        }
    }

}
