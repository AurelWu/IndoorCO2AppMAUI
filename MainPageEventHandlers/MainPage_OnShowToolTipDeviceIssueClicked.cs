

using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;
using System.Collections.Generic;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnShowToolTipDeviceIssueClicked(object sender, EventArgs e)
        {
           
            if(monitorType == CO2MonitorType.Aranet4 && BluetoothManager.currentCO2Reading == 0 && BluetoothManager.isGattA2DP == true) //this should only happen if smart home integration is disabled
            {
                var popup = new SmarthomeInfoPopUp();
                this.ShowPopup(popup);
                //TODO: display info text with images
            }
            else if (monitorType == CO2MonitorType.Aranet4 && BluetoothManager.outdatedVersion == true)
            {
                var popup = new UpdateAranetPopUp();
                this.ShowPopup(popup);
            }
        }            
    }

}
