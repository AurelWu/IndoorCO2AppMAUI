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
        

        
        private static string FilePathFavourites => Path.Combine(FileSystem.Current.AppDataDirectory, "FavouriteLocations.json");
        private static string FilePathCachedLocations => Path.Combine(FileSystem.Current.AppDataDirectory, "CachedLocations.json");

        public static async Task SaveFavouritesHashSetAsync(HashSet<string> hashSet)
        {
            var json = JsonConvert.SerializeObject(hashSet);
            // Use FileSystem API for writing
            bool existsBefore = File.Exists(FilePathFavourites);
            await File.WriteAllTextAsync(FilePathFavourites, json);
            bool existsAfter = File.Exists(FilePathFavourites);
            Console.WriteLine(existsBefore + "_" + existsAfter);
        }

        // Method duplicated for now as there might be differences compared to the favourites... if there arent than merge to 1 method
        public static async Task SaveCachedHashSetAsync(HashSet<LocationDataWithTimeStamp> cacheData)
        {
            var json = JsonConvert.SerializeObject(cacheData); //not sure if this works out of the box without any type hints...
            // Use FileSystem API for writing
            bool existsBefore = File.Exists(FilePathFavourites);
            await File.WriteAllTextAsync(FilePathFavourites, json);
            bool existsAfter = File.Exists(FilePathFavourites);
            Console.WriteLine(existsBefore + "_" + existsAfter);
        }


        public static async Task<HashSet<string>> LoadFavouritesHashSetAsync()
        {
            try
            {
                // Check if the file exists in AppDataDirectory
                if (!File.Exists(FilePathFavourites))
                {
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<string>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(FilePathFavourites))
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

        public static async Task<HashSet<LocationDataWithTimeStamp>> LoadCachedLocationsHashSetAsync()
        {
            try
            {
                // Check if the file exists in AppDataDirectory
                if (!File.Exists(FilePathFavourites))
                {
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<LocationDataWithTimeStamp>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(FilePathFavourites))
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
    }
}
