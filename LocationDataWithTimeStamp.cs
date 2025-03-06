using IndoorCO2App_Multiplatform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Android
{
    public class LocationDataWithTimeStamp : LocationData
    {
        public DateTime TimeLastSeen {  get; set; }
        public LocationDataWithTimeStamp(string type, long ID, string Name, double latitude, double longitude, double myLatitude, double myLongitude, DateTime timeLastSeen) : base(type, ID, Name, latitude, longitude, myLatitude, myLongitude)
        {
            base.Type = type;
            base.ID = ID;
            base.Name = Name;
            base.Latitude = latitude;
            base.Longitude = longitude;
            this.TimeLastSeen = timeLastSeen;
        }

        // Override Equals to compare just type and ID 
        public override bool Equals(object obj)
        {
            if (obj is LocationDataWithTimeStamp other)
            {
                return this.Type == other.Type && this.ID == other.ID;
            }
            return false;
        }

        // Override GetHashCode to use just type and ID
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Type, this.ID);
        }
    }

}
