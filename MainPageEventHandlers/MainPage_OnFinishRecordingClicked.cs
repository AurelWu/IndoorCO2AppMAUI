

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnFinishRecordingClicked(object sender, EventArgs e)
        {
            _FinishRecordingButton.Text = "Submitting Data";
            _FinishRecordingButton.IsEnabled = false; ;
            int trimStart = (int)Math.Floor(_StartTrimSlider.Value);
            int trimEnd = (int)Math.Floor(_EndTrimSlider.Value);
            BluetoothManager.FinishRecording(trimStart, trimEnd, manualRecordingMode, _ManualNameEditor.Text, _ManualAddressEditor.Text);
        }

    }

}
