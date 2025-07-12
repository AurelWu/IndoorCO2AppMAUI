
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{
    internal static class AirspotManager
    {
        public static DateTime startTime;
        public static DateTime currentTime;
        public static double timeSinceStart;

        public static List<int> co2Values = new();
        public static int updateInterval;
        public static int currentPage = -1;

        public static List<string> notifyResponses = new();
        public static List<string> writeResponses = new();

        public static IAdapter adapter = CrossBluetoothLE.Current.Adapter;
        public static IDevice connectedDevice;
        public static IService airspotService;
        public static ICharacteristic notifyCharacteristic;
        public static ICharacteristic writeCharacteristic;

        public static Guid airspotServiceGUID = new("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        public static Guid airspotWriteCharacteristicGUID = new("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        public static Guid airspotNotifyCharacteristicGUID = new("6e400003-b5a3-f393-e0a9-e50e24dcca9e");


        public static Dictionary<int, AirSpotDataPage> dataPages = new();


        //this is done by bluetooth Manager anyways, just kept for potential standalone implementations.

        //public static void Init()
        //{
        //    startTime = DateTime.Now;
        //    adapter.DeviceDiscovered += OnDeviceDiscovered;
        //    adapter.ScanTimeout = 10000;
        //    adapter.ScanMode = ScanMode.LowLatency;
        //
        //    StartScan();
        //}
        //
        //private static async void StartScan()
        //{
        //    Console.WriteLine("Scanning for BLE devices...");
        //    await adapter.StartScanningForDevicesAsync();
        //}

        //private static async void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        //{
        //    var device = args.Device;
        //
        //    try
        //    {
        //        var services = await device.GetServicesAsync();
        //        var service = services.FirstOrDefault(s => s.Id == airspotServiceGUID);
        //        if (service != null)
        //        {
        //            connectedDevice = device;
        //            airspotService = service;
        //
        //            Console.WriteLine($"Found Airspot service on device: {device.Name}");
        //            await adapter.StopScanningForDevicesAsync();
        //            await SetupCharacteristics();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error connecting to device: {ex.Message}");
        //    }
        //}

        //private static async Task SetupCharacteristics()
        //{
        //    var characteristics = await airspotService.GetCharacteristicsAsync();
        //    foreach (var ch in characteristics)
        //    {
        //        if (ch.Id == airspotNotifyCharacteristicGUID)
        //        {
        //            notifyCharacteristic = ch;
        //            notifyCharacteristic.ValueUpdated -= OnNotifyValueChanged;
        //            notifyCharacteristic.ValueUpdated += OnNotifyValueChanged;
        //            await notifyCharacteristic.StartUpdatesAsync();
        //            Console.WriteLine("Subscribed to notifications.");
        //        }
        //
        //        if (ch.Id == airspotWriteCharacteristicGUID)
        //        {
        //            writeCharacteristic = ch;
        //            Console.WriteLine("Write characteristic ready.");
        //        }
        //    }
        //}

        public static void OnNotifyValueChanged(object sender, CharacteristicUpdatedEventArgs args)
        {
            var data = args.Characteristic.Value;
            string hex = ByteArrayToString(data);
            Console.WriteLine("Airspot|Data: " + hex + " | " + DateTime.UtcNow);
            ProcessResponse(data);
        }

        public static byte CalculateAirSpotChecksum(byte[] commandWithoutChecksum)
        {
            int sum = 0;
            foreach (byte b in commandWithoutChecksum)
                sum += b;

            return (byte)(sum & 0xFF);
        }

        public static byte[] CreateAirSpotReadPageCommand(ushort pageNumber)
        {
            byte header1 = 0xFF;
            byte header2 = 0xAA;
            byte cmd = 0x0C;
            byte length = 0x02;

            byte highByte = (byte)(pageNumber >> 8);
            byte lowByte = (byte)(pageNumber & 0xFF);

            var command = new List<byte> { header1, header2, cmd, length, highByte, lowByte };
            byte checksum = CalculateAirSpotChecksum(command.ToArray());
            command.Add(checksum);

            return command.ToArray();           
        }

        public static byte[] ReadLiveDataCommand()
        {
            byte[] command = { 0xFF, 0xAA, 0x01, 0x01, 0x01 };
            byte checksum = CalculateAirSpotChecksum(command);
            byte[] fullCommand = command.Concat(new byte[] { checksum }).ToArray();
            //Console.WriteLine("SendToAirSpot: " + ByteArrayToString(fullCommand));
            return fullCommand;
        }

        public static byte[] ReadCurrentAirspotPageCommand()
        {
            var command = new List<byte> { 0xFF, 0xAA, 0x0B, 0x01, 0x00 };
            byte checksum = CalculateAirSpotChecksum(command.ToArray());
            command.Add(checksum);
            string hex = ByteArrayToString(command.ToArray());
            Console.WriteLine("SendToAirSpot: " + hex + " | " + DateTime.UtcNow);
            return command.ToArray();
        }

        public static async Task SendCommandAsync(byte[] command)
        {
            currentTime = DateTime.Now;
            if (writeCharacteristic == null)
                return;

            await writeCharacteristic.WriteAsync(command);
            string hex = ByteArrayToString(command.ToArray());
            Console.WriteLine("Command written to AirSpot: " + hex);
        }

        public static bool IsValidAirSpotResponseChecksum(byte[] response)
        {
            if (response == null || response.Length < 2) return false;
            byte calc = CalculateAirSpotChecksum(response.Take(response.Length - 1).ToArray());
            return calc == response.Last();
        }

        public static string ByteArrayToString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "");

        public static async void ProcessResponse(byte[] response)
        {
            currentTime = DateTime.Now;
            timeSinceStart = (currentTime - startTime).TotalSeconds;
            Console.WriteLine($"time since start in s: {timeSinceStart}");

            if (response.Length < 4)
            {
                Console.WriteLine("response too short, skipping");
                return;
            }

            if (response[0] == 0xFF && response[1] == 0xAA && response[2] == 0x0C && response[3] == 0x01)
            {
                if (response.Length < 7)
                {
                    Console.WriteLine("current Page Number response too short, skipping");
                    return;
                }

                ushort page = (ushort)((response[4] << 8) | response[5]);
                Console.WriteLine("page ID: " + page);
                currentPage = page;
                var readPageCommand = CreateAirSpotReadPageCommand(page);
                await SendCommandAsync(readPageCommand);
            }
            else if (response[2] == 0x01 && response[3] == 0x02)
            {
                if (response.Length >= 11)
                {
                    int value = (response[8] << 8) | response[9];
                    Console.WriteLine("current CO2: " + value);
                    BluetoothManager.currentCO2Reading = value;
                }
                else
                {
                    Console.WriteLine("live data response too short");
                }
            }
            else if (response[2] == 0x20 && response[3] == 0x01)
            {
                Console.WriteLine("automatic timer message, ignored");
            }
            else if (response[0] == 0xFF && response[1] == 0xAA && response[2] == 0x0C && response[3] == 0x80)
            {
                string hex = AirspotManager.ByteArrayToString(response);
                var page = new AirSpotDataPage(response);
                dataPages[page.pageID] = page;
                //SetCurrentCO2Value();

            }
            else
            {
                Console.WriteLine("Unknown response type");
            }
        }

        //public static void SetCurrentCO2Value()
        //{
        //    if (!dataPages.ContainsKey(currentPage)) return; 
        //    AirSpotDataPage curPage = dataPages[currentPage];
        //    int curValue = 0;
        //    for (int i = 0; i < curPage.timestamps.Count; i++)
        //    {
        //        if (curPage.timestamps[i] != 0xFFFFFFFF)// 0xFFFFFFFF = not filled yet, still empty page
        //        {
        //            curValue = curPage.CO2values[i];
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    BluetoothManager.UpdateAirspotLiveData(curValue);
        //}

        public static async Task CollectPageDataAsync()
        {
            Console.WriteLine("AirspotManager.CollectPageData called");

            int maxPageId = 16384; // 0-indexed: valid range is 0..16383
            int pagesToCollectCount = 10;
            List<int> pagesToCollect = new List<int>();

            if (currentPage != -1)
            {
                for (int i = 0; i < pagesToCollectCount; i++)
                {
                    int page = (currentPage - i + maxPageId) % maxPageId;
                    pagesToCollect.Add(page);
                }
            }

            foreach (var p in pagesToCollect)
            {
                if (dataPages.ContainsKey(p) && dataPages[p].finishedPage == true)
                {
                    continue;
                }

                Console.WriteLine("Collecting Page: " + p);
                var c = CreateAirSpotReadPageCommand((ushort)p);
                await SendCommandAsync(c);
            }

            Console.WriteLine("Amount of collected pages total: " + dataPages.Count);
        }

        public static void CreateHistoryData()
        {
            //uses current time and start time to calculate interval, then goes back from largest timestamp (excluding FFFFFFFF)
        }

        public static void SetRecordedData(int minutesinceBeginning)
        {
            List<(long, int)> timeValueTuples = new();

            foreach (var p in dataPages.Values)
            {
                for(int i = 0; i < p.timestamps.Count;i++)
                {
                    if (p.timestamps[i] != 0xFFFFFFFF)
                    {
                        timeValueTuples.Add((p.timestamps[i],p.CO2values[i]));
                    }                    
                }
            }

            var sorted = timeValueTuples.OrderByDescending(x => x.Item1).ToList();
            co2Values = new();
            if (sorted.Count < 2) return;

            int interval = (int)((long)sorted[0].Item1 - (long)sorted[1].Item1);

            int elements = ((minutesinceBeginning * 60) / interval) + 1;
            elements = Math.Min(elements,sorted.Count);
            for (int i = 0; i < elements; i++)
            {
                co2Values.Add(sorted[i].Item2);
            }
            co2Values.Reverse();
            List<SensorData> sensorData = new List<SensorData>();
            int c = 0;
            foreach (var s in co2Values)
            {
                SensorData d = new SensorData(s, c, DateTime.Now);
                c += interval / 60;
                sensorData.Add(d);
            }


            BluetoothManager.UpdateAirspotHistoryData(sensorData);
            //TODO: BluetoothManager.UpdateAirspotHistoryData()
        }
    }
}
