

using Microsoft.Maui.Controls.Internals;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnCheckBoxVentilation_CheckedChanged(object sender, EventArgs e)
        {
            hasVentilationSystem = _CheckBoxVentilation.IsChecked;
            RecoveryData.windowsOpen = hasVentilationSystem;
        }
    }

}
