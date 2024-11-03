

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnCO2MonitorPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstInit) return;
            BluetoothManager.discoveredDevices = null;
            string picked = _CO2DevicePicker.SelectedItem.ToString();
            int index = _CO2DevicePicker.SelectedIndex;

            Preferences.Set(SelectedMonitorPreferenceKey, index);

            if (picked != null)
            {
                if (picked == "Aranet")
                {
                    monitorType = CO2MonitorType.Aranet4;
                }
                else if (picked == "Airvalent")
                {
                    monitorType = CO2MonitorType.Airvalent;
                }
                else if (picked == "Inkbird IAM-T1")
                {
                    monitorType = CO2MonitorType.InkbirdIAMT1;
                }
                else if (picked == "airCoda")
                {
                    monitorType = CO2MonitorType.AirCoda;
                }
                else if(picked == "AirSpot Health")
                {
                    monitorType = CO2MonitorType.AirSpot;
                }
            }
        }

    }

}
