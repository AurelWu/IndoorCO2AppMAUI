

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnUpdateLocationsClicked(object sender, EventArgs e)
        {
            if (currentMenuMode.HasFlag(MenuMode.Standard))
            {
                OverpassModule.FetchNearbyBuildingsAsync(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange, this);
            }
            else if (currentMenuMode.HasFlag(MenuMode.TransportRecording | MenuMode.TransportSelection))
            {
                //TODO
            }
        }
    }

}
