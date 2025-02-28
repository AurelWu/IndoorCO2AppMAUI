﻿
using IndoorCO2App_Android;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{

    internal enum fetchResult //not yet used
    {
        failure,
        success,
        timeout,
        
    }
    internal static class OverpassModule
    {
        public static string overpassTurboURL = "https://overpass-api.de/api/interpreter";
        public static string privateCoffeeURL = "https://overpass.private.coffee/api/interpreter";

        public static bool useAlternative = false;

        //only buildings for now
        private static CircularBuffer<LocationDataWithTimeStamp> cachedBuildingLocations;
        private static CircularBuffer<LocationDataWithTimeStamp> cachedTransitStopLocations;
        private static CircularBuffer<TransitLineDataWithTimeStamp> cachedTransitLineLocations;


        public static List<LocationData> BuildingLocationData { get; private set; }
        public static List<LocationData> TransportStartLocationData { get; private set; }
        public static List<LocationData> TransportDestinationLocationData { get; private set; }

        public static List<TransitLineData> TransitLines { get; private set; }
        public static List<TransitLineData> filteredTransitLines { get; private set; }

        public static bool currentlyFetching = false;
        public static bool lastFetchWasSuccess = false;
        public static bool lastFetchWasSuccessButNoResults = false;
        public static bool lastFetchWasATimeout = false;
        public static bool everFetchedLocations = false;
        public static bool everFetchedTransitLocations = false;
        public static bool lastFetchWasFromCachedData = false; //make enum out of this mess
        public static DateTime startTimeOfFetch = DateTime.MinValue;

        static OverpassModule()
        {
            BuildingLocationData = new List<LocationData>();
            TransportStartLocationData = new List<LocationData>();
            TransportDestinationLocationData = new List<LocationData>();
            TransitLines = new List<TransitLineData>();
            cachedBuildingLocations = new CircularBuffer<LocationDataWithTimeStamp>(10000);
            cachedTransitStopLocations = new CircularBuffer<LocationDataWithTimeStamp>(10000);
            cachedTransitLineLocations = new CircularBuffer<TransitLineDataWithTimeStamp>(10000);
            LoadCachedBuildingLocations();
            LoadCachedTransitStopLocations();
            LoadCachedTransitLineLocations();
        }

        /// <summary>
        /// loads Locations stored in File Cache when app is started
        /// </summary>
        private static async void LoadCachedBuildingLocations()
        {
            var savedCachedLocationsHashSet = await FileStorage.LoadCachedLocationsHashSetAsync(CacheDataCategory.Building);
            List<LocationDataWithTimeStamp> cachedList = savedCachedLocationsHashSet.ToList();
            cachedList = cachedList.OrderBy(x => x.timeLastSeen).ToList();
            cachedBuildingLocations.Clear();
            foreach(var c in cachedList)
            {
                cachedBuildingLocations.Add(c);
            }

        }

        private static async void LoadCachedTransitStopLocations()
        {
            var savedCachedTransitLocationsHashSet = await FileStorage.LoadCachedLocationsHashSetAsync(CacheDataCategory.TransitStop);
            List<LocationDataWithTimeStamp> cachedList = savedCachedTransitLocationsHashSet.ToList();
            cachedList = cachedList.OrderBy(x => x.timeLastSeen).ToList();
            cachedTransitStopLocations.Clear();
            foreach(var c in cachedList)
            {
                cachedTransitStopLocations.Add(c);
            }

        }

        private static async void LoadCachedTransitLineLocations()
        {
            var savedCachedTransitLineHashset = await FileStorage.LoadCachedTransitLineLocationsHashSetAsync();
            List<TransitLineDataWithTimeStamp> cachedList = savedCachedTransitLineHashset.ToList();
            cachedList = cachedList.OrderBy(x=>x.TimeLastSeen).ToList();
            cachedTransitLineLocations.Clear();
            foreach(var c in cachedList)
            {
                cachedTransitLineLocations.Add(c);
            }

        }

        /// <summary>
        /// Adds Location to Cached Locations after Location Update and stores that to filestorage
        /// </summary>
        private static void AddToCachedBuildingLocations()
        {
            try
            {


                foreach (var l in BuildingLocationData)
                {
                    DateTime c = DateTime.Now;
                    LocationDataWithTimeStamp ld = new LocationDataWithTimeStamp(l.type, l.ID, l.Name, l.latitude, l.longitude, 0, 0, c);
                    if (cachedBuildingLocations.Contains(ld))
                    {
                        int pos = cachedBuildingLocations.IndexOf(ld);
                        cachedBuildingLocations[pos].timeLastSeen = c; // if already existing we update the time... as during deserialisation we order by date, we dont need to change position in this buffer instantly
                    }
                    else
                    {
                        cachedBuildingLocations.Add(ld);
                    }
                }
                WriteBuildingCacheToFileStorage();
            }
            catch
            {
                Logger.circularBuffer.Add("Error Adding Building Results to App Cache");
            }
        }

        private static void AddToCachedTransitStopLocations()
        {
            try
            {

                var tr = new List<LocationData>();
                tr.AddRange(TransportStartLocationData);
                tr.AddRange(TransportDestinationLocationData);
                foreach (var l in tr)
                {
                    DateTime c = DateTime.Now;
                    LocationDataWithTimeStamp ld = new LocationDataWithTimeStamp(l.type, l.ID, l.Name, l.latitude, l.longitude, 0, 0, c);
                    if (cachedTransitStopLocations.Contains(ld))
                    {
                        int pos = cachedTransitStopLocations.IndexOf(ld);
                        cachedTransitStopLocations[pos].timeLastSeen = c; // if already existing we update the time... as during deserialisation we order by date, we dont need to change position in this buffer instantly
                    }
                    else
                    {
                        cachedTransitStopLocations.Add(ld);
                    }
                }
                WriteTransitStopCachedToFileStorage();
            }
            catch
            {
                Logger.circularBuffer.Add("Error Adding Transit Stop Results to App Cache");
            }
        }

        private static void AddToCachedTransitLineLocations(double userLatitude, double userLongitude)
        {
            try
            {

                //allows same ID if rounded position is differently, so we can store multiple positions for each relation (better solution down the road which just has the actual routes is way more effort)
                var tr = new List<TransitLineData>();
                tr.AddRange(TransitLines);
                foreach (var l in tr)
                {
                    DateTime c = DateTime.Now;
                    TransitLineDataWithTimeStamp ld = new TransitLineDataWithTimeStamp(l.VehicleType, l.NWRType, l.ID, l.Name, c, double.Round(userLatitude, 3), double.Round(userLongitude, 3));
                    if (cachedTransitLineLocations.Contains(ld))
                    {
                        int pos = cachedTransitLineLocations.IndexOf(ld);
                        cachedTransitLineLocations[pos].TimeLastSeen = c;
                    }
                    else
                    {
                        cachedTransitLineLocations.Add(ld);
                    }
                }
                TransitLines = tr.DistinctBy(c => new { c.NWRType, c.ID }).ToList();
                WriteTransitLineCachedToFileStorage();
            }
            catch
            {
                Logger.circularBuffer.Add("Error Adding Transit Lines to App Cache");
            }
        }

        private static async void WriteBuildingCacheToFileStorage()
        {
            try
            {
                await FileStorage.SaveCachedHashSetAsync(cachedBuildingLocations.ToHashSet(), CacheDataCategory.Building);
            }
            catch
            {
                Logger.circularBuffer.Add("Error writing buildings to App Cache");
            }
            
        }

        private static async void WriteTransitStopCachedToFileStorage()
        {
            try
            {
                await FileStorage.SaveCachedHashSetAsync(cachedTransitStopLocations.ToHashSet(), CacheDataCategory.TransitStop);
            }
            catch
            {
                Logger.circularBuffer.Add("Error writing Transit Stops to App Cache");
            }
            
        }

        private static async void WriteTransitLineCachedToFileStorage()
        {
            try
            {
                await FileStorage.SaveCachedTransitLineHashSetAsync(cachedTransitLineLocations.ToHashSet());
            }
            catch
            {
                Logger.circularBuffer.Add("Error Writing Transit Lines to App Cache");
            }
            
        }

        public static void GetNearbyCachedBuildingLocations(double userLatitude, double userLongitude, double radius)
        {
            BuildingLocationData.Clear();
            Logger.circularBuffer.Add("# of Cached Building Locations Locations total: " + cachedBuildingLocations.Count);
            foreach (var c in cachedBuildingLocations)
            {
                double dist = Haversine.GetDistanceInMeters(userLatitude, userLongitude, c.latitude, c.longitude);
                if (dist <= radius)
                {
                    c.distanceToGivenLocation = dist;
                    BuildingLocationData.Add((LocationData)c);
                }
            }
            BuildingLocationData = BuildingLocationData.OrderBy(x => x.distanceToGivenLocation).ToList();
            SortFavouriteBuildingsToTop();
            Logger.circularBuffer.Add("# of Cached Building Locations Locations in range: " + BuildingLocationData.Count);
            lastFetchWasFromCachedData = true;
        }

        public static void GetNearbyCachedTransitstopLocations(double userLatitude, double userLongitude, double radius, bool origin)
        {
            List<LocationData> ld = TransportStartLocationData;            
            if(!origin)
            {
                ld = TransportDestinationLocationData;
            }
            ld.Clear();
            Logger.circularBuffer.Add("# of Cached Transit Stop Locations Locations total: " + cachedTransitStopLocations.Count);
            foreach (var c in cachedTransitStopLocations)
            {
                double dist = Haversine.GetDistanceInMeters(userLatitude, userLongitude, c.latitude, c.longitude);
                if (dist <= radius)
                {
                    c.distanceToGivenLocation = dist;
                    ld.Add((LocationData)c);
                }
            }
            ld = ld.OrderBy(x => x.distanceToGivenLocation).ToList();
            SortTransitStops();
            Logger.circularBuffer.Add("# of Cached Transit Stop Locations Locations in range: " + ld.Count);
            lastFetchWasFromCachedData = true;
        }

        public static void GetNearbyCachedTransitLineLocations(double userLatitude, double userLongitude, double radius)
        {
            List<TransitLineData> ld = TransitLines;
            ld.Clear();
            Logger.circularBuffer.Add("# of Cached Transitlines in total: " + cachedTransitLineLocations.Count);
            foreach(var c in cachedTransitLineLocations)
            {
                double dist = Haversine.GetDistanceInMeters(userLatitude, userLongitude, c.latitude, c.longitude);
                if (dist <= radius)
                {
                    ld.Add((TransitLineData)c);
                }
                ld = ld.DistinctBy(c => new { c.NWRType, c.ID }).ToList();
            }
            TransitLines = ld;
            SortTransitLines();
            UpdateFilteredTransitLines();
            lastFetchWasFromCachedData = true;
        }


        private static string BuildTransportOverpassQuery(double latitude, double longitude, double radius,bool startLocation)
        {
            if(startLocation)
            {
                TransportStartLocationData.Clear();
                TransitLines.Clear();
            }
        
            else
            {
                TransportDestinationLocationData.Clear();
            }
            string rString = radius.ToString(CultureInfo.InvariantCulture);
            string latString = latitude.ToString(CultureInfo.InvariantCulture);
            string lonString = longitude.ToString(CultureInfo.InvariantCulture);

            //TODO: don't query lines if it isnt start but destination request
            // Construct the Overpass query with the specified radius and location, only for tram stops
            if(startLocation)
            {
                return "[out:json];" +
                    "(" +
                    $"nwr(around:{rString},{latString},{lonString})[railway=tram_stop];" +
                    $"relation(around:{rString},{latString},{lonString})[route=tram];" +
                    $"nwr(around:{rString},{latString},{lonString})[highway=bus_stop];" +
                    $"relation(around:{rString},{latString},{lonString})[route=bus];" +
                    $"nwr(around:{rString},{latString},{lonString})[railway=subway_station];" +
                    $"relation(around:{rString},{latString},{lonString})[route=subway];" +
                    $"nwr(around:{rString},{latString},{lonString})[railway=station];" +     
                    $"nwr(around:{rString},{latString},{lonString})[railway=halt];" +
                    $"nwr(around:{rString},{latString},{lonString})[railway=stop];" +
                    $"relation(around:{rString},{latString},{lonString})[route=train];" +    
                    $"relation(around:{rString},{latString},{lonString})[route=light_rail];" +
                    $"relation(around:{rString},{latString},{lonString})[route=monorail];" +

                    ");" +
                    "out center tags qt;";
            }
            else //doesnt include lines as we just need target location
            {
                return "[out:json];" +
                  "(" +
                  $"nwr(around:{rString},{latString},{lonString})[railway=tram_stop];" +
                  $"nwr(around:{rString},{latString},{lonString})[highway=bus_stop];" +
                  $"nwr(around:{rString},{latString},{lonString})[railway=subway_station];" +
                  $"nwr(around:{rString},{latString},{lonString})[railway=station];" +
                  $"nwr(around:{rString},{latString},{lonString})[railway=stop];" +
                  $"nwr(around:{rString},{latString},{lonString})[railway=halt];" +        

                  ");" +
                  "out center tags qt;";
            }
            
        }

        private static string BuildOverpassQuery(double latitude, double longitude, double radius)
        {
            BuildingLocationData.Clear();
            string rString = radius.ToString(CultureInfo.InvariantCulture);
            string latString = latitude.ToString(CultureInfo.InvariantCulture);
            string lonString = longitude.ToString(CultureInfo.InvariantCulture);
            // Construct the Overpass query with the specified radius and location
            //TODO: add remaining categories of amenities and maybe other
            return "[out:json];" +
                "(" +
                $"nwr(around:{rString},{latString},{lonString})[office=employment_agency];" +
                $"nwr(around:{rString},{latString},{lonString})[office=lawyer];" +
                $"nwr(around:{rString},{latString},{lonString})[office=government];" +
                $"nwr(around:{rString},{latString},{lonString})[office=political_party];" +
                $"nwr(around:{rString},{latString},{lonString})[shop];" +
                $"nwr(around:{rString},{latString},{lonString})[craft];" +
                $"nwr(around:{rString},{latString},{lonString})[aeroway=aerodrome];" +
                $"nwr(around:{rString},{latString},{lonString})[aeroway=terminal];" +
                $"nwr(around:{rString},{latString},{lonString})[railway=station];" +
                $"nwr(around:{rString},{latString},{lonString})[public_transport=station];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=fitness_centre];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=bowling_alley];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=sports_centre];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=sports_hall];" +
                $"nwr(around:{rString},{latString},{lonString})[sport=swimming];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=swimming_pool];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=sauna];" +
                $"nwr(around:{rString},{latString},{lonString})[leisure=hackerspace];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=townhall];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=congress_centre];" +
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
                $"nwr(around:{rString},{latString},{lonString})[amenity=community_centre];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=playground][indoor=yes];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=research_institute];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=music_school];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=school];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=townhall];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=courthouse];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=post_office];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=university];" +
                $"nwr(around:{rString},{latString},{lonString})[building=university];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=hospital];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=clinic];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=dentist];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=doctors];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=pharmacy];" +
                $"nwr(around:{rString},{latString},{lonString})[amenity=veterinary];" +
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
                BuildingLocationData.Add(bd);
            }

            SortFavouriteBuildingsToTop();

            AddToCachedBuildingLocations();

            if (BuildingLocationData.Count == 0)
            {
                lastFetchWasSuccessButNoResults = true;
            }
            else
            {
                lastFetchWasSuccessButNoResults = false;
            }
        }

        public static void SortFavouriteBuildingsToTop()
        {
            BuildingLocationData.Sort((point1, point2) =>
            {
                bool isFavorite1 = MainPage.MainPageSingleton.favouredLocations.Contains(point1.type + "_" + point1.ID);
                bool isFavorite2 = MainPage.MainPageSingleton.favouredLocations.Contains(point2.type + "_" + point2.ID);

                // Sort favorites to the top
                if (isFavorite1 && !isFavorite2) return -1;
                if (!isFavorite1 && isFavorite2) return 1;

                // If both are favorites or both are non-favorites, sort by distance
                return point1.distanceToGivenLocation.CompareTo(point2.distanceToGivenLocation);
            });
        }

        private static void ParseTransitOverpassResponse(string response, double userLatitude, double userLongitude, bool isOrigin)
        {
            var elements = JsonDocument.Parse(response).RootElement.GetProperty("elements");
            HashSet<string> namesOfStops = new HashSet<string>(); //used to remove duplicates
            foreach (var element in elements.EnumerateArray())
            {
                var type = element.GetProperty("type").GetString();
                var id = element.GetProperty("id").GetInt64();

                // Check if the element has a "center" property
                JsonElement? center = element.TryGetProperty("center", out var centerProperty) ? centerProperty : null;
                var lon = center != null ? center.Value.GetProperty("lon").GetDouble() : element.GetProperty("lon").GetDouble();
                var lat = center != null ? center.Value.GetProperty("lat").GetDouble() : element.GetProperty("lat").GetDouble();

                // Get the tags of the element
                var tags = element.GetProperty("tags");

                // Determine if it's a tram, bus, or subway stop, or a line, and process accordingly

                //This should (and will) be all put in a method with only the strings being replaced for each type eventually but for now it works.
                // Tram stop
                if (tags.TryGetProperty("railway", out var railwayProperty) && railwayProperty.GetString() == "tram_stop")
                {
                    var name = tags.TryGetProperty("name", out var stopNameProperty) ? stopNameProperty.GetString() : "";
                    if (namesOfStops.Contains(name))
                    {
                        continue;
                    }
                    if (!namesOfStops.Contains(name))
                    {
                        namesOfStops.Add(name); ;
                    }

                    var bd = new LocationData(type, id, name, lat, lon, userLatitude, userLongitude);
                    if (isOrigin)
                    {
                        TransportStartLocationData.Add(bd);
                        AddToCachedTransitStopLocations();
                    }
                    else
                    {
                        TransportDestinationLocationData.Add(bd);
                        AddToCachedTransitStopLocations();
                    }
                }
                // Tram line (relation)
                else if (type == "relation" && tags.TryGetProperty("route", out var tramRouteProperty) && tramRouteProperty.GetString() == "tram")
                {
                    var lineName = tags.TryGetProperty("name", out var tramLineNameProperty) ? tramLineNameProperty.GetString() : "";
                    TransitLineData t = new TransitLineData("tram", type, id, lineName, userLatitude, userLongitude);
                    TransitLines.Add(t);
                }

                // Bus stop
                else if (tags.TryGetProperty("highway", out var highwayProperty) && highwayProperty.GetString() == "bus_stop")
                {
                    var name = tags.TryGetProperty("name", out var stopNameProperty) ? stopNameProperty.GetString() : "";
                    if (namesOfStops.Contains(name))
                    {
                        continue;
                    }
                    if (!namesOfStops.Contains(name))
                    {
                        namesOfStops.Add(name); ;
                    }

                    var bd = new LocationData(type, id, name, lat, lon, userLatitude, userLongitude);


                    if (isOrigin)
                    {
                        TransportStartLocationData.Add(bd);
                        AddToCachedTransitStopLocations();
                    }
                    else
                    {
                        TransportDestinationLocationData.Add(bd);
                        AddToCachedTransitStopLocations();
                    }
                }
                // Bus line (relation)
                else if (type == "relation" && tags.TryGetProperty("route", out var busRouteProperty) && busRouteProperty.GetString() == "bus")
                {
                    var lineName = tags.TryGetProperty("name", out var busLineNameProperty) ? busLineNameProperty.GetString() : "";
                    TransitLineData t = new TransitLineData("bus", type, id, lineName, userLatitude, userLongitude);
                    TransitLines.Add(t);
                }

                // Subway stop
                else if (tags.TryGetProperty("railway", out var subwayProperty) && subwayProperty.GetString() == "station" &&
                            tags.TryGetProperty("station", out var stationProperty) && stationProperty.GetString() == "subway")
                {
                    var name = tags.TryGetProperty("name", out var stopNameProperty) ? stopNameProperty.GetString() : "";
                    if (namesOfStops.Contains(name))
                    {
                        continue;
                    }
                    if (!namesOfStops.Contains(name))
                    {
                        namesOfStops.Add(name); ;
                    }
                    var bd = new LocationData(type, id, name, lat, lon, userLatitude, userLongitude);
                    if (isOrigin)
                    {
                        TransportStartLocationData.Add(bd);
                        AddToCachedTransitStopLocations();
                    }
                    else
                    {
                        TransportDestinationLocationData.Add(bd);
                        AddToCachedTransitStopLocations();
                    }
                }
                // Subway line (relation)
                else if (type == "relation" && tags.TryGetProperty("route", out var subwayRouteProperty) && subwayRouteProperty.GetString() == "subway")
                {
                    var lineName = tags.TryGetProperty("name", out var subwayLineNameProperty) ? subwayLineNameProperty.GetString() : "";
                    TransitLineData t = new TransitLineData("subway", type, id, lineName, userLatitude, userLongitude);
                    TransitLines.Add(t);
                }

                // Railway stop 
                else if (tags.TryGetProperty("railway", out var RailWayProperty) && new[] { "station", "halt", "stop" }.Contains(RailWayProperty.GetString()))
                {
                    var name = tags.TryGetProperty("name", out var stopNameProperty) ? stopNameProperty.GetString() : "";
                    if (namesOfStops.Contains(name))
                    {
                        continue;
                    }
                    if (!namesOfStops.Contains(name))
                    {
                        namesOfStops.Add(name);
                    }
                    var bd = new LocationData(type, id, name, lat, lon, userLatitude, userLongitude);
                    if (isOrigin)
                    {
                        TransportStartLocationData.Add(bd);
                        AddToCachedTransitStopLocations();
                    }
                    else
                    {
                        TransportDestinationLocationData.Add(bd);
                        AddToCachedTransitStopLocations();
                    }
                }

                // Light rail line (relation)
                else if (type == "relation" && tags.TryGetProperty("route", out var lightRailRouteProperty) && lightRailRouteProperty.GetString() == "light_rail")
                {
                    var lineName = tags.TryGetProperty("name", out var lightRailLineNameProperty) ? lightRailLineNameProperty.GetString() : "";
                    TransitLineData t = new TransitLineData("light_rail", type, id, lineName, userLatitude, userLongitude);
                    TransitLines.Add(t);
                }

                // Mono rail line (relation)
                else if (type == "relation" && tags.TryGetProperty("route", out var monoRailRouteProperty) && monoRailRouteProperty.GetString() == "monorail")
                {
                    var lineName = tags.TryGetProperty("name", out var lightRailLineNameProperty) ? lightRailLineNameProperty.GetString() : "";
                    TransitLineData t = new TransitLineData("light_rail", type, id, lineName, userLatitude, userLongitude); // we categorize monorail also as light_rail 
                    TransitLines.Add(t);
                }

                // Train line (relation)
                else if (type == "relation" && tags.TryGetProperty("route", out var trainRouteProperty) && trainRouteProperty.GetString() == "train")
                {
                    var lineName = tags.TryGetProperty("name", out var trainLineNameProperty) ? trainLineNameProperty.GetString() : "";
                    TransitLineData t = new TransitLineData("train", type, id, lineName, userLatitude, userLongitude);
                    TransitLines.Add(t);
                }

                //TODO ADD TRAIN AND LIGHTRAIL

            }
            SortTransitStops();
            //MainPage.MainPageSingleton.favouredLocations 
            SortTransitLines();
            UpdateFilteredTransitLines();
            if (TransportStartLocationData != null && TransportStartLocationData.Count == 0 && TransportDestinationLocationData != null && TransportDestinationLocationData.Count == 00)
            {
                lastFetchWasSuccessButNoResults = true;
            }
            AddToCachedTransitLineLocations(userLatitude, userLongitude);
        }

        public static void SortTransitStops()
        {
            if (TransportDestinationLocationData != null && TransportDestinationLocationData.Count > 0)
            {
                TransportDestinationLocationData = TransportDestinationLocationData
                 .OrderByDescending(x => MainPage.MainPageSingleton.favouredLocations.Contains(x.type + "_" + x.ID.ToString()))
                  .ThenBy(x => x.distanceToGivenLocation)
                  .ToList();
                
            }
            if (TransportStartLocationData != null && TransportStartLocationData.Count > 0)
            {
                TransportStartLocationData = TransportStartLocationData
  .OrderByDescending(x => MainPage.MainPageSingleton.favouredLocations.Contains(x.type + "_" + x.ID.ToString()))
  .ThenBy(x => x.distanceToGivenLocation)
  .ToList();

            }
        }

        public static void SortTransitLines()
        {
            if (TransitLines != null && TransitLines.Count > 0)
            {
                TransitLines = TransitLines
                .OrderByDescending(x => MainPage.MainPageSingleton.favouredLocations.Contains(x.NWRType + "_" + x.ID.ToString()))  // Moves favorites to the top
                  .ThenBy(x => x.Name, new MultiNumberStringComparer())  // Sorts alphabetically within favorites and non-favorites
                  .ToList();
            }
        }

        public static void UpdateFilteredTransitLines()
        {
            if (TransitLines == null) return;
            filteredTransitLines = new List<TransitLineData>();
            if (MainPage.TransitFilter == TransitFilterMode.All)
            {
                filteredTransitLines = TransitLines;
            }
            else if (MainPage.TransitFilter == TransitFilterMode.Bus)
            {
                filteredTransitLines = TransitLines.Where(x => x.VehicleType.ToLower() == "bus").ToList();
            }
            else if (MainPage.TransitFilter == TransitFilterMode.Tram)
            {
                filteredTransitLines = TransitLines.Where(x => x.VehicleType.ToLower() == "tram").ToList();
            }
            else if (MainPage.TransitFilter == TransitFilterMode.Subway)
            {
                filteredTransitLines = TransitLines.Where(x => x.VehicleType.ToLower() == "subway").ToList();
            }
            else if (MainPage.TransitFilter == TransitFilterMode.LightRail)
            {
                filteredTransitLines = TransitLines.Where(x => x.VehicleType.ToLower() == "light_rail").ToList();
            }
            else if(MainPage.TransitFilter == TransitFilterMode.Train)
            {
                filteredTransitLines = TransitLines.Where(x => x.VehicleType.ToLower() == "train").ToList();
            }

            string filterString = MainPage.MainPageSingleton._TransitLineSearchFilterEditor.Text;
            if (filterString != null && filterString.Length > 0)
            {
                filteredTransitLines = filteredTransitLines.Where(x => x.ShortenedName.ToLower().Contains(filterString.ToLower())).ToList();
            }
        }


        public static async Task FetchNearbyBuildingsAsync(double userLatitude, double userLongitude, double searchRadius, MainPage mainPage)
        {
            //Logger.circularBuffer.Add("location send to Overpass Building request: " + userLatitude + "|" + userLongitude); //TODO remove again
            everFetchedLocations = true;
            //userLatitude = 51.1828806;
            //userLongitude = 7.1872148;
            //searchRadius = 250;
            if (currentlyFetching) return;
            currentlyFetching = true;
            lastFetchWasSuccess = false;
            lastFetchWasSuccessButNoResults = false;
            lastFetchWasATimeout = false;
            startTimeOfFetch = DateTime.Now;
            Console.WriteLine("Fetch NearbyBuildings called");
            var overpassQuery = BuildOverpassQuery(userLatitude, userLongitude, searchRadius);
            //var content = new StringContent("data=" + overpassQuery);
            var content = new StringContent("data=" + Uri.EscapeDataString(overpassQuery), Encoding.UTF8, "application/x-www-form-urlencoded");
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("IndoorCO2DataRecorder/1.0 (https://indoorco2Map.com; aurelwuensch@proton.me)");

            try
            {
                string endpoint = privateCoffeeURL;
                if (useAlternative == true)
                {
                    endpoint = overpassTurboURL;
                }
                using var response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    ParseOverpassResponse(jsonData, userLatitude, userLongitude);
                    mainPage.UpdateLocationPicker(true);
                    lastFetchWasSuccess = true;
                    // Update UI on the main thread if necessary
                }
                else
                {
                    useAlternative = !useAlternative;
                    lastFetchWasSuccess = false;
                    Logger.circularBuffer.Add($"fetching overpass building data not successful, returned: {response.StatusCode} | {response.ReasonPhrase} | ${DateTime.Now}");
                    // Handle unsuccessful response
                }
                currentlyFetching = false;
            }
            catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
            {
                useAlternative = !useAlternative;
                lastFetchWasSuccess = false;
                Logger.circularBuffer.Add($"fetching overpass building data caused exception: |  {ex.Message} | ${DateTime.Now}");
                Console.WriteLine("The request timed out.");
            }
            catch (Exception ex)
            {
                useAlternative = !useAlternative;
                lastFetchWasSuccess = false;
                Logger.circularBuffer.Add($"fetching overpass building data caused exception: |  {ex.Message} | ${DateTime.Now}");
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                currentlyFetching = false;                
            }

        }

        public static async Task FetchNearbyTransitAsync(double userLatitude, double userLongitude, double searchRadius, MainPage mainPage, bool transitOrigin)
        {
            //Logger.circularBuffer.Add("location send to Overpass Transit request: " + userLatitude + "|" + userLongitude); //TODO remove again
            everFetchedTransitLocations = true;
            if (currentlyFetching) return;
            currentlyFetching = true;
            lastFetchWasSuccess = false;
            lastFetchWasSuccessButNoResults = false;
            lastFetchWasATimeout = false;
            startTimeOfFetch = DateTime.Now;
            Console.WriteLine("Fetch NearbyTransit called");
            var overpassQuery = BuildTransportOverpassQuery(userLatitude, userLongitude, searchRadius, transitOrigin);
            var content = new StringContent("data=" + Uri.EscapeDataString(overpassQuery), Encoding.UTF8, "application/x-www-form-urlencoded");
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("IndoorCO2DataRecorder/1.0 (https://indoorco2Map.com; aurelwuensch@proton.me)");

            try
            {
                string endpoint = privateCoffeeURL;
                if(useAlternative == true)
                {
                    endpoint = overpassTurboURL;
                }
                using var response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    //ParseOverpassResponse(jsonData, userLatitude, userLongitude);
                    //mainPage.UpdateLocationPicker();
                    ParseTransitOverpassResponse(jsonData, userLatitude, userLongitude, transitOrigin);
                    if (transitOrigin)
                    {
                        mainPage.UpdateTransitOriginPicker(true);
                        mainPage.UpdateTransitLinesPicker(true);
                    }
                    else mainPage.UpdateTransitDestinationPicker(true);


                    lastFetchWasSuccess = true;
                }
                else
                {
                    useAlternative = !useAlternative;
                    Logger.circularBuffer.Add($"fetching overpass building data not successful, returned: {response.StatusCode} | {response.ReasonPhrase} | ${DateTime.Now}");
                    lastFetchWasSuccess = false;
                    // Handle unsuccessful response
                }
            }
            catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
            {
                useAlternative = !useAlternative;
                lastFetchWasSuccess = false;
                Console.WriteLine("The request timed out.");
                Logger.circularBuffer.Add($"fetching overpass building data caused exception: |  {ex.Message} | ${DateTime.Now}");
            }
            catch (Exception ex)
            {
                useAlternative = !useAlternative;
                lastFetchWasSuccess = false;
                Logger.circularBuffer.Add($"fetching overpass building data caused exception: |  {ex.Message} | ${DateTime.Now}");
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                currentlyFetching = false;                
            }
        }
    }
}
