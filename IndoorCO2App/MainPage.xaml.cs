
//TODO: check if works on Android
//TODO: Overpass Module
//TODO: iphone Developer Account ?!?
//TODO: Layout
//TODO: Button Functionality

using Microsoft.Maui;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace IndoorCO2App;


//TODO: create lineChart
//TODO: create double Sided Slider
//TODO: create Loggint TextView
//TODO: fix that first minute the value of t-1 is also shown, which then later disappears (cause probably that minimum 2 values are fetched?)11

public partial class MainPage : ContentPage
{
    bool startTrimSliderHasBeenUsed = false;
    bool endTrimSliderHasBeenUsed = false;

    public static int startTrimSliderValue = 0;
    public static int endTrimSliderValue = 1;

    bool gpsGranted = false;
    bool gpsActive = false;
    bool btActive = false;
    bool btGranted = false;

	int count = 0;
    private readonly PeriodicTimer _timer;
    int searchRange = 100;
    private DateTime timeOfLastGPSUpdate = DateTime.MinValue;
    internal List<LocationData> Locations { get; set; }
    LocationData selectedLocation;
    public static MainPage MainPageSingleton;

    public static bool hasOpenWindowsDoors;
    public static bool hasVentilationSystem;
    //public static IWakeLockService wakeLockService;

    //public const string CHANNEL_ID = "com.companyname.indoorco2app.channel";

    public MainPage()
	{
        MainPageSingleton = this;
        Locations = new List<LocationData>();        
        //TODO => Overpass search range based on radio button instead of hardcoded 100
        //TODO: => record data when recording mode
        //TODO: => draw Linechart when recording
        //TODO: => add cancel button when recording (with confirmation)
        //TODO: => add submit Data Button (with info if success or not stuff)
        //TODO: => Data submission

        InitializeComponent();
        BluetoothManager.Init();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(0.4));

