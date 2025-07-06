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
            get => _resourceManager?.GetString(key, CultureInfo.CurrentUICulture) ?? $"!{key}!";
        }

        public void SetCulture(CultureInfo culture)
        {
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}