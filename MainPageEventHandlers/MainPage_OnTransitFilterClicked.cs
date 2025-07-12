

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnTransitFilterClicked(object sender, EventArgs e)
        {
            try
            {

                if (currentMenuMode == MenuMode.TransportRecording)
                {
                    bool r = await DisplayFilterConfirmationDialogAsync();
                    if (!r) return;
                }

                Button clickedButton = (Button)sender;
                // Reset both buttons to inactive state
                _TransitFilterAllButton.BackgroundColor = Colors.LightGray;
                _TransitFilterBusButton.BackgroundColor = Colors.LightGray;
                _TransitFilterTramButton.BackgroundColor = Colors.LightGray;
                _TransitFilterSubwayButton.BackgroundColor = Colors.LightGray;
                _TransitFilterLightRailButton.BackgroundColor = Colors.LightGray;
                _TransitFilterTrainButton.BackgroundColor = Colors.LightGray;
                _TransitFilterAllButton.TextColor = Colors.Black;
                _TransitFilterBusButton.TextColor = Colors.Black;
                _TransitFilterTramButton.TextColor = Colors.Black;
                _TransitFilterSubwayButton.TextColor = Colors.Black;
                _TransitFilterLightRailButton.TextColor = Colors.Black;
                _TransitFilterTrainButton.TextColor = Colors.Black;

                // Set the clicked button to active state
                clickedButton.BackgroundColor = Color.Parse("#512BD4");
                clickedButton.TextColor = Colors.White;
                //set filter to this text
                //this will not work well with translation!
                if (clickedButton.ClassId.ToLower() == "all")
                {
                    TransitFilter = TransitFilterMode.All;
                }
                else if (clickedButton.ClassId.ToLower() == "bus")
                {
                    TransitFilter = TransitFilterMode.Bus;
                }
                else if (clickedButton.ClassId.ToLower() == "tram")
                {
                    TransitFilter = TransitFilterMode.Tram;
                }
                else if (clickedButton.ClassId.ToLower() == "subway")
                {
                    TransitFilter = TransitFilterMode.Subway;
                }
                else if (clickedButton.ClassId.ToLower() == "lightrail")
                {
                    TransitFilter = TransitFilterMode.LightRail;
                }
                else if (clickedButton.ClassId.ToLower() == "train")
                {
                    TransitFilter = TransitFilterMode.Train;
                }
                UpdateTransitLinesPicker(false);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnTransitFilterClicked: {ex}", false);
            }

        }

        private async Task<bool> DisplayFilterConfirmationDialogAsync()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Change Transport Mode Filter?", // Title
                "Are you sure you want to change Filter Settings? This might remove the currently selected Line.", // Message
                "Confirm", // Confirm button
                "Cancel" // Cancel button
            );

            return answer; // Returns true if 'Confirm' is pressed, false if 'Cancel' is pressed
        }
    }

}