        UISetup();
        SwitchToStandardUI();
        Update();
    }

    private void UISetup()
    {
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        // Calculate 70% of the screen width
        var buttonWidth70Percent = screenWidth * 0.7;
        var buttonWidth60Percent = screenWidth * 0.6;
        var buttonWidth50Percent = screenWidth * 0.5;
        var buttonWidth30Percent = screenWidth * 0.3;
        var buttonWidth25Percent = screenWidth * 0.25;

        // Set the button's minimum width
        StartRecordingButton.MinimumWidthRequest = buttonWidth70Percent;
        UpdateLocationsButton.MinimumWidthRequest = buttonWidth70Percent;
        OpenMapButton.MinimumWidthRequest = buttonWidth70Percent;
        OpenImprintButton.MinimumWidthRequest = buttonWidth70Percent;
        FinishRecordingButton.MinimumWidthRequest = buttonWidth60Percent;
        FinishRecordingButton.MaximumWidthRequest = buttonWidth60Percent;
        RequestCancelRecordingButton.MinimumWidthRequest = buttonWidth25Percent;
        RequestCancelRecordingButton.MaximumWidthRequest = buttonWidth25Percent;
        ConfirmCancelRecordingButton.MinimumWidthRequest = buttonWidth25Percent;
        ConfirmCancelRecordingButton.MaximumWidthRequest = buttonWidth25Percent;        
        RadioButton100m.IsChecked = true;
    }

    private void SwitchToRecordingUI()
    {
        //wakeLockService.AcquireWakeLock();
        //StackRangeTrimmer.IsVisible = true;        
        FinishRecordingButton.Text = "Submit Data";
        FinishRecordingButton.IsEnabled = true;
        FinishRecordingButton.IsVisible = true;
        RequestCancelRecordingButton.IsVisible = true;
        ConfirmCancelRecordingButton.IsVisible = false;
        //RecordedDataLabel.IsVisible = true; //RecordedDataLabel is temporary instead of lineChart
        LocationLabelRecording.IsVisible = true;
        LocationLabel.IsVisible = false;
        SearchRangeStackLayout.IsVisible = false;
        LocationStackLayout.IsVisible = false;
        StartRecordingButton.IsVisible = false;
        OpenImprintButton.IsVisible = false;
        OpenMapButton.IsVisible = false;
        UpdateLocationsButton.IsVisible = false;
        //StackLayoutTrimSliderStart.IsVisible = true;
        //StackLayoutTrimSliderEnd.IsVisible = true;        
        StackCheckboxesDoorVentilation.IsVisible = true;
        StackNotes.IsVisible = false;
        lineChartView.IsVisible = true;
        startTrimSlider.IsVisible = true;
        endTrimSlider.IsVisible = true;

        startTrimSliderHasBeenUsed = false;
        endTrimSliderHasBeenUsed = false;
    }

    private void SwitchToStandardUI() 
    {
        //wakeLockService.ReleaseWakeLock();
        //StackRangeTrimmer.IsVisible = true;
        FinishRecordingButton.IsVisible = false;
        RequestCancelRecordingButton.IsVisible = false;
        ConfirmCancelRecordingButton.IsVisible = false;
        //RecordedDataLabel.IsVisible = false; //RecordedDataLabel is temporary instead of lineChart
        LocationLabelRecording.IsVisible = false;
        LocationLabel.IsVisible = true;
        SearchRangeStackLayout.IsVisible = true;
        LocationStackLayout.IsVisible = true;
        StartRecordingButton.IsVisible = true;
        OpenImprintButton.IsVisible = true;
        OpenMapButton.IsVisible = true;
        UpdateLocationsButton.IsVisible = true;
        //StackLayoutTrimSliderStart.IsVisible = false;
        //StackLayoutTrimSliderEnd.IsVisible = false;        
        StackCheckboxesDoorVentilation.IsVisible = false;
        StackNotes.IsVisible = false; ;
        lineChartView.IsVisible = false;
        startTrimSlider.IsVisible = false;
        endTrimSlider.IsVisible = false;
    }

    private void OnUpdateLocationsClicked(object sender, EventArgs e)
    {
        //SemanticScreenReader.Announce(UpdateLocationsButton.Text);
        OverpassModule.FetchNearbyBuildingsAsync(SpatialManager.currentLocation.Latitude, SpatialManager.currentLocation.Longitude, searchRange,this); //TODO => 
    }

    private void OnStartRecordingClicked(object sender, EventArgs e)
    {    	    
    	//SemanticScreenReader.Announce(StartRecordingButton.Text);

        //Console.WriteLine(LocationPicker);
        if(LocationPicker != null && Locations.Count>0) 
        {
            if(LocationPicker.SelectedItem != null)
            {
                selectedLocation = (LocationData)LocationPicker.SelectedItem;
            }
            SwitchToRecordingUI();
            BluetoothManager.StartNewRecording(selectedLocation, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }        
    }

    private void OnFinishRecordingClicked(object sender, EventArgs e)
    {
        FinishRecordingButton.Text = "Submitting Data";
        FinishRecordingButton.IsEnabled = false; ;
        int trimStart = (int)Math.Floor(startTrimSlider.Value);
        int trimEnd = (int)Math.Floor(endTrimSlider.Value);
        BluetoothManager.FinishRecording(trimStart,trimEnd);
    }
    
    private void OnRequestCancelRecordingClicked(object sender, EventArgs e)
    {
        RequestCancelRecordingButton.IsVisible = false;
        ConfirmCancelRecordingButton.IsVisible = true;
    }

    private void OnConfirmCancelRecordingClicked(object sender, EventArgs e)
    {
        BluetoothManager.StopRecording();
        SwitchToStandardUI();
    }

    private void OnShowMapInBrowserClicked(object sender, EventArgs e)
    {
        //SemanticScreenReader.Announce(OpenMapButton.Text);
        var url = "http://indoorco2map.s3-website.eu-central-1.amazonaws.com/";
        Launcher.OpenAsync(url);

    }

    private void OnImprintClicked(object sender, EventArgs e)
    {
        var url = "https://www.bluestats.net/Datenschutz.html";
        Launcher.OpenAsync(url);
    }

    private void OnRadioButtonCheckedChanged(object sender, EventArgs e)
    {        
        if (RadioButton50m.IsChecked)
        {
            searchRange = 50;
        }
        else if (RadioButton100m.IsChecked)
        {
            searchRange = 100;
        }
        else if (RadioButton250m.IsChecked)
        {
            searchRange = 250;
        }        
    }


    private async void Update()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync())
            {
                UpdateUI();
                
                
                BluetoothManager.Update();

                //non-UI Stuff now done in foreground Service
                if (DateTime.Now - timeOfLastGPSUpdate > TimeSpan.FromSeconds(15))
                {
                    SpatialManager.UpdateLocation();
                    timeOfLastGPSUpdate = DateTime.Now;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Timer was stopped
        }
    }

    private void UpdateUI()
    {
        UpdateGPSStatusButton();
        UpdateGPSPermissionButton();
        UpdateBluetoothStatusButton();
        UpdateBluetoothPermissionsButton();
        

        UpdateLocationLabel();
        UpdateLocationRecordingLabel();
        //UpdateRecordedDataLabel();
        UpdateLineChart();
        UpdateStatusLabel();
        UpdateDeviceLabel();
        UpdateStartRecordingButton();
        UpdateFinishRecordingButton();
    }

    private void UpdateStartRecordingButton()
    {
        if(gpsActive && gpsGranted && btGranted && btActive && OverpassModule.LocationData.Count > 0 && BluetoothManager.discoveredDevices.Count>0 && BluetoothManager.currentCO2Reading > 0)
        {                        
            StartRecordingButton.IsEnabled = true;
        }
        else
        {
            StartRecordingButton.IsEnabled = false;
        }
    }

    private void UpdateFinishRecordingButton()
    {
        int original = BluetoothManager.recordedData.Count;
        int afterTrimmedStart = original - (int)Math.Floor(startTrimSlider.Value);
        int afterTrimmedEnd = afterTrimmedStart - (BluetoothManager.recordedData.Count - (int)((BluetoothManager.recordedData.Count - (int)Math.Floor(endTrimSlider.Value))));
        if ( afterTrimmedEnd  >= 5 && BluetoothManager.isRecording)
        {
            FinishRecordingButton.IsEnabled = true;
            FinishRecordingButton.Text = "Submit Data";
        }
        else if(BluetoothManager.isRecording)
        {
            FinishRecordingButton.IsEnabled = false;
            FinishRecordingButton.Text = "Submit Data (needs 5 Minutes of Data)";
        }
    }

    private void UpdateStatusLabel()
    {
        {
            StatusLabel.Text=("");
            if (!gpsActive) StatusLabel.Text += "GPS not enabled | ";            
            if (!gpsGranted) StatusLabel.Text += "Location Permission missing |";
            if (!btActive) StatusLabel.Text += "Bluetooth not enabled |";
            if (!btGranted) StatusLabel.Text += "Bluetooth permission not granted";
            if (gpsActive && gpsGranted && btActive && btGranted) StatusLabel.Text = "GPS & Bluetoooth Permissions and Status okay";
        }
    }

    private async void UpdateGPSPermissionButton()
    {
        gpsGranted = await SpatialManager.IsLocationPermissionGrantedAsync();
        if(gpsGranted)
        {             
            ButtonGPSPermission.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonGPSPermission.BackgroundColor = Color.Parse("Red");
        }        
    }

    private void UpdateGPSStatusButton()
    {
        gpsActive = SpatialManager.CheckIfGpsIsEnabled();
        if(gpsActive)
        {
            ButtonGPSStatus.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonGPSStatus.BackgroundColor = Color.Parse("Red");
        }
    }

    private void UpdateBluetoothStatusButton()
    {
        btActive = BluetoothManager.bluetoothService.IsBluetoothEnabled();
        if(btActive)
        {
            ButtonBluetoothStatus.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonBluetoothStatus.BackgroundColor = Color.Parse("Red");
        }
    }

    private void UpdateBluetoothPermissionsButton()
    {
        btGranted = BluetoothPermissions.CheckStatus();
        if(btGranted) 
        {
            ButtonBluetoothPermissions.BackgroundColor = Color.Parse("Green");
        }
        else
        {
            ButtonBluetoothPermissions.BackgroundColor = Color.Parse("Red");
        }
    }

    private void UpdateLocationLabel()
    {
        if (gpsActive && gpsGranted)
        {
            if (SpatialManager.currentLocation.Latitude != 0 || SpatialManager.currentLocation.Longitude != 0)
            {
                LocationLabel.Text = "Lat: " + SpatialManager.currentLocation.Latitude.ToString("0.######") + " | Lon:" + SpatialManager.currentLocation.Longitude.ToString("0.######");
            }
            else
            {
                LocationLabel.Text = "GPS enabled and Location permissions granted. Getting first Location Info. This might take a minute";
            }
        }
        else
        {
            LocationLabel.Text = ("");
            if (!gpsActive) LocationLabel.Text += ("GPS not enabled ");
            if (!gpsGranted) LocationLabel.Text += ("Location Permission missing");            
        }
    }

    private void UpdateDeviceLabel()
    {
        try
        {
            if (!btGranted || !btActive)
            {
                DeviceLabel.Text = "Bluetooth not enabled or permissions missing, can not fetch Sensor Data";
            }
            else if (BluetoothManager.discoveredDevices.Count == 0)
            {
                DeviceLabel.Text = "Aranet Device not yet found. This might take a while.";

                if (BluetoothManager.lastAttemptFailed)
                {
                    DeviceLabel.Text += " | previous update failed";
                }
            }
            else if (BluetoothManager.sensorUpdateInterval > 60)
            {
                DeviceLabel.Text = "Device found but Update Interval not set to 1 Minute, change to 1 Minute using official App. next attempt in " + BluetoothManager.timeToNextUpdate + "s";
            }
            else if (BluetoothManager.currentCO2Reading != 0 && BluetoothManager.gattStatus == 0) //TODO also add check if last reading was a success maybe?         
            {
                DeviceLabel.Text = "CO2 Levels: " + BluetoothManager.currentCO2Reading + " |  initiating Update in: " + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | Gatt Status: " + BluetoothManager.gattStatus;
                if (BluetoothManager.lastAttemptFailed)
                {
                    DeviceLabel.Text += " | previous update failed";
                }
            }
            else if (BluetoothManager.currentCO2Reading == 0 && BluetoothManager.isGattA2DP == true)
            {
                DeviceLabel.Text = "Sensor found, but the required 'Smart Home Integration' is disabled.\r\n Please enable it using the official Aranet App (use the Gears Icon)";
            }
            else if (BluetoothManager.currentCO2Reading == 0)
            {
                DeviceLabel.Text = "initiating first Update in:" + BluetoothManager.timeToNextUpdate + "s" + "\r\n | rssi: " + BluetoothManager.rssi + " | Gatt Status: " + BluetoothManager.gattStatus;
                if (BluetoothManager.lastAttemptFailed)
                {
                    DeviceLabel.Text += " | previous update failed";
                }
            }
        }
        catch
        {
            DeviceLabel.Text = "update failed - next attempt in: " + BluetoothManager.timeToNextUpdate;
            Debug.WriteLine("UpdateDeviceLabel - exception caught");
        }
        

    }

    private void UpdateLocationRecordingLabel()
    {
        if (selectedLocation != null)
        {
            LocationLabelRecording.Text = "Location: " + selectedLocation.Name;
        }
        else
        {
            LocationLabelRecording.Text = "No Location Selected";
        }
    }

    private void UpdateRecordedDataLabel()
    {
        //var formattedString = new FormattedString();
        //
        //// Create spans with different colors
        //var labelspan = new Span
        //{
        //    Text = "recorded CO2-Values: ",
        //    //TextColor = Colors.Black
        //};
        //var dataSpan = new Span
        //{
        //    Text = "recorded CO2-Values: ",
        //    //TextColor = Colors.Black
        //};
        //formattedString.Spans.Add(labelspan);
        ////formattedString.Spans.Add(dataSpan);
        //
        //
        //
        ////string s = String.Join(" ", BluetoothManager.recordedData);
        ////RecordedDataLabel.FormattedText = " recorded CO2-Values: ";
        //
        //try
        //{
        //    for (int i = 0; i < BluetoothManager.recordedData.Count; i++) //what happens if value changes during looping over it? =>
        //    {
        //        //var x = testSlider;
        //        int start = (int)Math.Floor(sliderStart.Value);
        //        int end = (int)((BluetoothManager.recordedData.Count - 1)-Math.Floor(sliderEnd.Value)); //last Index
        //        if (i < start || i > end)
        //        {
        //            Span s = new Span
        //            {
        //                Text = BluetoothManager.recordedData[i].CO2ppm.ToString() + " ",
        //                TextColor = Colors.Gray
        //            };
        //            formattedString.Spans.Add(s);
        //        }
        //        else
        //        {
        //            Span s = new Span
        //            {
        //                Text = BluetoothManager.recordedData[i].CO2ppm.ToString() + " ",
        //                //TextColor = Colors.Black
        //            };
        //            formattedString.Spans.Add(s);
        //        }
        //    }
        //    RecordedDataLabel.FormattedText = formattedString;
        //}
        //catch (Exception)
        //{
        //    RecordedDataLabel.Text = "retrieving Data failed, next try in 1 Minute"; //maybe keep old Data in case this happens?
        //}
    }

    private void UpdateLineChart()
    {
        lineChartView.SetData(BluetoothManager.recordedData);
        int maxSliderVal = BluetoothManager.recordedData.Count;

        startTrimSlider.Minimum = 0;
        startTrimSlider.Maximum = maxSliderVal;
        endTrimSlider.Minimum = 0;        
        
        
        if (endTrimSlider.Maximum == 0)
        {
            endTrimSlider.Maximum = 1;
            endTrimSlider.Value = 1;
        }
        else if(maxSliderVal>0) 
        {            
            endTrimSlider.Maximum = maxSliderVal;
            
        }

        if (!endTrimSliderHasBeenUsed && maxSliderVal>0)
        {
            endTrimSlider.Value = endTrimSlider.Maximum;
        }
        startTrimSliderValue = (int)startTrimSlider.Value;
        endTrimSliderValue = (int)endTrimSlider.Value;

        
    }

    private async void OnRequestBluetoothEnableDialog(object sender, EventArgs e)
    {
        bool isActive = BluetoothManager.bluetoothService.IsBluetoothEnabled();
        if (isActive) return; // won't do anything already active
        bool result = await BluetoothManager.bluetoothService.ShowEnableBluetoothDialogAsync();
        //TODO
    }

    private async void OnRequestGPSEnableDialog(object sender, EventArgs e)
    {
        bool isActive = SpatialManager.CheckIfGpsIsEnabled();
        if (isActive) return; // won't do anything already active
        bool result = await SpatialManager.ShowEnableGPSDialogAsync();
    }

    private async void OnRequestBluetoothPermissionsDialog(object sender, EventArgs e)
    {
        bool granted = BluetoothPermissions.CheckStatus();
        if (granted) return; // won't do anything if we already got permissions;
        //TODO
    }

    private async void OnRequestGPSPermissionDialog(object sender, EventArgs e)
    {
        bool granted = await SpatialManager.IsLocationPermissionGrantedAsync();
        if (granted) return; // won't do anything if we already got permission;
        //TODO
    }

    public void OnTransmissionFailed(string msg)
    {
        FinishRecordingButton.Text = msg;
        FinishRecordingButton.IsEnabled = true;
        //TODO: add Delay til returning to other UI
    }
    public void OnTransmissionSuccess(string msg)
    {
        FinishRecordingButton.Text = "Transmission successful!";
        Application.Current.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(4), () =>
        {
            SwitchToStandardUI();
        });
        //Submitting seems to work but Lambda doesnt write to DB so maybe message not correct yet? Compare with android message        
    }

    public void UpdateLocationPicker()
    {        
        Locations = OverpassModule.LocationData;
        if (Locations.Count == 0)
        {
            return;
        }
        LocationPicker.ItemsSource = null;
        LocationPicker.Items.Clear();
        LocationPicker.ItemsSource = Locations;


        if (Locations.Count > 0)
        {
            LocationPicker.SelectedItem = Locations[0];
        }            
    }

    private void OnSliderStartValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Update the label with the new slider value
        //sliderStartValueLabel.Text = "Remove first " + $"{(int)Math.Floor(e.NewValue)}";
        //UpdateFinishRecordingButton();
        startTrimSliderHasBeenUsed= true;
        //UpdateLineChart();
    }

    private void OnSliderEndValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Update the label with the new slider value
        //sliderEndValueLabel.Text = "Remove last " + $"{(int)Math.Floor(e.NewValue)}";
        //UpdateFinishRecordingButton();
        endTrimSliderHasBeenUsed = true;
        //UpdateLineChart();
    }

    private void OnEditorFocused(object sender, EventArgs e)
    {

    }

    private void OnEditorUnfocused(object sender, EventArgs e)
    {

    }

    #region
    public void Stop()
    {
        _timer.Dispose();
    }
    #endregion

    private void CheckBoxVentilation_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {        
        hasVentilationSystem= CheckBoxVentilation.IsChecked;
    }

    private void CheckBoxDoorsWindows_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        hasOpenWindowsDoors = CheckBoxDoorsWindows.IsChecked;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        lineChartView.WidthRequest = 0.9 * width;
        lineChartView.HeightRequest = 0.25 * height;
        startTrimSlider.WidthRequest = 0.4 * width;
        endTrimSlider.WidthRequest = 0.4 * width;
    }
}

