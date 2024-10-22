

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnFinishRecordingClicked(object sender, EventArgs e)
        {
            _FinishRecordingButton.Text = "Submitting Data";
            _FinishRecordingButton.IsEnabled = false; ;
            int trimStart = (int)Math.Floor(_TrimSlider.RangeStart);
            int trimEnd = (int)Math.Floor(_TrimSlider.RangeEnd);
            BluetoothManager.FinishRecording(trimStart, trimEnd, submissionMode ,_ManualNameEditor.Text, _ManualAddressEditor.Text);
            ResetRecordingState();
        }

    }

}
