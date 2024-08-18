
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IndoorCO2App_Android
{
    internal static class OverpassModule
    {

        public static List<LocationData> LocationData { get; private set; }
        public static bool currentlyFetching = false;
        public static bool lastFetchWasSuccess = false;
        public static bool lastFetchWasSuccessButNoResults = false;
        public static bool everFetchedLocations = false;

        static OverpassModule()
        {
            LocationData = new List<LocationData>();
        }

        private static string BuildOverpassQuery(double latitude, double longitude, double radius)
        {
            LocationData.Clear();
            string rString = radius.ToString(CultureInfo.InvariantCulture);
            string latString = latitude.ToString(CultureInfo.InvariantCulture);
            string lonString = longitude.ToString(CultureInfo.InvariantCulture);
            // Construct the Overpass query with the specified radius and location
            //TODO: add remaining categories of amenities and maybe other
            return "[out:json];" +
                "(" +
                $"nwr(around:{rString},{latString},{lonString})[shop];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=fitness_centre];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=bowling_alley];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=sports_centre];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=sports_hall];" +
                $"nwr(around:{rString},{latString},{lonString})[sport=swimming];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=swimming_pool];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=events_centre];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=bar];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=place_of_worship];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=pub];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=restaurant];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=cafe];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=fast_food];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=food_court];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=ice_cream];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=college];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=dancing_school];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=driving_school];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=kindergarten];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=language_school];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=library];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=cinema];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=theatre];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=music_venue];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=arts_centre];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=brothel];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=love_hotel];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=nightclub];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=planetarium];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=stripclub];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=social_centre];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=playground][indoor=yes];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=research_institute];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=music_school];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=school];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=townhall];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=courthouse];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=post_office];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=university];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=hospital];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=clinic];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=dentist];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=doctors];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=pharmacy];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=social_facility];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=bank];" +
                $"nwr(around:{rString},{latString},{lonString})[healthcare];" +
                $"nwr(around:{rString},{latString},{lonString})[tourism=museum];" +
                $"nwr(around:{rString},{latString},{lonString})[tourism=gallery];" +
                $"nwr(around:{rString},{latString},{lonString})[tourism=hotel];" +
                ");" +
                "out center qt;";
        }

        private static void ParseOverpassResponse(string response, double userLatitude, double userLongitude)
        {
            var elements = JsonDocument.Parse(response).RootElement.GetProperty("elements");

            foreach (var element in elements.EnumerateArray())
            {
                var type = element.GetProperty("type").GetString();
                var id = element.GetProperty("id").GetInt64();
                JsonElement? center = element.TryGetProperty("center", out var centerProperty) ? centerProperty : null;
                var lon = center != null ? center.Value.GetProperty("lon").GetDouble() : element.GetProperty("lon").GetDouble();
                var lat = center != null ? center.Value.GetProperty("lat").GetDouble() : element.GetProperty("lat").GetDouble();
                var tags = element.GetProperty("tags");
                var name = tags.TryGetProperty("name", out var nameProperty) ? nameProperty.GetString() : "";
                var bd = new LocationData(type, id, name, lat, lon, userLatitude, userLongitude);
                LocationData.Add(bd);
            }

            LocationData.Sort((point1, point2) => point1.distanceToGivenLocation.CompareTo(point2.distanceToGivenLocation));
            if (LocationData.Count == 0)
            {
                lastFetchWasSuccessButNoResults = true;
            }
            else
            {
                lastFetchWasSuccessButNoResults = false;
            }
            //spatialManager.MainActivity.InvalidateLocations = true;  //dirty flag for some UI stuff, might not be needed or maybe might
        }

        public static async Task FetchNearbyBuildingsAsync(double userLatitude, double userLongitude, double searchRadius, MainPage mainPage)
        {
            everFetchedLocations = true;
            //userLatitude = 51.1828806;
            //userLongitude = 7.1872148;
            //searchRadius = 250;
            if (currentlyFetching) return;
            currentlyFetching = true;
            lastFetchWasSuccess = false;
            lastFetchWasSuccessButNoResults = false;
            Console.WriteLine("Fetch NearbyBuildings called");
            var overpassQuery = BuildOverpassQuery(userLatitude, userLongitude, searchRadius);
            //var content = new StringContent("data=" + overpassQuery);
            var content = new StringContent("data=" + Uri.EscapeDataString(overpassQuery), Encoding.UTF8, "application/x-www-form-urlencoded");
            using var client = new HttpClient();
            using var response = await client.PostAsync("https://overpass-api.de/api/interpreter", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                ParseOverpassResponse(jsonData, userLatitude, userLongitude);
                mainPage.UpdateLocationPicker();
                lastFetchWasSuccess = true;
                // Update UI on the main thread if necessary
            }
            else
            {
                lastFetchWasSuccess = false;
                // Handle unsuccessful response
            }
            currentlyFetching = false;
        }
    }
}
