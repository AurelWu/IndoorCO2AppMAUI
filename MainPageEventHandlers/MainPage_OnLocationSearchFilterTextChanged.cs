

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private void OnLocationSearchFilterTextChanged(object sender, EventArgs e)
        {
            if(currentMenuMode == MenuMode.TransportSelection)
            {
                UpdateTransitLinesPicker(false);
            }
            else if(currentMenuMode== MenuMode.Standard)
            {
                //=> need to add differentiation between filtered and unfiltered Results so not implemented yet
            }
        }
    }

}
