

using IndoorCO2App_Android;
using System.Collections.Generic;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnFavouriteBuildingIconClicked(object sender, EventArgs e)
        {
            try
            {


                ImageButton clickedButton = (ImageButton)sender;
                if (_LocationPicker.SelectedItem == null)
                {
                    return;
                }

                LocationData d = (LocationData)_LocationPicker.SelectedItem;
                if (d != null)
                {
                    long id = d.ID;
                    string type = d.Type;
                    string combined = type + "_" + id.ToString();
                    if (!favouredLocations.Add(combined))
                    {
                        favouredLocations.Remove(combined);
                    }
                    await FileStorage.SaveFavouritesHashSetAsync(favouredLocations);
                    UpdateLocationPicker(false);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnFavouriteBuildingIconClicked: {ex}", false);
            }
        }

        private async void OnFavouriteTransitOriginIconClicked(object sender, EventArgs e)
        {
            try
            {


                //TODO: replace with transit origin specific code
                ImageButton clickedButton = (ImageButton)sender;
                if (_TransitOriginPicker.SelectedItem == null)
                {
                    return;
                }

                LocationData d = (LocationData)_TransitOriginPicker.SelectedItem;
                if (d != null)
                {
                    long id = d.ID;
                    string type = d.Type;
                    string combined = type + "_" + id.ToString();
                    if (!favouredLocations.Add(combined))
                    {
                        favouredLocations.Remove(combined);
                    }
                    await FileStorage.SaveFavouritesHashSetAsync(favouredLocations);
                    UpdateTransitOriginPicker(false);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnFavouriteTransitOriginIconClicked: {ex}", false);
            }
        }

        private async void OnFavouriteTransitDestinationIconClicked(object sender, EventArgs e)
        {
            try
            {
                //TODO: replace with transit origin specific code
                ImageButton clickedButton = (ImageButton)sender;
                if (_TransitDestinationPicker.SelectedItem == null)
                {
                    return;
                }

                LocationData d = (LocationData)_TransitDestinationPicker.SelectedItem;
                if (d != null)
                {
                    long id = d.ID;
                    string type = d.Type;
                    string combined = type + "_" + id.ToString();
                    if (!favouredLocations.Add(combined))
                    {
                        favouredLocations.Remove(combined);
                    }
                    await FileStorage.SaveFavouritesHashSetAsync(favouredLocations);

                    UpdateTransitDestinationPicker(false);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnFavouriteTransitDestinationIconClicked: {ex}", false);
            }
        }

        private async void OnFavouriteTransitLineIconClicked(object sender, EventArgs e)
        {
            try
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
                    await FileStorage.SaveFavouritesHashSetAsync(favouredLocations);
                    UpdateTransitLinesPicker(false);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error when calling OnFavouriteTransitLineIconClicked: {ex}", false);
            }
        }
    }

}
