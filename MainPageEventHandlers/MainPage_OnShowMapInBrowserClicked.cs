

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnShowMapInBrowserClicked(object sender, EventArgs e)
        {
            var url = "https://indoorco2map.com/";
            Launcher.OpenAsync(url);
        }

    }

}
