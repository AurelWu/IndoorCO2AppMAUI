

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnResumeRecordingClicked(object sender, EventArgs e)
        {
            string mode = RecoveryData.recordingMode;
            if(mode == "Building")
            {
                StartRecording(submissionMode, true);
            }
            else if(mode == "Transit")
            {
                StartTransportRecording(true);
            }
            
        }
    }

}
