﻿using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Storage;
using IndoorCO2App_Android;
#if IOS
using Foundation;
#endif

namespace IndoorCO2App_Multiplatform
{
    public static class FileStorage
    {



        private static string fileNameFavourites = "FavouriteLocations.json";

        //we split the cached categories up so we can have bigger size of them without naive distance calculation getting slowed down (might not be needed at all but for now its just a quick way to avoid issues)

        private static string fileNameBuildingCachedLocations = "CachedBuildingLocations.json";
        private static string fileNameCachedTransitStopLocations = "CachedTransitStopLocation.json";
        private static string fileNameCachedTransitLineLocations => "CachedTransitLineLocations.json";

        public static async Task SaveFavouritesHashSetAsync(HashSet<string> hashSet)
        {
            try
            {
                string path = string.Empty;
#if IOS
                    string iosLibraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
                    path = Path.Combine(iosLibraryPath, fileNameFavourites);
#endif
#if ANDROID
                    path = Path.Combine(FileSystem.Current.AppDataDirectory, fileNameFavourites);
#endif

                var json = JsonConvert.SerializeObject(hashSet);
                // Use FileSystem API for writing
                bool existsBefore = File.Exists(path);
                Logger.WriteToLog($"cached file for favourites exists already: " + existsBefore, false);
                await File.WriteAllTextAsync(path, json);
                bool existsAfter = File.Exists(path);
                Logger.WriteToLog($"cached file for favourites exists after save attempt: " + existsBefore, false);
                Console.WriteLine(existsBefore + "_" + existsAfter);
            }
            catch (Exception ex) 
            {
                Logger.WriteToLog($"Error saving Favourites to Cached File {ex.Message}", true);
            }
        }


        public static async Task SaveCachedHashSetAsync(HashSet<LocationDataWithTimeStamp> cacheData, CacheDataCategory category)
        {
            try
            {
                string path = string.Empty;
                if (category == CacheDataCategory.Building)
                {
#if IOS
                    string iosLibraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
                    path = Path.Combine(iosLibraryPath, fileNameBuildingCachedLocations);
#endif
#if ANDROID
                   path = Path.Combine(FileSystem.Current.AppDataDirectory, fileNameBuildingCachedLocations);
#endif
                }
                else if (category == CacheDataCategory.TransitStop)
                {

#if IOS
                    string iosLibraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
                    path = Path.Combine(iosLibraryPath, fileNameCachedTransitStopLocations);
#endif
#if ANDROID
                    path = Path.Combine(FileSystem.Current.AppDataDirectory, fileNameCachedTransitStopLocations);
#endif
                }
                else
                {
                    return;
                }
                var json = JsonConvert.SerializeObject(cacheData); //not sure if this works out of the box without any type hints...
                                                                   // Use FileSystem API for writing
                bool existsBefore = File.Exists(path);
                Logger.WriteToLog($"cached file for category {category} exists already: " + existsBefore,false);
                await File.WriteAllTextAsync(path, json);
                bool existsAfter = File.Exists(path);
                Logger.WriteToLog($"cached file for category {category} exists after save attempt: " + existsBefore, false);
                Console.WriteLine(existsBefore + "_" + existsAfter);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error Writing Locations to Cache File: {ex.Message}",true);
            }
        }

        public static async Task SaveCachedTransitLineHashSetAsync(HashSet<TransitLineDataWithTimeStamp> cacheData)
        {
            try
            {
                string path = string.Empty;

#if IOS
                    string iosLibraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
                    path = Path.Combine(iosLibraryPath, fileNameCachedTransitLineLocations);
#endif
#if ANDROID
                path = Path.Combine(FileSystem.Current.AppDataDirectory, fileNameCachedTransitLineLocations);
#endif

                var json = JsonConvert.SerializeObject(cacheData); 
                                                                   
                bool existsBefore = File.Exists(path);
                Logger.WriteToLog($"cached file for transit Lines exists already: " + existsBefore, false);
                await File.WriteAllTextAsync(path, json);
                bool existsAfter = File.Exists(path);
                Logger.WriteToLog($"cached file for transit Lines exists after save attempt: " + existsBefore, false);
                Console.WriteLine(existsBefore + "_" + existsAfter);
            }
            catch (Exception ex) 
            {
                Logger.WriteToLog($"Error Writing Transit Lines to Cache File: {ex.Message}", true );
            }
            
        }


