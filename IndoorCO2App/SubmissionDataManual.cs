using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IndoorCO2App
{
    internal class SubmissionDataManual
    {
        public string sensorID;
        public long startTime;
        public List<SensorData> sensorData;

        public List<Double> LatitudeData;
        public List<Double> LongitudeData;

        public bool openWindowsDoors;
        public bool ventilationSystem;
        public string OccupancyLevel;
        public string AdditionalNotes;

        public string LocationName;

        public string LocationAddress;

        public SubmissionDataManual(string sensorID, long startTime)
        {
            this.sensorID = sensorID;
            this.startTime = startTime;
            this.LatitudeData = new List<Double>();
            this.LongitudeData = new List<Double>();
            this.LocationName = "";
            this.LocationAddress = "";
        }
        public string ToJson(int RangeSliderMin, int RangeSliderMax)
        {
               
            JsonObject json = new JsonObject();
            int arraySize = (RangeSliderMax - RangeSliderMin) + 1;
            String[] ppmArray = new String[arraySize];


            //String[] timestampArray = new String[arraySize];
            if (RangeSliderMax + 1 > sensorData.Count())
            {
                throw new ArgumentOutOfRangeException("RangeSliderMax +1 > SensorData Array - this should not happen");
            }


            //
            int arrayIndex = 0;
            ////TODO: use limits defined by RangeSlider
            for (int i = RangeSliderMin; i < RangeSliderMax + 1; i++)
            {
                SensorData data = sensorData[i];
                ppmArray[arrayIndex] = data.CO2ppm.ToString();
                arrayIndex++;
            }

            //
            try
            {                
                json.Add("d", sensorID);
                json.Add("b", startTime);
                json.Add("l", LocationName);
                json.Add("ad", LocationAddress);
                json.Add("w", openWindowsDoors);
                json.Add("v", ventilationSystem);
                json.Add("o", OccupancyLevel);
                json.Add("a", AdditionalNotes);
                json.Add("c", Converter.ArrayToString(ppmArray, ";"));
                json.Add("la", string.Join(";",LatitudeData));
                json.Add("lo", string.Join(";", LongitudeData));
            }
            catch (JsonException e)
            {
                Console.Write(e);
            }
            return json.ToString();
        }
    }


}
