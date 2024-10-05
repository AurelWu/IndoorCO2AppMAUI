

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnPrerecordingSwitchToggled(object sender, EventArgs e)
        {
            prerecording = _PrerecordingSwitch.IsToggled;
        }

    }

}