        public static async Task<HashSet<string>> LoadFavouritesHashSetAsync()
        {
            try
            {
                string path = string.Empty;
#if IOS
                    string iosLibraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
                    path = Path.Combine(iosLibraryPath, fileNameFavourites);
                    Logger.WriteToLog("path attemptint to load favourites from: " + path,true);
#endif
#if ANDROID
                path = Path.Combine(FileSystem.Current.AppDataDirectory, fileNameFavourites);
#endif

                // Check if the file exists in AppDataDirectory
                if (!File.Exists(path))
                {
                    Logger.WriteToLog("No favourites File existing/found",true);
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<string>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(path))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    Logger.WriteToLog("after favourite json read - before deserialisation", true);
                    Logger.WriteToLog("lengthOf favourite json: " + json.Length, true);
                    Logger.WriteToLog("content of favourite json: " + json, true);
                    return JsonConvert.DeserializeObject<HashSet<string>>(json) ?? new HashSet<string>();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error reading favourites file: {ex.Message}",true);
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new HashSet<string>();
            }
        }

        public static async Task<HashSet<LocationDataWithTimeStamp>> LoadCachedLocationsHashSetAsync(CacheDataCategory category )
        {
            string path = string.Empty;
            if (category == CacheDataCategory.Building)
            {
#if IOS
                    string iosLibraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
                    path = Path.Combine(iosLibraryPath, fileNameBuildingCachedLocations);
#endif
#if ANDROID
                path = Path.Combine(FileSystem.Current.AppDataDirectory, fileNameBuildingCachedLocations);
#endif
            }
            else if (category == CacheDataCategory.TransitStop)
            {
#if IOS
                    string iosLibraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
                    path = Path.Combine(iosLibraryPath, fileNameCachedTransitStopLocations);
#endif
#if ANDROID
                path = Path.Combine(FileSystem.Current.AppDataDirectory, fileNameCachedTransitStopLocations);
#endif
            }
            else
            {
                return new HashSet<LocationDataWithTimeStamp>();
            }

            try
            {
                Logger.WriteToLog($"path attempting to load cached location {category} from: " + path, true);
                // Check if the file exists in AppDataDirectory
                if (!File.Exists(path))
                {
                    Logger.WriteToLog($"No Cached File existing/found for category {category}", false);
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<LocationDataWithTimeStamp>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(path))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    Logger.WriteToLog($"after reading location json with length: {json.Length}, before deserialistion", false);
                    HashSet<LocationDataWithTimeStamp> data = JsonConvert.DeserializeObject<HashSet<LocationDataWithTimeStamp>>(json) ?? new HashSet<LocationDataWithTimeStamp>();
                    Logger.WriteToLog($"after deserialisation", false);
                    return data;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error reading location cache file: {ex.Message}", true);
                Console.WriteLine($"Error favourites reading file: {ex.Message}");
                return new HashSet<LocationDataWithTimeStamp>();
            }
        }

        public static async Task<HashSet<TransitLineDataWithTimeStamp>> LoadCachedTransitLineLocationsHashSetAsync()
        {
            string path = string.Empty;

#if IOS
                    string iosLibraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
                    path = Path.Combine(iosLibraryPath, fileNameCachedTransitLineLocations);
#endif
#if ANDROID
            path = Path.Combine(FileSystem.Current.AppDataDirectory, fileNameCachedTransitLineLocations);
#endif

            try
            {
                Logger.WriteToLog($"path attempting to load cached transitlines from: " + path, true);
                // Check if the file exists in AppDataDirectory
                if (!File.Exists(path))
                {
                    Logger.WriteToLog($"No Cached File existing/found for Transit Lines", false);
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<TransitLineDataWithTimeStamp>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(path))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    Logger.WriteToLog($"after reading transitline json with length: {json.Length}, before deserialistion", false);
                    var data = JsonConvert.DeserializeObject<HashSet<TransitLineDataWithTimeStamp>>(json) ?? new HashSet<TransitLineDataWithTimeStamp>();
                    Logger.WriteToLog($"after deserialisation", false);
                    return data;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog($"Error reading transit line cache file: {ex.Message}", true);
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new HashSet<TransitLineDataWithTimeStamp>();
            }
        }
    }
}
