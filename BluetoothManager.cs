using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IndoorCO2App_Android;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.VisualStudio.Composition;
using Microsoft.VisualStudio.Settings;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

namespace IndoorCO2App_Multiplatform
{
    internal static class BluetoothManager
    {
        public static bool lowCO2ValueDetected = false;
        public static string deviceName ="";
        private static CancellationTokenSource btCancellationTokenSource;
        private static bool isScanning = false;
        public static IBluetoothLE ble;
        public static IAdapter adapter;
        public static BluetoothState state;
        public static int prerecordingLength = 0;
        public static int currentCO2Reading = 0;
        public static int sensorUpdateInterval = 0;
        public static IReadOnlyList<IDevice> discoveredDevices;

        public static bool subscribedAlreadyToReportChar = false;

        static Guid airspotServiceGUID = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        static Guid airspotWriteCharacteristicGUID = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e"); //probably write characteristic to change settings (and maybe request history?)
        static Guid airspotNotifyCharacteristic = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");


        public static Guid InkbirdServiceUUID = Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb");
        public static Guid InkbirdCO2NotifyCharacteristic = Guid.Parse("0000ffe4-0000-1000-8000-00805f9b34fb");

        //Airvalent Approach still experimental and might not be the best
        //Tested only with Firmware Version: v1.41
        public static Guid AirvalentServiceUUID = Guid.Parse("B81C94A4-6B2B-4D41-9357-0C8229EA02DF");
        public static Guid AirvalentUpdateIntervalCharacteristic = Guid.Parse("b1c48eea-4f5c-44f7-9797-73e0ce294881");
        public static Guid AirvalentHistoryCharacteristic = Guid.Parse("426d4fa2-50ea-4a8d-b88c-c58b3e78f857");
        public static Guid AirvalentDataChunkCount = Guid.Parse("a6cf90e4-7ec0-46b2-a90a-5c2580f85a43"); // 0 => just current data in History Characteristic is available (not sure yet what happens if we set the airvalentHistoryPointerCharacteristic to a value where no data is yet? does it return old data / random data / crash / ignore? 
        //	↑	↑	↑	↑	↑
        //First 8 Bytes are some headers/meta data, after that 8 Bytes per minute: Layout:
        //14 Bit: CO2 Value
        //10 Bit: Temperature 
        //24 Bit: unknown / probably Humidity and something else? or unused?)
        //16 Bit: Timer/Counter, Byte 6 increments by 15 every minute, Byte 7 increases whenever Byte 7 overflows (maybe Bit 5 is also timer/counter?)

        public static Guid airvalentHistoryPointerCharacteristic = Guid.Parse("cdbde84d-2dc6-46e4-8d6b-f3ababf560aa");

        public static Guid AranetServiceUUID = Guid.Parse("0000FCE0-0000-1000-8000-00805f9b34fb");
        public static Guid OldAranetServiceUUID = Guid.Parse("f0cd1400-95da-4f4b-9ac8-aa55d312af0c");

        public static string sensorVersion = "";
        public static bool outdatedVersion = false;

        public static Guid Aranet_CharacteristicUUID = Guid.Parse("f0cd3001-95da-4f4b-9ac8-aa55d312af0c"); //Characteristic which has the data we need
        public static Guid ARANET_WRITE_CHARACTERISTIC_UUID = Guid.Parse("f0cd1402-95da-4f4b-9ac8-aa55d312af0c");
        public static Guid ARANET_HISTORY_V2_CHARACTERISTIC_UUID = Guid.Parse("f0cd2005-95da-4f4b-9ac8-aa55d312af0c");
        public static Guid ARANET_TOTAL_READINGS_CHARACTERISTIC_UUID = Guid.Parse("f0cd2001-95da-4f4b-9ac8-aa55d312af0c");

        public static Guid AranetVersionServiceUUID = Guid.Parse("0000fce0-0000-1000-8000-00805f9b34fb");
        public static Guid ARANET_VersionNumber_CHARACTERISTIC_UUID = Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb");

        public static DateTime previousUpdate = DateTime.MinValue;
        public static double timeToNextUpdate = 60;
        public static double refreshTime = 60;
        public static double refreshTimeWhenNotSucess = 20;
        public static DateTime timeOfLastNotifyUpdate = DateTime.MinValue;

        public static bool isRecording;
        public static bool isTransportRecording;
        public static long startingTime;
        public static List<SensorData> recordedData;
        public static SubmissionData submissionData;
        public static SubmissionDataManual submissionDataManual;
        public static SubmissionDataTransport submissionDataTransport;
        public static string deviceID;
        public static bool isGattA2DP;
        public static bool isBonded = false;
        public static int rssi;
        //public static int txPower;
        public static int gattStatus;
        //public static bool currentlyUpdating; //not yet used as updates seem to be reasonable fast
        public static bool lastAttemptFailed = false;
        public static bool NotifyCharacteristicAlreadyHookedUp = false;
        public static bool directConnectToBondedDevice = false;

        public static Dictionary<CO2MonitorType, Guid> serviceUUIDByMonitorType;
        public static ICharacteristic inkbirdCO2NotifyCharacteristic;
        public static ICharacteristic airspotCO2NotifyCharacteristic;



