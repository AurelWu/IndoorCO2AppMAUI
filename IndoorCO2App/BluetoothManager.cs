using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace IndoorCO2App
{
    internal static class BluetoothManager
    {
        public static IBluetoothLE ble;
        public static IAdapter adapter;
        public static BluetoothState state;
        public static int currentCO2Reading = 0;
        public static int sensorUpdateInterval = 0;
        public static IReadOnlyList<IDevice> discoveredDevices;
        public static Guid AranetServiceUUID = Guid.Parse("0000FCE0-0000-1000-8000-00805f9b34fb");
        public static Guid Aranet_CharacteristicUUID = Guid.Parse("f0cd3001-95da-4f4b-9ac8-aa55d312af0c"); //Characteristic which has the data we need
        public static Guid ARANET_WRITE_CHARACTERISTIC_UUID = Guid.Parse("f0cd1402-95da-4f4b-9ac8-aa55d312af0c");
        public static Guid ARANET_HISTORY_V2_CHARACTERISTIC_UUID = Guid.Parse("f0cd2005-95da-4f4b-9ac8-aa55d312af0c");
        public static Guid ARANET_TOTAL_READINGS_CHARACTERISTIC_UUID = Guid.Parse("f0cd2001-95da-4f4b-9ac8-aa55d312af0c");

        public static DateTime previousUpdate = DateTime.MinValue;
        public static double timeToNextUpdate = 60;
        public static double refreshTime = 60;

        public static bool isRecording;
        public static long startingTime;
        public static BluetoothService bluetoothService;
        public static List<SensorData> recordedData;
        public static SubmissionData submissionData;
        public static string deviceID;
        public static bool isGattA2DP;
        public static int rssi;
        public static int txPower;
        public static int gattStatus;

        internal async static void Init()
        {

#if ANDROID
                bluetoothService = new BluetoothServiceAndroid();
#elif IOS
                bluetoothService = new BluetoothService();
#elif WINDOWS
                bluetoothService = new BluetoothServiceWindows();
#endif

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

        internal static void Update()
        {
            DateTime currentTime = DateTime.Now;            
            timeToNextUpdate = (int)(refreshTime - ((currentTime - previousUpdate).TotalSeconds));

            if (currentTime - previousUpdate > TimeSpan.FromSeconds(refreshTime))
            {
                previousUpdate = currentTime;
                ScanForDevices();
            }
            
        }

        public static void StartNewRecording(LocationData location, long startTime)
        {
            isRecording = true;
            recordedData = new List<SensorData>();
            submissionData = new SubmissionData(UserIDManager.GetEncryptedID(deviceID), location.type, location.ID, location.Name, location.latitude, location.longitude, startTime);
            startingTime = startTime;
        }

        public static void StopRecording()
        {
            isRecording = false; 
        }

        public static void FinishRecording()
        {
            //Add co2 measurements to submissionData
            //ApiGatewayCaller.SendJsonToApiGateway(submissionData.ToJson());
            isRecording= false;
            submissionData.SensorData = recordedData;
            ApiGatewayCaller.SendJsonToApiGateway(submissionData.ToJson(0,submissionData.SensorData.Count-1));
        }

        internal static async void ScanForDevices()
        {
            bool checkPermissions = BluetoothPermissions.CheckStatus();
            await BluetoothPermissions.RequestAsync();
            if (ble == null)
            {
                Init();
            }
            
            if (ble == null) return; // if still null after trying to Init we abort
            var scanFilterOptions = new ScanFilterOptions();
            scanFilterOptions.ServiceUuids = new[] { AranetServiceUUID };
            adapter.ScanMatchMode = ScanMatchMode.STICKY;
            adapter.ScanMode = ScanMode.LowLatency;
            await adapter.StartScanningForDevicesAsync(scanFilterOptions);
            await adapter.StopScanningForDevicesAsync();
            if(discoveredDevices.Count > 0 && adapter.DiscoveredDevices.Count ==0)
            {
                //we keep the old discoveredDevices.
            }
            else
            {
                discoveredDevices = adapter.DiscoveredDevices;
            }
            
            if (discoveredDevices.Count > 0)
            {
                try
                {
                    await adapter.ConnectToDeviceAsync(discoveredDevices[0]);
                }
                catch
                {
                    Console.WriteLine("Connecting to Device failed");
                    return;
                }
                IReadOnlyList<IService> results = await discoveredDevices[0].GetServicesAsync();
                //TODO Handle case of multiple Devices => Idea is that User can specify the MAC of the device? => or rather in a Advanced Menu, sees all Devices shown with Details and then picks it and we store it
                //for now we always just use the first device (and in case of multiple, the idea is to filter it down to 1 as in mentioned in comment above
                ConnectToDevice(discoveredDevices[0]);
            }
        }

        internal static async void ConnectToDevice(IDevice device)
        {
            long cur = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int elapsedSeconds = (int)Math.Ceiling((cur - startingTime)/1000d);
            int elapsedMinutes = ConvertSecondsToFullMinutes(elapsedSeconds);

            deviceID = device.Id.ToString();
            IService r = await device.GetServiceAsync(AranetServiceUUID);
            IReadOnlyList<IService> results = await device.GetServicesAsync();
            if (results.Count > 0)
            {
                foreach (var service in results)
                {
                    var aranet4CharacteristicLive = await service.GetCharacteristicAsync(Aranet_CharacteristicUUID);
                    var aranet4CharacteristicTotalDataPoints = await service.GetCharacteristicAsync(ARANET_TOTAL_READINGS_CHARACTERISTIC_UUID);
                    var aranet4CharacteristicWriter = await service.GetCharacteristicAsync(ARANET_WRITE_CHARACTERISTIC_UUID);
                    var aranet4CharacteristicHistoryV2 = await service.GetCharacteristicAsync(ARANET_HISTORY_V2_CHARACTERISTIC_UUID);

                    int totalDataPoints = 0;
                    if (aranet4CharacteristicTotalDataPoints != null)
                    {
                        var result = await aranet4CharacteristicTotalDataPoints.ReadAsync();
                        byte[] totalDataPointsRaw = result.data;
                        totalDataPoints= (totalDataPointsRaw[1] << 8) | (totalDataPointsRaw[0] & 0xFF);
                    }
                    if (isRecording)
                    {
                        if (aranet4CharacteristicHistoryV2 != null && aranet4CharacteristicWriter != null && totalDataPoints > 0)
                        {
                            //ushort timeSinceRecordingStart = 0;
                            //TODO calculalte actual timeSinceRecordingstart and always use the ceiling
                            ushort start = (ushort)(totalDataPoints - (0 + elapsedMinutes)); //change to higher value to grab a bit of historical data
                            if (start < 0) start = 0;
                            var requestData = PackDataRequestCO2History(start);
                            var response = await aranet4CharacteristicWriter.WriteAsync(requestData);
                            gattStatus = response;
                            if (response != 0)
                            {
                                
                                if (response == 2)
                                {
                                    isGattA2DP=true;
                                }
                                return; //returning here should be fine
                            }
                            isGattA2DP = false;

                            var history = await aranet4CharacteristicHistoryV2.ReadAsync();
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
                            Console.WriteLine("historyArrayLength: " + co2dataArray.Length);
                            recordedData.Clear();
                            foreach (var e in co2dataArray)
                            {
                                recordedData.Add(new SensorData(e, 0));
                            }
                            //WE keep a few historical data points which we transmit so we can also get data if sensor is calibrated okay but in UI it should only start with actual recoding
                        }
                    }

                    if (aranet4CharacteristicLive != null)
                    {
                        var result = await aranet4CharacteristicLive.ReadAsync();
                        var data = result.data;
                        if (data.Length >= 9)
                        {                            
                            currentCO2Reading = (data[1] << 8) | (data[0] & 0xFF);
                            sensorUpdateInterval = (data[10] << 8) | (data[9] & 0xFF);
                    
                            //SensorData sd = new SensorData(currentCO2Reading, 0);
                            //if (isRecording)
                            //{
                            //    long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            //    recordedData.Add(new SensorData(currentCO2Reading, timeStamp));
                            //}
                        }
                    
                    
                        else
                        {
                            Console.WriteLine("Byte array does not contain enough data");
                        }
                    }
                }
            }
        }

        public static byte[] PackDataRequestCO2History(ushort startIndex)
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
                System.Diagnostics.Debug.WriteLine("Sent data: " + BitConverter.ToString(data));
                return memoryStream.ToArray();
            }
        }

        static int ConvertSecondsToFullMinutes(int seconds)
        {
            return (int)Math.Ceiling(seconds / 60.0);
        }
    }
}
