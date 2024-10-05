

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (_RadioButton50m.IsChecked)
            {
                searchRange = 50;
            }
            else if (_RadioButton100m.IsChecked)
            {
                searchRange = 100;
            }
            else if (_RadioButton250m.IsChecked)
            {
                searchRange = 250;
            }

        }

    }

}