        internal static void Init()
        {            
            serviceUUIDByMonitorType = new Dictionary<CO2MonitorType, Guid>();
            serviceUUIDByMonitorType.Add(CO2MonitorType.Aranet4, AranetServiceUUID);
            serviceUUIDByMonitorType.Add(CO2MonitorType.Airvalent, AirvalentServiceUUID);
            serviceUUIDByMonitorType.Add(CO2MonitorType.InkbirdIAMT1, InkbirdServiceUUID);
            serviceUUIDByMonitorType.Add(CO2MonitorType.AirSpot, airspotServiceGUID);
            recordedData = new List<SensorData>();
            ble = CrossBluetoothLE.Current;
            discoveredDevices = new List<IDevice>(); // we init a dummy to avoid null checks
            if (ble == null)
            {
                return;
            }

            if (ble != null)
            {
                adapter = CrossBluetoothLE.Current.Adapter;
            }

            state = ble.State;
            adapter.ScanMode = ScanMode.LowLatency;
            adapter.ScanTimeout = 10000; //ms
            deviceID = String.Empty;


        }

        internal static async void Update(CO2MonitorType monitorType, string nameFilter, IBluetoothHelper bluetoothHelper)
        {
            //Logger.circularBuffer.Add($"Update() called for: {monitorType} with nameFilter: {nameFilter} | " + DateTime.Now);
            DateTime currentTime = DateTime.Now;
            timeToNextUpdate = (int)(refreshTime - ((currentTime - previousUpdate).TotalSeconds));

            double updateFreq = refreshTime;
            if(String.IsNullOrEmpty(deviceID))
            {
                updateFreq = refreshTimeWhenNotSucess;
            }
            if(lastAttemptFailed)
            {
                updateFreq = refreshTimeWhenNotSucess;
            }


            if (currentTime - previousUpdate > TimeSpan.FromSeconds(updateFreq))
            {
                if (submissionDataManual != null && SpatialManager.currentLocation != null)
                {
                    var lat = SpatialManager.currentLocation.Latitude;
                    var lon = SpatialManager.currentLocation.Longitude;                    
                    submissionDataManual.LatitudeData.Add(lat);
                    submissionDataManual.LongitudeData.Add(lon);
                    //submissionDataTransport.timeStampsOfGPSData.Add(DateTime.Now);
                }

                if (submissionDataTransport != null && SpatialManager.currentLocation != null)
                {
                    var lat = SpatialManager.currentLocation.Latitude;
                    var lon = SpatialManager.currentLocation.Longitude;
                    if(lat!=0 && lon !=0)
                    {
                        submissionDataTransport.LatitudeData.Add(lat);
                        submissionDataTransport.LongitudeData.Add(lon);
                        submissionDataTransport.timeStampsOfGPSData.Add(DateTime.Now);
                    }
                    
                }

                previousUpdate = currentTime;
                try
                {
                    if(monitorType== CO2MonitorType.AirSpot)
                    {
                        //discoveredDevices = null;
                    }
                    //Logger.circularBuffer.Add($"ScanForDevices() called | " + DateTime.Now);
                    ScanForDevices(monitorType, nameFilter, bluetoothHelper);
                }
                catch
                {
                    lastAttemptFailed = true;
                }
            }

        }

        public static void StartNewRecording(CO2MonitorType monitorType, LocationData location, long startTime, bool prerecording)
        {
            previousUpdate = DateTime.Now.AddMinutes(-3);
            isRecording = true;
            recordedData = new List<SensorData>();
            startingTime = startTime;
            submissionData = new SubmissionData(monitorType.ToString(), UserIDManager.GetEncryptedID(deviceID,false), location.Type, location.ID, location.Name, location.Latitude, location.Longitude, startTime);
            NotifyCharacteristicAlreadyHookedUp = false;
            if (prerecording)
            {
                prerecordingLength = 15;
            }
            else
            {
                prerecordingLength = 0;
            }
        }

        public static void StartNewManualRecording(LocationData location, long startTime, bool prerecording)
        {
            isRecording = true;
            isTransportRecording = false;
            recordedData = new List<SensorData>();
            submissionDataManual = new SubmissionDataManual(UserIDManager.GetEncryptedID(deviceID,false), startTime);
            startingTime = startTime;
            NotifyCharacteristicAlreadyHookedUp = false;
            if (prerecording)
            {
                prerecordingLength = 15;
            }
            else
            {
                prerecordingLength = 0;
            }
        }

        public static void StartTransportRecording(CO2MonitorType monitorType,long startTime, bool prerecording, LocationData startLocation, TransitLineData transitLineData)
        {
            isRecording = true;
            isTransportRecording = true;
            recordedData = new List<SensorData>();
            startingTime = startTime;
            submissionDataTransport = new SubmissionDataTransport(monitorType.ToString(), UserIDManager.GetEncryptedID(deviceID,true), startTime,transitLineData.ID,transitLineData.NWRType,transitLineData.Name, startLocation.ID,startLocation.Type,startLocation.Name);
            NotifyCharacteristicAlreadyHookedUp = false;
            prerecordingLength = 0; // no prerecording for now
            Logger.WriteToLog("Transportrecording starttime used: " + startingTime + " | current Time: " + DateTime.UtcNow, false);
            //throw new System.NotImplementedException();
        }

        public static void StopRecording()
        {
            isRecording = false;
        }

