

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnSliderEndValueChanged(object sender, ValueChangedEventArgs e)
        {
            endTrimSliderHasBeenUsed = true;
            if (_EndTrimSlider.Value < _EndTrimSlider.Maximum)
            {
                endtrimSliderIsAtmax = false;
            }
        }
    }

}
