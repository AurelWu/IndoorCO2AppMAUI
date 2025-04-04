﻿

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
            _TransitModeButton.TextColor = Colors.Black;
            _BuildingModeButton.TextColor = Colors.Black;

            // Set the clicked button to active state
            clickedButton.BackgroundColor = Color.Parse("#512BD4");
            clickedButton.TextColor = Colors.White;

            if (clickedButton.Text=="Buildings")
            {
                ChangeToStandardUI(true);
                UpdateUI();
            }
            else if(clickedButton.Text=="Transit")
            {
                ChangeToTransportSelectionUI(true);
                UpdateUI();
            }
        }
    }

}
