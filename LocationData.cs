using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{
    public class LocationData
    {
        [JsonProperty("t")]
        public string type;
        [JsonProperty("i")]
        public long ID;
        [JsonProperty("n")]
        public string Name;
        [JsonProperty("lat")]
        public double latitude;
        [JsonProperty("lon")]
        public double longitude;

        [JsonProperty("dist")]
        public double distanceToGivenLocation;

        public LocationData(string type, long ID, string Name, double latitude, double longitude, double myLatitude, double myLongitude)
        {
            this.type = type;
            this.ID = ID;
            this.Name = Name;
            this.latitude = latitude;
            this.longitude = longitude;
            CalculateDistanceToGivenLocation(myLatitude, myLongitude);
        }
        private void CalculateDistanceToGivenLocation(double myLatitude, double myLongitude)
        {
            distanceToGivenLocation = Haversine.GetDistanceInMeters(myLatitude, myLongitude, latitude, longitude);
        }

        public override string ToString()
        {
            if(Name.Length == 0)
            {
                if(type == "relation")
                {
                    return "nameless relation " + ID;
                }
                if (type == "node")
                {
                    return "nameless node " + ID;
                }
                if (type == "way")
                {
                    return "nameless way " + ID;
                }
                else
                {
                    return "nameless entry " + ID;
                }
            }
            else if (MainPage.MainPageSingleton.favouredLocations.Contains(type + "_" + ID.ToString()))
            {
                return $"★ {Name} | {(int)distanceToGivenLocation}m";
            }
            else
            {
                return $"{Name} | {(int)distanceToGivenLocation}m";
            }
            
                
        }
    }
}