        public static async Task<bool> FinishRecordingAsync(int start, int end, SubmissionMode submissionMode, string locationNameManual, string locationAddressManual)
        {
            bool success = false;
            if (recordedData == null && recordedData.Count == 0)
            {
                return false;
            }
            //Add co2 measurements to submissionData
            //ApiGatewayCaller.SendJsonToApiGateway(submissionData.ToJson());
            if (submissionMode== SubmissionMode.Building)
            {
                isRecording = false;
                if(recordedData==null && recordedData.Count== 0)
                {
                    return false;
                }
                submissionData.SensorData = recordedData;
                
                string response = await ApiGatewayCaller.SendJsonToApiGatewayAsync(submissionData.ToJson(start, end), SubmissionMode.Building);
                if (response == "success")
                {
                    success = true;
                }
                else success = false;
                
            }
            else if(submissionMode== SubmissionMode.BuildingManual) 
            {
                isRecording = false;
                submissionDataManual.sensorData = recordedData;
                submissionDataManual.LocationName = locationNameManual;
                submissionDataManual.LocationAddress = locationAddressManual;
                string response = await ApiGatewayCaller.SendJsonToApiGatewayAsync(submissionDataManual.ToJson(start, end), SubmissionMode.BuildingManual);
                if (response == "success")
                {
                    success = true;
                }
                else success = false;
            }
            else if(submissionMode== SubmissionMode.Transit)
            {
                isRecording = false;
                submissionDataTransport.sensorData = recordedData;
                MainPage.MainPageSingleton.selectedTransitTargetLocation = (LocationData)MainPage.MainPageSingleton._TransitDestinationPicker.SelectedItem;
                if(MainPage.MainPageSingleton.selectedTransitTargetLocation != null)
                {
                    submissionDataTransport.EndPointID = MainPage.MainPageSingleton.selectedTransitTargetLocation.ID;
                    submissionDataTransport.EndPointNWRType = MainPage.MainPageSingleton.selectedTransitTargetLocation.Type;
                    submissionDataTransport.EndPointName = MainPage.MainPageSingleton.selectedTransitTargetLocation.Name;                    
                }
                submissionDataTransport.TransportID = MainPage.MainPageSingleton.selectedTransitLine.ID;
                submissionDataTransport.TransportNWRType = MainPage.MainPageSingleton.selectedTransitLine.NWRType;
                submissionDataTransport.TransportName = MainPage.MainPageSingleton.selectedTransitLine.Name;

                string response = await ApiGatewayCaller.SendJsonToApiGatewayAsync(submissionDataTransport.ToJson(start, end), SubmissionMode.Transit);
                if (response == "success")
                {
                    success = true;
                }
                else success = false;
            }
            return success;
        }

