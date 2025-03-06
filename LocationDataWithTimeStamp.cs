using IndoorCO2App_Multiplatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;


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

        public string ToJson()
        {
            var jsonObject = new Dictionary<string, string>
            {
                { "type", this.Type },
                { "id", this.ID.ToString() },  // Store as string
                { "name", this.Name },
                { "latitude", this.Latitude.ToString(CultureInfo.InvariantCulture) }, // Avoid locale issues
                { "longitude", this.Longitude.ToString(CultureInfo.InvariantCulture) },
                { "timeLastSeen", new DateTimeOffset(this.TimeLastSeen).ToUnixTimeSeconds().ToString() } // Store Unix Epoch as string
            };


            return JsonSerializer.Serialize(jsonObject);
        }

        public static LocationDataWithTimeStamp FromJson(string json)
        {
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            var type = jsonObject["type"];
            var id = long.Parse(jsonObject["id"]);
            var name = jsonObject["name"];
            var latitude = double.Parse(jsonObject["latitude"], CultureInfo.InvariantCulture);
            var longitude = double.Parse(jsonObject["longitude"], CultureInfo.InvariantCulture);
            var timeLastSeen = DateTimeOffset.FromUnixTimeSeconds(long.Parse(jsonObject["timeLastSeen"])).UtcDateTime;

            return new LocationDataWithTimeStamp(
                type,
                id,
                name,
                latitude,
                longitude,
                0,
                0,
                timeLastSeen
            );
        }

    }

}
