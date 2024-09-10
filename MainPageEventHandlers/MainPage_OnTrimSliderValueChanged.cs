

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        private void OnTrimSliderValueChanged(object sender, Syncfusion.Maui.Sliders.RangeSliderValueChangedEventArgs e)
        {             
            //if this doesnt work then we change script in MainPageUI entirely to here?
            endTrimSliderHasBeenUsed = true;
            if (e.NewRangeEnd < _TrimSlider.Maximum)
            {
                endtrimSliderIsAtmax = false;
            }
        }
    }

}
