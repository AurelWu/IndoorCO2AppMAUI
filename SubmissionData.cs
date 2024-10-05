using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IndoorCO2App_Multiplatform
{
    public class SubmissionData
    {
        internal string SensorID { get; set; }
        internal string NwrType { get; set; }
        internal long NwrID { get; set; }
        internal string NwrName { get; set; }
        internal double NwrLatitude { get; set; }
        internal double NwrLongitude { get; set; }
        internal long StartTime { get; set; }
        internal List<SensorData> SensorData { get; set; }
        internal bool OpenWindowsDoors { get; set; }
        internal bool VentilationSystem { get; set; }
        internal string OccupancyLevel { get; set; }
        internal string AdditionalNotes { get; set; }

        public SubmissionData(string sensorID, string nwrType, long nwrID, string nwrName, double nwrLat, double nwrLon, long startTime)
        {
            SensorID = sensorID;
            NwrType = nwrType;
            NwrID = nwrID;
            NwrName = nwrName;
            StartTime = startTime;
            NwrLatitude = nwrLat;
            NwrLongitude = nwrLon;
            OccupancyLevel = "undefined";
            AdditionalNotes = String.Empty;
            SensorData = new List<SensorData>();
        }

        public string ToJson(int rangeSliderMin, int rangeSliderMax)
        {
            JObject json = new JObject();
            int arraySize = ((rangeSliderMax+1) - rangeSliderMin);
            string[] ppmArray = new string[arraySize];
            string[] timestampArray = new string[arraySize];

            if (rangeSliderMin + 1 > SensorData.Count)
            {
                throw new IndexOutOfRangeException("RangeSliderMin +1 > SensorData Array - this should not happen");
            }

            if (rangeSliderMax > SensorData.Count)
            {
                throw new IndexOutOfRangeException("RangeSliderMax +1 > SensorData Array - this should not happen");
            }

            int arrayIndex = 0;
            for (int i = rangeSliderMin; i <= rangeSliderMax; i++)
            {
                SensorData data = SensorData[i];
                ppmArray[arrayIndex] = data.CO2ppm.ToString();
                timestampArray[arrayIndex] = data.timeStamp.ToString();
                arrayIndex++;
            }

            OpenWindowsDoors = MainPage.hasOpenWindowsDoors;
            VentilationSystem = MainPage.hasVentilationSystem;
            AdditionalNotes = MainPage.MainPageSingleton.GetNotesEditorText();

            json["d"] = SensorID;
            json["p"] = NwrType;
            json["i"] = NwrID;
            json["n"] = NwrName;
            json["b"] = StartTime;
            json["x"] = NwrLongitude; // x = lon
            json["y"] = NwrLatitude; // y = lat
            json["w"] = OpenWindowsDoors;
            json["v"] = VentilationSystem;
            json["o"] = OccupancyLevel;
            json["a"] = AdditionalNotes;
            json["c"] = string.Join(";", ppmArray);
            json["t"] = string.Join(";", timestampArray);

            return json.ToString();
        }
    }

    public static class Converter
    {
        public static string ArrayToString(string[] array, string separator)
        {
            return string.Join(separator, array);
        }
    }
}