        internal static async void ScanForDevices(CO2MonitorType monitorType, string nameFilter, IBluetoothHelper bluetoothHelper)
        {
            Logger.WriteToLog($"ScanForDevices() called for: {monitorType} with nameFilter: {nameFilter} | " + DateTime.Now, false);
            //doesnt work like it should yet
            //directConnectToBondedDevice = false;
            //bool checkStatus = BluetoothHelper.CheckStatus();
            //if (!checkStatus) return;
            //var bonded = await CheckBondedDevicesForCO2Sensor(monitorType,MainPage.bluetoothHelper);
            //if(bonded!=null && deviceID==null)
            //{
            //    rssi = bonded.Rssi;
            //    try
            //    {
            //        
            //        await adapter.ConnectToDeviceAsync(bonded);
            //        lastAttemptFailed = false;
            //        directConnectToBondedDevice = true;
            //        ConnectToDevice(bonded, monitorType);
            //        return;
            //    }
            //    catch
            //    {
            //        lastAttemptFailed = true;
            //        discoveredDevices = null;
            //        Console.WriteLine("Connecting to Device failed");
            //        //if this fails we try the old way
            //    }
            //    
            //}


            bool checkPermissions = bluetoothHelper.CheckStatus();
            if (!checkPermissions) return;
            bool checkBTEnabled = bluetoothHelper.CheckIfBTEnabled();
            if (!checkBTEnabled) return;

            //if we already have a discoveredDevice we can take a shortcut
            if (discoveredDevices!=null && discoveredDevices.Count>0 && discoveredDevices[0].State == DeviceState.Connected)
            {
                if (!discoveredDevices[0].Name.ToLower().Contains(nameFilter.Trim().ToLower()))
                {
                    discoveredDevices = null;
                    Logger.WriteToLog("current Device not matching namefilter, removing it and redoing connection", false);
                    return;
                }
                //if (discoveredDevices[0].Name)
                ConnectToDevice(discoveredDevices[0],monitorType);
                return;
            }

            Guid serviceUUID;
            if (serviceUUIDByMonitorType.ContainsKey(monitorType))
            {
                serviceUUID = serviceUUIDByMonitorType[monitorType];
            }
            else
            {
                throw new System.NotImplementedException("monitorType not supported");
            }
            lastAttemptFailed = false;

            await bluetoothHelper.RequestAsync();
            if (ble == null)
            {
                Init();
            }

            if (ble == null) return; // if still null after trying to Init we abort
            var scanFilterOptions = new ScanFilterOptions();
            if (monitorType != CO2MonitorType.InkbirdIAMT1 && monitorType != CO2MonitorType.AirSpot)
            {
                Logger.WriteToLog($"setting serviceUUId filter to: {serviceUUID}", false);
                scanFilterOptions.ServiceUuids = new[] { serviceUUID };
                if(monitorType== CO2MonitorType.Aranet4)
                {
                    scanFilterOptions.ServiceUuids = new[] { serviceUUID, OldAranetServiceUUID }; 
                }
            }
            adapter.ScanMatchMode = ScanMatchMode.STICKY;
            adapter.ScanMode = ScanMode.LowLatency;
            adapter.ScanTimeout = 14500;
            adapter.DeviceDiscovered += (s, e) =>
            {                
                var device = e.Device;
                Logger.WriteToLog($"Discovered Device: {device.Name} ({device.Id}) |",false);
                Console.WriteLine($"Discovered Device: {device.Name} ({device.Id})");

                // Check if the discovered device matches criteria, as we currently only check serviceUUIDs of airvalent and aranet we only do this for them (only  they are necessarily paired)
                if ((monitorType == CO2MonitorType.Aranet4 || monitorType == CO2MonitorType.Airvalent) && device.Name.ToLower().Contains(nameFilter.Trim().ToLower()))
                {
                    if (monitorType == CO2MonitorType.Aranet4)
                    {


                        var adRecords = device.AdvertisementRecords;
                        Logger.WriteToLog("Target device found. Stopping scan early", false);

                        int majorVersion = 0;
                        int minorVersion = 0;
                        bool found24byteEntry = false;
                        foreach (var r in adRecords)
                        {
                            if (r.Data.Length == 24)
                            {
                                found24byteEntry = true;
                                Logger.WriteToLog("found advertisement record with 24 byte length", false);
                                majorVersion = r.Data[5];
                                Logger.WriteToLog("major Version [byte at index 5]: " + majorVersion, false);
                                minorVersion = r.Data[4];
                                Logger.WriteToLog("minor Version [byte at index 4]: " + minorVersion, false);
                                if (majorVersion < 1 || (majorVersion == 1 && minorVersion < 2))
                                {
                                    outdatedVersion = true;
                                }
                                else
                                {
                                    outdatedVersion = false;
                                }
                            }
                        }
                        if (!found24byteEntry)
                        {
                            outdatedVersion = true;
                        }
                    }

                    //todo version check for Airvalent
                    Console.WriteLine("Target device found. Stopping scan...");
                    btCancellationTokenSource.Cancel(); // Stop scanning
                }
            };


            if (discoveredDevices == null)
            {
                btCancellationTokenSource = new CancellationTokenSource();
                isScanning = true;
                await adapter.StartScanningForDevicesAsync(scanFilterOptions,btCancellationTokenSource.Token);
                await adapter.StopScanningForDevicesAsync();
                isScanning=false;
                discoveredDevices = adapter.DiscoveredDevices;
                if (monitorType == CO2MonitorType.InkbirdIAMT1)
                {
                    List<IDevice> inkbirdFilter = discoveredDevices.ToList();
                    inkbirdFilter.RemoveAll(x => !x.Name.Contains("Inkbird"));
                    discoveredDevices = inkbirdFilter;
                }
               
                else if (monitorType == CO2MonitorType.AirSpot)
                {
                    List<IDevice> airSpotFilter = discoveredDevices.ToList();
                    airSpotFilter.RemoveAll(x => x.Name == null);
                    airSpotFilter.RemoveAll(x => !x.Name.ToLower().Contains("airspot"));
                    discoveredDevices = airSpotFilter;
                }


                if (discoveredDevices.Count > 0)
                {
                    rssi = discoveredDevices[0].Rssi;
                }

                if (nameFilter != null && nameFilter.Length > 0)
                {
                    List<IDevice> unfilteredDevices = discoveredDevices.ToList();
                    List<IDevice> filteredDevices = new List<IDevice>();
                    for (int i = 0; i < unfilteredDevices.Count; i++)
                    {
                        var d = unfilteredDevices[i];
                        if (d.Name != null && d.Name.ToLower().Contains(nameFilter.Trim().ToLower()))
                        {
                            filteredDevices.Add(d);
                            break;
                        }
                    }
                    discoveredDevices = filteredDevices;
                }

            }

            if (discoveredDevices != null && discoveredDevices.Count == 0)
            {
                btCancellationTokenSource = new CancellationTokenSource();
                isScanning = true;
                await adapter.StartScanningForDevicesAsync(scanFilterOptions,btCancellationTokenSource.Token);
                await adapter.StopScanningForDevicesAsync();
                isScanning = false;
                discoveredDevices = adapter.DiscoveredDevices;

                foreach(var d in discoveredDevices)
                {
                    Console.WriteLine(d.Name);
                }

                if (monitorType == CO2MonitorType.InkbirdIAMT1)
                {
                    List<IDevice> unfilteredDevices = discoveredDevices.ToList();
                    List<IDevice> filteredDevices = new List<IDevice>();
                    for (int i = 0; i < unfilteredDevices.Count; i++)
                    {
                        var d = unfilteredDevices[i];
                        if (d.Name != null && d.Name.Contains("Ink@IAM-T1"))
                        {
                            filteredDevices.Add(d);
                        }
                    }
                    discoveredDevices = filteredDevices;
                }
                
                else if (monitorType == CO2MonitorType.AirSpot)
                {
                    List<IDevice> airSpotFilter = discoveredDevices.ToList();
                    airSpotFilter.RemoveAll(x => x.Name == null);
                    airSpotFilter.RemoveAll(x => !x.Name.ToLower().Contains("airspot"));
                    discoveredDevices = airSpotFilter;
                }

                else if(nameFilter!=null && nameFilter.Length> 0)
                {
                    List<IDevice> unfilteredDevices = discoveredDevices.ToList();
                    List<IDevice> filteredDevices = new List<IDevice>();
                    for (int i = 0; i < unfilteredDevices.Count; i++)
                    {
                        var d = unfilteredDevices[i];
                        if (d.Name != null && d.Name.ToLower().Contains(nameFilter.Trim().ToLower()))
                        {
                            filteredDevices.Add(d);
                            break;
                        }
                    }
                    discoveredDevices = filteredDevices;
                }

                if (discoveredDevices.Count > 0)
                {
                    rssi = discoveredDevices[0].Rssi;
                }
            }
            

            if (discoveredDevices != null && discoveredDevices.Count > 0)
            {
                deviceName = discoveredDevices[0].Name;
                try
                {

                        Logger.WriteToLog($"ConnectToDeviceAsync() to device: {deviceName}",false);
                        await adapter.ConnectToDeviceAsync(discoveredDevices[0]);                    
                }
                catch
                {
                    lastAttemptFailed = true;
                    discoveredDevices = null;
                    Logger.WriteToLog($"ConnectToDeviceAsync() to Device failed: {deviceName}", false);
                    Console.WriteLine("Connecting to Device failed");
                    return;
                }

                //might have changed after the await before
                if(discoveredDevices.Count>0)
                {
                    try
                    {
                        ConnectToDevice(discoveredDevices[0], monitorType);
                    }
                    catch
                    {
                        Logger.WriteToLog($"ConnectToDevice() failed for: {deviceName} | " +$"namefilter set to: {nameFilter}",false );
                    }
                    
                }
                
            }

            //if (discoveredDevices == null)
            //{
            //    await adapter.StartScanningForDevicesAsync(scanFilterOptions);
            //    await adapter.StopScanningForDevicesAsync();
            //    discoveredDevices = adapter.DiscoveredDevices;
            //    if (monitorType == CO2MonitorType.InkbirdIAMT1)
            //    {
            //        List<IDevice> inkbirdFilter = discoveredDevices.ToList();
            //        inkbirdFilter.RemoveAll(x => !x.Name.Contains("Inkbird"));
            //        discoveredDevices = inkbirdFilter;
            //    }
            //    if (discoveredDevices.Count > 0)
            //    {
            //        rssi = discoveredDevices[0].Rssi;
            //    }
            //}
        }

