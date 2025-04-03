using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

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

        public string ToJson()
        {
            var jsonObject = new Dictionary<string, string>
    {
        { "vehicleType", this.VehicleType },
        { "nwrType", this.NWRType },
        { "id", this.ID.ToString() },
        { "name", this.Name },
        { "shortenedName", this.ShortenedName },
        { "latitude", this.Latitude.ToString(CultureInfo.InvariantCulture) }, // Ensures decimal format consistency
        { "longitude", this.Longitude.ToString(CultureInfo.InvariantCulture) },
        { "timeLastSeen", new DateTimeOffset(this.TimeLastSeen).ToUnixTimeSeconds().ToString() } // Unix timestamp for consistency
    };

        return JsonSerializer.Serialize(jsonObject);
        }

        public static TransitLineDataWithTimeStamp FromJson(string json)
        {
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            return new TransitLineDataWithTimeStamp(
                jsonObject["vehicleType"],
                jsonObject["nwrType"],
                long.Parse(jsonObject["id"]),
                jsonObject["name"],
                DateTimeOffset.FromUnixTimeSeconds(long.Parse(jsonObject["timeLastSeen"])).UtcDateTime, // Parse Unix timestamp
                double.Parse(jsonObject["latitude"], CultureInfo.InvariantCulture), // Ensure decimal parsing consistency
                double.Parse(jsonObject["longitude"], CultureInfo.InvariantCulture)
            );
        }

    }
}
