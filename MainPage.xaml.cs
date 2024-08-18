

using IndoorCO2App_Android.Controls;

namespace IndoorCO2App_Android
{
    public partial class MainPage : ContentPage
    {
        public static int startTrimSliderValue = 0;
        public static int endTrimSliderValue = 1;
        Dictionary<VisualElement, MenuMode> MenuModesOfUIElements;

        public MainPage()
        {
            InitializeComponent();
            InitUIElements();
            ChangeToStandardUI();
        }

        private void ChangeToUI(MenuMode mode)
        {
            foreach (var element in MenuModesOfUIElements)
            {
                var k = element.Key;
                var flags = element.Value;
                if (flags.HasFlag(mode))
                {
                    k.IsVisible = true;
                }
                else
                {
                    k.IsVisible = false;
                }
            }
        }

        public void ChangeToStandardUI()
        {
            ChangeToUI(MenuMode.Standard);
        }

        public void ChangeToRecordingUI()
        {
            ChangeToUI(MenuMode.Recording);
        }

        public void ChangeToManualRecordingUI()
        {
            ChangeToUI(MenuMode.ManualRecording);
        }

        public void InitUIElements()
        {
            MenuModesOfUIElements = new Dictionary<VisualElement, MenuMode>();
            MenuModesOfUIElements.Add(this.FindByName<ImageButton>("ButtonGPSStatus"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<ImageButton>("ButtonGPSPermission"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<ImageButton>("ButtonBluetoothStatus"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<ImageButton>("ButtonBluetoothPermissions"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("CO2MonitorPickerStackLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Label>("StatusLabel"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Label>("DeviceLabel"), MenuMode.Recording | MenuMode.ManualRecording | MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Label>("LocationLabel"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("SearchRangeStackLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("UpdateLocationsButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<VerticalStackLayout>("LocationStackLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("ResumeRecordingButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("StartRecordingButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("StartManualRecordingButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("PrerecordingLayout"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("OpenMapButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("OpenImprintButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Button>("DeleteLastSubmissionButton"), MenuMode.Standard);
            MenuModesOfUIElements.Add(this.FindByName<Label>("LocationLabelRecording"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("RecordingModeButtonStackLayout"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackManualName"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackManualAddress"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<LineChartView>("lineChartView"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Label>("RecordedDataLabel"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("TrimSliderLayout"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Label>("TrimSliderInfoText"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<Grid>("StackNotes"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("StackCheckboxesDoor"), MenuMode.Recording | MenuMode.ManualRecording);
            MenuModesOfUIElements.Add(this.FindByName<HorizontalStackLayout>("StackCheckboxesVentilation"), MenuMode.Recording | MenuMode.ManualRecording);
        }
    }

}
