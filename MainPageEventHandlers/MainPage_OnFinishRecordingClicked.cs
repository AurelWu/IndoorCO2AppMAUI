
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnFinishRecordingClicked(object sender, EventArgs e)
        {
            try
            {

                if (submissionMode == SubmissionMode.Transit)
                {
                    selectedTransitTargetLocation = (LocationData)_TransitDestinationPicker.SelectedItem;
                    selectedTransitLine = (TransitLineData)_TransitLinePicker.SelectedItem;
                    if (selectedTransitTargetLocation == null)
                    {
                        bool result = await DisplayTransitSubmissionNoDestinationConfirmationDialogAsync();
                        if (result == false)
                        {
                            return;
                        }
                    }
                    if (selectedTransitLine == null)
                    {
                        return;
                    }

                }
                _FinishRecordingButton.Text = "Submitting Data";
                _FinishRecordingButton.IsEnabled = false; ;

                int trimStart = (int)Math.Floor(_TrimSlider.RangeStart);
                int trimEnd = (int)Math.Floor(_TrimSlider.RangeEnd);
                bool success = await BluetoothManager.FinishRecordingAsync(trimStart, trimEnd, submissionMode, _ManualNameEditor.Text, _ManualAddressEditor.Text);
                if (success)
                {
                    ResetRecordingStateAsync();
                    await ShowSuccessNotificationAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnFinishRecordingClicked: {ex}", false);
            }
        }

        private async Task ShowSuccessNotificationAsync()
        {
            
            _SuccessNotificationLabel.IsVisible = true;            
            await Task.Delay(2500);
            _SuccessNotificationLabel.IsVisible = false;
        }
    }

}
