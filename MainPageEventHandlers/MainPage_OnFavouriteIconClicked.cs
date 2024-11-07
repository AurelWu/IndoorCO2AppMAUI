

using IndoorCO2App_Android;
using System.Collections.Generic;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnFavouriteBuildingIconClicked(object sender, EventArgs e)
        {
            ImageButton clickedButton = (ImageButton)sender;
            if(_LocationPicker.SelectedItem== null)
            {
                return;
            }

            LocationData d = (LocationData)_LocationPicker.SelectedItem;
            if (d != null)
            {
                long id =d.ID;
                string type = d.type;
                string combined = type + "_" + id.ToString();
                if (!favouredLocations.Add(combined))
                {
                    favouredLocations.Remove(combined);
                }
                await FileStorage.SaveHashSetAsync(favouredLocations);
            }
        }

        private async void OnFavouriteTransitLineIconClicked(object sender, EventArgs e)
        {
            ImageButton clickedButton = (ImageButton)sender;
            if (_TransitLinePicker.SelectedItem == null)
            {
                return;
            }

            TransitLineData d = (TransitLineData)_TransitLinePicker.SelectedItem;
            if (d != null)
            {
                long id = d.ID;
                string type = d.NWRType;
                string combined = type + "_" + id.ToString();
                if (!favouredLocations.Add(combined))
                {
                    favouredLocations.Remove(combined);
                }
                await FileStorage.SaveHashSetAsync(favouredLocations);
            }
        }
    }

}
