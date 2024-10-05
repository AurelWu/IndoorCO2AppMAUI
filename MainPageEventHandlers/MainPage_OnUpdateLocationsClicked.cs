

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnUpdateLocationsClicked(object sender, EventArgs e)
        {
            OverpassModule.FetchNearbyBuildingsAsync(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange, this);
        }
    }

}
