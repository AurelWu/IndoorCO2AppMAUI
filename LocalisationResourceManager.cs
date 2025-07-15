using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace IndoorCO2App_Multiplatform
{
    public class LocalisationResourceManager : INotifyPropertyChanged
    {
        private static LocalisationResourceManager _instance;
        private ResourceManager _resourceManager;

        public event PropertyChangedEventHandler PropertyChanged;

        public static LocalisationResourceManager Instance => _instance ??= new LocalisationResourceManager();

        public LocalisationResourceManager() { }

        public void Init(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public string this[string key]
        {            
            get => _resourceManager?.GetString(key, App.appCulture) ?? $"!{key}!";
        }

        public string GetString(string key)
        {
            //Console.WriteLine("currentCulture when calling GetString from LocalisationManager: " + CultureInfo.CurrentUICulture);
            return _resourceManager?.GetString(key, App.appCulture) ?? $"!{key}!";
        }

        public void SetCulture(CultureInfo culture)
        {
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}