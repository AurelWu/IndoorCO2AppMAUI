

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnTransitFilterClicked(object sender, EventArgs e)
        {
            if(currentMenuMode == MenuMode.TransportRecording)
            {
                bool r = await DisplayFilterConfirmationDialog();
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
            if (clickedButton.Text.ToLower()=="all")
            {
                TransitFilter = TransitFilterMode.All;
            }
            else if(clickedButton.Text.ToLower() == "bus")
            {
                TransitFilter = TransitFilterMode.Bus;
            }
            else if (clickedButton.Text.ToLower() == "tram")
            {
                TransitFilter = TransitFilterMode.Tram;
            }
            else if(clickedButton.Text.ToLower() == "subway")
            {
                TransitFilter = TransitFilterMode.Subway;
            }
            else if(clickedButton.Text.ToLower() == "light_rail")
            {
                TransitFilter = TransitFilterMode.LightRail;
            }
            else if (clickedButton.Text.ToLower() == "train")
            {
                TransitFilter = TransitFilterMode.Train;
            }
            UpdateTransitLinesPicker();            
        }

        private async Task<bool> DisplayFilterConfirmationDialog()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Change Transport Mode Filter?", // Title
                "Are you sure you want to change Filter Settings, this will remove the current selected Line?", // Message
                "Confirm", // Confirm button
                "Cancel" // Cancel button
            );

            return answer; // Returns true if 'Confirm' is pressed, false if 'Cancel' is pressed
        }
    }

}
