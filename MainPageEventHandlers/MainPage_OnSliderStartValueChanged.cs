

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnSliderStartValueChanged(object sender, ValueChangedEventArgs e)
        {
            startTrimSliderHasBeenUsed = true;
        }
    }

}
