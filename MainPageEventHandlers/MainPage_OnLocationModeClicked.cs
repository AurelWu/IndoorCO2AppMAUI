

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnLocationModeClicked(object sender, EventArgs e)
        {
            try
            {


                Button clickedButton = (Button)sender;
                // Reset both buttons to inactive state
                _TransitModeButton.BackgroundColor = Colors.LightGray;
                _BuildingModeButton.BackgroundColor = Colors.LightGray;
                _TransitModeButton.TextColor = Colors.Black;
                _BuildingModeButton.TextColor = Colors.Black;

                // Set the clicked button to active state
                clickedButton.BackgroundColor = Color.Parse("#512BD4");
                clickedButton.TextColor = Colors.White;

                if (clickedButton.Text == "Buildings")
                {
                    await ChangeToStandardUI(true);
                    UpdateUI();
                }
                else if (clickedButton.Text == "Transit")
                {
                    await ChangeToTransportSelectionUI(true);
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnLocationModeClicked {ex}", false);
            }
        }
    }

}
