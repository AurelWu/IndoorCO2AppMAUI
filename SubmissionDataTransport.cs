using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IndoorCO2App_Multiplatform
{
    internal class SubmissionDataTransport
    {
        public string sensorID;
        public long startTime;
        public List<SensorData> sensorData;

        public List<Double> LatitudeData;
        public List<Double> LongitudeData;

        //public bool openWindowsDoors;
        //public bool ventilationSystem;
        //public string OccupancyLevel;
        public string AdditionalNotes;

        //public string TransportMode;
        public long TransportID;
        public string TransportNWRType;
        public long StartingPointID;
        public string StartingNWRType;
        public long EndPointID;
        public string EndPointNWRType;


        public SubmissionDataTransport(string sensorID, long startTime, long transportID, string transportNWRType, long startingPointID, string startingPointNWRType)
        {
            this.sensorID = sensorID;
            this.startTime = startTime;
            this.LatitudeData = new List<Double>();
            this.LongitudeData = new List<Double>();
            //this.TransportMode = "";
            this.TransportID = transportID;
            this.StartingPointID = startingPointID;
            this.EndPointID = 0;
            this.TransportNWRType = transportNWRType;
            this.StartingNWRType = startingPointNWRType;
            this.EndPointNWRType = "";
        }
        public string ToJson(int rangeSliderMin, int rangeSliderMax)
        {

            JObject json = new JObject();
            int arraySize = (rangeSliderMax - rangeSliderMin);
            string[] ppmArray = new string[arraySize];
            string[] timestampArray = new string[arraySize];

            if (rangeSliderMin + 1 > sensorData.Count)
            {
                throw new IndexOutOfRangeException("RangeSliderMin +1 > SensorData Array - this should not happen");
            }

            if (rangeSliderMax > sensorData.Count)
            {
                throw new IndexOutOfRangeException("RangeSliderMax +1 > SensorData Array - this should not happen");
            }

            int arrayIndex = 0;
            for (int i = rangeSliderMin; i < rangeSliderMax; i++)
            {
                SensorData data = sensorData[i];
                ppmArray[arrayIndex] = data.CO2ppm.ToString();
                timestampArray[arrayIndex] = data.timeStamp.ToString();
                arrayIndex++;
            }

            //
            try
            {
                json.Add("d", sensorID);
                json.Add("b", startTime);
                json.Add("si", StartingPointID);
                json.Add("st", StartingNWRType);
                json.Add("di", EndPointID);
                json.Add("dt", EndPointNWRType);
                json.Add("li", TransportID);
                json.Add("lt", TransportNWRType);
                //json.Add("ad", LocationAddress);
                //json.Add("w", openWindowsDoors);
                //json.Add("v", ventilationSystem);
                //json.Add("o", OccupancyLevel);
                json.Add("a", AdditionalNotes);
                json.Add("c", Converter.ArrayToString(ppmArray, ";"));
                json.Add("la", string.Join(";", LatitudeData));
                json.Add("lo", string.Join(";", LongitudeData));
                json.Add("t", string.Join(";", timestampArray));
            }
            catch (JsonException e)
            {
                Console.Write(e);
            }
            return json.ToString();
        }
    }


}