        internal static async void ConnectToDevice(IDevice device, CO2MonitorType monitorType)
        {
            long cur = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int elapsedSeconds = (int)Math.Ceiling((cur - startingTime) / 1000d);
            int elapsedIntervals = ConvertSecondsToFullMinutes(elapsedSeconds);
            if(sensorUpdateInterval == 120)
            {
                elapsedIntervals = elapsedIntervals / 2;
            }
            else if(sensorUpdateInterval == 300)
            {
                elapsedIntervals = elapsedIntervals / 5;
            }
            else if(sensorUpdateInterval == 600)
            {
                elapsedIntervals = elapsedIntervals / 10;
            }
            //TODO if interval is 
            
            deviceID = device.Id.ToString();
            rssi = device.Rssi;
            DeviceState deviceState = device.State;
            Logger.WriteToLog($"deviceState of {device.Name} : {deviceState} | ",false);

            try
            {
                IService service = null;
                //IReadOnlyList<IService> results;
               
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(8)); // Cancel after 10 seconds.

                try
                {
                    service = await device.GetServiceAsync(serviceUUIDByMonitorType[monitorType], cts.Token);
                    if (service == null)
                    {
                        Logger.WriteToLog($".GetServiceAsync(serviceUUIDByMonitorType[monitorType]({serviceUUIDByMonitorType[monitorType]})) returned null",false);
                    }
                    else
                    {
                        Logger.WriteToLog($".GetServiceAsync(serviceUUIDByMonitorType[monitorType]) returned serviceID:  {service.Id}",false);
                    }
                }
                catch (TaskCanceledException)
                {
                    Logger.WriteToLog($".GetServiceAsync(serviceUUIDByMonitorType[monitorType]({serviceUUIDByMonitorType[monitorType]})) TaskCanceledException",false);
                }
                catch (Exception ex)
                {
                    Logger.WriteToLog($".GetServiceAsync(serviceUUIDByMonitorType[monitorType]({serviceUUIDByMonitorType[monitorType]})) Error during service discovery: {ex.Message}",false);
                }                                                                                                                          
                                            

                if (service != null)
                {
                    if (monitorType == CO2MonitorType.Aranet4)
                    {
                        

                        Logger.WriteToLog($"trying to read characteristics from {device.Name}", false);
                        

                        ICharacteristic aranet4CharacteristicLive = await service.GetCharacteristicAsync(Aranet_CharacteristicUUID);
                        ICharacteristic aranet4CharacteristicTotalDataPoints = await service.GetCharacteristicAsync(ARANET_TOTAL_READINGS_CHARACTERISTIC_UUID);
                        ICharacteristic aranet4CharacteristicWriter = await service.GetCharacteristicAsync(ARANET_WRITE_CHARACTERISTIC_UUID);
                        ICharacteristic aranet4CharacteristicHistoryV2 = await service.GetCharacteristicAsync(ARANET_HISTORY_V2_CHARACTERISTIC_UUID);

                        if(aranet4CharacteristicLive == null)
                        {
                            Logger.WriteToLog($"aranet4CharacteristicLive is null", false);
                        }
                        if(aranet4CharacteristicTotalDataPoints == null)
                        {
                            Logger.WriteToLog($"aranet4TotalDataPointsCharacteristic null", false);
                        }
                        if (aranet4CharacteristicLive == null)
                        {
                            Logger.WriteToLog($"aranet4CharacteristicWriter is null", false);
                        }
                        if(aranet4CharacteristicHistoryV2 == null)
                        {
                            Logger.WriteToLog($"aranet4CharacteristicHistoryV2  is null", false);
                        }

                        if (aranet4CharacteristicLive != null)
                        {
                            (byte[] data, int resultCode) result;
                            try
                            {
                                result = await aranet4CharacteristicLive.ReadAsync();                                
                            }
                            catch (Exception e)
                            {
                                Logger.WriteToLog($"calling aranet4CharacteristicLive.ReadAsync() raised an exception: {e}",false);
                                return;
                            }                            
                            gattStatus = result.resultCode;
                            //if(gattStatus == result.resultCode== CAN)
                            if (gattStatus != 0)
                            {
                                Logger.WriteToLog($"GattStatus is not 0 but {gattStatus}",false);
                                if (gattStatus == 2)
                                {
                                    isGattA2DP = true;
                                }
                                return; //returning here should be fine
                            }
                            isGattA2DP = false;

                            var data = result.data;
                            if (data.Length >= 9)
                            {
                                currentCO2Reading = (data[1] << 8) | (data[0] & 0xFF);
                                sensorUpdateInterval = (data[10] << 8) | (data[9] & 0xFF);
                                Logger.WriteToLog($"current CO Reading from {device.Name}: {currentCO2Reading}", false);
                            }

                            else
                            {
                                Logger.WriteToLog($"aranet4CharacteristicLive.ReadAsync() returned smaller array than expected",false);
                                Console.WriteLine("Byte array does not contain enough data");
                            }
                        }

                        int totalDataPoints = 0;

                        if (aranet4CharacteristicTotalDataPoints != null)
                        {
                            try
                            {
                                var result = await aranet4CharacteristicTotalDataPoints.ReadAsync();
                                byte[] totalDataPointsRaw = result.data;
                                totalDataPoints = (totalDataPointsRaw[1] << 8) | (totalDataPointsRaw[0] & 0xFF);
                                if(isRecording)
                                {
                                    Logger.WriteToLog("************",false);
                                    Logger.WriteToLog(System.DateTime.Now.ToLongTimeString() + "|Aranet|TotalDataPoints: " + totalDataPoints,false);
                                }
                                else
                                {
                                    Logger.WriteToLog($"aranet4CharacteristicTotalDataPoints, TotalDataPoints: {totalDataPoints}", false);
                                }
                                
                            }
                            catch
                            {                                
                                Logger.WriteToLog($"reading aranet4CharacteristicTotalDataPoints failed | + Bondstate: {device.BondState}",true);
                                return;
                            }

                        }

                        if (isRecording)
                        {
                            if (aranet4CharacteristicHistoryV2 != null && aranet4CharacteristicWriter != null && totalDataPoints > 0)
                            {
                                //ushort timeSinceRecordingStart = 0;
                                //TODO calculalte actual timeSinceRecordingstart and always use the ceiling
                                ushort start = (ushort)(totalDataPoints - (prerecordingLength + elapsedIntervals)); //change to higher value to grab a bit of historical data
                                if (start < 0) start = 0;
                                var requestData = AranetPackDataRequestCO2History(start);
                                int response = -999;
                                int retryCounter = 0;
                                RETRYREAD:
                                try
                                {
                                    Logger.WriteToLog("|Aranet|Requesting History with startIndex: " + start, false);
                                    response = await aranet4CharacteristicWriter.WriteAsync(requestData);
                                    Logger.WriteToLog("|Aranet|Response to Request: " + response, false);
                                }
                                catch
                                {
                                    return;
                                }



                                (byte[] data, int resultCode) history;
                                try
                                {
                                    await Task.Delay(35);
                                    history = await aranet4CharacteristicHistoryV2.ReadAsync();
                                    Logger.WriteToLog("|Aranet|ReadingHistoryV2|data Size: " + history.data.Length, false);
                                    Logger.WriteToLog("|Aranet|ReadingHistoryV2|result Code: " + history.resultCode, false);
                                    if(history.data.Length<=10 && retryCounter<10)
                                    {
                                        retryCounter++;
                                        Logger.WriteToLog("|Aranet|Historyread Retry: " + retryCounter, false);
                                        await Task.Delay(35);
                                        goto RETRYREAD;
                                    }
                                }
                                catch
                                {
                                    return;
                                }

                                var historyDataRaw = history.data;

                                byte paramID = historyDataRaw[0];
                                ushort Interval = BitConverter.ToUInt16(historyDataRaw, 1);
                                ushort totalReadings = BitConverter.ToUInt16(historyDataRaw, 3);
                                ushort ago = BitConverter.ToUInt16(historyDataRaw, 5);
                                ushort startIndex = BitConverter.ToUInt16(historyDataRaw, 7);
                                byte count = historyDataRaw[9];

                                ushort[] co2dataArray = new ushort[count];
                                for (int i = 0; i < co2dataArray.Length; i++) // first ten bytes are the header
                                {
                                    co2dataArray[i] = BitConverter.ToUInt16(historyDataRaw, 10 + (i * 2));
                                }
                                Logger.WriteToLog("Aranet|historyArrayLength: " + co2dataArray.Length, false);
                                //TODO => dont clear if something went wrong but for now we want to find out what sometimes goes wrong first.
                                recordedData.Clear();
                                int t = 0;
                                foreach (var e in co2dataArray)
                                {
                                    recordedData.Add(new SensorData(e, t,DateTime.Now));
                                    if(sensorUpdateInterval==60)
                                    {
                                        t++;
                                    }
                                    else if(sensorUpdateInterval==120)
                                    {
                                        t+= 2;
                                    }
                                    else if(sensorUpdateInterval==300)
                                    {
                                        t += 5;
                                    }
                                    else if(sensorUpdateInterval==600)
                                    {
                                        t+= 10;
                                    }
                                    
                                }
                                RecoveryData.timeOfLastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                                RecoveryData.WriteToPreferences();
                                //WE keep a few historical data points which we transmit so we can also get data if sensor is calibrated okay but in UI it should only start with actual recoding
                            }
                        }
                    }
                    else if (monitorType == CO2MonitorType.Airvalent)
                    {
                        ICharacteristic airValentUpdateInterval = await service.GetCharacteristicAsync(AirvalentUpdateIntervalCharacteristic);
                        ICharacteristic airValentHistory = await service.GetCharacteristicAsync(AirvalentHistoryCharacteristic);
                        ICharacteristic airValentHistoryPointer = await service.GetCharacteristicAsync(airvalentHistoryPointerCharacteristic);
                        ICharacteristic airValentChunkCounter = await service.GetCharacteristicAsync(AirvalentDataChunkCount);

                        if (airValentUpdateInterval != null)
                        {
                            var reply = await airValentUpdateInterval.ReadAsync();
                            byte[] intervalBytes = reply.data;
                            ushort interval = BitConverter.ToUInt16(intervalBytes, 0);
                            sensorUpdateInterval = interval;
                        }

                        ushort chunkCount = 0;
                        if (airValentChunkCounter != null)
                        {
                            var reply = await airValentChunkCounter.ReadAsync();
                            byte[] chunkCounterBytes = reply.data;
                            chunkCount = BitConverter.ToUInt16(chunkCounterBytes, 0);
                        }


                        if (chunkCount > 0)
                        {
                            int response = -999;
                            byte[] msg = AirvalentSetHistoryPointerMsgData();
                            try
                            {
                                response = await airValentHistoryPointer.WriteAsync(msg);
                                Logger.WriteToLog("airvalent|Response|HistoryRequest: " + response, false);

                            }
                            catch
                            {
                                return;
                            }
                        }

                        var reply1 = await airValentHistory.ReadAsync();
                        List<byte> bytes1 = reply1.data.ToList();
                        bytes1.RemoveRange(0, 8); //first entry is not the time series we are interested in (we might use some info of it later, so far not checked what's in it)
                        var reply2 = await airValentHistory.ReadAsync();
                        List<byte> bytes2 = reply2.data.ToList();
                        bytes2.RemoveRange(0, 8); //first entry is not the time series we are interested in (we might use some info of it later, so far not checked what's in it)
                        if (chunkCount > 0)
                        {
                            bytes1.AddRange(bytes2);
                        }

                        List<ushort> co2values = new List<ushort>();
                        for (int i = 0; i < bytes1.Count; i += 8)
                        {
                            byte co2byte2shift = (byte)(bytes1[i + 1] << 2); //we remove first 2 byte
                            co2byte2shift = (byte)(co2byte2shift >> 2); //we shift it back
                            byte[] co2bytes = new byte[2] { bytes1[i + 0], co2byte2shift };
                            ushort co2Value = BitConverter.ToUInt16(co2bytes, 0);
                            co2values.Add(co2Value);
                        }

                        int amountOfValuesToTake = elapsedIntervals + 1 + prerecordingLength; //we always take at least 1
                        if (elapsedIntervals > 120) elapsedIntervals = 120;
                        if (amountOfValuesToTake > co2values.Count)
                        {
                            amountOfValuesToTake = co2values.Count;
                        }

                        co2values.Reverse(); // we order it so that the newest is first so we can easier take the n-th newest ones
                        List<ushort> valuesTaken = new List<ushort>();
                        for (int i = 0; i < amountOfValuesToTake; i++)
                        {
                            valuesTaken.Add(co2values[i]);
                        }
                        valuesTaken.Reverse(); // we reverse the order again so its sorted from oldest to newest again

                        recordedData.Clear();
                        int t = 0;
                        foreach (var e in valuesTaken)
                        {
                            recordedData.Add(new SensorData(e, t,DateTime.Now));
                            if (sensorUpdateInterval == 60)
                            {
                                t++;
                            }
                            else if (sensorUpdateInterval == 120)
                            {
                                t += 2;
                            }
                            else if (sensorUpdateInterval == 300)
                            {
                                t += 5;
                            }
                            else if (sensorUpdateInterval == 600)
                            {
                                t += 10;
                            };
                        }
                        if (recordedData.Count > 0)
                        {
                            currentCO2Reading = recordedData.Last().CO2ppm;
                        }
                        RecoveryData.timeOfLastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        RecoveryData.WriteToPreferences();

                        //write to HistoryPointer if chunkCount >0
                        //collect all data
                        //create CO2-Datalist
                        //=> trim to start of recording
                        //:-)                        
                    }
                    else if (monitorType == CO2MonitorType.InkbirdIAMT1)
                    {
                        sensorUpdateInterval = 60; //TODO => read actual interval!
                        if (NotifyCharacteristicAlreadyHookedUp) return;
                        inkbirdCO2NotifyCharacteristic = await service.GetCharacteristicAsync(InkbirdCO2NotifyCharacteristic);
                        inkbirdCO2NotifyCharacteristic.ValueUpdated -= OnInkbirdCO2haracteristicValueChanged; //if already had been subscribed - not sure if that works?
                        inkbirdCO2NotifyCharacteristic.ValueUpdated += OnInkbirdCO2haracteristicValueChanged;

                        //await characteristic.StartUpdatesAsync();
                        await inkbirdCO2NotifyCharacteristic.StartUpdatesAsync();
                        NotifyCharacteristicAlreadyHookedUp = true;

                    }
                    else if (monitorType == CO2MonitorType.AirSpot)
                    {

                        if (NotifyCharacteristicAlreadyHookedUp) return;
                        airspotCO2NotifyCharacteristic =  await service.GetCharacteristicAsync(airspotNotifyCharacteristic);
                        airspotCO2NotifyCharacteristic.ValueUpdated -= AirspotManager.OnNotifyValueChanged; 
                        airspotCO2NotifyCharacteristic.ValueUpdated += AirspotManager.OnNotifyValueChanged;

                        await airspotCO2NotifyCharacteristic.StartUpdatesAsync();
                        NotifyCharacteristicAlreadyHookedUp = true;

                        AirspotManager.ReadCurrentAirspotPageCommand();

                        //TODO get the data which is stored in AirspotManager (or at least will be once finished)
                        //and put that into recordedData
                    }


                    if(currentCO2Reading > 0 && currentCO2Reading <380)
                    {
                        lowCO2ValueDetected = true;
                    }
                    else
                    { 
                        lowCO2ValueDetected = false; 
                    }
                }
            }
            catch
            {
                lastAttemptFailed = true;
            }

        }

