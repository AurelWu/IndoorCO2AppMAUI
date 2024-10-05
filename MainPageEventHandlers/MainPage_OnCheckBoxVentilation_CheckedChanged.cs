

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnCheckBoxVentilation_CheckedChanged(object sender, EventArgs e)
        {
            hasOpenWindowsDoors = _CheckBoxDoorsWindows.IsChecked;
        }
    }

}
