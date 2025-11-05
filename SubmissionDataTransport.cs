using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace IndoorCO2App_Multiplatform
{
    internal class SubmissionDataTransport
    {
        public string sensorID;
        public string sensorType;
        public long startTime;
        public List<SensorData> sensorData;

        public List<Double> LatitudeData;
        public List<Double> LongitudeData;
        public List<DateTime> timeStampsOfGPSData;
        public DateTime timeOfLastLocationUpdate; //from this we calculate together with the trimmed data where to cut the Data.

        //public bool openWindowsDoors;
        //public bool ventilationSystem;
        //public string OccupancyLevel;
        public string AdditionalNotes;

        //public string TransportMode;
        public long TransportID;
        public string TransportNWRType;
        public string TransportName;
        public long StartingPointID;
        public string StartingNWRType;
        public string StartingPointName;
        public long EndPointID;
        public string EndPointName;
        public string EndPointNWRType;


        public SubmissionDataTransport(string sensorType,string sensorID, long startTime, long transportID, string transportNWRType, string transportName, long startingPointID, string startingPointNWRType, string startingPointName, bool isRecovery)
        {
            this.sensorID = sensorID;            
            this.sensorType = sensorType;
            this.startTime = startTime;
            this.LatitudeData = new List<Double>();
            this.LongitudeData = new List<Double>();
            this.timeStampsOfGPSData = new List<DateTime>();
            
            //this.TransportMode = "";
            this.TransportID = transportID;
            this.TransportName = transportName;
            this.TransportNWRType = transportNWRType;
            this.StartingPointID = startingPointID;
            this.StartingPointName = startingPointName;
            this.StartingNWRType = startingPointNWRType;
            this.EndPointID = 0;
            this.EndPointName = "";                        
            this.EndPointNWRType = "";
            AdditionalNotes = String.Empty;
            if (MainPage.MainPageSingleton._NotesEditor.Text != null && isRecovery)
            {
                AdditionalNotes = MainPage.MainPageSingleton._NotesEditor.Text;
            }
        }
        public string ToJson(int rangeSliderMin, int rangeSliderMax)
        {

            
            JObject json = new JObject();
            int arraySize = ((rangeSliderMax + 1) - rangeSliderMin);
            string[] ppmArray = new string[arraySize];
            string[] timestampArray = new string[arraySize];
            string[] latitudeArray = new string[arraySize];
            string[] longitudeArray = new string[arraySize];

            if (rangeSliderMin + 1 > sensorData.Count)
            {
                throw new IndexOutOfRangeException("RangeSliderMin +1 > SensorData Array - this should not happen");
            }

            if (rangeSliderMax > sensorData.Count)
            {
                throw new IndexOutOfRangeException("RangeSliderMax +1 > SensorData Array - this should not happen");
            }

            DateTime timeOfLastMeasurement = DateTime.Now;
            int timeSinceStartOfMeasurement = 0;
            if (sensorData.Count>0)
            {
                timeOfLastMeasurement = sensorData.Last().dateTime;
                timeSinceStartOfMeasurement =(int)sensorData.Last().relativeTimeStamp;
            }

            int arrayIndex = 0;
            for (int i = rangeSliderMin; i <= rangeSliderMax; i++)
            {
                SensorData data = sensorData[i];
                ppmArray[arrayIndex] = data.CO2ppm.ToString();
                timestampArray[arrayIndex] = data.relativeTimeStamp.ToString();
                arrayIndex++;
            }
            AdditionalNotes = MainPage.MainPageSingleton.GetNotesEditorText();

            int trimmedCountAtEnd = sensorData.Count - rangeSliderMax - 1;
            int trimmedCountAtStart = rangeSliderMin;
            int trimmedMinutesStart = trimmedCountAtStart;
            int trimmedMinutesEnd = trimmedCountAtEnd;
            
            
            if (sensorData.Count>=2)
            {
                long a = sensorData[0].relativeTimeStamp;
                long b = sensorData[1].relativeTimeStamp;
                long delta = b - a; // if update interval isnt 1 we need to multiply
                trimmedMinutesEnd = trimmedCountAtEnd * (int)delta;
                trimmedMinutesStart = trimmedCountAtStart * (int)delta;
            }

            DateTime cutOffBegin = timeOfLastMeasurement - TimeSpan.FromMinutes(timeSinceStartOfMeasurement)+TimeSpan.FromMinutes(trimmedMinutesStart) ;
            DateTime cutOffEnd = timeOfLastMeasurement - TimeSpan.FromMinutes(trimmedMinutesEnd);

            List<Double> trimmedLatData = new List<Double>();
            List<Double> trimmedLonData = new List<Double>();
            //int gpsEntryCounter = 0;
            //int startIndex = 999999;
            //int endIndex = 0;
            for(int i = 0; i < timeStampsOfGPSData.Count;i++)
            {
                if (timeStampsOfGPSData[i] > cutOffBegin && timeStampsOfGPSData[i] < cutOffEnd)
                {
                    trimmedLatData.Add(LatitudeData[i]);
                    trimmedLonData.Add(LongitudeData[i]);
                }
            }



            //
            try
            {
                json.Add("d", MainPage.MainPageSingleton.appVersion + "_" + sensorType.ToString() + "_" + sensorID); 
                json.Add("b", startTime);
                json.Add("st", StartingNWRType);
                json.Add("si", StartingPointID);
                json.Add("sn", StartingPointName);
                json.Add("dt", EndPointNWRType);
                json.Add("di", EndPointID);
                json.Add("dn", EndPointName);
                json.Add("lt", TransportNWRType);
                json.Add("li", TransportID);
                json.Add("ln", TransportName);
                //json.Add("ad", LocationAddress);
                //json.Add("w", openWindowsDoors);
                //json.Add("v", ventilationSystem);
                //json.Add("o", OccupancyLevel);
                json.Add("c", Converter.ArrayToString(ppmArray, ";"));
                json.Add("t", string.Join(";", timestampArray));
                json.Add("la", string.Join(";", trimmedLatData));
                json.Add("lo", string.Join(";", trimmedLonData));
                json.Add("a", AdditionalNotes);
            }
            catch (JsonException e)
            {
                Console.Write(e);
            }
            return json.ToString();
        }
    }


}
