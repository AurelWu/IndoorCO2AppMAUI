using Newtonsoft.Json;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{    
    public class TransitLineDataWithTimeStamp : TransitLineData
    {
        [JsonProperty("tls")]
        public DateTime TimeLastSeen;
        public TransitLineDataWithTimeStamp(string vehicleType, string NWRType, long ID, string name, DateTime timeLastSeen, double latitude, double longitude) : base(vehicleType, NWRType, ID, name, latitude,longitude)
        {
            base.VehicleType = vehicleType;
            base.NWRType = NWRType;
            base.ID = ID;
            base.Name = name;
            base.ShortenedName = Regex.Replace(Name, @":\s*.*?=>\s*", ": ");
            base.latitude= latitude;
            base.longitude= longitude;
            this.TimeLastSeen = timeLastSeen;

        }

        // Override Equals to compare just type and ID 
        public override bool Equals(object obj)
        {
            if (obj is TransitLineDataWithTimeStamp other)
            {
                return this.NWRType == other.NWRType && this.ID == other.ID && this.latitude == other.latitude && this.longitude == other.longitude;
            }
            return false;
        }

        // Override GetHashCode to use just type and ID
        public override int GetHashCode()
        {
            return HashCode.Combine(this.NWRType, this.ID, this.latitude,this.longitude);
        }
    }
}
