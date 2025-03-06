using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;

namespace IndoorCO2App_Multiplatform
{    
    public class TransitLineDataWithTimeStamp : TransitLineData
    {        
        public DateTime TimeLastSeen { get; set; }
        public TransitLineDataWithTimeStamp(string vehicleType, string NWRType, long ID, string name, DateTime timeLastSeen, double latitude, double longitude) : base(vehicleType, NWRType, ID, name, latitude,longitude)
        {
            base.VehicleType = vehicleType;
            base.NWRType = NWRType;
            base.ID = ID;
            base.Name = name;
            base.ShortenedName = Regex.Replace(Name, @":\s*.*?=>\s*", ": ");
            base.Latitude= latitude;
            base.Longitude= longitude;
            this.TimeLastSeen = timeLastSeen;

        }

        // Override Equals to compare just type and ID 
        public override bool Equals(object obj)
        {
            if (obj is TransitLineDataWithTimeStamp other)
            {
                return this.NWRType == other.NWRType && this.ID == other.ID && this.Latitude == other.Latitude && this.Longitude == other.Longitude;
            }
            return false;
        }

        // Override GetHashCode to use just type and ID
        public override int GetHashCode()
        {
            return HashCode.Combine(this.NWRType, this.ID, this.Latitude,this.Longitude);
        }
    }
}