        public static byte[] AranetPackDataRequestCO2History(ushort startIndex)
        {
            using (var memoryStream = new MemoryStream())
            {
                byte header = 0x61;
                byte co2ID = 0x04;
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(header);       // Write 1 byte
                    binaryWriter.Write(co2ID);   // Write 1 byte
                    binaryWriter.Write(startIndex);        // Write 2 bytes (little-endian by default)
                }

                byte[] data = memoryStream.ToArray();


                //System.Diagnostics.Debug.WriteLine("Sent data: " + BitConverter.ToString(data));
                return memoryStream.ToArray();
            }
            
        }

        public static byte[] AirvalentSetHistoryPointerMsgData() //sets it to the last but one data array (the newest already completely filled one)
        {
            using (var memoryStream = new MemoryStream())
            {
                byte b1 = 0xbf;
                byte b2 = 0x04;
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(b1);
                    binaryWriter.Write(b2);
                }
                byte[] data = memoryStream.ToArray();
                return memoryStream.ToArray();
            }
        }

        static int ConvertSecondsToFullMinutes(int seconds)
        {
            return (int)Math.Ceiling(seconds / 60.0);
        }


        public static void OnInkbirdCO2haracteristicValueChanged(object sender, CharacteristicUpdatedEventArgs e)
        {            
            if (sender == null) return;
            var data = e.Characteristic.Value;
            if (data == null) return;
            if (data.Length < 11) return;
            if (data.Length != 16) return;
            Logger.WriteToLog("|Ink|Data: " + ByteArrayToString(data), false);
            byte fb = data[9];
            byte sb = data[10];
            byte[] c = new byte[] { sb, fb };
            ushort CO2LiveValue = BitConverter.ToUInt16(c, 0);
            if(CO2LiveValue<100 || CO2LiveValue >=10000) //sanity check
            {
                return;
            }
            currentCO2Reading = CO2LiveValue;
            timeOfLastNotifyUpdate = DateTime.Now;
            if (isRecording)
            {
                int t = recordedData.Count;
                recordedData.Add(new SensorData(CO2LiveValue, t,DateTime.Now));
            }
        }

        



        public async static Task<IDevice> CheckBondedDevicesForCO2SensorAsync(CO2MonitorType monitorType, IBluetoothHelper bluetoothHelper)
        {
            var uuid = serviceUUIDByMonitorType[monitorType];
            // Ensure Bluetooth is turned on
            bool allOk = bluetoothHelper.CheckStatus();
            if (!allOk)
            {
                return null;
            }

            // Get bonded devices (paired devices)
            var bondedDevices = adapter.BondedDevices;

            if (bondedDevices == null) return null;
            foreach (var device in bondedDevices)
            {
                // Connect to the device
                try
                {
                    await adapter.ConnectToDeviceAsync(device);
                }
                catch (Exception)
                {
                    continue;
                }
                

                // Discover services
                var services = await device.GetServicesAsync();
                foreach (var service in services)
                {
                    if (service.Id == uuid)
                    {
                        
                            return device; // Return the device with the desired characteristic
                    }
                }

                // Disconnect after checking
                await adapter.DisconnectDeviceAsync(device);
            }

            return null; // No device found with the specific service characteristic
        }


        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static void CancelScanRequest()
        {
            if (isScanning && btCancellationTokenSource != null && btCancellationTokenSource.IsCancellationRequested == false)
                btCancellationTokenSource.Cancel();
        }   
        
        public static void UpdateAirspotLiveData(int co2Value) //called by AirspotManager
        {
            currentCO2Reading = co2Value;
        }

        public static void UpdateAirspotHistoryData(List<SensorData> sensorData) //called by AirspotManager
        {
            recordedData = sensorData;
        }

        public static void ToggleAranet4SmartHomeIntegration()
        {

        }
    }
}


