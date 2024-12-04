

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnDeviceNameFilterValueChanged(object sender, EventArgs e)
        {
            var entry = sender as Entry;

            if (entry != null)
            {
                string s = entry.Text;
                Preferences.Set(DeviceNameFilterPreferenceKey, s);                
            }
        }

    }

}
