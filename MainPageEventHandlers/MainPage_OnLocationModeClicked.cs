

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnLocationModeClicked(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            // Reset both buttons to inactive state
            _TransitModeButton.BackgroundColor = Colors.LightGray;
            _BuildingModeButton.BackgroundColor = Colors.LightGray;

            // Set the clicked button to active state
            clickedButton.BackgroundColor = Colors.LightBlue;

            if(clickedButton.Text=="Buildings")
            {
                ChangeToStandardUI();
            }
            else if(clickedButton.Text=="Transit")
            {
                ChangeToTransportSelectionUI();
            }
        }
    }

}
