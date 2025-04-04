﻿
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnFinishRecordingClicked(object sender, EventArgs e)
        {
            if (submissionMode == SubmissionMode.Transit)
            {
                selectedTransitTargetLocation = (LocationData)_TransitDestinationPicker.SelectedItem;
                selectedTransitLine = (TransitLineData)_TransitLinePicker.SelectedItem;
                if (selectedTransitTargetLocation == null)
                {
                    bool result = await DisplayTransitSubmissionNoDestinationConfirmationDialog();
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
            bool success = await BluetoothManager.FinishRecording(trimStart, trimEnd, submissionMode ,_ManualNameEditor.Text, _ManualAddressEditor.Text);            
            if(success)
            {
                ResetRecordingState();
                await ShowSuccessNotificationAsync();
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
