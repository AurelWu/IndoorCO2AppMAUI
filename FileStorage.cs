using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Storage;
using IndoorCO2App_Android;

namespace IndoorCO2App_Multiplatform
{
    public static class FileStorage
    {
        

        
        private static string filePathFavourites => Path.Combine(FileSystem.Current.AppDataDirectory, "FavouriteLocations.json");

        //we split the cached categories up so we can have bigger size of them without naive distance calculation getting slowed down (might not be needed at all but for now its just a quick way to avoid issues)

        private static string filePathBuildingCachedLocations => Path.Combine(FileSystem.Current.AppDataDirectory, "CachedBuildingLocations.json");
        private static string filePathCachedTransitStopLocations => Path.Combine(FileSystem.Current.AppDataDirectory, "CachedTransitStopLocations.json");
        private static string filePathCachedTransitLineLocations => Path.Combine(FileSystem.Current.AppDataDirectory, "CachedTransitLineLocations.json");

        public static async Task SaveFavouritesHashSetAsync(HashSet<string> hashSet)
        {
            var json = JsonConvert.SerializeObject(hashSet);
            // Use FileSystem API for writing
            bool existsBefore = File.Exists(filePathFavourites);
            await File.WriteAllTextAsync(filePathFavourites, json);
            bool existsAfter = File.Exists(filePathFavourites);
            Console.WriteLine(existsBefore + "_" + existsAfter);
        }


        public static async Task SaveCachedHashSetAsync(HashSet<LocationDataWithTimeStamp> cacheData, CacheDataCategory category)
        {
            string path = string.Empty;
            if(category== CacheDataCategory.Building)
            {
                path = filePathBuildingCachedLocations;
            }
            else if (category== CacheDataCategory.TransitStop) 
            {
                path = filePathCachedTransitStopLocations;
            }
            else
            {
                return; 
            }
            var json = JsonConvert.SerializeObject(cacheData); //not sure if this works out of the box without any type hints...
            // Use FileSystem API for writing
            bool existsBefore = File.Exists(path);
            await File.WriteAllTextAsync(path, json);
            bool existsAfter = File.Exists(path);
            Console.WriteLine(existsBefore + "_" + existsAfter);
        }

        public static async Task SaveCachedTransitLineHashSetAsync(HashSet<TransitLineDataWithTimeStamp> cacheData)
        {
            string path = filePathCachedTransitLineLocations;

            var json = JsonConvert.SerializeObject(cacheData); //not sure if this works out of the box without any type hints...
            // Use FileSystem API for writing
            bool existsBefore = File.Exists(path);
            await File.WriteAllTextAsync(path, json);
            bool existsAfter = File.Exists(path);
            Console.WriteLine(existsBefore + "_" + existsAfter);
        }


        public static async Task<HashSet<string>> LoadFavouritesHashSetAsync()
        {
            try
            {
                // Check if the file exists in AppDataDirectory
                if (!File.Exists(filePathFavourites))
                {
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<string>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(filePathFavourites))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<HashSet<string>>(json) ?? new HashSet<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new HashSet<string>();
            }
        }

        public static async Task<HashSet<LocationDataWithTimeStamp>> LoadCachedLocationsHashSetAsync(CacheDataCategory category )
        {
            string path = string.Empty;
            if (category == CacheDataCategory.Building)
            {
                path = filePathBuildingCachedLocations;
            }
            else if (category == CacheDataCategory.TransitStop)
            {
                path = filePathCachedTransitStopLocations;
            }
            else
            {
                return new HashSet<LocationDataWithTimeStamp>();
            }

            try
            {
                // Check if the file exists in AppDataDirectory
                if (!File.Exists(path))
                {
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<LocationDataWithTimeStamp>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(path))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<HashSet<LocationDataWithTimeStamp>>(json) ?? new HashSet<LocationDataWithTimeStamp>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new HashSet<LocationDataWithTimeStamp>();
            }
        }

        public static async Task<HashSet<TransitLineDataWithTimeStamp>> LoadCachedTransitLineLocationsHashSetAsync()
        {
            string path = filePathCachedTransitLineLocations;

            try
            {
                // Check if the file exists in AppDataDirectory
                if (!File.Exists(path))
                {
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<TransitLineDataWithTimeStamp>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(path))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<HashSet<TransitLineDataWithTimeStamp>>(json) ?? new HashSet<TransitLineDataWithTimeStamp>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new HashSet<TransitLineDataWithTimeStamp>();
            }
        }
    }
}
