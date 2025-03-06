using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{
    public class LocationData
    {
        public string Type { get; set; }
        public long ID { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double MyLatitude { get; set; }
        public double MyLongitude { get; set; }

        public double DistanceToGivenLocation { get; set; }

        public LocationData(string type, long id, string name, double latitude, double longitude, double myLatitude, double myLongitude)
        {
            this.Type = type;
            this.ID = id;
            this.Name = name;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.MyLatitude = myLatitude;
            this.MyLongitude = myLongitude;
            CalculateDistanceToGivenLocation(myLatitude, myLongitude);
        }
        private void CalculateDistanceToGivenLocation(double myLatitude, double myLongitude)
        {
            DistanceToGivenLocation = Haversine.GetDistanceInMeters(myLatitude, myLongitude, Latitude, Longitude);
        }

        public override string ToString()
        {
            if(Name.Length == 0)
            {
                if(Type == "relation")
                {
                    return "nameless relation " + ID;
                }
                if (Type == "node")
                {
                    return "nameless node " + ID;
                }
                if (Type == "way")
                {
                    return "nameless way " + ID;
                }
                else
                {
                    return "nameless entry " + ID;
                }
            }
            else if (MainPage.MainPageSingleton.favouredLocations.Contains(Type + "_" + ID.ToString()))
            {
                return $"★ {Name} | {(int)DistanceToGivenLocation}m";
            }
            else
            {
                return $"{Name} | {(int)DistanceToGivenLocation}m";
            }
            
                
        }
    }
}
