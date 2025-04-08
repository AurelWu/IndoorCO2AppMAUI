

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnStartTransportRecordingClicked(object sender, EventArgs e)
        {
            StartTransportRecordingAsync(false);
        }

    }

}
