using System;
using System.Collections.Generic;
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
        public static int updateInterval = 0;
        public static IReadOnlyList<IDevice> discoveredDevices;
        public static Guid AranetServiceUUID = Guid.Parse("0000FCE0-0000-1000-8000-00805f9b34fb");
        public static Guid Aranet_CharacteristicUUID = Guid.Parse("f0cd3001-95da-4f4b-9ac8-aa55d312af0c"); //Characteristic which has the data we need
        static DateTime previousUpdate = DateTime.MinValue;

        public static bool isRecording;
        public static BluetoothService bluetoothService;
        public static List<SensorData> recordedData;

        internal async static void Init()
        {

#if ANDROID
                bluetoothService = new BluetoothServiceAndroid();
#elif IOS
                bluetoothService = new BluetoothServiceiOS();
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

        }

        internal static void Update()
        {
            DateTime currentTime = DateTime.Now;
            if (currentTime - previousUpdate > TimeSpan.FromSeconds(60))
            {
                previousUpdate = currentTime;
                ScanForDevices();
            }
            
        }

        public static void StartNewRecording()
        {
            isRecording = true;
            recordedData = new List<SensorData>();
        }

        public static void StopRecording()
        {
            isRecording = false; 

        }

        internal static async void ScanForDevices()
        {
            bool checkPermissions = await BluetoothPermissions.CheckBluetoothPermissionStatus();
            bool requestedPermissionsResult = await BluetoothPermissions.RequestBluetoothAccess();
            if (ble == null)
            {
                Init();
            }
            
            if (ble == null) return; // if still null after trying to Init we abort
            var scanFilterOptions = new ScanFilterOptions();
            scanFilterOptions.ServiceUuids = new[] { AranetServiceUUID };
            adapter.ScanMatchMode = ScanMatchMode.STICKY;            
            await adapter.StartScanningForDevicesAsync(scanFilterOptions);
            await adapter.StopScanningForDevicesAsync();
            discoveredDevices = adapter.DiscoveredDevices;
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
            IService r = await device.GetServiceAsync(AranetServiceUUID);
            IReadOnlyList<IService> results = await device.GetServicesAsync();
            if (results.Count > 0)
            {
                foreach (var service in results)
                {
                    var aranet4Characteristic = await service.GetCharacteristicAsync(Aranet_CharacteristicUUID);

                    if (aranet4Characteristic != null)
                    {
                        var result = await aranet4Characteristic.ReadAsync();
                        var data = result.data;
                        if (data.Length >= 9)
                        {                            
                            currentCO2Reading = (data[1] << 8) | (data[0] & 0xFF);
                            updateInterval = (data[10] << 8) | (data[9] & 0xFF);

                            SensorData sd = new SensorData(currentCO2Reading, 0);
                            if (isRecording)
                            {
                                long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                                recordedData.Add(new SensorData(currentCO2Reading, timeStamp));
                            }
                        }


                        else
                        {
                            Console.WriteLine("Byte array does not contain enough data");
                        }
                    }
                }
            }
        }        
    }
}
