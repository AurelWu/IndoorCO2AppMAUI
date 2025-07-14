

using CommunityToolkit.Maui.Views;
using IndoorCO2App_Android;
using Microsoft.VisualStudio.Utilities;

namespace IndoorCO2App_Multiplatform
{
    public partial class MainPage : ContentPage
    {
        private async void OnLanguagePickerChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            var selectedLanguage = picker.SelectedItem as string;

            // Example: handle language change logic
            switch (selectedLanguage)
            {
                case "English":
                    App.SetCulture("en");
                    Preferences.Set("AppLanguage", "en");                    
                    break;
                case "Français":
                    App.SetCulture("fr");
                    Preferences.Set("AppLanguage", "fr");
                    break;
                case "Deutsch":
                    App.SetCulture("de");
                    Preferences.Set("AppLanguage", "de");
                    break;
                case "Português":
                    App.SetCulture("pt");
                    Preferences.Set("AppLanguage", "pt");
                    break;
            }                        
        }
    }

}
