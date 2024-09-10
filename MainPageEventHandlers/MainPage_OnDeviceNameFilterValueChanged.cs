

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnDeviceNameFilterValueChanged(object sender, EventArgs e)
        {
            var editor = sender as Editor;

            if (editor != null)
            {
                string s = editor.Text;
                Preferences.Set(DeviceNameFilterPreferenceKey, s);
            }
        }

    }

}
