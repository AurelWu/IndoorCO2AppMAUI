using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IndoorCO2App_Android;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.VisualStudio.Composition;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace IndoorCO2App_Multiplatform
{
    internal static class BluetoothManager
    {
        public static IBluetoothLE ble;
        public static IAdapter adapter;
        public static BluetoothState state;
        public static int prerecordingLength = 0;
        public static int currentCO2Reading = 0;
        public static int sensorUpdateInterval = 0;
        public static IReadOnlyList<IDevice> discoveredDevices;

        public static Guid aircodaUUID = Guid.Parse("0000feaa-0000-1000-8000-00805f9b34fb");

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

        public static Guid Aranet_CharacteristicUUID = Guid.Parse("f0cd3001-95da-4f4b-9ac8-aa55d312af0c"); //Characteristic which has the data we need
        public static Guid ARANET_WRITE_CHARACTERISTIC_UUID = Guid.Parse("f0cd1402-95da-4f4b-9ac8-aa55d312af0c");
        public static Guid ARANET_HISTORY_V2_CHARACTERISTIC_UUID = Guid.Parse("f0cd2005-95da-4f4b-9ac8-aa55d312af0c");
        public static Guid ARANET_TOTAL_READINGS_CHARACTERISTIC_UUID = Guid.Parse("f0cd2001-95da-4f4b-9ac8-aa55d312af0c");

        public static DateTime previousUpdate = DateTime.MinValue;
        public static double timeToNextUpdate = 60;
        public static double refreshTime = 60;
        public static double refreshTimeWhenNotSucess = 20;

        public static bool isRecording;
        public static bool isTransportRecording;
        public static long startingTime;
        public static List<SensorData> recordedData;
        public static SubmissionData submissionData;
        public static SubmissionDataManual submissionDataManual;
        public static SubmissionDataTransport submissionDataTransport;
        public static string deviceID;
        public static bool isGattA2DP;
        public static int rssi;
        public static int txPower;
        public static int gattStatus;
        public static bool currentlyUpdating; //not yet used as updates seem to be reasonable fast
        public static bool lastAttemptFailed = false;
        public static bool InkbirdAlreadyHookedUp = false;
        public static bool directConnectToBondedDevice = false;

        public static Dictionary<CO2MonitorType, Guid> serviceUUIDByMonitorType;
        public static ICharacteristic inkbirdCO2NotifyCharacteristic;


        internal async static void Init()
        {

            serviceUUIDByMonitorType = new Dictionary<CO2MonitorType, Guid>();
            serviceUUIDByMonitorType.Add(CO2MonitorType.Aranet4, AranetServiceUUID);
            serviceUUIDByMonitorType.Add(CO2MonitorType.Airvalent, AirvalentServiceUUID);
            serviceUUIDByMonitorType.Add(CO2MonitorType.InkbirdIAMT1, InkbirdServiceUUID);
            serviceUUIDByMonitorType.Add(CO2MonitorType.AirCoda, InkbirdServiceUUID);
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

        internal static void Update(CO2MonitorType monitorType, string nameFilter, IBluetoothHelper bluetoothHelper)
        {
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
                }

                if (submissionDataTransport != null && SpatialManager.currentLocation != null)
                {
                    var lat = SpatialManager.currentLocation.Latitude;
                    var lon = SpatialManager.currentLocation.Longitude;
                    submissionDataTransport.LatitudeData.Add(lat);
                    submissionDataTransport.LongitudeData.Add(lon);
                }

                previousUpdate = currentTime;
                try
                {
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
            isRecording = true;
            recordedData = new List<SensorData>();
            submissionData = new SubmissionData(monitorType.ToString(), UserIDManager.GetEncryptedID(deviceID), location.type, location.ID, location.Name, location.latitude, location.longitude, startTime);
            startingTime = startTime;
            InkbirdAlreadyHookedUp = false;
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
            submissionDataManual = new SubmissionDataManual(UserIDManager.GetEncryptedID(deviceID), startTime);
            startingTime = startTime;
            InkbirdAlreadyHookedUp = false;
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
            submissionDataTransport = new SubmissionDataTransport(monitorType.ToString(), UserIDManager.GetEncryptedID(deviceID), startTime,transitLineData.ID,transitLineData.NWRType,transitLineData.Name, startLocation.ID,startLocation.type,startLocation.Name);
            startingTime = startTime;
            InkbirdAlreadyHookedUp = false;
            prerecordingLength = 0; // no prerecording for now

            //throw new System.NotImplementedException();
        }

        public static void StopRecording()
        {
            isRecording = false;
        }

        public static async void FinishRecording(int start, int end, SubmissionMode submissionMode, string locationNameManual, string locationAddressManual)
        {
            //Add co2 measurements to submissionData
            //ApiGatewayCaller.SendJsonToApiGateway(submissionData.ToJson());
            if (submissionMode== SubmissionMode.Building)
            {
                isRecording = false;
                submissionData.SensorData = recordedData;
                await ApiGatewayCaller.SendJsonToApiGateway(submissionData.ToJson(start, end), SubmissionMode.Building);
            }
            else if(submissionMode== SubmissionMode.BuildingManual) 
            {
                isRecording = false;
                submissionDataManual.sensorData = recordedData;
                submissionDataManual.LocationName = locationNameManual;
                submissionDataManual.LocationAddress = locationAddressManual;
                await ApiGatewayCaller.SendJsonToApiGateway(submissionDataManual.ToJson(start, end), SubmissionMode.BuildingManual);
            }
            else if(submissionMode== SubmissionMode.Transit)
            {
                isRecording = false;
                submissionDataTransport.sensorData = recordedData;
                if(MainPage.MainPageSingleton.selectedTransitTargetLocation != null)
                {
                    submissionDataTransport.EndPointID = MainPage.MainPageSingleton.selectedTransitTargetLocation.ID;
                    submissionDataTransport.EndPointNWRType = MainPage.MainPageSingleton.selectedTransitTargetLocation.type;
                    
                }
                await ApiGatewayCaller.SendJsonToApiGateway(submissionDataTransport.ToJson(start, end), SubmissionMode.Transit);
            }
        }

        internal static async void ScanForDevices(CO2MonitorType monitorType, string nameFilter, IBluetoothHelper bluetoothHelper)
        {
            //doesnt work like it should yet
            //directConnectToBondedDevice = false;
            //bool checkStatus = BluetoothHelper.CheckStatus();
            //if (!checkStatus) return;
            //var bonded = await CheckBondedDevicesForCO2Sensor(monitorType);
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
            bool checkPermissions = bluetoothHelper.CheckStatus();
            if (!checkPermissions) return;
            await bluetoothHelper.RequestAsync();
            if (ble == null)
            {
                Init();
            }

            if (ble == null) return; // if still null after trying to Init we abort
            var scanFilterOptions = new ScanFilterOptions();
            if (monitorType != CO2MonitorType.InkbirdIAMT1)
            {
                scanFilterOptions.ServiceUuids = new[] { serviceUUID };
            }
            adapter.ScanMatchMode = ScanMatchMode.STICKY;
            adapter.ScanMode = ScanMode.LowLatency;
            adapter.ScanTimeout = 15000;

            if (discoveredDevices == null)
            {
                await adapter.StartScanningForDevicesAsync(scanFilterOptions);
                await adapter.StopScanningForDevicesAsync();
                discoveredDevices = adapter.DiscoveredDevices;
                if (monitorType == CO2MonitorType.InkbirdIAMT1)
                {
                    List<IDevice> inkbirdFilter = discoveredDevices.ToList();
                    inkbirdFilter.RemoveAll(x => !x.Name.Contains("Inkbird"));
                    discoveredDevices = inkbirdFilter;
                }
                else if(monitorType == CO2MonitorType.AirCoda)
                {
                    List<IDevice> airCodaFilter = discoveredDevices.ToList();
                    airCodaFilter.RemoveAll(x => !x.Name.ToLower().Contains("aircoda"));
                    discoveredDevices = airCodaFilter;
                }


                if (discoveredDevices.Count > 0)
                {
                    rssi = discoveredDevices[0].Rssi;
                }

            }

            else if (discoveredDevices != null && discoveredDevices.Count == 0)
            {
                await adapter.StartScanningForDevicesAsync(scanFilterOptions);
                await adapter.StopScanningForDevicesAsync();
                discoveredDevices = adapter.DiscoveredDevices;
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
                else if (monitorType == CO2MonitorType.AirCoda)
                {
                    List<IDevice> airCodaFilter = discoveredDevices.ToList();
                    airCodaFilter.RemoveAll(x => !x.Name.ToLower().Contains("aircoda"));
                    discoveredDevices = airCodaFilter;
                }

                else if(nameFilter!=null && nameFilter.Length> 0)
                {
                    List<IDevice> unfilteredDevices = discoveredDevices.ToList();
                    List<IDevice> filteredDevices = new List<IDevice>();
                    for (int i = 0; i < unfilteredDevices.Count; i++)
                    {
                        var d = unfilteredDevices[i];
                        if (d.Name != null && d.Name.Contains(nameFilter))
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
                try
                {
                    await adapter.ConnectToDeviceAsync(discoveredDevices[0]);
                }
                catch
                {
                    lastAttemptFailed = true;
                    discoveredDevices = null;
                    Console.WriteLine("Connecting to Device failed");
                    return;
                }

                //TODO Handle case of multiple Devices => Idea is that User can specify the MAC of the device? => or rather in a Advanced Menu, sees all Devices shown with Details and then picks it and we store it
                //for now we always just use the first device (and in case of multiple, the idea is to filter it down to 1 as in mentioned in comment above
                ConnectToDevice(discoveredDevices[0], monitorType);
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


            try
            {
                IService service = await device.GetServiceAsync(serviceUUIDByMonitorType[monitorType]);
                IReadOnlyList<IService> results = await device.GetServicesAsync();

                if (service != null)
                {
                    if (monitorType == CO2MonitorType.Aranet4)
                    {
                        ICharacteristic aranet4CharacteristicLive = await service.GetCharacteristicAsync(Aranet_CharacteristicUUID);
                        ICharacteristic aranet4CharacteristicTotalDataPoints = await service.GetCharacteristicAsync(ARANET_TOTAL_READINGS_CHARACTERISTIC_UUID);
                        ICharacteristic aranet4CharacteristicWriter = await service.GetCharacteristicAsync(ARANET_WRITE_CHARACTERISTIC_UUID);
                        ICharacteristic aranet4CharacteristicHistoryV2 = await service.GetCharacteristicAsync(ARANET_HISTORY_V2_CHARACTERISTIC_UUID);



                        if (aranet4CharacteristicLive != null)
                        {
                            (byte[] data, int resultCode) result;
                            try
                            {
                                result = await aranet4CharacteristicLive.ReadAsync();
                            }
                            catch
                            {
                                return;
                            }

                            gattStatus = result.resultCode;
                            if (gattStatus != 0)
                            {

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

                            }


                            else
                            {
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
                                    Logger.circularBuffer.Add("************");
                                    Logger.circularBuffer.Add(System.DateTime.Now.ToLongTimeString() + "|Aranet|TotalDataPoints: " + totalDataPoints);
                                }
                                
                            }
                            catch
                            {
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
                                    Logger.circularBuffer.Add(System.DateTime.Now.ToLongTimeString()+ "|Aranet|Requesting History with startIndex: " +start);
                                    response = await aranet4CharacteristicWriter.WriteAsync(requestData);
                                    Logger.circularBuffer.Add(System.DateTime.Now.ToLongTimeString() + "|Aranet|Response to Request: " + response);
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
                                    Logger.circularBuffer.Add(System.DateTime.Now.ToLongTimeString() + "|Aranet|ReadingHistoryV2|data Size: " + history.data.Length);
                                    Logger.circularBuffer.Add(System.DateTime.Now.ToLongTimeString() + "|Aranet|ReadingHistoryV2|result Code: " + history.resultCode);
                                    if(history.data.Length<=10 && retryCounter<10)
                                    {
                                        retryCounter++;
                                        Logger.circularBuffer.Add(System.DateTime.Now.ToLongTimeString() +"|Aranet|Historyread Retry: " + retryCounter);
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
                                Logger.circularBuffer.Add("Aranet|historyArrayLength: " + co2dataArray.Length);
                                //TODO => dont clear if something went wrong but for now we want to find out what sometimes goes wrong first.
                                recordedData.Clear();
                                int t = 0;
                                foreach (var e in co2dataArray)
                                {
                                    recordedData.Add(new SensorData(e, t));
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
                                Logger.circularBuffer.Add("airvalent|Response|HistoryRequest: " + response);

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
                            recordedData.Add(new SensorData(e, t));
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
                        sensorUpdateInterval = 60;
                        if (InkbirdAlreadyHookedUp) return;
                        inkbirdCO2NotifyCharacteristic = await service.GetCharacteristicAsync(InkbirdCO2NotifyCharacteristic);
                        inkbirdCO2NotifyCharacteristic.ValueUpdated -= OnInkbirdCO2haracteristicValueChanged; //if already had been subscribed - not sure if that works?
                        inkbirdCO2NotifyCharacteristic.ValueUpdated += OnInkbirdCO2haracteristicValueChanged;

                        //await characteristic.StartUpdatesAsync();
                        await inkbirdCO2NotifyCharacteristic.StartUpdatesAsync();
                        InkbirdAlreadyHookedUp = true;

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
            Logger.circularBuffer.Add(System.DateTime.Now.ToLongTimeString() + "|Ink|Data: " + ByteArrayToString(data));
            byte fb = data[9];
            byte sb = data[10];
            byte[] c = new byte[] { sb, fb };
            ushort CO2LiveValue = BitConverter.ToUInt16(c, 0);
            if(CO2LiveValue<100 || CO2LiveValue >=10000)
            {
                return;
            }
            currentCO2Reading = CO2LiveValue;
            if (isRecording)
            {
                int t = recordedData.Count;
                recordedData.Add(new SensorData(CO2LiveValue, t));
            }
        }



        public async static Task<IDevice> CheckBondedDevicesForCO2Sensor(CO2MonitorType monitorType, IBluetoothHelper bluetoothHelper)
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
    }
}

