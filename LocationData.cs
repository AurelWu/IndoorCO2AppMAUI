using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Android
{
    internal class LocationData
    {
        public string type;
        public long ID;
        public string Name;
        public double latitude;
        public double longitude;

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
            return $"{Name} | {(int)distanceToGivenLocation}m";
        }
    }
}
