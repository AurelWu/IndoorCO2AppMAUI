﻿

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnCheckBoxDoorsWindows_CheckedChanged(object sender, EventArgs e)
        {
            hasOpenWindowsDoors = _CheckBoxDoorsWindows.IsChecked;
            RecoveryData.windowsOpen = hasOpenWindowsDoors;
        }
    }

}
