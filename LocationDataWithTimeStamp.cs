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
        [JsonProperty("timeLastSeen")]
        public DateTime timeLastSeen;
        public LocationDataWithTimeStamp(string type, long ID, string Name, double latitude, double longitude, double myLatitude, double myLongitude, DateTime timeLastSeen) : base(type, ID, Name, latitude, longitude, myLatitude, myLongitude)
        {
            base.type = type;
            base.ID = ID;
            base.Name = Name;
            base.latitude = latitude;
            base.longitude = longitude;
            this.timeLastSeen = timeLastSeen;
        }

        // Override Equals to compare just type and ID 
        public override bool Equals(object obj)
        {
            if (obj is LocationDataWithTimeStamp other)
            {
                return this.type == other.type && this.ID == other.ID;
            }
            return false;
        }

        // Override GetHashCode to use just type and ID
        public override int GetHashCode()
        {
            return HashCode.Combine(this.type, this.ID);
        }
    }

}
